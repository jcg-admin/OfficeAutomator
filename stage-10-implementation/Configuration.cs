using System;

namespace OfficeAutomator.Core
{
    /// CLASS: Configuration
    /// 
    /// Purpose: Central data class tracking user selections and system state through
    /// the entire OfficeAutomator workflow (UC-001 through UC-005)
    /// 
    /// Responsibilities:
    ///   • Store version, language, and app exclusion selections
    ///   • Track validation results
    ///   • Track installation path and state
    ///   • Maintain error context
    ///   • Provide single source of truth for all workflow data
    /// 
    /// Property Ownership (Write-Once Principle):
    ///   • version: Owned by UC-001 (VersionSelector)
    ///   • languages: Owned by UC-002 (LanguageSelector)
    ///   • excludedApps: Owned by UC-003 (AppExclusionSelector)
    ///   • configPath, validationPassed: Owned by UC-004 (ConfigValidator)
    ///   • odtPath: Owned by UC-005 (InstallationExecutor)
    ///   • state: Managed by OfficeAutomatorStateMachine
    ///   • errorResult: Populated by ErrorHandler
    ///   • timestamp: Updated by each UC
    /// 
    /// Lifecycle:
    ///   INIT (all null) → UC-001 (version set) → UC-002 (languages set)
    ///   → UC-003 (excludedApps set) → UC-004 (configPath, validationPassed set)
    ///   → UC-005 (odtPath set) → INSTALL_COMPLETE (final state)
    /// 
    /// Error Handling:
    ///   If any UC fails, errorResult is populated and state transitions to
    ///   INSTALL_FAILED (or appropriate error state), triggering rollback
    /// 
    /// Reference: T-019 (State Management Design), T-028 (Configuration Lifecycle)
    public class Configuration
    {
        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: Configuration()
        /// 
        /// Initializes Configuration with all properties in their default state.
        /// Called once at application startup.
        /// 
        /// Post-condition:
        ///   • version = null
        ///   • languages = null
        ///   • excludedApps = null
        ///   • configPath = null
        ///   • validationPassed = false
        ///   • odtPath = null
        ///   • state = "INIT"
        ///   • errorResult = null
        ///   • timestamp = DateTime.Now
        public Configuration()
        {
            version = null;
            languages = null;
            excludedApps = null;
            configPath = null;
            validationPassed = false;
            odtPath = null;
            state = "INIT";
            errorResult = null;
            timestamp = DateTime.Now;
        }

        // ===== PROPERTIES =====

        /// PROPERTY: version
        /// 
        /// Office version selected by user (UC-001)
        /// 
        /// Valid values: "2024", "2021", "2019"
        /// Default: null
        /// Owner: VersionSelector (UC-001)
        /// First set: During UC-001 (SELECT_VERSION state)
        /// Accessed by: UC-002, UC-003, UC-004 (validation), UC-005 (download)
        /// 
        /// Example workflow:
        ///   UC-001: if (IsValidVersion(userSelection)) { config.version = userSelection; }
        ///   UC-004: if (IsVersionAvailable(config.version)) { /* continue */ }
        /// 
        /// Reference: T-022 (UC-001 State Flows)
        public string version { get; set; }

        /// PROPERTY: languages
        /// 
        /// Language(s) selected by user (UC-002)
        /// 
        /// Valid values: ["en-US"], ["es-MX"], or ["en-US", "es-MX"]
        /// Default: null
        /// Owner: LanguageSelector (UC-002)
        /// First set: During UC-002 (SELECT_LANGUAGE state)
        /// Accessed by: UC-003, UC-004 (validation), UC-005 (download)
        /// 
        /// Example workflow:
        ///   UC-002: if (IsValidLanguageSelection(userSelection)) { config.languages = userSelection; }
        ///   UC-004: if (AreLanguagesSupported(config.languages)) { /* continue */ }
        /// 
        /// Reference: T-022 (UC-002 State Flows)
        public string[] languages { get; set; }

        /// PROPERTY: excludedApps
        /// 
        /// Applications to exclude from installation (UC-003)
        /// 
        /// Valid values: Subset of ["Teams", "OneDrive", "Groove", "Lync", "Bing"]
        /// Default: null (no apps excluded)
        /// Owner: AppExclusionSelector (UC-003)
        /// First set: During UC-003 (SELECT_APPS state)
        /// Accessed by: UC-004 (validation), UC-005 (installation configuration)
        /// 
        /// Example workflow:
        ///   UC-003: if (IsValidExclusionSet(userSelection)) { config.excludedApps = userSelection; }
        ///   UC-005: BuildConfigXml(config.excludedApps); // Exclude selected apps
        /// 
        /// Reference: T-023 (UC-003 State & XML Design)
        public string[] excludedApps { get; set; }

        /// PROPERTY: configPath
        /// 
        /// File system path to generated config.xml (UC-004)
        /// 
        /// Format: C:\Users\{username}\AppData\Local\OfficeAutomator\config_{TIMESTAMP}.xml
        /// Default: null
        /// Owner: ConfigGenerator (UC-004)
        /// First set: After successful XML generation during UC-004 (VALIDATE state)
        /// Accessed by: InstallationExecutor (UC-005, passed to setup.exe)
        /// 
        /// Example workflow:
        ///   UC-004: string xmlPath = GenerateConfigXml(config);
        ///           config.configPath = xmlPath;
        ///   UC-005: ProcessStartInfo.Arguments = $"/configure \"{config.configPath}\"";
        /// 
        /// Reference: T-024 (UC-004 Validation Design)
        public string configPath { get; set; }

        /// PROPERTY: validationPassed
        /// 
        /// Boolean flag indicating if UC-004 validation completed successfully
        /// 
        /// Values: true (validation passed) or false (validation failed)
        /// Default: false
        /// Owner: ConfigValidator (UC-004)
        /// First set: After all 8 validation steps pass during UC-004
        /// Accessed by: StateMachine (gates entry to INSTALL_READY state)
        /// 
        /// Example workflow:
        ///   UC-004: if (Step0 && Step1 && ... && Step7) { config.validationPassed = true; }
        ///   UC-005: if (config.validationPassed) { /* proceed to installation */ }
        /// 
        /// Reset on rollback: config.validationPassed = false; // Allow retry
        /// 
        /// Reference: T-024 (UC-004 8-Step Validation)
        public bool validationPassed { get; set; }

        /// PROPERTY: odtPath
        /// 
        /// File system path to Office setup.exe after installation (UC-005)
        /// 
        /// Format: C:\Program Files\Microsoft Office\root\Office16\setup.exe
        /// Default: null
        /// Owner: InstallationExecutor (UC-005)
        /// First set: After installation validation during UC-005 (INSTALL_COMPLETE state)
        /// Accessed by: Applications (Office launched using this path)
        /// 
        /// Example workflow:
        ///   UC-005: if (Office installed successfully) { config.odtPath = FindSetupPath(); }
        ///   Later: LaunchOffice(config.odtPath);
        /// 
        /// Not set on failure: Remains null if installation fails (triggers rollback)
        /// 
        /// Reference: T-025 (UC-005 Installation & Rollback)
        public string odtPath { get; set; }

        /// PROPERTY: state
        /// 
        /// Current state in the workflow state machine (11 states total)
        /// 
        /// Valid values:
        ///   INIT (initial state)
        ///   SELECT_VERSION (UC-001)
        ///   SELECT_LANGUAGE (UC-002)
        ///   SELECT_APPS (UC-003)
        ///   GENERATE_CONFIG (transition)
        ///   VALIDATE (UC-004)
        ///   INSTALL_READY (confirmation before installation)
        ///   INSTALLING (UC-005 in progress)
        ///   INSTALL_COMPLETE (success)
        ///   INSTALL_FAILED (error, triggers rollback)
        ///   ROLLED_BACK (rollback complete, ready for retry)
        /// 
        /// Default: "INIT"
        /// Owner: OfficeAutomatorStateMachine
        /// First set: During initialization
        /// Updated by: StateMachine.TransitionTo() method
        /// 
        /// Example workflow:
        ///   INIT → SELECT_VERSION → SELECT_LANGUAGE → SELECT_APPS
        ///   → VALIDATE → INSTALL_READY → INSTALLING → INSTALL_COMPLETE
        /// 
        /// Error path:
        ///   INSTALLING → INSTALL_FAILED → (rollback) → ROLLED_BACK
        /// 
        /// Reference: T-019 (State Management Design), T-026 (Integration)
        public string state { get; set; }

        /// PROPERTY: errorResult
        /// 
        /// Error information when workflow fails (populated by ErrorHandler)
        /// 
        /// Type: ErrorResult (code, message, technicalDetails)
        /// Default: null (no error)
        /// Owner: ErrorHandler (error handling infrastructure)
        /// Set when: Any UC detects an error and invokes ErrorHandler
        /// Accessed by: UI (display to user), logging (for IT support)
        /// 
        /// Example workflow:
        ///   config.errorResult = new ErrorResult {
        ///       code = "OFF-CONFIG-001",
        ///       message = "Invalid version",
        ///       technicalDetails = "Version not in [2024, 2021, 2019]"
        ///   };
        /// 
        /// Cleared on success: Remains null on successful completion
        /// Retained on rollback: Not cleared (used for logging/audit)
        /// 
        /// Error codes: See T-021 (19 total codes across 6 categories)
        /// 
        /// Reference: T-020 (Error Propagation Strategy), T-027 (Error Scenarios)
        public ErrorResult errorResult { get; set; }

        /// PROPERTY: timestamp
        /// 
        /// UTC timestamp of last configuration update
        /// 
        /// Format: DateTime (UTC)
        /// Default: DateTime.Now (set at construction)
        /// Updated by: Each UC when it modifies configuration
        /// Purpose: Logging, audit trail, identifying stale data
        /// 
        /// Example workflow:
        ///   UC-001: config.version = "2024"; config.timestamp = DateTime.Now;
        ///   UC-002: config.languages = [...]; config.timestamp = DateTime.Now;
        ///   ... (each UC updates timestamp)
        /// 
        /// Not used for business logic: Timestamp is informational only
        /// 
        /// Reference: T-028 (Configuration Lifecycle)
        public DateTime timestamp { get; set; }

        // ===== METHODS =====

        /// METHOD: ToString()
        /// 
        /// Returns string representation of current configuration state
        /// Useful for logging and debugging
        /// 
        /// Format:
        ///   Configuration [state: INIT]
        ///     version: null
        ///     languages: null
        ///     excludedApps: null
        ///     configPath: null
        ///     validationPassed: false
        ///     odtPath: null
        ///     errorResult: null
        ///     timestamp: 2026-05-17T10:30:00Z
        /// 
        /// Returns: String representation of current state
        /// Side effects: None (read-only)
        public override string ToString()
        {
            return $"Configuration [state: {state}]\n" +
                   $"  version: {version}\n" +
                   $"  languages: {(languages != null ? string.Join(", ", languages) : "null")}\n" +
                   $"  excludedApps: {(excludedApps != null ? string.Join(", ", excludedApps) : "null")}\n" +
                   $"  configPath: {configPath}\n" +
                   $"  validationPassed: {validationPassed}\n" +
                   $"  odtPath: {odtPath}\n" +
                   $"  errorResult: {(errorResult != null ? errorResult.code : "null")}\n" +
                   $"  timestamp: {timestamp:O}";
        }

        // ===== INTERNAL CLASSES =====

        /// CLASS: ErrorResult
        /// Represents an error that occurred during workflow
        /// Reference: T-020 (Error Propagation)
        public class ErrorResult
        {
            /// code: Error code identifier (e.g., OFF-CONFIG-001, OFF-SECURITY-101)
            /// Used by: IT support (runbooks), UI (error display), logging
            public string code { get; set; }

            /// message: User-facing error message
            /// Used by: UI (display to user)
            /// Must be: Clear, non-technical
            public string message { get; set; }

            /// technicalDetails: Technical information for IT/developers
            /// Used by: Logging, IT support debugging
            /// Contains: Exception message, error codes, stack traces
            public string technicalDetails { get; set; }

            public override string ToString()
            {
                return $"ErrorResult [code: {code}, message: {message}]";
            }
        }
    }
}
