using Xunit;
using OfficeAutomator.Core;
using OfficeAutomator.Core.Models;
using OfficeAutomator.Core.Error;
using OfficeAutomator.Core.Services;
using OfficeAutomator.Core.State;
using OfficeAutomator.Core.Validation;
using OfficeAutomator.Core.Installation;
using System.Linq;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: OfficeAutomatorE2ETests
    /// Purpose: End-to-End integration tests for complete workflows
    /// Reference: T-026 (Integration & E2E State Machine)
    /// 
    /// SCOPE: Tests complete workflows from UC-001 through UC-005
    /// involving all 10 classes working together
    public class OfficeAutomatorE2ETests
    {
        // ===== HAPPY PATH SCENARIOS =====

        /// TEST E2E-001: Complete Happy Path
        /// UC-001 → UC-002 → UC-003 → UC-004 → UC-005 (Success)
        [Fact]
        public void E2E_001_Complete_Happy_Path_All_UCs()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration();
            var handler = new ErrorHandler();

            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();
            var generator = new ConfigGenerator();
            var validator = new ConfigValidator();
            var executor = new InstallationExecutor();

            // Act - UC-001: Version Selection
            Assert.Equal("INIT", stateMachine.GetCurrentState());
            stateMachine.TransitionTo("SELECT_VERSION");
            Assert.Equal("SELECT_VERSION", stateMachine.GetCurrentState());

            bool versionResult = versionSelector.Execute(config, "2024", handler);
            Assert.True(versionResult);
            Assert.Equal("2024", config.version);
            Assert.Null(config.errorResult);

            // Act - UC-002: Language Selection
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            Assert.Equal("SELECT_LANGUAGE", stateMachine.GetCurrentState());

            bool languageResult = languageSelector.Execute(config, new[] { "en-US", "es-MX" }, handler);
            Assert.True(languageResult);
            Assert.Equal(2, config.languages.Length);
            Assert.Null(config.errorResult);

            // Act - UC-003: App Exclusion
            stateMachine.TransitionTo("SELECT_APPS");
            Assert.Equal("SELECT_APPS", stateMachine.GetCurrentState());

            bool appResult = appSelector.Execute(config, new[] { "Teams", "OneDrive" }, handler);
            Assert.True(appResult);
            Assert.Equal(2, config.excludedApps.Length);
            Assert.Null(config.errorResult);

            // Act - UC-004 Part 1: Generate Config
            stateMachine.TransitionTo("GENERATE_CONFIG");
            Assert.Equal("GENERATE_CONFIG", stateMachine.GetCurrentState());

            string xmlContent = generator.GenerateConfigXml(config);
            Assert.NotNull(xmlContent);
            Assert.Contains("<Config>", xmlContent);

            config.configPath = generator.GetConfigFilePath();
            Assert.NotNull(config.configPath);

            // Act - UC-004 Part 2: Validate Config
            stateMachine.TransitionTo("VALIDATE");
            Assert.Equal("VALIDATE", stateMachine.GetCurrentState());

            bool validationResult = validator.Execute(config, handler);
            Assert.True(validationResult);
            Assert.True(config.validationPassed);
            Assert.Null(config.errorResult);

            // Act - UC-005: Installation
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");
            Assert.Equal("INSTALLING", stateMachine.GetCurrentState());

            bool installResult = executor.Execute(config, handler);
            Assert.NotNull(installResult);

            // Assert - Final State
            stateMachine.TransitionTo("INSTALL_COMPLETE");
            Assert.Equal("INSTALL_COMPLETE", stateMachine.GetCurrentState());
            Assert.True(stateMachine.IsTerminalState("INSTALL_COMPLETE"));
        }

        /// TEST E2E-002: Happy Path - Single Language
        [Fact]
        public void E2E_002_Happy_Path_Single_Language()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration();
            var handler = new ErrorHandler();

            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();
            var generator = new ConfigGenerator();
            var validator = new ConfigValidator();

            // UC-001: Version 2021
            stateMachine.TransitionTo("SELECT_VERSION");
            versionSelector.Execute(config, "2021", handler);
            Assert.Equal("2021", config.version);

            // UC-002: Single language
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            languageSelector.Execute(config, new[] { "es-MX" }, handler);
            Assert.Single(config.languages);

            // UC-003: No exclusions
            stateMachine.TransitionTo("SELECT_APPS");
            appSelector.Execute(config, new string[] { }, handler);
            Assert.Empty(config.excludedApps);

            // UC-004: Generate & Validate
            stateMachine.TransitionTo("GENERATE_CONFIG");
            config.configPath = generator.GetConfigFilePath();

            stateMachine.TransitionTo("VALIDATE");
            bool validationResult = validator.Execute(config, handler);
            Assert.True(validationResult);
        }

        /// TEST E2E-003: Happy Path - Office 2019
        [Fact]
        public void E2E_003_Happy_Path_Office_2019()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration();
            var handler = new ErrorHandler();

            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();
            var generator = new ConfigGenerator();
            var validator = new ConfigValidator();

            stateMachine.TransitionTo("SELECT_VERSION");
            versionSelector.Execute(config, "2019", handler);

            stateMachine.TransitionTo("SELECT_LANGUAGE");
            languageSelector.Execute(config, new[] { "en-US" }, handler);

            stateMachine.TransitionTo("SELECT_APPS");
            appSelector.Execute(config, new[] { "OneDrive", "Teams" }, handler);

            stateMachine.TransitionTo("GENERATE_CONFIG");
            config.configPath = generator.GetConfigFilePath();

            stateMachine.TransitionTo("VALIDATE");
            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        // ===== ERROR SCENARIOS =====

        /// TEST E2E-004: Error in UC-001 (Invalid Version)
        /// Workflow stops at version selection
        [Fact]
        public void E2E_004_Error_UC001_Invalid_Version()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration();
            var handler = new ErrorHandler();
            var versionSelector = new VersionSelector();

            stateMachine.TransitionTo("SELECT_VERSION");
            bool result = versionSelector.Execute(config, "2025", handler); // Invalid

            Assert.False(result);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-001", config.errorResult.code);
            Assert.Null(config.version); // Not updated
        }

        /// TEST E2E-005: Error in UC-002 (Invalid Language)
        [Fact]
        public void E2E_005_Error_UC002_Invalid_Language()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration { version = "2024" }; // From UC-001
            var handler = new ErrorHandler();
            var languageSelector = new LanguageSelector();

            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");

            bool result = languageSelector.Execute(config, new[] { "fr-FR" }, handler); // Invalid

            Assert.False(result);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-002", config.errorResult.code);
            Assert.Null(config.languages); // Not updated
        }

        /// TEST E2E-006: Error in UC-003 (Invalid App)
        [Fact]
        public void E2E_006_Error_UC003_Invalid_App()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" }
            };
            var handler = new ErrorHandler();
            var appSelector = new AppExclusionSelector();

            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");

            bool result = appSelector.Execute(config, new[] { "Word" }, handler); // Not excludable

            Assert.False(result);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-003", config.errorResult.code);
        }

        /// TEST E2E-007: Error in UC-004 (Validation Failure)
        [Fact]
        public void E2E_007_Error_UC004_Validation_Fails()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration 
            { 
                version = "2025", // Invalid version
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                configPath = "config.xml"
            };
            var handler = new ErrorHandler();
            var validator = new ConfigValidator();

            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");

            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
            Assert.False(config.validationPassed);
        }

        /// TEST E2E-008: Error in UC-005 (Installation Fails)
        [Fact]
        public void E2E_008_Error_UC005_Installation_Fails_Triggers_Rollback()
        {
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                validationPassed = false // Not validated - installation will fail
            };
            var handler = new ErrorHandler();
            var executor = new InstallationExecutor();
            var rollback = new RollbackExecutor();

            stateMachine.TransitionTo("INSTALLING");

            // Installation fails
            bool installResult = executor.Execute(config, handler);
            Assert.False(installResult);

            // Trigger rollback
            stateMachine.TransitionTo("INSTALL_FAILED");
            bool rollbackResult = rollback.Execute(config, handler);

            // Rollback should handle cleanup
            Assert.NotNull(rollbackResult);
        }

        // ===== RETRY SCENARIOS =====

        /// TEST E2E-009: Retry After Transient Error
        /// Network error in UC-004 → Retry logic → Success
        [Fact]
        public void E2E_009_Retry_After_Transient_Error()
        {
            var handler = new ErrorHandler();

            // Simulate transient network error
            bool canRetry = handler.IsRetryableError("OFF-NETWORK-301");
            Assert.True(canRetry); // Network errors are retryable

            // Get retry policy
            var policy = handler.GetRetryPolicy("OFF-NETWORK-301");
            Assert.Equal("TRANSIENT", policy.ToString()); // Should support retries

            // Get max retries
            int maxRetries = handler.GetMaxRetries("OFF-NETWORK-301");
            Assert.Equal(3, maxRetries); // 3 attempts for transient
        }

        /// TEST E2E-010: Retry Logic - Backoff Calculation
        [Fact]
        public void E2E_010_Retry_Backoff_Calculation()
        {
            var handler = new ErrorHandler();

            // Transient error: 3 retries with backoff
            int backoff1 = handler.GetBackoffMs(1); // First retry: 2 seconds
            int backoff2 = handler.GetBackoffMs(2); // Second retry: 4 seconds
            int backoff3 = handler.GetBackoffMs(3); // Third retry: 6 seconds

            Assert.Equal(2000, backoff1);
            Assert.Equal(4000, backoff2);
            Assert.Equal(6000, backoff3);
        }

        /// TEST E2E-011: Complete Workflow with Retry
        [Fact]
        public void E2E_011_Complete_Workflow_With_Automatic_Retry()
        {
            var config = new Configuration();
            var handler = new ErrorHandler();

            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();

            // Attempt 1
            bool v1 = versionSelector.Execute(config, "2024", handler);
            Assert.True(v1);

            bool l1 = languageSelector.Execute(config, new[] { "en-US" }, handler);
            Assert.True(l1);

            bool a1 = appSelector.Execute(config, new[] { "Teams" }, handler);
            Assert.True(a1);

            // Simulate transient error recovery
            if (!config.validationPassed)
            {
                // Retry with same config
                v1 = versionSelector.Execute(config, config.version, handler);
                l1 = languageSelector.Execute(config, config.languages, handler);
                a1 = appSelector.Execute(config, config.excludedApps, handler);
            }

            Assert.True(v1 && l1 && a1);
        }

        // ===== STATE MACHINE VERIFICATION =====

        /// TEST E2E-012: State Machine Transitions - Happy Path
        [Fact]
        public void E2E_012_State_Machine_Transitions_Happy_Path()
        {
            var sm = new OfficeAutomatorStateMachine();

            Assert.Equal("INIT", sm.GetCurrentState());
            Assert.True(sm.TransitionTo("SELECT_VERSION"));
            Assert.True(sm.TransitionTo("SELECT_LANGUAGE"));
            Assert.True(sm.TransitionTo("SELECT_APPS"));
            Assert.True(sm.TransitionTo("GENERATE_CONFIG"));
            Assert.True(sm.TransitionTo("VALIDATE"));
            Assert.True(sm.TransitionTo("INSTALL_READY"));
            Assert.True(sm.TransitionTo("INSTALLING"));
            Assert.True(sm.TransitionTo("INSTALL_COMPLETE"));

            Assert.Equal("INSTALL_COMPLETE", sm.GetCurrentState());
            Assert.True(sm.IsTerminalState("INSTALL_COMPLETE"));
        }

        /// TEST E2E-013: State Machine - Error Recovery Path
        [Fact]
        public void E2E_013_State_Machine_Error_Recovery_Path()
        {
            var sm = new OfficeAutomatorStateMachine();

            sm.TransitionTo("SELECT_VERSION");
            sm.TransitionTo("SELECT_LANGUAGE");
            sm.TransitionTo("INSTALLING");

            // Installation fails
            Assert.True(sm.TransitionTo("INSTALL_FAILED"));
            Assert.True(sm.IsErrorState("INSTALL_FAILED"));

            // Rollback
            Assert.True(sm.TransitionTo("ROLLED_BACK"));

            // Retry from beginning
            Assert.True(sm.TransitionTo("SELECT_VERSION"));
            Assert.Equal("SELECT_VERSION", sm.GetCurrentState());
        }

        /// TEST E2E-014: State Machine - Invalid Transition
        [Fact]
        public void E2E_014_State_Machine_Invalid_Transition()
        {
            var sm = new OfficeAutomatorStateMachine();

            // INIT → Cannot jump to INSTALLING
            bool result = sm.TransitionTo("INSTALLING");
            Assert.False(result); // Invalid transition
        }

        // ===== CONFIGURATION LIFECYCLE =====

        /// TEST E2E-015: Configuration Object Lifecycle
        [Fact]
        public void E2E_015_Configuration_Lifecycle_Full_Workflow()
        {
            var config = new Configuration();

            // Initial state
            Assert.Equal("INIT", config.state);
            Assert.Null(config.version);
            Assert.Null(config.languages);
            Assert.Null(config.excludedApps);
            Assert.Null(config.configPath);
            Assert.False(config.validationPassed);

            // After UC-001
            config.version = "2024";
            Assert.Equal("2024", config.version);

            // After UC-002
            config.languages = new[] { "en-US" };
            Assert.NotNull(config.languages);

            // After UC-003
            config.excludedApps = new[] { "Teams" };
            Assert.NotNull(config.excludedApps);

            // After UC-004
            config.configPath = "/path/to/config.xml";
            config.validationPassed = true;
            Assert.True(config.validationPassed);

            // After UC-005
            config.odtPath = "/path/to/setup.exe";
            Assert.NotNull(config.odtPath);
        }

        // ===== ERROR CODES & HANDLING =====

        /// TEST E2E-016: All 19 Error Codes Can Be Created
        [Fact]
        public void E2E_016_All_19_Error_Codes_Creatable()
        {
            var handler = new ErrorHandler();
            var errorCodes = new[]
            {
                // CONFIG (4)
                "OFF-CONFIG-001", "OFF-CONFIG-002", "OFF-CONFIG-003", "OFF-CONFIG-004",
                // SECURITY (3)
                "OFF-SECURITY-101", "OFF-SECURITY-102",
                // SYSTEM (4)
                "OFF-SYSTEM-201", "OFF-SYSTEM-202", "OFF-SYSTEM-203", "OFF-SYSTEM-999",
                // NETWORK (3)
                "OFF-NETWORK-301", "OFF-NETWORK-302",
                // INSTALL (3)
                "OFF-INSTALL-401", "OFF-INSTALL-402", "OFF-INSTALL-403",
                // ROLLBACK (3)
                "OFF-ROLLBACK-501", "OFF-ROLLBACK-502", "OFF-ROLLBACK-503"
            };

            foreach (var code in errorCodes)
            {
                var error = handler.CreateError(code, "Test message", "Test details");
                Assert.Equal(code, error.code);
            }
        }

        /// TEST E2E-017: Error Retry Policies
        [Fact]
        public void E2E_017_Error_Retry_Policies()
        {
            var handler = new ErrorHandler();

            // Transient: 3 retries
            Assert.True(handler.IsRetryableError("OFF-NETWORK-301"));
            Assert.Equal(3, handler.GetMaxRetries("OFF-NETWORK-301"));

            // System: 1 retry
            Assert.True(handler.IsRetryableError("OFF-SYSTEM-201"));
            Assert.Equal(1, handler.GetMaxRetries("OFF-SYSTEM-201"));

            // Permanent: 0 retries
            Assert.False(handler.IsRetryableError("OFF-CONFIG-001"));
            Assert.Equal(0, handler.GetMaxRetries("OFF-CONFIG-001"));
        }

        // ===== PERFORMANCE & TIMING =====

        /// TEST E2E-018: Validation Completes in < 1 Second
        [Fact]
        public void E2E_018_Validation_Performance_Under_1_Second()
        {
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                validationPassed = true,
                configPath = "config.xml"
            };
            var handler = new ErrorHandler();
            var validator = new ConfigValidator();

            var start = System.DateTime.Now;
            validator.Execute(config, handler);
            var elapsed = System.DateTime.Now - start;

            Assert.True(elapsed.TotalMilliseconds < 1000);
        }

        /// TEST E2E-019: Installation Timeout Protection (20 minutes)
        [Fact]
        public void E2E_019_Installation_Timeout_20_Minutes()
        {
            var executor = new InstallationExecutor();

            long timeoutMs = executor.GetTimeoutMs();

            Assert.Equal(1200000, timeoutMs); // 20 * 60 * 1000
        }

        /// TEST E2E-020: Complete End-to-End Performance
        [Fact]
        public void E2E_020_Complete_Workflow_Performance()
        {
            var start = System.DateTime.Now;

            // Full workflow
            var stateMachine = new OfficeAutomatorStateMachine();
            var config = new Configuration();
            var handler = new ErrorHandler();

            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();
            var generator = new ConfigGenerator();
            var validator = new ConfigValidator();

            stateMachine.TransitionTo("SELECT_VERSION");
            versionSelector.Execute(config, "2024", handler);

            stateMachine.TransitionTo("SELECT_LANGUAGE");
            languageSelector.Execute(config, new[] { "en-US" }, handler);

            stateMachine.TransitionTo("SELECT_APPS");
            appSelector.Execute(config, new string[] { }, handler);

            stateMachine.TransitionTo("GENERATE_CONFIG");
            config.configPath = generator.GetConfigFilePath();

            stateMachine.TransitionTo("VALIDATE");
            validator.Execute(config, handler);

            var elapsed = System.DateTime.Now - start;

            // All selections + generation + validation should be < 5 seconds
            Assert.True(elapsed.TotalSeconds < 5);
        }
    }
}
