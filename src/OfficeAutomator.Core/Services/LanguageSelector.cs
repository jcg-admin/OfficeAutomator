using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeAutomator.Core.Services
{
    /// CLASS: LanguageSelector
    /// 
    /// Purpose: Implements UC-002 (Language Selection) - Allows user to select
    /// which language(s) to install Office in (English US, Spanish Mexico, or both).
    /// 
    /// Responsibilities:
    ///   • Provide list of available Office languages
    ///   • Validate user's language selection(s)
    ///   • Support single or multiple language selection
    ///   • Store selected languages in Configuration
    ///   • Create error if selection invalid
    /// 
    /// UC-002 Workflow:
    ///   1. StateMachine transitions to SELECT_LANGUAGE
    ///   2. UI displays available languages (from GetLanguageOptions())
    ///   3. User selects one or more languages (checkboxes)
    ///   4. UI calls Execute() with selected language array
    ///   5. LanguageSelector validates and updates Configuration
    ///   6. StateMachine transitions to SELECT_APPS
    /// 
    /// Valid Languages:
    ///   • en-US (English - United States)
    ///   • es-MX (Spanish - Mexico)
    ///   Can select one or both
    /// 
    /// Error Codes:
    ///   • OFF-CONFIG-002: Invalid or unsupported language selection
    /// 
    /// Property Ownership:
    ///   • Writes: config.languages (first and only write)
    ///   • Reads: None
    ///   • Updates error: config.errorResult (on failure)
    /// 
    /// Reference: T-022 (UC-002 State Flows), T-028 (Config Lifecycle)
    public class LanguageSelector
    {
        // ===== FIELDS =====

        /// List of valid languages
        private readonly List<string> validLanguages;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: LanguageSelector()
        public LanguageSelector()
        {
            validLanguages = new List<string> { "en-US", "es-MX" };
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: GetLanguageOptions() → List<string>
        /// 
        /// Returns list of available languages for UI display.
        /// Called by UI to populate language selection checkboxes/dropdown.
        /// 
        /// Returns:
        ///   List of language codes: ["en-US", "es-MX"]
        /// 
        /// Example:
        ///   var langs = selector.GetLanguageOptions();
        ///   // UI displays as checkboxes:
        ///   // ☐ English (US)
        ///   // ☐ Spanish (Mexico)
        public List<string> GetLanguageOptions()
        {
            return new List<string>(validLanguages); // Return copy
        }

        /// METHOD: IsValidLanguageSelection(string[] languages) → bool
        /// 
        /// Validates if the given language selection is valid.
        /// 
        /// Parameters:
        ///   languages: Array of language codes (e.g., ["en-US"] or ["en-US", "es-MX"])
        /// 
        /// Returns:
        ///   true if selection is valid, false otherwise
        /// 
        /// Validation rules:
        ///   • Not null
        ///   • Not empty (at least one language required)
        ///   • No null elements
        ///   • All languages in valid list
        ///   • Case-sensitive (exact match)
        /// 
        /// Examples:
        ///   IsValidLanguageSelection(["en-US"]) → true
        ///   IsValidLanguageSelection(["es-MX"]) → true
        ///   IsValidLanguageSelection(["en-US", "es-MX"]) → true
        ///   IsValidLanguageSelection(["fr-FR"]) → false (not supported)
        ///   IsValidLanguageSelection([]) → false (empty)
        ///   IsValidLanguageSelection(null) → false
        public bool IsValidLanguageSelection(string[] languages)
        {
            if (languages == null || languages.Length == 0)
            {
                return false;
            }

            // All languages must be valid
            foreach (var lang in languages)
            {
                if (string.IsNullOrWhiteSpace(lang) || !validLanguages.Contains(lang))
                {
                    return false;
                }
            }

            return true;
        }

        /// METHOD: Execute(Configuration config, string[] selectedLanguages, ErrorHandler handler) → bool
        /// 
        /// Main UC-002 workflow: User selected language(s), validate and store.
        /// 
        /// Parameters:
        ///   config: Configuration object to update
        ///   selectedLanguages: Array of languages selected by user
        ///   handler: ErrorHandler for creating error objects
        /// 
        /// Returns:
        ///   true if selection valid and stored successfully
        ///   false if selection invalid (error created)
        /// 
        /// Pre-conditions:
        ///   • config is not null
        ///   • handler is not null
        ///   • config.version is already set (from UC-001)
        /// 
        /// Post-conditions (if true):
        ///   • config.languages = selectedLanguages
        ///   • config.errorResult = null
        ///   • Ready for UC-003 (AppExclusionSelector)
        /// 
        /// Post-conditions (if false):
        ///   • config.languages = null (unchanged)
        ///   • config.errorResult = ErrorResult with OFF-CONFIG-002
        ///   • User must retry with valid language(s)
        /// 
        /// Error Handling:
        ///   • Invalid selection → OFF-CONFIG-002
        ///   • User message: Clear explanation
        ///   • Technical details: Selection received, valid options
        /// 
        /// Example (success):
        ///   bool result = selector.Execute(config, ["en-US", "es-MX"], handler);
        ///   // config.languages = ["en-US", "es-MX"]
        /// 
        /// Example (failure):
        ///   bool result = selector.Execute(config, ["fr-FR"], handler);
        ///   // config.errorResult.code = "OFF-CONFIG-002"
        public bool Execute(Configuration config, string[] selectedLanguages, ErrorHandler handler)
        {
            if (config == null || handler == null)
            {
                return false;
            }

            // Validate selection
            if (!IsValidLanguageSelection(selectedLanguages))
            {
                config.errorResult = handler.CreateError(
                    "OFF-CONFIG-002",
                    "Please select at least one language. Supported languages: English (US), Spanish (Mexico).",
                    $"Invalid language selection: {(selectedLanguages != null ? string.Join(", ", selectedLanguages) : "null")}. Valid: en-US, es-MX"
                );
                return false;
            }

            // Valid selection - store in config
            config.languages = selectedLanguages;
            config.errorResult = null; // Clear any previous errors
            return true;
        }

        // ===== HELPER METHODS =====

        /// METHOD: GetLanguageDescription(string language) → string
        /// 
        /// Returns user-friendly description of the language.
        /// 
        /// Returns:
        ///   Description (e.g., "English - United States")
        public string GetLanguageDescription(string language)
        {
            return language switch
            {
                "en-US" => "English - United States",
                "es-MX" => "Spanish - Mexico",
                _ => "Unknown language"
            };
        }
    }
}
