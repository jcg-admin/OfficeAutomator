using Xunit;
using OfficeAutomator.Core.Validation;
using OfficeAutomator.Core.Models;
using OfficeAutomator.Core.Error;
using OfficeAutomator.Core.Services;
using System;
using System.Diagnostics;
using System.Linq;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: ConfigValidatorTests
    /// Purpose: TDD tests for 8-step validation (UC-004)
    /// Reference: T-024 (UC-004 Validation Design), T-026 (Integration)
    public class ConfigValidatorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void ConfigValidator_Initializes_Successfully()
        {
            var validator = new ConfigValidator();
            Assert.NotNull(validator);
        }

        /// TEST 2: Happy Path - All Steps Pass
        /// Complete validation workflow with valid config
        [Fact]
        public void ConfigValidator_Execute_Happy_Path_All_Steps_Pass()
        {
            // Arrange
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            // Pre-step: Generate valid config
            string xmlContent = generator.GenerateConfigXml(config);
            config.configPath = generator.GetConfigFilePath();

            var handler = new ErrorHandler();

            // Act
            bool result = validator.Execute(config, handler);

            // Assert
            Assert.True(result);
            Assert.True(config.validationPassed);
            Assert.Null(config.errorResult);
        }

        /// TEST 3: Step 0 - Config File Exists
        [Fact]
        public void ConfigValidator_Step0_ConfigFileExists()
        {
            var validator = new ConfigValidator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            var handler = new ErrorHandler();

            // Config without configPath should fail
            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
            // Should fail at Step 0
        }

        /// TEST 4: Step 1 - XML Schema Valid
        [Fact]
        public void ConfigValidator_Step1_XMLSchemaValid()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            config.configPath = generator.GetConfigFilePath();
            string xml = generator.GenerateConfigXml(config);
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.True(result);
        }

        /// TEST 5: Step 2 - Version Available
        [Fact]
        public void ConfigValidator_Step2_VersionAvailable_2024()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 6: Step 2 - Version Available 2021
        [Fact]
        public void ConfigValidator_Step2_VersionAvailable_2021()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2021", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 7: Step 2 - Version Available 2019
        [Fact]
        public void ConfigValidator_Step2_VersionAvailable_2019()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2019", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 8: Step 3 - Languages Supported
        [Fact]
        public void ConfigValidator_Step3_LanguagesSupported()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US", "es-MX" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 9: Step 4 - Hash Verification (Mock Success)
        /// Hash verification would normally be transient-retryable
        [Fact]
        public void ConfigValidator_Step4_HashVerification()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 10: Step 5 - Apps Valid
        [Fact]
        public void ConfigValidator_Step5_AppsValid()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new[] { "Teams" } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 11: Step 6 - Office Not Installed
        [Fact]
        public void ConfigValidator_Step6_OfficeNotInstalled()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 12: Step 7 - Summary Display
        [Fact]
        public void ConfigValidator_Step7_SummaryDisplay()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);
            Assert.True(result);
        }

        /// TEST 13: Validation Passes All Steps
        [Fact]
        public void ConfigValidator_Execute_Sets_ValidationPassed_Flag()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();

            Assert.False(config.validationPassed); // Initially false

            var handler = new ErrorHandler();
            validator.Execute(config, handler);

            Assert.True(config.validationPassed); // Set to true after success
        }

        /// TEST 14: Error - Invalid Version
        [Fact]
        public void ConfigValidator_Error_InvalidVersion()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2025", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 15: Error - Invalid Language
        [Fact]
        public void ConfigValidator_Error_InvalidLanguage()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2024", languages = new[] { "fr-FR" }, excludedApps = new string[] { } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 16: Error - Invalid App
        [Fact]
        public void ConfigValidator_Error_InvalidApp()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new[] { "Word" } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 17: Timeout Handling - Must Complete < 1000ms
        [Fact]
        public void ConfigValidator_Execute_Completes_Under_Timeout()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            var stopwatch = Stopwatch.StartNew();
            validator.Execute(config, handler);
            stopwatch.Stop();

            // Should complete in < 1000ms
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Validation took {stopwatch.ElapsedMilliseconds}ms (should be < 1000ms)");
        }

        /// TEST 18: Empty Languages Array
        [Fact]
        public void ConfigValidator_Error_EmptyLanguages()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2024", languages = new string[] { }, excludedApps = new string[] { } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.False(result);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 19: Multiple Languages Valid
        [Fact]
        public void ConfigValidator_Execute_Multiple_Languages_Valid()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US", "es-MX" }, excludedApps = new string[] { } };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.True(result);
        }

        /// TEST 20: All Apps Excluded Valid
        [Fact]
        public void ConfigValidator_Execute_All_Apps_Excluded_Valid()
        {
            var validator = new ConfigValidator();
            var generator = new ConfigGenerator();
            var config = new Configuration 
            { 
                version = "2024", 
                languages = new[] { "en-US" }, 
                excludedApps = new[] { "Teams", "OneDrive", "Groove", "Lync", "Bing" } 
            };
            config.configPath = generator.GetConfigFilePath();
            var handler = new ErrorHandler();

            bool result = validator.Execute(config, handler);

            Assert.True(result);
        }

        /// TEST 21: Complete UC-004 Validation Workflow
        [Fact]
        public void ConfigValidator_Complete_UC004_Workflow()
        {
            // Arrange - Simulate complete UC-001 through UC-004
            var versionSelector = new VersionSelector();
            var languageSelector = new LanguageSelector();
            var appSelector = new AppExclusionSelector();
            var generator = new ConfigGenerator();
            var validator = new ConfigValidator();
            var handler = new ErrorHandler();

            var config = new Configuration();

            // UC-001: Version Selection
            versionSelector.Execute(config, "2024", handler);
            Assert.Equal("2024", config.version);

            // UC-002: Language Selection
            languageSelector.Execute(config, new[] { "en-US" }, handler);
            Assert.Single(config.languages);

            // UC-003: App Exclusion
            appSelector.Execute(config, new[] { "Teams" }, handler);
            Assert.Single(config.excludedApps);

            // UC-004: Validation
            config.configPath = generator.GetConfigFilePath();
            bool result = validator.Execute(config, handler);

            Assert.True(result);
            Assert.True(config.validationPassed);
            Assert.Null(config.errorResult);
        }

        /// TEST 22: Error Result Contains Code
        [Fact]
        public void ConfigValidator_Error_Result_Contains_Error_Code()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2025", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            validator.Execute(config, handler);

            Assert.NotNull(config.errorResult);
            Assert.NotNull(config.errorResult.code);
            Assert.NotEmpty(config.errorResult.code);
        }

        /// TEST 23: Null Config Handling
        [Fact]
        public void ConfigValidator_Execute_Null_Config()
        {
            var validator = new ConfigValidator();
            var handler = new ErrorHandler();

            // Should handle gracefully
            bool result = validator.Execute(null, handler);

            Assert.False(result);
        }

        /// TEST 24: Null Handler Handling
        [Fact]
        public void ConfigValidator_Execute_Null_Handler()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };

            bool result = validator.Execute(config, null);

            Assert.False(result);
        }

        /// TEST 25: Validation Flag Not Set on Error
        [Fact]
        public void ConfigValidator_ValidationPassed_False_On_Error()
        {
            var validator = new ConfigValidator();
            var config = new Configuration { version = "2025", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            config.configPath = "somepath.xml";
            var handler = new ErrorHandler();

            validator.Execute(config, handler);

            Assert.False(config.validationPassed); // Should remain false
        }
    }
}
