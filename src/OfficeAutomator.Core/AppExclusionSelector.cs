using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeAutomator.Core
{
    /// CLASS: AppExclusionSelector
    /// 
    /// Purpose: Implements UC-003 (App Exclusion Selection) - Allows user to
    /// exclude optional Office applications from installation.
    /// 
    /// UC-003 Workflow:
    ///   1. StateMachine transitions to SELECT_APPS
    ///   2. UI displays excludable apps (from GetExcludableApps())
    ///   3. User unchecks apps to exclude
    ///   4. UI calls Execute() with excluded apps
    ///   5. AppExclusionSelector validates and updates Configuration
    ///   6. StateMachine transitions to GENERATE_CONFIG
    /// 
    /// Excludable Applications (5 optional components):
    ///   • Teams (Microsoft Teams desktop client)
    ///   • OneDrive (Cloud storage integration)
    ///   • Groove (Music streaming - deprecated)
    ///   • Lync (Legacy communications - Skype replacement)
    ///   • Bing (Search integration)
    /// 
    /// Core Applications (Cannot exclude):
    ///   • Word, Excel, PowerPoint, Outlook (Always installed)
    /// 
    /// Default Behavior:
    ///   • All optional apps installed by default
    ///   • User unchecks to exclude
    /// 
    /// Error Codes:
    ///   • OFF-CONFIG-003: Invalid app selection
    /// 
    /// Property Ownership:
    ///   • Writes: config.excludedApps (first and only write)
    /// 
    /// Reference: T-023 (UC-003 State & XML Design), T-028 (Config Lifecycle)
    public class AppExclusionSelector
    {
        // ===== FIELDS =====

        /// List of apps that can be excluded
        private readonly string[] excludableApps;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: AppExclusionSelector()
        public AppExclusionSelector()
        {
            excludableApps = new[] { "Teams", "OneDrive", "Groove", "Lync", "Bing" };
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: GetExcludableApps() → string[]
        /// 
        /// Returns list of applications that can be excluded from installation.
        /// Called by UI to display exclusion options.
        /// 
        /// Returns:
        ///   Array of app names: ["Teams", "OneDrive", "Groove", "Lync", "Bing"]
        /// 
        /// Example:
        ///   var apps = selector.GetExcludableApps();
        ///   // UI displays as checkboxes:
        ///   // ☑ Teams
        ///   // ☑ OneDrive
        ///   // ☑ Groove
        ///   // ☑ Lync
        ///   // ☑ Bing
        ///   // User unchecks to exclude
        public string[] GetExcludableApps()
        {
            return (string[])excludableApps.Clone(); // Return copy
        }

        /// METHOD: IsValidExclusionSet(string[] apps) → bool
        /// 
        /// Validates if the given exclusion set is valid.
        /// 
        /// Parameters:
        ///   apps: Array of app names to exclude (can be empty for no exclusions)
        /// 
        /// Returns:
        ///   true if all apps are excludable, false if any invalid
        /// 
        /// Validation rules:
        ///   • Can be null (treated as false)
        ///   • Empty array is valid (exclude nothing)
        ///   • All apps must be in excludable list
        ///   • No null elements
        ///   • Case-sensitive
        /// 
        /// Examples:
        ///   IsValidExclusionSet([]) → true (exclude nothing)
        ///   IsValidExclusionSet(["Teams"]) → true
        ///   IsValidExclusionSet(["Teams", "OneDrive"]) → true
        ///   IsValidExclusionSet(["Word"]) → false (not excludable)
        ///   IsValidExclusionSet(null) → false
        public bool IsValidExclusionSet(string[] apps)
        {
            if (apps == null)
            {
                return false;
            }

            // Empty is valid (exclude nothing)
            if (apps.Length == 0)
            {
                return true;
            }

            // All apps must be excludable
            foreach (var app in apps)
            {
                if (string.IsNullOrWhiteSpace(app) || !excludableApps.Contains(app))
                {
                    return false;
                }
            }

            return true;
        }

        /// METHOD: Execute(Configuration config, string[] applicationsToExclude, ErrorHandler handler) → bool
        /// 
        /// Main UC-003 workflow: User selected apps to exclude, validate and store.
        /// 
        /// Parameters:
        ///   config: Configuration object to update
        ///   applicationsToExclude: Array of app names to exclude
        ///   handler: ErrorHandler for creating error objects
        /// 
        /// Returns:
        ///   true if selection valid and stored successfully
        ///   false if selection invalid (error created)
        /// 
        /// Pre-conditions:
        ///   • config is not null
        ///   • handler is not null
        ///   • config.version and config.languages already set
        /// 
        /// Post-conditions (if true):
        ///   • config.excludedApps = applicationsToExclude
        ///   • config.errorResult = null
        ///   • Ready for UC-004 (ConfigGenerator)
        /// 
        /// Post-conditions (if false):
        ///   • config.excludedApps = null (unchanged)
        ///   • config.errorResult = ErrorResult with OFF-CONFIG-003
        ///   • User must retry with valid apps
        /// 
        /// Special case:
        ///   • Empty array is valid (exclude nothing)
        ///   • User can proceed with all apps installed
        /// 
        /// Example (no exclusions):
        ///   selector.Execute(config, [], handler);
        ///   // config.excludedApps = []
        ///   // All Office apps installed
        /// 
        /// Example (some exclusions):
        ///   selector.Execute(config, ["Teams", "OneDrive"], handler);
        ///   // config.excludedApps = ["Teams", "OneDrive"]
        ///   // Only those apps excluded
        public bool Execute(Configuration config, string[] applicationsToExclude, ErrorHandler handler)
        {
            if (config == null || handler == null)
            {
                return false;
            }

            // Validate exclusion set
            if (!IsValidExclusionSet(applicationsToExclude))
            {
                config.errorResult = handler.CreateError(
                    "OFF-CONFIG-003",
                    "Invalid application selection. Please select only from the available applications.",
                    $"Invalid apps to exclude: {(applicationsToExclude != null ? string.Join(", ", applicationsToExclude) : "null")}. Valid: {string.Join(", ", excludableApps)}"
                );
                return false;
            }

            // Valid selection - store in config
            config.excludedApps = applicationsToExclude ?? new string[] { };
            config.errorResult = null; // Clear any previous errors
            return true;
        }

        // ===== HELPER METHODS =====

        /// METHOD: GetAppDescription(string app) → string
        /// 
        /// Returns user-friendly description of the app.
        /// 
        /// Returns:
        ///   Description (e.g., "Microsoft Teams - Collaboration tool")
        public string GetAppDescription(string app)
        {
            return app switch
            {
                "Teams" => "Microsoft Teams - Collaboration and messaging",
                "OneDrive" => "OneDrive - Cloud storage integration",
                "Groove" => "Groove Music - Music streaming (legacy)",
                "Lync" => "Lync/Skype - Communications (legacy)",
                "Bing" => "Bing Search - Search engine integration",
                _ => "Unknown application"
            };
        }

        /// METHOD: IsAppExcluded(string[] excludedApps, string appName) → bool
        /// 
        /// Checks if a specific app is in the exclusion list.
        /// Helper for configuration generation.
        public bool IsAppExcluded(string[] excludedApps, string appName)
        {
            if (excludedApps == null || string.IsNullOrWhiteSpace(appName))
            {
                return false;
            }

            return excludedApps.Contains(appName);
        }
    }
}
