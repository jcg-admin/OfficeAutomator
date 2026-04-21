using System;
using OfficeAutomator.Core.Models;
using OfficeAutomator.Core.Error;
using System.Collections.Generic;
using System.Linq;

namespace OfficeAutomator.Core.Services
{
    /// CLASS: VersionSelector
    /// 
    /// Purpose: Implements UC-001 (Version Selection) - Allows user to select
    /// which Microsoft Office version to install (2024, 2021, or 2019).
    /// 
    /// Responsibilities:
    ///   • Provide list of available Office versions
    ///   • Validate user's version selection
    ///   • Store selected version in Configuration
    ///   • Create error if selection invalid
    /// 
    /// Single Responsibility: Version validation and selection
    /// 
    /// UC-001 Workflow:
    ///   1. StateMachine transitions to SELECT_VERSION
    ///   2. UI displays available versions (from GetVersionOptions())
    ///   3. User clicks a version
    ///   4. UI calls Execute() with selected version
    ///   5. VersionSelector validates and updates Configuration
    ///   6. StateMachine transitions to SELECT_LANGUAGE
    /// 
    /// Valid Versions:
    ///   • 2024 (Latest, recommended)
    ///   • 2021 (LTS - Long-term support)
    ///   • 2019 (Legacy, support ending)
    /// 
    /// Error Codes:
    ///   • OFF-CONFIG-001: Invalid or unavailable version
    /// 
    /// Property Ownership:
    ///   • Writes: config.version (first and only write)
    ///   • Reads: None
    ///   • Updates error: config.errorResult (on failure)
    /// 
    /// Reference: T-022 (UC-001 State Flows), T-028 (Config Lifecycle)
    public class VersionSelector
    {
        // ===== FIELDS =====

        /// List of valid Office versions
        private readonly List<string> validVersions;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: VersionSelector()
        /// 
        /// Initializes version selector with list of valid versions.
        /// 
        /// Post-condition:
        ///   • validVersions populated with [2024, 2021, 2019]
        ///   • Ready to validate selections
        public VersionSelector()
        {
            validVersions = new List<string> { "2024", "2021", "2019" };
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: GetVersionOptions() → List<string>
        /// 
        /// Returns list of available Office versions for UI display.
        /// Called by UI to populate version selection dropdown/buttons.
        /// 
        /// Returns:
        ///   List of version strings: ["2024", "2021", "2019"]
        ///   (always in descending order - newest first)
        /// 
        /// Post-condition:
        ///   • Returns new list (caller can modify without affecting selector)
        ///   • Never null
        ///   • Always contains exactly 3 versions
        /// 
        /// Example:
        ///   var versions = selector.GetVersionOptions();
        ///   // Returns: ["2024", "2021", "2019"]
        ///   // UI displays as radio buttons or dropdown
        /// 
        /// Reference: T-022 (UC-001 Display)
        public List<string> GetVersionOptions()
        {
            return new List<string>(validVersions); // Return copy
        }

        /// METHOD: IsValidVersion(string version) → bool
        /// 
        /// Validates if the given version is supported.
        /// Called to check user input before processing.
        /// 
        /// Parameters:
        ///   version: Version string to validate (e.g., "2024")
        /// 
        /// Returns:
        ///   true if version is valid and supported, false otherwise
        /// 
        /// Validation rules:
        ///   • Not null
        ///   • Not empty or whitespace
        ///   • Exact match (case-sensitive)
        ///   • In valid versions list
        /// 
        /// Pre-condition:
        ///   • version may be null or any string
        /// 
        /// Post-condition:
        ///   • No side effects (pure function)
        ///   • Does not modify Configuration or error state
        /// 
        /// Examples:
        ///   IsValidVersion("2024") → true
        ///   IsValidVersion("2021") → true
        ///   IsValidVersion("2025") → false
        ///   IsValidVersion("2024 ") → false (trailing space)
        ///   IsValidVersion(null) → false
        ///   IsValidVersion("") → false
        /// 
        /// Reference: T-022 (UC-001 Validation)
        public bool IsValidVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            return validVersions.Contains(version);
        }

        /// METHOD: Execute(Configuration config, string selectedVersion, ErrorHandler handler) → bool
        /// 
        /// Main UC-001 workflow: User selected a version, validate and store.
        /// 
        /// Parameters:
        ///   config: Configuration object to update with selection
        ///   selectedVersion: Version selected by user (e.g., "2024")
        ///   handler: ErrorHandler for creating error objects
        /// 
        /// Returns:
        ///   true if selection valid and stored successfully
        ///   false if selection invalid (error created)
        /// 
        /// Pre-conditions:
        ///   • config is not null
        ///   • handler is not null
        ///   • config.state is "SELECT_VERSION" (not enforced, but expected)
        /// 
        /// Post-conditions (if true):
        ///   • config.version = selectedVersion
        ///   • config.errorResult = null
        ///   • Ready for UC-002 (LanguageSelector)
        /// 
        /// Post-conditions (if false):
        ///   • config.version = null (unchanged)
        ///   • config.errorResult = ErrorResult with OFF-CONFIG-001
        ///   • User must retry with valid version
        /// 
        /// Error Handling:
        ///   • Invalid version → OFF-CONFIG-001
        ///   • User message: Clear explanation of valid versions
        ///   • Technical details: Version received, valid options
        /// 
        /// Side Effects:
        ///   • Updates config.version (if valid)
        ///   • Updates config.errorResult (if invalid)
        ///   • DOES NOT change config.state (state machine handles)
        /// 
        /// Example (success):
        ///   bool result = selector.Execute(config, "2024", handler);
        ///   if (result) {
        ///       Assert(config.version == "2024");
        ///       Assert(config.errorResult == null);
        ///   }
        /// 
        /// Example (failure):
        ///   bool result = selector.Execute(config, "2025", handler);
        ///   if (!result) {
        ///       Assert(config.version == null);
        ///       Assert(config.errorResult.code == "OFF-CONFIG-001");
        ///       // UI shows error message, allows retry
        ///   }
        /// 
        /// Reference: T-022 (UC-001 Complete), T-028 (Config Update)
        public bool Execute(Configuration config, string selectedVersion, ErrorHandler handler)
        {
            if (config == null || handler == null)
            {
                return false; // Invalid parameters
            }

            // Validate user selection
            if (!IsValidVersion(selectedVersion))
            {
                // Create error
                config.errorResult = handler.CreateError(
                    "OFF-CONFIG-001",
                    "The Office version you selected is not available. Choose from: 2024 (latest), 2021 (LTS), or 2019 (legacy).",
                    $"Invalid version selected: '{selectedVersion}'. Valid options: {string.Join(", ", validVersions)}"
                );
                return false;
            }

            // Valid selection - store in config
            config.version = selectedVersion;
            config.errorResult = null; // Clear any previous errors
            return true;
        }

        // ===== HELPER METHODS =====

        /// METHOD: GetVersionDescription(string version) → string
        /// 
        /// Returns user-friendly description of the version.
        /// Used by UI to show version details.
        /// 
        /// Parameters:
        ///   version: Version string (e.g., "2024")
        /// 
        /// Returns:
        ///   Description string (e.g., "Office 2024 - Latest (recommended)")
        /// 
        /// Example:
        ///   GetVersionDescription("2024") → "Office 2024 - Latest (recommended)"
        ///   GetVersionDescription("2021") → "Office 2021 - Long-term support"
        ///   GetVersionDescription("2019") → "Office 2019 - Legacy (support ending)"
        /// 
        /// Reference: T-022 (UC-001 UI Display)
        public string GetVersionDescription(string version)
        {
            return version switch
            {
                "2024" => "Office 2024 - Latest (recommended)",
                "2021" => "Office 2021 - Long-term support",
                "2019" => "Office 2019 - Legacy (support ending)",
                _ => "Unknown version"
            };
        }
    }
}
