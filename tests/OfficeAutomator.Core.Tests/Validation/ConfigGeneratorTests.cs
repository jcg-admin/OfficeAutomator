using Xunit;
using OfficeAutomator.Core;
using System;
using System.Xml.Linq;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: ConfigGeneratorTests
    /// Purpose: TDD tests for config XML generation (UC-004)
    /// Reference: T-024 (UC-004 Validation Design), T-023 (UC-003 XML Design)
    public class ConfigGeneratorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void ConfigGenerator_Initializes_Successfully()
        {
            var generator = new ConfigGenerator();
            Assert.NotNull(generator);
        }

        /// TEST 2: Generate Config - Basic Structure
        /// Config XML has correct root and required elements
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Creates_Valid_XML_Structure()
        {
            // Arrange
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            // Act
            string xmlContent = generator.GenerateConfigXml(config);

            // Assert
            Assert.NotNull(xmlContent);
            Assert.NotEmpty(xmlContent);
            Assert.Contains("<?xml", xmlContent); // XML declaration
            Assert.Contains("<Config", xmlContent); // Root element
            Assert.Contains("</Config>", xmlContent);
        }

        /// TEST 3: Parse Generated XML
        /// Can parse as valid XML
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Produces_Parseable_XML()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);

            // Parse should not throw
            XDocument doc = XDocument.Parse(xmlContent);
            Assert.NotNull(doc.Root);
            Assert.Equal("Config", doc.Root.Name.LocalName);
        }

        /// TEST 4: Version Element
        /// Config contains correct version
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Contains_Version()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var versionElement = doc.Root?.Element("Version");
            Assert.NotNull(versionElement);
            Assert.Equal("2024", versionElement.Value);
        }

        /// TEST 5: Languages Element
        /// Config contains all selected languages
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Contains_Languages()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US", "es-MX" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var languagesElement = doc.Root?.Element("Languages");
            Assert.NotNull(languagesElement);
            var langItems = languagesElement.Elements("Language");
            Assert.Equal(2, langItems.Count());
            Assert.Contains(langItems, l => l.Value == "en-US");
            Assert.Contains(langItems, l => l.Value == "es-MX");
        }

        /// TEST 6: ExcludedApps Element
        /// Config contains excluded apps
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Contains_ExcludedApps()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new[] { "Teams", "OneDrive" }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var appsElement = doc.Root?.Element("ExcludedApps");
            Assert.NotNull(appsElement);
            var appItems = appsElement.Elements("App");
            Assert.Equal(2, appItems.Count());
            Assert.Contains(appItems, a => a.Value == "Teams");
            Assert.Contains(appItems, a => a.Value == "OneDrive");
        }

        /// TEST 7: Empty Excluded Apps
        /// ExcludedApps element exists even if empty
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Empty_ExcludedApps()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var appsElement = doc.Root?.Element("ExcludedApps");
            Assert.NotNull(appsElement);
            Assert.Empty(appsElement.Elements("App"));
        }

        /// TEST 8: Timestamp Element
        /// Config includes generation timestamp
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Contains_Timestamp()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var timestampElement = doc.Root?.Element("Timestamp");
            Assert.NotNull(timestampElement);
            Assert.NotEmpty(timestampElement.Value);
            // Verify it's a valid datetime
            Assert.True(DateTime.TryParse(timestampElement.Value, out _));
        }

        /// TEST 9: Configuration File Path
        /// Generates valid file path for config.xml
        [Fact]
        public void ConfigGenerator_GetConfigFilePath_Returns_Valid_Path()
        {
            var generator = new ConfigGenerator();

            string path = generator.GetConfigFilePath();

            Assert.NotNull(path);
            Assert.NotEmpty(path);
            Assert.Contains("config_", path);
            Assert.EndsWith(".xml", path);
            Assert.Contains("OfficeAutomator", path); // Expected folder
        }

        /// TEST 10: Config File Path Timestamp
        /// File path includes timestamp (unique per generation)
        [Fact]
        public void ConfigGenerator_GetConfigFilePath_Includes_Unique_Timestamp()
        {
            var generator = new ConfigGenerator();

            string path1 = generator.GetConfigFilePath();
            System.Threading.Thread.Sleep(100); // Small delay
            string path2 = generator.GetConfigFilePath();

            Assert.NotEqual(path1, path2); // Different timestamps
        }

        /// TEST 11: Validate Structure
        /// Generated XML has all required elements
        [Fact]
        public void ConfigGenerator_ValidateStructure_Returns_True_For_Valid_Config()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);

            bool result = generator.ValidateStructure(xmlContent);

            Assert.True(result);
        }

        /// TEST 12: Validate Structure - Missing Element
        /// Invalid XML fails validation
        [Fact]
        public void ConfigGenerator_ValidateStructure_Returns_False_For_Invalid_XML()
        {
            var generator = new ConfigGenerator();
            string invalidXml = "<?xml version=\"1.0\"?><Invalid></Invalid>";

            bool result = generator.ValidateStructure(invalidXml);

            Assert.False(result);
        }

        /// TEST 13: Validate Structure - Malformed XML
        [Fact]
        public void ConfigGenerator_ValidateStructure_Returns_False_For_Malformed_XML()
        {
            var generator = new ConfigGenerator();
            string malformedXml = "<?xml version=\"1.0\"?><Config><Version>2024";

            bool result = generator.ValidateStructure(malformedXml);

            Assert.False(result);
        }

        /// TEST 14: Single Language
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Single_Language()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2021",
                languages = new[] { "es-MX" },
                excludedApps = new[] { "Teams" }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            Assert.Equal("2021", doc.Root?.Element("Version")?.Value);
            Assert.Single(doc.Root?.Element("Languages")?.Elements("Language") ?? Enumerable.Empty<XElement>());
            Assert.Single(doc.Root?.Element("ExcludedApps")?.Elements("App") ?? Enumerable.Empty<XElement>());
        }

        /// TEST 15: All Excluded Apps
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_All_Apps_Excluded()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new[] { "Teams", "OneDrive", "Groove", "Lync", "Bing" }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            var appElements = doc.Root?.Element("ExcludedApps")?.Elements("App");
            Assert.Equal(5, appElements?.Count());
        }

        /// TEST 16: Complete UC-004 Workflow
        [Fact]
        public void ConfigGenerator_Complete_UC004_Workflow()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new[] { "Teams" }
            };

            // Generate XML
            string xmlContent = generator.GenerateConfigXml(config);
            Assert.NotNull(xmlContent);

            // Validate structure
            bool valid = generator.ValidateStructure(xmlContent);
            Assert.True(valid);

            // Get file path
            string filePath = generator.GetConfigFilePath();
            Assert.NotNull(filePath);

            // Update configuration
            config.configPath = filePath;
            Assert.Equal(filePath, config.configPath);
        }

        /// TEST 17: XML Indentation
        /// Generated XML is nicely formatted
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Is_Formatted()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);

            // Should have line breaks (formatted)
            Assert.Contains("\n", xmlContent);
            // Should have indentation
            Assert.Contains("  ", xmlContent);
        }

        /// TEST 18: Version 2019
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Version_2019()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2019",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            Assert.Equal("2019", doc.Root?.Element("Version")?.Value);
        }

        /// TEST 19: Version 2021
        [Fact]
        public void ConfigGenerator_GenerateConfigXml_Version_2021()
        {
            var generator = new ConfigGenerator();
            var config = new Configuration
            {
                version = "2021",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            string xmlContent = generator.GenerateConfigXml(config);
            XDocument doc = XDocument.Parse(xmlContent);

            Assert.Equal("2021", doc.Root?.Element("Version")?.Value);
        }

        /// TEST 20: Multiple Generations
        /// Can generate multiple configs
        [Fact]
        public void ConfigGenerator_Multiple_Generations()
        {
            var generator = new ConfigGenerator();

            var config1 = new Configuration { version = "2024", languages = new[] { "en-US" }, excludedApps = new string[] { } };
            var config2 = new Configuration { version = "2021", languages = new[] { "es-MX" }, excludedApps = new[] { "Teams" } };

            string xml1 = generator.GenerateConfigXml(config1);
            string xml2 = generator.GenerateConfigXml(config2);

            XDocument doc1 = XDocument.Parse(xml1);
            XDocument doc2 = XDocument.Parse(xml2);

            Assert.Equal("2024", doc1.Root?.Element("Version")?.Value);
            Assert.Equal("2021", doc2.Root?.Element("Version")?.Value);
        }
    }
}
