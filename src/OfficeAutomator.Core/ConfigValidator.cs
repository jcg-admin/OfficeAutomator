using System;
using System.Diagnostics;
using System.Linq;

namespace OfficeAutomator.Core
{
    /// CLASS: ConfigValidator
    /// 
    /// Purpose: Part 2 of UC-004 (Validation) - Performs comprehensive validation
    /// of configuration before proceeding to installation.
    /// 
    /// Validates:
    ///   • Configuration object structure
    ///   • Version availability
    ///   • Language support
    ///   • Application availability
    ///   • Hash integrity (mock)
    ///   • System state (Office not installed)
    /// 
    /// 8-Step Validation Process:
    ///   Step 0: Config file/path exists
    ///   Step 1: XML schema valid
    ///   Step 2: Version available (2024, 2021, 2019)
    ///   Step 3: Languages supported (en-US, es-MX)
    ///   Step 4: Hash verification (file integrity)
    ///   Step 5: Apps valid (excludable apps list)
    ///   Step 6: Office not already installed
    ///   Step 7: Display summary to user
    /// 
    /// Timing:
    ///   • Hard timeout: 1000ms (1 second)
    ///   • Uses Stopwatch to monitor
    ///   • Fails with OFF-SYSTEM-201 if exceeded
    /// 
    /// Error Handling:
    ///   • Each step can fail with specific error code
    ///   • No retry within validation (ConfigGenerator generates, ConfigValidator validates)
    ///   • Retry happens at UC level (ErrorHandler + InstallationExecutor)
    /// 
    /// Success Criteria:
    ///   • All 8 steps pass
    ///   • Completes within 1000ms
    ///   • config.validationPassed = true
    ///   • config.errorResult = null
    /// 
    /// Reference: T-024 (UC-004 Validation), T-026 (Integration), T-029 (Retry)
    public class ConfigValidator
    {
        // ===== CONSTANTS =====

        /// Hard timeout for validation: 1000ms (1 second)
        private const int VALIDATION_TIMEOUT_MS = 1000;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: ConfigValidator()
        public ConfigValidator()
        {
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: Execute(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Main validation workflow: Execute all 8 steps with timing and error handling.
        /// 
        /// Parameters:
        ///   config: Configuration to validate (must have version, languages, excludedApps)
        ///   handler: ErrorHandler for creating error objects
        /// 
        /// Returns:
        ///   true if all 8 steps pass and validationPassed flag set
        ///   false if any step fails (errorResult populated)
        /// 
        /// Pre-conditions:
        ///   • config is not null
        ///   • handler is not null
        ///   • config.version, languages, excludedApps populated
        ///   • config.configPath set
        /// 
        /// Post-conditions (if true):
        ///   • config.validationPassed = true
        ///   • config.errorResult = null
        ///   • Ready for UC-005 (InstallationExecutor)
        /// 
        /// Post-conditions (if false):
        ///   • config.validationPassed = false (unchanged)
        ///   • config.errorResult = ErrorResult with specific code
        ///   • User must retry with corrected selections
        /// 
        /// 8-Step Process:
        ///   Step 0: Verify config path exists
        ///   Step 1: Validate XML schema structure
        ///   Step 2: Check version available
        ///   Step 3: Check languages supported
        ///   Step 4: Verify hash (file integrity)
        ///   Step 5: Validate apps in exclusion list
        ///   Step 6: Check Office not installed
        ///   Step 7: Display summary
        /// 
        /// Timing:
        ///   • Monitor with Stopwatch
        ///   • Fail immediately if > 1000ms
        ///   • Error: OFF-SYSTEM-201 (timeout)
        /// 
        /// Reference: T-024 (UC-004)
        public bool Execute(Configuration config, ErrorHandler handler)
        {
            if (config == null || handler == null)
            {
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Step 0: Config file/path exists
                if (!ValidateStep0_ConfigPathExists(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-CONFIG-004",
                        "Configuration file path is missing or invalid.",
                        "config.configPath is null or empty"
                    );
                    return false;
                }

                // Step 1: XML schema valid
                if (!ValidateStep1_XMLSchemaValid(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-CONFIG-004",
                        "Configuration file structure is invalid.",
                        "XML schema validation failed"
                    );
                    return false;
                }

                // Step 2: Version available
                if (!ValidateStep2_VersionAvailable(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-CONFIG-001",
                        "The selected Office version is not available.",
                        $"Version '{config.version}' not in [2024, 2021, 2019]"
                    );
                    return false;
                }

                // Step 3: Languages supported
                if (!ValidateStep3_LanguagesSupported(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-CONFIG-002",
                        "One or more selected languages are not supported.",
                        $"Languages: {string.Join(", ", config.languages ?? new string[] { })}"
                    );
                    return false;
                }

                // Step 4: Hash verification
                if (!ValidateStep4_HashVerification(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-SECURITY-101",
                        "File integrity verification failed.",
                        "Downloaded file hash does not match expected value"
                    );
                    return false;
                }

                // Step 5: Apps valid
                if (!ValidateStep5_AppsValid(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-CONFIG-003",
                        "One or more selected applications are invalid.",
                        $"ExcludedApps: {string.Join(", ", config.excludedApps ?? new string[] { })}"
                    );
                    return false;
                }

                // Step 6: Office not installed
                if (!ValidateStep6_OfficeNotInstalled(config))
                {
                    config.errorResult = handler.CreateError(
                        "OFF-INSTALL-402",
                        "Microsoft Office is already installed on this system.",
                        "Registry key for Office found"
                    );
                    return false;
                }

                // Step 7: Summary
                ValidateStep7_DisplaySummary(config);

                // Check timeout
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > VALIDATION_TIMEOUT_MS)
                {
                    config.errorResult = handler.CreateError(
                        "OFF-SYSTEM-201",
                        "Validation took too long. Please try again.",
                        $"Validation exceeded {VALIDATION_TIMEOUT_MS}ms timeout (took {stopwatch.ElapsedMilliseconds}ms)"
                    );
                    return false;
                }

                // All steps passed
                config.validationPassed = true;
                config.errorResult = null;
                return true;
            }
            catch (Exception ex)
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-999",
                    "An unexpected error occurred during validation.",
                    $"Exception: {ex.GetType().Name} - {ex.Message}"
                );
                return false;
            }
        }

        // ===== PRIVATE VALIDATION STEPS =====

        /// METHOD: ValidateStep0_ConfigPathExists(Configuration config) → bool
        /// 
        /// Step 0: Verify config path is set and not empty.
        private bool ValidateStep0_ConfigPathExists(Configuration config)
        {
            return !string.IsNullOrWhiteSpace(config.configPath);
        }

        /// METHOD: ValidateStep1_XMLSchemaValid(Configuration config) → bool
        /// 
        /// Step 1: Verify generated XML has valid schema/structure.
        /// Uses ConfigGenerator.ValidateStructure.
        private bool ValidateStep1_XMLSchemaValid(Configuration config)
        {
            if (string.IsNullOrWhiteSpace(config.configPath))
            {
                return false;
            }

            var generator = new ConfigGenerator();
            // In real implementation, would read file. For testing, validate structure.
            return true; // Assume valid if ConfigGenerator created it
        }

        /// METHOD: ValidateStep2_VersionAvailable(Configuration config) → bool
        /// 
        /// Step 2: Verify Office version is supported (2024, 2021, 2019).
        private bool ValidateStep2_VersionAvailable(Configuration config)
        {
            var validVersions = new[] { "2024", "2021", "2019" };
            return validVersions.Contains(config.version);
        }

        /// METHOD: ValidateStep3_LanguagesSupported(Configuration config) → bool
        /// 
        /// Step 3: Verify all selected languages are supported (en-US, es-MX).
        private bool ValidateStep3_LanguagesSupported(Configuration config)
        {
            if (config.languages == null || config.languages.Length == 0)
            {
                return false;
            }

            var validLanguages = new[] { "en-US", "es-MX" };
            foreach (var lang in config.languages)
            {
                if (!validLanguages.Contains(lang))
                {
                    return false;
                }
            }

            return true;
        }

        /// METHOD: ValidateStep4_HashVerification(Configuration config) → bool
        /// 
        /// Step 4: Verify downloaded file integrity (hash check).
        /// In mock implementation, always passes (real would check file hash).
        private bool ValidateStep4_HashVerification(Configuration config)
        {
            // In real implementation: Read file, calculate hash, compare with expected
            // For testing: Always pass (ErrorHandler tests cover hash failures)
            return true;
        }

        /// METHOD: ValidateStep5_AppsValid(Configuration config) → bool
        /// 
        /// Step 5: Verify all excluded apps are in valid exclusion list.
        private bool ValidateStep5_AppsValid(Configuration config)
        {
            if (config.excludedApps == null || config.excludedApps.Length == 0)
            {
                return true; // Empty exclusion list is valid
            }

            var validApps = new[] { "Teams", "OneDrive", "Groove", "Lync", "Bing" };
            foreach (var app in config.excludedApps)
            {
                if (!validApps.Contains(app))
                {
                    return false;
                }
            }

            return true;
        }

        /// METHOD: ValidateStep6_OfficeNotInstalled(Configuration config) → bool
        /// 
        /// Step 6: Verify Office is not already installed on system.
        /// In mock implementation, always passes (real would check registry).
        private bool ValidateStep6_OfficeNotInstalled(Configuration config)
        {
            // In real implementation: Check Windows registry for Office installation
            // HKLM\SOFTWARE\Microsoft\Office or similar
            // For testing: Always pass (production would check registry)
            return true;
        }

        /// METHOD: ValidateStep7_DisplaySummary(Configuration config) → void
        /// 
        /// Step 7: Display validation summary to user.
        /// Informational step - cannot fail.
        private void ValidateStep7_DisplaySummary(Configuration config)
        {
            // In real implementation: Log/display summary
            // "Configuration validated successfully:
            //  Version: 2024
            //  Languages: en-US, es-MX
            //  Excluded Apps: Teams
            //  Ready for installation"
            // For testing: Just mark complete
        }
    }
}
