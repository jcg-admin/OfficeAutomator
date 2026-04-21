using Xunit;
using OfficeAutomator.Core.Services;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: LanguageSelectorTests
    /// Purpose: TDD tests for language selection (UC-002)
    /// Reference: T-022 (UC-002 State Flows), T-028 (Config Lifecycle)
    public class LanguageSelectorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void LanguageSelector_Initializes_Successfully()
        {
            var selector = new LanguageSelector();
            Assert.NotNull(selector);
        }

        /// TEST 2: Get Available Languages
        [Fact]
        public void LanguageSelector_GetLanguageOptions_Returns_Two_Languages()
        {
            var selector = new LanguageSelector();
            var options = selector.GetLanguageOptions();

            Assert.NotNull(options);
            Assert.Equal(2, options.Count);
            Assert.Contains("en-US", options);
            Assert.Contains("es-MX", options);
        }

        /// TEST 3: Validate Single Language - en-US
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Single_EnUS()
        {
            var selector = new LanguageSelector();
            var selection = new[] { "en-US" };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.True(result);
        }

        /// TEST 4: Validate Single Language - es-MX
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Single_EsMX()
        {
            var selector = new LanguageSelector();
            var selection = new[] { "es-MX" };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.True(result);
        }

        /// TEST 5: Validate Multiple Languages
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Multiple()
        {
            var selector = new LanguageSelector();
            var selection = new[] { "en-US", "es-MX" };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.True(result);
        }

        /// TEST 6: Validate Invalid Language
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Invalid_Language()
        {
            var selector = new LanguageSelector();
            var selection = new[] { "fr-FR" };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.False(result);
        }

        /// TEST 7: Validate Empty Array
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Empty_Array()
        {
            var selector = new LanguageSelector();
            var selection = new string[] { };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.False(result); // At least one language required
        }

        /// TEST 8: Validate Null Array
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Null_Array()
        {
            var selector = new LanguageSelector();

            bool result = selector.IsValidLanguageSelection(null);

            Assert.False(result);
        }

        /// TEST 9: Validate Null Element in Array
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Null_Element()
        {
            var selector = new LanguageSelector();
            var selection = new string[] { "en-US", null, "es-MX" };

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.False(result); // Null element invalid
        }

        /// TEST 10: Validate Mixed Valid and Invalid
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Mixed_Valid_Invalid()
        {
            var selector = new LanguageSelector();
            var selection = new[] { "en-US", "fr-FR" }; // One valid, one invalid

            bool result = selector.IsValidLanguageSelection(selection);

            Assert.False(result); // All must be valid
        }

        /// TEST 11: Validate Case Sensitivity
        [Fact]
        public void LanguageSelector_IsValidLanguageSelection_Case_Sensitive()
        {
            var selector = new LanguageSelector();

            Assert.True(selector.IsValidLanguageSelection(new[] { "en-US" }));
            Assert.False(selector.IsValidLanguageSelection(new[] { "EN-US" })); // Uppercase
            Assert.False(selector.IsValidLanguageSelection(new[] { "en-us" })); // Lowercase
        }

        /// TEST 12: Execute - Happy Path Single Language
        [Fact]
        public void LanguageSelector_Execute_Single_Language_Success()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" }; // Must have version first
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "en-US" }, handler);

            Assert.True(result);
            Assert.NotNull(config.languages);
            Assert.Single(config.languages);
            Assert.Equal("en-US", config.languages[0]);
            Assert.Null(config.errorResult);
        }

        /// TEST 13: Execute - Happy Path Multiple Languages
        [Fact]
        public void LanguageSelector_Execute_Multiple_Languages_Success()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "en-US", "es-MX" }, handler);

            Assert.True(result);
            Assert.Equal(2, config.languages.Length);
            Assert.Contains("en-US", config.languages);
            Assert.Contains("es-MX", config.languages);
            Assert.Null(config.errorResult);
        }

        /// TEST 14: Execute - Invalid Language
        [Fact]
        public void LanguageSelector_Execute_Invalid_Language_Fails()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "fr-FR" }, handler);

            Assert.False(result);
            Assert.Null(config.languages); // Not updated
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-002", config.errorResult.code);
        }

        /// TEST 15: Execute - Empty Selection
        [Fact]
        public void LanguageSelector_Execute_Empty_Selection_Fails()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new string[] { }, handler);

            Assert.False(result);
            Assert.Null(config.languages);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-002", config.errorResult.code);
        }

        /// TEST 16: Execute - Null Selection
        [Fact]
        public void LanguageSelector_Execute_Null_Selection_Fails()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, null, handler);

            Assert.False(result);
            Assert.Null(config.languages);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 17: Execute - Does Not Change State
        [Fact]
        public void LanguageSelector_Execute_Does_Not_Change_State()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();
            var initialState = config.state;

            selector.Execute(config, new[] { "en-US" }, handler);

            Assert.Equal(initialState, config.state); // State unchanged
        }

        /// TEST 18: Execute - Error Message Clear
        [Fact]
        public void LanguageSelector_Execute_Error_Has_Clear_Message()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            selector.Execute(config, new[] { "fr-FR" }, handler);

            Assert.NotNull(config.errorResult.message);
            Assert.NotEmpty(config.errorResult.message);
        }

        /// TEST 19: Multiple Executions
        [Fact]
        public void LanguageSelector_Multiple_Executions()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" };
            var handler = new ErrorHandler();

            selector.Execute(config, new[] { "en-US" }, handler);
            Assert.Single(config.languages);

            selector.Execute(config, new[] { "es-MX" }, handler);
            Assert.Single(config.languages);
            Assert.Equal("es-MX", config.languages[0]);
        }

        /// TEST 20: Complete UC-002 Workflow
        [Fact]
        public void LanguageSelector_Complete_UC002_Workflow()
        {
            var selector = new LanguageSelector();
            var config = new Configuration { version = "2024" }; // From UC-001
            var stateMachine = new OfficeAutomatorStateMachine();
            var handler = new ErrorHandler();

            stateMachine.TransitionTo("SELECT_VERSION");
            Assert.Equal("SELECT_VERSION", stateMachine.GetCurrentState());

            // UC-002: Select languages
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            bool executed = selector.Execute(config, new[] { "en-US", "es-MX" }, handler);

            Assert.True(executed);
            Assert.Equal(2, config.languages.Length);
            Assert.Equal("SELECT_LANGUAGE", stateMachine.GetCurrentState());
        }
    }
}
