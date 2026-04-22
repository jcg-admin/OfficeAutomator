using Xunit;
using OfficeAutomator.Core.Models;
using OfficeAutomator.Core.Error;
using System;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: ConfigurationTests
    /// Purpose: TDD tests for Configuration data class
    /// Reference: T-019 (Configuration Class Structure), T-028 (Config Lifecycle)
    public class ConfigurationTests
    {
        /// TEST 1: Initialization
        /// Verify Configuration initializes with all properties null/default
        [Fact]
        public void Configuration_Initializes_With_All_Properties_Null()
        {
            // Arrange & Act
            var config = new Configuration();

            // Assert - All properties should be null or default on initialization
            Assert.Null(config.version);
            Assert.Null(config.languages);
            Assert.Null(config.excludedApps);
            Assert.Null(config.configPath);
            Assert.False(config.validationPassed);
            Assert.Null(config.odtPath);
            Assert.Equal("INIT", config.state);
            Assert.Null(config.errorResult);
            Assert.NotEqual(default(DateTime), config.timestamp); // Timestamp should be set
        }

        /// TEST 2: Version Property
        /// Verify version property can be set and retrieved
        [Fact]
        public void Configuration_Version_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            string testVersion = "2024";

            // Act
            config.version = testVersion;

            // Assert
            Assert.Equal(testVersion, config.version);
        }

        /// TEST 3: Languages Property
        /// Verify languages property can be set with multiple languages
        [Fact]
        public void Configuration_Languages_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            string[] testLanguages = { "en-US", "es-MX" };

            // Act
            config.languages = testLanguages;

            // Assert
            Assert.Equal(testLanguages, config.languages);
            Assert.Equal(2, config.languages.Length);
        }

        /// TEST 4: Excluded Apps Property
        /// Verify excludedApps property can be set with multiple apps
        [Fact]
        public void Configuration_ExcludedApps_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            string[] testApps = { "Teams", "OneDrive" };

            // Act
            config.excludedApps = testApps;

            // Assert
            Assert.Equal(testApps, config.excludedApps);
            Assert.Equal(2, config.excludedApps.Length);
        }

        /// TEST 5: Config Path Property
        /// Verify configPath property can be set (XML file path)
        [Fact]
        public void Configuration_ConfigPath_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            string testPath = "C:\\Users\\test\\AppData\\Local\\OfficeAutomator\\config_20260516_100000.xml";

            // Act
            config.configPath = testPath;

            // Assert
            Assert.Equal(testPath, config.configPath);
        }

        /// TEST 6: Validation Passed Flag
        /// Verify validationPassed flag can be set and read
        [Fact]
        public void Configuration_ValidationPassed_Flag_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            Assert.False(config.validationPassed); // Initially false

            // Act
            config.validationPassed = true;

            // Assert
            Assert.True(config.validationPassed);
        }

        /// TEST 7: ODT Path Property
        /// Verify odtPath property (Office installation path) can be set
        [Fact]
        public void Configuration_OdtPath_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            string testOdtPath = "C:\\Program Files\\Microsoft Office\\root\\Office16\\setup.exe";

            // Act
            config.odtPath = testOdtPath;

            // Assert
            Assert.Equal(testOdtPath, config.odtPath);
        }

        /// TEST 8: State Property
        /// Verify state property can be changed through workflow
        [Fact]
        public void Configuration_State_Property_TransitionsCorrectly()
        {
            // Arrange
            var config = new Configuration();
            Assert.Equal("INIT", config.state); // Initial state

            // Act & Assert - Verify state transitions
            config.state = "SELECT_VERSION";
            Assert.Equal("SELECT_VERSION", config.state);

            config.state = "SELECT_LANGUAGE";
            Assert.Equal("SELECT_LANGUAGE", config.state);

            config.state = "SELECT_APPS";
            Assert.Equal("SELECT_APPS", config.state);

            config.state = "VALIDATE";
            Assert.Equal("VALIDATE", config.state);

            config.state = "INSTALL_READY";
            Assert.Equal("INSTALL_READY", config.state);

            config.state = "INSTALLING";
            Assert.Equal("INSTALLING", config.state);

            config.state = "INSTALL_COMPLETE";
            Assert.Equal("INSTALL_COMPLETE", config.state);
        }

        /// TEST 9: Error Result Property
        /// Verify errorResult can be set when errors occur
        [Fact]
        public void Configuration_ErrorResult_Property_CanBeSet()
        {
            // Arrange
            var config = new Configuration();
            Assert.Null(config.errorResult); // Initially null

            var testError = new Configuration.ErrorResult
            {
                code = "OFF-CONFIG-001",
                message = "Invalid version",
                technicalDetails = "Version not in whitelist"
            };

            // Act
            config.errorResult = testError;

            // Assert
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-001", config.errorResult.code);
            Assert.Equal("Invalid version", config.errorResult.message);
        }

        /// TEST 10: Timestamp Property
        /// Verify timestamp is set on initialization and can be updated
        [Fact]
        public void Configuration_Timestamp_Property_IsSetAndUpdatable()
        {
            // Arrange
            var config = new Configuration();
            var initialTimestamp = config.timestamp;

            // Assert - Timestamp should be recent
            Assert.NotEqual(default(DateTime), initialTimestamp);
            Assert.True((DateTime.Now - initialTimestamp).TotalSeconds < 5);

            // Act - Update timestamp
            var newTimestamp = DateTime.Now.AddSeconds(10);
            config.timestamp = newTimestamp;

            // Assert
            Assert.Equal(newTimestamp, config.timestamp);
            Assert.NotEqual(initialTimestamp, config.timestamp);
        }

        /// TEST 11: Complete Configuration Workflow
        /// Verify all properties work together in a realistic workflow
        [Fact]
        public void Configuration_Complete_Workflow_Scenario()
        {
            // Arrange
            var config = new Configuration();

            // Act - Simulate UC-001 through UC-004 workflow
            config.version = "2024";
            config.state = "SELECT_LANGUAGE";
            Assert.Equal("2024", config.version);

            config.languages = new[] { "en-US" };
            config.state = "SELECT_APPS";
            Assert.NotEmpty(config.languages);

            config.excludedApps = new[] { "Teams" };
            config.state = "VALIDATE";
            Assert.NotEmpty(config.excludedApps);

            config.configPath = "C:\\config.xml";
            config.validationPassed = true;
            config.state = "INSTALL_READY";
            Assert.True(config.validationPassed);

            config.odtPath = "C:\\Office\\setup.exe";
            config.state = "INSTALL_COMPLETE";
            Assert.NotNull(config.odtPath);

            // Assert - Complete workflow snapshot
            Assert.Equal("2024", config.version);
            Assert.Single(config.languages);
            Assert.Single(config.excludedApps);
            Assert.NotNull(config.configPath);
            Assert.True(config.validationPassed);
            Assert.NotNull(config.odtPath);
            Assert.Equal("INSTALL_COMPLETE", config.state);
        }

        /// TEST 12: Error Scenario - Hash Mismatch
        /// Verify error handling during validation
        [Fact]
        public void Configuration_Error_Scenario_HashMismatch()
        {
            // Arrange
            var config = new Configuration();
            config.version = "2024";
            config.languages = new[] { "en-US" };
            config.excludedApps = new string[] { };

            // Act - Simulate hash mismatch error
            config.errorResult = new Configuration.ErrorResult
            {
                code = "OFF-SECURITY-101",
                message = "Hash verification failed",
                technicalDetails = "Downloaded file hash does not match expected hash"
            };
            config.state = "VALIDATE"; // Still in validation state

            // Assert
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-SECURITY-101", config.errorResult.code);
            Assert.Equal("VALIDATE", config.state); // State unchanged
        }

        /// TEST 13: Rollback Scenario
        /// Verify configuration state during rollback
        [Fact]
        public void Configuration_Rollback_Scenario()
        {
            // Arrange
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new[] { "Teams" },
                configPath = "C:\\config.xml",
                validationPassed = true,
                odtPath = "C:\\Office\\setup.exe",
                state = "INSTALLING"
            };

            // Act - Simulate installation failure and rollback
            config.errorResult = new Configuration.ErrorResult
            {
                code = "OFF-INSTALL-401",
                message = "setup.exe failed",
                technicalDetails = "Exit code: 1"
            };
            config.state = "INSTALL_FAILED";

            // Simulate successful rollback
            config.configPath = null; // Cleared for potential retry
            config.validationPassed = false; // Reset for retry
            config.state = "ROLLED_BACK";

            // Assert
            Assert.Null(config.configPath);
            Assert.False(config.validationPassed);
            Assert.Equal("ROLLED_BACK", config.state);
            Assert.NotNull(config.errorResult); // Error kept for logging
        }
    }
}
