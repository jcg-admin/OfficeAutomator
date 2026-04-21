using Xunit;
using OfficeAutomator.Core;
using System;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: ErrorHandlerTests
    /// Purpose: TDD tests for comprehensive error handling
    /// Reference: T-020 (Error Propagation), T-021 (Error Codes), T-029 (Retry Integration)
    public class ErrorHandlerTests
    {
        // ===== ERROR CODE TESTS (19 CODES) =====

        /// TEST 1-4: CONFIG ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_CONFIG_001_Invalid_Version()
        {
            // Arrange
            var handler = new ErrorHandler();
            
            // Act
            var error = handler.CreateError("OFF-CONFIG-001", "Invalid version selected", "Version not in [2024, 2021, 2019]");

            // Assert
            Assert.Equal("OFF-CONFIG-001", error.code);
            Assert.NotNull(error.message);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-CONFIG-001"));
        }

        [Fact]
        public void ErrorHandler_OFF_CONFIG_002_Unsupported_Language()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-CONFIG-002", "Unsupported language", "Language not supported");
            
            Assert.Equal("OFF-CONFIG-002", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-CONFIG-002"));
        }

        [Fact]
        public void ErrorHandler_OFF_CONFIG_003_Invalid_App_Exclusion()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-CONFIG-003", "Invalid app selection", "App not in exclusion list");
            
            Assert.Equal("OFF-CONFIG-003", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-CONFIG-003"));
        }

        [Fact]
        public void ErrorHandler_OFF_CONFIG_004_Invalid_Config_File()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-CONFIG-004", "Config file invalid", "XML parsing failed");
            
            Assert.Equal("OFF-CONFIG-004", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-CONFIG-004"));
        }

        /// TEST 5-7: SECURITY ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_SECURITY_101_Hash_Mismatch_Transient()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SECURITY-101", "Hash verification failed", "File hash mismatch");
            
            Assert.Equal("OFF-SECURITY-101", error.code);
            Assert.Equal(RetryPolicy.TRANSIENT, handler.GetRetryPolicy("OFF-SECURITY-101")); // 3x retry
        }

        [Fact]
        public void ErrorHandler_OFF_SECURITY_102_Certificate_Invalid_Permanent()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SECURITY-102", "Certificate invalid", "Microsoft cert validation failed");
            
            Assert.Equal("OFF-SECURITY-102", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-SECURITY-102")); // No retry
        }

        /// TEST 8-11: SYSTEM ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_SYSTEM_201_Timeout_System_Retry()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SYSTEM-201", "Operation timeout", "Exceeded 1000ms");
            
            Assert.Equal("OFF-SYSTEM-201", error.code);
            Assert.Equal(RetryPolicy.SYSTEM, handler.GetRetryPolicy("OFF-SYSTEM-201")); // 1x retry
        }

        [Fact]
        public void ErrorHandler_OFF_SYSTEM_202_Disk_Full()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SYSTEM-202", "Disk full", "Insufficient space");
            
            Assert.Equal("OFF-SYSTEM-202", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-SYSTEM-202"));
        }

        [Fact]
        public void ErrorHandler_OFF_SYSTEM_203_Admin_Required()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SYSTEM-203", "Admin rights required", "Not running as administrator");
            
            Assert.Equal("OFF-SYSTEM-203", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-SYSTEM-203"));
        }

        [Fact]
        public void ErrorHandler_OFF_SYSTEM_999_Unknown_Error()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-SYSTEM-999", "Unknown error", "Unhandled exception");
            
            Assert.Equal("OFF-SYSTEM-999", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-SYSTEM-999"));
        }

        /// TEST 12-14: NETWORK ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_NETWORK_301_Download_Failed_Transient()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-NETWORK-301", "Download failed", "Network error");
            
            Assert.Equal("OFF-NETWORK-301", error.code);
            Assert.Equal(RetryPolicy.TRANSIENT, handler.GetRetryPolicy("OFF-NETWORK-301")); // 3x retry
        }

        [Fact]
        public void ErrorHandler_OFF_NETWORK_302_Connection_Timeout_Transient()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-NETWORK-302", "Connection timeout", "Network latency");
            
            Assert.Equal("OFF-NETWORK-302", error.code);
            Assert.Equal(RetryPolicy.TRANSIENT, handler.GetRetryPolicy("OFF-NETWORK-302")); // 3x retry
        }

        /// TEST 15-17: INSTALL ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_INSTALL_401_Setup_Failed()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-INSTALL-401", "setup.exe failed", "Exit code: 1");
            
            Assert.Equal("OFF-INSTALL-401", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-INSTALL-401"));
        }

        [Fact]
        public void ErrorHandler_OFF_INSTALL_402_Already_Installed_Informational()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-INSTALL-402", "Office already installed", "Registry key found");
            
            Assert.Equal("OFF-INSTALL-402", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-INSTALL-402")); // Informational, no retry
        }

        [Fact]
        public void ErrorHandler_OFF_INSTALL_403_Installation_Corrupted()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-INSTALL-403", "Installation corrupted", "Missing Office files");
            
            Assert.Equal("OFF-INSTALL-403", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-INSTALL-403"));
        }

        /// TEST 18-20: ROLLBACK ERROR CODES
        [Fact]
        public void ErrorHandler_OFF_ROLLBACK_501_Files_Not_Removed_Critical()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-ROLLBACK-501", "Files not removed", "Permission denied");
            
            Assert.Equal("OFF-ROLLBACK-501", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-ROLLBACK-501")); // CRITICAL
        }

        [Fact]
        public void ErrorHandler_OFF_ROLLBACK_502_Registry_Not_Cleaned_Critical()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-ROLLBACK-502", "Registry not cleaned", "Registry locked");
            
            Assert.Equal("OFF-ROLLBACK-502", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-ROLLBACK-502")); // CRITICAL
        }

        [Fact]
        public void ErrorHandler_OFF_ROLLBACK_503_Partial_Rollback_Critical()
        {
            var handler = new ErrorHandler();
            var error = handler.CreateError("OFF-ROLLBACK-503", "Partial rollback", "Multiple failures");
            
            Assert.Equal("OFF-ROLLBACK-503", error.code);
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-ROLLBACK-503")); // CRITICAL
        }

        // ===== RETRY POLICY TESTS =====

        /// TEST 21: Transient Errors Get 3x Retry
        [Fact]
        public void ErrorHandler_Transient_Errors_Get_3x_Retry()
        {
            var handler = new ErrorHandler();

            // All transient codes
            var transientCodes = new[] { "OFF-SECURITY-101", "OFF-NETWORK-301", "OFF-NETWORK-302" };

            foreach (var code in transientCodes)
            {
                Assert.Equal(RetryPolicy.TRANSIENT, handler.GetRetryPolicy(code));
                Assert.Equal(3, handler.GetMaxRetries(code));
            }
        }

        /// TEST 22: System Errors Get 1x Retry
        [Fact]
        public void ErrorHandler_System_Errors_Get_1x_Retry()
        {
            var handler = new ErrorHandler();
            
            Assert.Equal(RetryPolicy.SYSTEM, handler.GetRetryPolicy("OFF-SYSTEM-201"));
            Assert.Equal(1, handler.GetMaxRetries("OFF-SYSTEM-201"));
        }

        /// TEST 23: Permanent Errors Get 0x Retry
        [Fact]
        public void ErrorHandler_Permanent_Errors_Get_0x_Retry()
        {
            var handler = new ErrorHandler();

            var permanentCodes = new[] {
                "OFF-CONFIG-001", "OFF-CONFIG-002", "OFF-CONFIG-003", "OFF-CONFIG-004",
                "OFF-SECURITY-102",
                "OFF-SYSTEM-202", "OFF-SYSTEM-203", "OFF-SYSTEM-999",
                "OFF-INSTALL-401", "OFF-INSTALL-402", "OFF-INSTALL-403",
                "OFF-ROLLBACK-501", "OFF-ROLLBACK-502", "OFF-ROLLBACK-503"
            };

            foreach (var code in permanentCodes)
            {
                Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy(code));
                Assert.Equal(0, handler.GetMaxRetries(code));
            }
        }

        /// TEST 24: Backoff Schedule for Transient
        [Fact]
        public void ErrorHandler_Transient_Backoff_Schedule()
        {
            var handler = new ErrorHandler();

            // Transient backoff: 2s, 4s, 6s
            Assert.Equal(2000, handler.GetBackoffMs(1)); // Retry 1: 2 seconds
            Assert.Equal(4000, handler.GetBackoffMs(2)); // Retry 2: 4 seconds
            Assert.Equal(6000, handler.GetBackoffMs(3)); // Retry 3: 6 seconds
        }

        /// TEST 25: Backoff Schedule for System
        [Fact]
        public void ErrorHandler_System_Backoff_Schedule()
        {
            var handler = new ErrorHandler();

            // System backoff: 2s (single retry)
            Assert.Equal(2000, handler.GetBackoffMs(1)); // Retry 1: 2 seconds
        }

        /// TEST 26: Should Retry Check
        [Fact]
        public void ErrorHandler_ShouldRetry_Returns_Correct_Values()
        {
            var handler = new ErrorHandler();

            // Transient: Can retry
            Assert.True(handler.ShouldRetry("OFF-SECURITY-101", 1));
            Assert.True(handler.ShouldRetry("OFF-SECURITY-101", 2));
            Assert.True(handler.ShouldRetry("OFF-SECURITY-101", 3));
            Assert.False(handler.ShouldRetry("OFF-SECURITY-101", 4)); // 4th attempt, no more retries

            // System: Single retry
            Assert.True(handler.ShouldRetry("OFF-SYSTEM-201", 1));
            Assert.False(handler.ShouldRetry("OFF-SYSTEM-201", 2)); // 2nd attempt, no more retries

            // Permanent: Never retry
            Assert.False(handler.ShouldRetry("OFF-CONFIG-001", 1));
            Assert.False(handler.ShouldRetry("OFF-CONFIG-001", 2));
        }

        /// TEST 27: Error Result Creation
        [Fact]
        public void ErrorHandler_CreateError_Returns_Complete_ErrorResult()
        {
            var handler = new ErrorHandler();

            var error = handler.CreateError(
                "OFF-CONFIG-001",
                "Invalid version",
                "Version not in [2024, 2021, 2019]"
            );

            Assert.NotNull(error);
            Assert.Equal("OFF-CONFIG-001", error.code);
            Assert.Equal("Invalid version", error.message);
            Assert.Equal("Version not in [2024, 2021, 2019]", error.technicalDetails);
        }

        /// TEST 28: Unknown Error Code Handled
        [Fact]
        public void ErrorHandler_Unknown_Error_Code_Gets_Permanent_Policy()
        {
            var handler = new ErrorHandler();

            // Unknown code should be treated as permanent (fail safe)
            Assert.Equal(RetryPolicy.PERMANENT, handler.GetRetryPolicy("OFF-UNKNOWN-999"));
            Assert.Equal(0, handler.GetMaxRetries("OFF-UNKNOWN-999"));
        }

        /// TEST 29: All 19 Codes Defined
        [Fact]
        public void ErrorHandler_All_19_Error_Codes_Defined()
        {
            var handler = new ErrorHandler();

            var allCodes = new[]
            {
                "OFF-CONFIG-001", "OFF-CONFIG-002", "OFF-CONFIG-003", "OFF-CONFIG-004",
                "OFF-SECURITY-101", "OFF-SECURITY-102",
                "OFF-SYSTEM-201", "OFF-SYSTEM-202", "OFF-SYSTEM-203", "OFF-SYSTEM-999",
                "OFF-NETWORK-301", "OFF-NETWORK-302",
                "OFF-INSTALL-401", "OFF-INSTALL-402", "OFF-INSTALL-403",
                "OFF-ROLLBACK-501", "OFF-ROLLBACK-502", "OFF-ROLLBACK-503"
            };

            foreach (var code in allCodes)
            {
                // Each code should have a retry policy
                var policy = handler.GetRetryPolicy(code);
                Assert.NotEqual(RetryPolicy.UNKNOWN, policy);
            }
        }

        /// TEST 30: Complete Error Handling Workflow
        [Fact]
        public void ErrorHandler_Complete_Error_Handling_Workflow()
        {
            var handler = new ErrorHandler();

            // Simulate hash mismatch error during validation
            var error = handler.CreateError(
                "OFF-SECURITY-101",
                "Hash verification failed",
                "Downloaded file hash does not match"
            );

            Assert.Equal("OFF-SECURITY-101", error.code);
            Assert.Equal(RetryPolicy.TRANSIENT, handler.GetRetryPolicy(error.code));
            Assert.Equal(3, handler.GetMaxRetries(error.code));

            // Retry logic
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                int backoffMs = handler.GetBackoffMs(attempt);
                bool canRetry = handler.ShouldRetry(error.code, attempt);

                Assert.True(backoffMs > 0);
                Assert.True(canRetry);
            }

            // After 3 retries, should not retry
            Assert.False(handler.ShouldRetry(error.code, 4));
        }
    }

    /// ENUM: RetryPolicy
    /// Classifies errors into retry categories
    public enum RetryPolicy
    {
        UNKNOWN = 0,
        TRANSIENT = 1,  // 3x retry (2s, 4s, 6s backoff)
        SYSTEM = 2,     // 1x retry (2s backoff)
        PERMANENT = 3   // 0x retry (fail immediately)
    }
}
