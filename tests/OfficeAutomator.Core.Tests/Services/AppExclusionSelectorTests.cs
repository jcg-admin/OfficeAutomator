using Xunit;
using OfficeAutomator.Core.Services;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: AppExclusionSelectorTests
    /// Purpose: TDD tests for app exclusion selection (UC-003)
    /// Reference: T-023 (UC-003 State & XML Design)
    public class AppExclusionSelectorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void AppExclusionSelector_Initializes_Successfully()
        {
            var selector = new AppExclusionSelector();
            Assert.NotNull(selector);
        }

        /// TEST 2: Get Excludable Apps
        [Fact]
        public void AppExclusionSelector_GetExcludableApps_Returns_Five_Apps()
        {
            var selector = new AppExclusionSelector();
            var apps = selector.GetExcludableApps();

            Assert.NotNull(apps);
            Assert.Equal(5, apps.Length);
            Assert.Contains("Teams", apps);
            Assert.Contains("OneDrive", apps);
            Assert.Contains("Groove", apps);
            Assert.Contains("Lync", apps);
            Assert.Contains("Bing", apps);
        }

        /// TEST 3: Validate Empty Exclusion (No Apps Excluded)
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Empty_Returns_True()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new string[] { };

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.True(result); // Excluding nothing is valid
        }

        /// TEST 4: Validate Single Valid App
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Single_Valid_App()
        {
            var selector = new AppExclusionSelector();

            Assert.True(selector.IsValidExclusionSet(new[] { "Teams" }));
            Assert.True(selector.IsValidExclusionSet(new[] { "OneDrive" }));
            Assert.True(selector.IsValidExclusionSet(new[] { "Groove" }));
            Assert.True(selector.IsValidExclusionSet(new[] { "Lync" }));
            Assert.True(selector.IsValidExclusionSet(new[] { "Bing" }));
        }

        /// TEST 5: Validate Multiple Apps
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Multiple_Apps()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new[] { "Teams", "OneDrive" };

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.True(result);
        }

        /// TEST 6: Validate All Apps
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_All_Apps()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new[] { "Teams", "OneDrive", "Groove", "Lync", "Bing" };

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.True(result);
        }

        /// TEST 7: Validate Invalid App
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Invalid_App()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new[] { "Word" }; // Not excludable

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.False(result);
        }

        /// TEST 8: Validate Mixed Valid and Invalid
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Mixed()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new[] { "Teams", "Word" }; // One valid, one invalid

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.False(result);
        }

        /// TEST 9: Validate Null Array
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Null_Array()
        {
            var selector = new AppExclusionSelector();

            bool result = selector.IsValidExclusionSet(null);

            Assert.False(result);
        }

        /// TEST 10: Validate Null Element
        [Fact]
        public void AppExclusionSelector_IsValidExclusionSet_Null_Element()
        {
            var selector = new AppExclusionSelector();
            var exclusions = new string[] { "Teams", null, "OneDrive" };

            bool result = selector.IsValidExclusionSet(exclusions);

            Assert.False(result);
        }

        /// TEST 11: Execute - No Exclusions
        [Fact]
        public void AppExclusionSelector_Execute_No_Exclusions()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new string[] { }, handler);

            Assert.True(result);
            Assert.NotNull(config.excludedApps);
            Assert.Empty(config.excludedApps);
            Assert.Null(config.errorResult);
        }

        /// TEST 12: Execute - Single App Excluded
        [Fact]
        public void AppExclusionSelector_Execute_Single_App()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "Teams" }, handler);

            Assert.True(result);
            Assert.Single(config.excludedApps);
            Assert.Equal("Teams", config.excludedApps[0]);
            Assert.Null(config.errorResult);
        }

        /// TEST 13: Execute - Multiple Apps Excluded
        [Fact]
        public void AppExclusionSelector_Execute_Multiple_Apps()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "Teams", "OneDrive" }, handler);

            Assert.True(result);
            Assert.Equal(2, config.excludedApps.Length);
            Assert.Contains("Teams", config.excludedApps);
            Assert.Contains("OneDrive", config.excludedApps);
            Assert.Null(config.errorResult);
        }

        /// TEST 14: Execute - Invalid App
        [Fact]
        public void AppExclusionSelector_Execute_Invalid_App()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "Word" }, handler);

            Assert.False(result);
            Assert.Null(config.excludedApps); // Not updated
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-003", config.errorResult.code);
        }

        /// TEST 15: Execute - Mixed Valid and Invalid
        [Fact]
        public void AppExclusionSelector_Execute_Mixed_Valid_Invalid()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, new[] { "Teams", "InvalidApp" }, handler);

            Assert.False(result);
            Assert.Null(config.excludedApps);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-003", config.errorResult.code);
        }

        /// TEST 16: Execute - Null Array
        [Fact]
        public void AppExclusionSelector_Execute_Null_Array()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            bool result = selector.Execute(config, null, handler);

            Assert.False(result);
            Assert.Null(config.excludedApps);
            Assert.NotNull(config.errorResult);
        }

        /// TEST 17: Execute - Does Not Change State
        [Fact]
        public void AppExclusionSelector_Execute_Does_Not_Change_State()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();
            var initialState = config.state;

            selector.Execute(config, new[] { "Teams" }, handler);

            Assert.Equal(initialState, config.state);
        }

        /// TEST 18: Execute - All Apps Excluded
        [Fact]
        public void AppExclusionSelector_Execute_All_Apps_Excluded()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();
            var allApps = selector.GetExcludableApps();

            bool result = selector.Execute(config, allApps, handler);

            Assert.True(result);
            Assert.Equal(5, config.excludedApps.Length);
            Assert.Null(config.errorResult);
        }

        /// TEST 19: Multiple Executions
        [Fact]
        public void AppExclusionSelector_Multiple_Executions()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration { version = "2024", languages = new[] { "en-US" } };
            var handler = new ErrorHandler();

            selector.Execute(config, new[] { "Teams" }, handler);
            Assert.Single(config.excludedApps);

            selector.Execute(config, new[] { "OneDrive", "Groove" }, handler);
            Assert.Equal(2, config.excludedApps.Length);
        }

        /// TEST 20: Complete UC-003 Workflow
        [Fact]
        public void AppExclusionSelector_Complete_UC003_Workflow()
        {
            var selector = new AppExclusionSelector();
            var config = new Configuration
            {
                version = "2024",           // From UC-001
                languages = new[] { "en-US" } // From UC-002
            };
            var handler = new ErrorHandler();
            var stateMachine = new OfficeAutomatorStateMachine();

            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");

            bool executed = selector.Execute(config, new[] { "Teams", "OneDrive" }, handler);

            Assert.True(executed);
            Assert.Equal(2, config.excludedApps.Length);
            Assert.Equal("SELECT_APPS", stateMachine.GetCurrentState());
        }
    }
}
