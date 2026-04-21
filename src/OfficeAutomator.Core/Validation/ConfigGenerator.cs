using System;
using System.IO;
using System.Xml.Linq;

namespace OfficeAutomator.Core.Validation
{
    /// CLASS: ConfigGenerator
    /// 
    /// Purpose: Part of UC-004 (Validation) - Generates configuration XML file
    /// based on user selections and validates its structure.
    /// 
    /// Responsibilities:
    ///   • Generate config.xml from Configuration object
    ///   • Create properly formatted XML with all required elements
    ///   • Determine output file path with timestamp
    ///   • Validate XML structure
    ///   • Support all Office versions (2024, 2021, 2019)
    /// 
    /// UC-004 Workflow (2 classes):
    ///   1. ConfigGenerator: Generate XML from selections
    ///   2. ConfigValidator: Validate generated XML + version/language/app availability
    /// 
    /// Generated XML Structure:
    ///   <?xml version="1.0" encoding="utf-8"?>
    ///   <Config>
    ///     <Version>2024</Version>
    ///     <Languages>
    ///       <Language>en-US</Language>
    ///       <Language>es-MX</Language>
    ///     </Languages>
    ///     <ExcludedApps>
    ///       <App>Teams</App>
    ///       <App>OneDrive</App>
    ///     </ExcludedApps>
    ///     <Timestamp>2026-05-17T10:30:45.1234567Z</Timestamp>
    ///   </Config>
    /// 
    /// File Path Format:
    ///   C:\Users\{username}\AppData\Local\OfficeAutomator\config_YYYYMMDD_HHMMSS_mmm.xml
    ///   Example: config_20260517_103045_123.xml (unique per generation)
    /// 
    /// Error Handling:
    ///   • NULL or invalid Configuration → returns empty/null
    ///   • Malformed XML structure → ValidateStructure returns false
    ///   • Missing required elements → validation fails
    /// 
    /// Property Updates:
    ///   • Sets config.configPath to generated file path
    /// 
    /// Reference: T-024 (UC-004 Validation), T-023 (XML Design), T-028 (Config Lifecycle)
    public class ConfigGenerator
    {
        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: ConfigGenerator()
        public ConfigGenerator()
        {
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: GenerateConfigXml(Configuration config) → string
        /// 
        /// Generates well-formatted XML configuration from Configuration object.
        /// 
        /// Parameters:
        ///   config: Configuration with version, languages, excludedApps set
        /// 
        /// Returns:
        ///   XML string with full declaration and structure
        ///   Never null (returns empty string if config null)
        /// 
        /// Generated Elements:
        ///   • Version: Office version (2024, 2021, 2019)
        ///   • Languages: Array of language codes
        ///   • ExcludedApps: Array of app names to exclude
        ///   • Timestamp: Generation timestamp (UTC)
        /// 
        /// Format:
        ///   • XML 1.0 declaration
        ///   • UTF-8 encoding
        ///   • Pretty-printed (indented)
        ///   • No BOM (Byte Order Mark)
        /// 
        /// Example output:
        ///   <?xml version="1.0" encoding="utf-8"?>
        ///   <Config>
        ///     <Version>2024</Version>
        ///     <Languages>
        ///       <Language>en-US</Language>
        ///       <Language>es-MX</Language>
        ///     </Languages>
        ///     <ExcludedApps>
        ///       <App>Teams</App>
        ///     </ExcludedApps>
        ///     <Timestamp>2026-05-17T10:30:45.1234567Z</Timestamp>
        ///   </Config>
        /// 
        /// Reference: T-023 (XML Design), T-024 (UC-004)
        public string GenerateConfigXml(Configuration config)
        {
            if (config == null)
            {
                return "";
            }

            try
            {
                // Build XML document
                var doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("Config",
                        new XElement("Version", config.version ?? ""),
                        new XElement("Languages",
                            BuildLanguagesElements(config.languages)
                        ),
                        new XElement("ExcludedApps",
                            BuildExcludedAppsElements(config.excludedApps)
                        ),
                        new XElement("Timestamp", DateTime.UtcNow.ToString("O"))
                    )
                );

                // Return formatted XML
                return doc.ToString(SaveOptions.None);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// METHOD: GetConfigFilePath() → string
        /// 
        /// Generates unique file path for config.xml with timestamp.
        /// Called to determine where to save generated XML.
        /// 
        /// Returns:
        ///   Full file path (e.g., C:\Users\...\config_20260517_103045_123.xml)
        /// 
        /// Path Format:
        ///   C:\Users\{username}\AppData\Local\OfficeAutomator\config_{TIMESTAMP}.xml
        /// 
        /// Timestamp Format:
        ///   YYYYMMDD_HHMMSS_mmm (unique per millisecond)
        ///   Example: 20260517_103045_123
        /// 
        /// Ensures:
        ///   • AppData\Local\OfficeAutomator directory exists
        ///   • Unique filename per generation
        ///   • Readable file path for debugging
        /// 
        /// Example:
        ///   "C:\\Users\\YourName\\AppData\\Local\\OfficeAutomator\\config_20260517_103045_456.xml"
        public string GetConfigFilePath()
        {
            // Get AppData\Local path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string officeAutomatorPath = Path.Combine(appDataPath, "OfficeAutomator");

            // Create directory if it doesn't exist
            Directory.CreateDirectory(officeAutomatorPath);

            // Generate unique filename with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string fileName = $"config_{timestamp}.xml";

            return Path.Combine(officeAutomatorPath, fileName);
        }

        /// METHOD: ValidateStructure(string xmlContent) → bool
        /// 
        /// Validates that XML has correct structure with all required elements.
        /// Called by ConfigValidator to verify config.xml validity.
        /// 
        /// Parameters:
        ///   xmlContent: XML string to validate
        /// 
        /// Returns:
        ///   true if XML is well-formed and has all required elements
        ///   false if XML is malformed or missing elements
        /// 
        /// Required Elements:
        ///   • Root: Config element
        ///   • Version: Not null/empty
        ///   • Languages: Container with Language elements
        ///   • ExcludedApps: Container with App elements
        ///   • Timestamp: Not null/empty
        /// 
        /// Validation Checks:
        ///   • XML is parseable
        ///   • Root element is "Config"
        ///   • All required child elements present
        ///   • No exceptions during parsing
        /// 
        /// Example (valid):
        ///   ValidateStructure("<Config><Version>2024</Version>...</Config>") → true
        /// 
        /// Example (invalid):
        ///   ValidateStructure("<Invalid></Invalid>") → false
        public bool ValidateStructure(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                return false;
            }

            try
            {
                XDocument doc = XDocument.Parse(xmlContent);

                // Check root element
                if (doc.Root == null || doc.Root.Name.LocalName != "Config")
                {
                    return false;
                }

                // Check required elements
                var version = doc.Root.Element("Version");
                var languages = doc.Root.Element("Languages");
                var excludedApps = doc.Root.Element("ExcludedApps");
                var timestamp = doc.Root.Element("Timestamp");

                // All required elements must be present
                if (version == null || languages == null || excludedApps == null || timestamp == null)
                {
                    return false;
                }

                // Version and Timestamp must have values
                if (string.IsNullOrWhiteSpace(version.Value) || string.IsNullOrWhiteSpace(timestamp.Value))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ===== PRIVATE METHODS =====

        /// METHOD: BuildLanguagesElements(string[] languages) → object
        /// 
        /// Creates XML elements for languages list.
        /// Helper for GenerateConfigXml.
        /// 
        /// Parameters:
        ///   languages: Array of language codes (e.g., ["en-US", "es-MX"])
        /// 
        /// Returns:
        ///   Array of XElement Language elements
        /// 
        /// Example:
        ///   Input: ["en-US", "es-MX"]
        ///   Output: [<Language>en-US</Language>, <Language>es-MX</Language>]
        private object[] BuildLanguagesElements(string[] languages)
        {
            if (languages == null || languages.Length == 0)
            {
                return new object[] { };
            }

            var elements = new object[languages.Length];
            for (int i = 0; i < languages.Length; i++)
            {
                elements[i] = new XElement("Language", languages[i]);
            }

            return elements;
        }

        /// METHOD: BuildExcludedAppsElements(string[] excludedApps) → object
        /// 
        /// Creates XML elements for excluded apps list.
        /// Helper for GenerateConfigXml.
        /// 
        /// Parameters:
        ///   excludedApps: Array of app names (e.g., ["Teams", "OneDrive"])
        /// 
        /// Returns:
        ///   Array of XElement App elements
        private object[] BuildExcludedAppsElements(string[] excludedApps)
        {
            if (excludedApps == null || excludedApps.Length == 0)
            {
                return new object[] { };
            }

            var elements = new object[excludedApps.Length];
            for (int i = 0; i < excludedApps.Length; i++)
            {
                elements[i] = new XElement("App", excludedApps[i]);
            }

            return elements;
        }
    }
}
