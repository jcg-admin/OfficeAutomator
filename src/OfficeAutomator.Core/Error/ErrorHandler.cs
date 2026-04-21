using System;
using OfficeAutomator.Core.Models;
using System.Collections.Generic;

namespace OfficeAutomator.Core.Error
{
    /// CLASS: ErrorHandler
    /// 
    /// Purpose: Comprehensive error handling infrastructure managing 19 error codes,
    /// 3 retry policies, and intelligent retry logic with exponential backoff.
    /// 
    /// Responsibilities:
    ///   • Create and manage error objects (ErrorResult)
    ///   • Classify errors into retry categories (transient/system/permanent)
    ///   • Determine retry attempts and backoff delays
    ///   • Provide retry decision logic
    ///   • Map error codes to user messages and IT runbooks
    /// 
    /// Error Categories (19 total):
    ///   CONFIG (4): Version, language, exclusion, file errors
    ///   SECURITY (3): Hash verification, certificate, fallback
    ///   SYSTEM (4): Timeout, disk, admin, unknown
    ///   NETWORK (3): Download, connection timeout
    ///   INSTALL (3): Failure, already installed, corrupted
    ///   ROLLBACK (3): File removal, registry cleanup, partial (CRITICAL)
    /// 
    /// Retry Policies (3 total):
    ///   TRANSIENT: 3x retry with backoff (2s, 4s, 6s)
    ///     • OFF-SECURITY-101 (hash mismatch - likely network corruption)
    ///     • OFF-NETWORK-301, 302 (download/connection issues)
    ///     • Auto-retry may succeed without user intervention
    /// 
    ///   SYSTEM: 1x retry with backoff (2s)
    ///     • OFF-SYSTEM-201 (timeout - may be temporary resource lock)
    ///     • Single retry to allow system to stabilize
    /// 
    ///   PERMANENT: 0x retry (fail immediately)
    ///     • CONFIG errors: User selection invalid
    ///     • SECURITY-102: Certificate invalid (possible MITM)
    ///     • SYSTEM errors: Disk, admin, unknown
    ///     • INSTALL errors: Setup failure, corruption
    ///     • ROLLBACK errors: Critical system inconsistency
    /// 
    /// Reference: T-020 (Error Propagation), T-021 (Error Codes), T-029 (Retry Integration)
    public class ErrorHandler
    {
        // ===== FIELDS =====

        /// Maps error codes to retry policies
        private readonly Dictionary<string, RetryPolicy> retryPolicies;

        /// Maps error codes to max retry attempts
        private readonly Dictionary<string, int> maxRetries;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: ErrorHandler()
        /// 
        /// Initializes error handler with all 19 error codes and retry policies.
        /// Called once during application startup.
        /// 
        /// Post-condition:
        ///   • All 19 error codes configured
        ///   • Retry policies defined
        ///   • Ready to handle errors
        public ErrorHandler()
        {
            retryPolicies = InitializeRetryPolicies();
            maxRetries = InitializeMaxRetries();
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: CreateError(string code, string message, string technicalDetails) → ErrorResult
        /// 
        /// Creates an error object with the specified code and messages.
        /// Used by UCs when they detect an error.
        /// 
        /// Parameters:
        ///   code: Error code (e.g., "OFF-CONFIG-001", "OFF-SECURITY-101")
        ///   message: User-facing message (clear, non-technical)
        ///   technicalDetails: Technical info for IT support (exception, stack trace)
        /// 
        /// Returns:
        ///   ErrorResult object with code, message, technicalDetails populated
        /// 
        /// Pre-condition:
        ///   • code is not null
        ///   • message is not null
        /// 
        /// Post-condition:
        ///   • Returns new ErrorResult instance
        ///   • No side effects (pure function)
        /// 
        /// Example:
        ///   var error = handler.CreateError(
        ///       "OFF-CONFIG-001",
        ///       "Invalid version selected",
        ///       "Version not in [2024, 2021, 2019]"
        ///   );
        /// 
        /// Reference: T-020 (Error Objects)
        public Configuration.ErrorResult CreateError(string code, string message, string technicalDetails)
        {
            return new Configuration.ErrorResult
            {
                code = code,
                message = message,
                technicalDetails = technicalDetails
            };
        }

        /// METHOD: GetRetryPolicy(string code) → RetryPolicy
        /// 
        /// Returns the retry policy for the given error code.
        /// Used to determine if/how to retry when error occurs.
        /// 
        /// Parameters:
        ///   code: Error code (e.g., "OFF-CONFIG-001")
        /// 
        /// Returns:
        ///   RetryPolicy: TRANSIENT (3x), SYSTEM (1x), or PERMANENT (0x)
        /// 
        /// Post-condition:
        ///   • Never returns null (defaults to PERMANENT for unknown codes)
        ///   • No side effects (pure query)
        /// 
        /// Example:
        ///   var policy = handler.GetRetryPolicy("OFF-SECURITY-101");
        ///   if (policy == RetryPolicy.TRANSIENT) {
        ///       // Can retry up to 3 times
        ///   }
        /// 
        /// Reference: T-029 (Retry Policies)
        public RetryPolicy GetRetryPolicy(string code)
        {
            if (retryPolicies.ContainsKey(code))
            {
                return retryPolicies[code];
            }

            // Unknown codes treated as permanent (fail safe)
            return RetryPolicy.PERMANENT;
        }

        /// METHOD: GetMaxRetries(string code) → int
        /// 
        /// Returns the maximum number of retry attempts for the given error code.
        /// 
        /// Parameters:
        ///   code: Error code
        /// 
        /// Returns:
        ///   int: Number of retries (0, 1, or 3)
        ///   • 0 = PERMANENT (no retries, fail immediately)
        ///   • 1 = SYSTEM (single retry)
        ///   • 3 = TRANSIENT (three retries)
        /// 
        /// Post-condition:
        ///   • Always returns valid count (never null)
        ///   • Defaults to 0 for unknown codes (fail safe)
        /// 
        /// Example:
        ///   int maxRetries = handler.GetMaxRetries("OFF-SECURITY-101");
        ///   for (int attempt = 1; attempt <= maxRetries; attempt++) {
        ///       // Retry logic
        ///   }
        /// 
        /// Reference: T-029 (Retry Count)
        public int GetMaxRetries(string code)
        {
            if (maxRetries.ContainsKey(code))
            {
                return maxRetries[code];
            }

            // Unknown codes: no retries (fail safe)
            return 0;
        }

        /// METHOD: ShouldRetry(string code, int attemptNumber) → bool
        /// 
        /// Determines if a retry should be attempted for the given code and attempt number.
        /// Called before each retry to decide whether to retry or fail.
        /// 
        /// Parameters:
        ///   code: Error code
        ///   attemptNumber: Current attempt number (1-based)
        ///     • attemptNumber=1: First attempt failed, deciding on first retry
        ///     • attemptNumber=2: Second retry being considered
        ///     • etc.
        /// 
        /// Returns:
        ///   true if retry is allowed, false if exhausted or permanent error
        /// 
        /// Example workflow:
        ///   Attempt 1: Hash mismatch (OFF-SECURITY-101)
        ///   → ShouldRetry("OFF-SECURITY-101", 1) = true
        ///   → Wait 2 seconds, retry
        ///   
        ///   Attempt 2: Still fails
        ///   → ShouldRetry("OFF-SECURITY-101", 2) = true
        ///   → Wait 4 seconds, retry
        ///   
        ///   Attempt 3: Still fails
        ///   → ShouldRetry("OFF-SECURITY-101", 3) = true
        ///   → Wait 6 seconds, retry
        ///   
        ///   Attempt 4: Still fails
        ///   → ShouldRetry("OFF-SECURITY-101", 4) = false
        ///   → No more retries, fail permanently
        /// 
        /// Reference: T-029 (Retry Logic)
        public bool ShouldRetry(string code, int attemptNumber)
        {
            if (!maxRetries.ContainsKey(code))
            {
                return false; // Unknown code, no retry
            }

            int maxRetriesForCode = maxRetries[code];
            return attemptNumber <= maxRetriesForCode;
        }

        /// METHOD: GetBackoffMs(int retryAttempt) → int
        /// 
        /// Returns the backoff delay in milliseconds before attempting the given retry.
        /// Used to implement exponential backoff strategy.
        /// 
        /// Parameters:
        ///   retryAttempt: Which retry attempt (1, 2, 3, etc.)
        /// 
        /// Returns:
        ///   Milliseconds to wait before retry
        /// 
        /// Backoff Schedule:
        ///   Attempt 1: 2000ms (2 seconds)
        ///   Attempt 2: 4000ms (4 seconds)
        ///   Attempt 3: 6000ms (6 seconds)
        ///   Attempt 4+: 6000ms (caps at 6 seconds)
        /// 
        /// Purpose:
        ///   • Gives transient issues time to resolve
        ///   • Prevents overwhelming servers with rapid retries
        ///   • Linear backoff (simple but effective for our use case)
        /// 
        /// Example:
        ///   int backoffMs = handler.GetBackoffMs(1); // 2000
        ///   System.Threading.Thread.Sleep(backoffMs);
        ///   // Retry operation
        /// 
        /// Reference: T-029 (Backoff Strategy)
        public int GetBackoffMs(int retryAttempt)
        {
            // Linear backoff: 2s, 4s, 6s (caps at 6s)
            return Math.Min(2000 * retryAttempt, 6000);
        }

        /// METHOD: IsRetryableError(string code) → bool
        /// 
        /// Quick check: Does this error code allow retries?
        /// 
        /// Parameters:
        ///   code: Error code
        /// 
        /// Returns:
        ///   true if retryable (transient or system), false if permanent
        /// 
        /// Example:
        ///   if (handler.IsRetryableError(error.code)) {
        ///       // Show retry option to user
        ///   } else {
        ///       // Show escalation option
        ///   }
        public bool IsRetryableError(string code)
        {
            var policy = GetRetryPolicy(code);
            return policy == RetryPolicy.TRANSIENT || policy == RetryPolicy.SYSTEM;
        }

        /// METHOD: IsCriticalError(string code) → bool
        /// 
        /// Identifies errors that require immediate IT escalation.
        /// Critical errors indicate system-level problems (rollback failures, security).
        /// 
        /// Parameters:
        ///   code: Error code
        /// 
        /// Returns:
        ///   true if error is critical (escalate immediately), false otherwise
        /// 
        /// Critical codes:
        ///   OFF-SECURITY-102: Possible MITM attack
        ///   OFF-ROLLBACK-501/502/503: System stuck (files/registry not cleaned)
        ///   OFF-SYSTEM-999: Unknown error (needs investigation)
        /// 
        /// Example:
        ///   if (handler.IsCriticalError(error.code)) {
        ///       NotifyITSecurityTeam(error);
        ///   }
        public bool IsCriticalError(string code)
        {
            var criticalCodes = new[]
            {
                "OFF-SECURITY-102",    // Possible MITM attack
                "OFF-ROLLBACK-501",    // System stuck - files not removed
                "OFF-ROLLBACK-502",    // System stuck - registry not cleaned
                "OFF-ROLLBACK-503",    // System stuck - partial rollback
                "OFF-SYSTEM-999"       // Unknown error
            };

            return Array.Exists(criticalCodes, element => element == code);
        }

        // ===== PRIVATE METHODS =====

        /// METHOD: InitializeRetryPolicies() → Dictionary<string, RetryPolicy>
        /// 
        /// Defines retry policy for each of the 19 error codes.
        /// Called during construction.
        /// 
        /// Returns:
        ///   Dictionary mapping error code → retry policy
        /// 
        /// Policies:
        ///   TRANSIENT (3x): Off-SECURITY-101, OFF-NETWORK-301, OFF-NETWORK-302
        ///   SYSTEM (1x): OFF-SYSTEM-201
        ///   PERMANENT (0x): All others
        /// 
        /// Reference: T-021 (Error Codes), T-029 (Retry Policies)
        private Dictionary<string, RetryPolicy> InitializeRetryPolicies()
        {
            var policies = new Dictionary<string, RetryPolicy>
            {
                // CONFIG: All permanent (user selection errors)
                { "OFF-CONFIG-001", RetryPolicy.PERMANENT },
                { "OFF-CONFIG-002", RetryPolicy.PERMANENT },
                { "OFF-CONFIG-003", RetryPolicy.PERMANENT },
                { "OFF-CONFIG-004", RetryPolicy.PERMANENT },

                // SECURITY: Hash transient (network corruption), cert permanent
                { "OFF-SECURITY-101", RetryPolicy.TRANSIENT },  // 3x retry
                { "OFF-SECURITY-102", RetryPolicy.PERMANENT },  // CRITICAL

                // SYSTEM: Timeout system (single retry), others permanent
                { "OFF-SYSTEM-201", RetryPolicy.SYSTEM },       // 1x retry
                { "OFF-SYSTEM-202", RetryPolicy.PERMANENT },    // Disk full
                { "OFF-SYSTEM-203", RetryPolicy.PERMANENT },    // Admin required
                { "OFF-SYSTEM-999", RetryPolicy.PERMANENT },    // Unknown - CRITICAL

                // NETWORK: All transient (network issues)
                { "OFF-NETWORK-301", RetryPolicy.TRANSIENT },   // 3x retry
                { "OFF-NETWORK-302", RetryPolicy.TRANSIENT },   // 3x retry

                // INSTALL: All permanent (setup errors)
                { "OFF-INSTALL-401", RetryPolicy.PERMANENT },   // Setup failed
                { "OFF-INSTALL-402", RetryPolicy.PERMANENT },   // Already installed
                { "OFF-INSTALL-403", RetryPolicy.PERMANENT },   // Corrupted

                // ROLLBACK: All permanent and CRITICAL (system stuck)
                { "OFF-ROLLBACK-501", RetryPolicy.PERMANENT },  // Files not removed - CRITICAL
                { "OFF-ROLLBACK-502", RetryPolicy.PERMANENT },  // Registry not cleaned - CRITICAL
                { "OFF-ROLLBACK-503", RetryPolicy.PERMANENT },  // Partial rollback - CRITICAL
            };

            return policies;
        }

        /// METHOD: InitializeMaxRetries() → Dictionary<string, int>
        /// 
        /// Maps each error code to maximum retry attempts.
        /// 
        /// Returns:
        ///   Dictionary mapping error code → max retries
        /// 
        /// Retry counts:
        ///   Transient: 3
        ///   System: 1
        ///   Permanent: 0
        /// 
        /// Reference: T-029 (Retry Counts)
        private Dictionary<string, int> InitializeMaxRetries()
        {
            var retries = new Dictionary<string, int>
            {
                // CONFIG: 0 retries
                { "OFF-CONFIG-001", 0 },
                { "OFF-CONFIG-002", 0 },
                { "OFF-CONFIG-003", 0 },
                { "OFF-CONFIG-004", 0 },

                // SECURITY: 3 for hash, 0 for cert
                { "OFF-SECURITY-101", 3 },  // Transient
                { "OFF-SECURITY-102", 0 },  // Permanent

                // SYSTEM: 1 for timeout, 0 for others
                { "OFF-SYSTEM-201", 1 },    // System
                { "OFF-SYSTEM-202", 0 },    // Permanent
                { "OFF-SYSTEM-203", 0 },    // Permanent
                { "OFF-SYSTEM-999", 0 },    // Permanent

                // NETWORK: 3 retries each
                { "OFF-NETWORK-301", 3 },   // Transient
                { "OFF-NETWORK-302", 3 },   // Transient

                // INSTALL: 0 retries each
                { "OFF-INSTALL-401", 0 },
                { "OFF-INSTALL-402", 0 },
                { "OFF-INSTALL-403", 0 },

                // ROLLBACK: 0 retries (critical)
                { "OFF-ROLLBACK-501", 0 },
                { "OFF-ROLLBACK-502", 0 },
                { "OFF-ROLLBACK-503", 0 },
            };

            return retries;
        }
    }

    /// ENUM: RetryPolicy
    /// Classifies errors into retry categories
    /// 
    /// Values:
    ///   UNKNOWN: Error code not recognized (treat as permanent for safety)
    ///   TRANSIENT: 3x retry with exponential backoff (network/hash issues)
    ///   SYSTEM: 1x retry with backoff (timeout/resource lock)
    ///   PERMANENT: 0x retry (fail immediately - config/security/install errors)
    /// 
    /// Reference: T-029 (Retry Policies)
    public enum RetryPolicy
    {
        UNKNOWN = 0,
        TRANSIENT = 1,  // 3x retry (2s, 4s, 6s backoff)
        SYSTEM = 2,     // 1x retry (2s backoff)
        PERMANENT = 3   // 0x retry (fail immediately)
    }
}
