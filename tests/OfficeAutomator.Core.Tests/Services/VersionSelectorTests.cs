using Xunit;
using OfficeAutomator.Core.Services;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: VersionSelectorTests
    /// Purpose: TDD tests for version selection (UC-001)
    /// Reference: T-022 (UC-001 State Flows), T-023 (UC-002 Design)
    public class VersionSelectorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void VersionSelector_Initializes_Successfully()
        {
            // Arrange & Act
            var selector = new VersionSelector();

            // Assert
            Assert.NotNull(selector);
        }

        /// TEST 2: Get Available Versions
        /// Returns list of supported versions
        [Fact]
        public void VersionSelector_GetVersionOptions_Returns_Three_Versions()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            var options = selector.GetVersionOptions();

            // Assert
            Assert.NotNull(options);
            Assert.Equal(3, options.Count);
            Assert.Contains("2024", options);
            Assert.Contains("2021", options);
            Assert.Contains("2019", options);
        }

        /// TEST 3: Validate Valid Version - 2024
        [Fact]
        public void VersionSelector_IsValidVersion_2024_Returns_True()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("2024");

            // Assert
            Assert.True(result);
        }

        /// TEST 4: Validate Valid Version - 2021
        [Fact]
        public void VersionSelector_IsValidVersion_2021_Returns_True()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("2021");

            // Assert
            Assert.True(result);
        }

        /// TEST 5: Validate Valid Version - 2019
        [Fact]
        public void VersionSelector_IsValidVersion_2019_Returns_True()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("2019");

            // Assert
            Assert.True(result);
        }

        /// TEST 6: Validate Invalid Version - 2025
        [Fact]
        public void VersionSelector_IsValidVersion_2025_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("2025");

            // Assert
            Assert.False(result);
        }

        /// TEST 7: Validate Invalid Version - 2020
        [Fact]
        public void VersionSelector_IsValidVersion_2020_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("2020");

            // Assert
            Assert.False(result);
        }

        /// TEST 8: Validate Null Version
        [Fact]
        public void VersionSelector_IsValidVersion_Null_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion(null);

            // Assert
            Assert.False(result);
        }

        /// TEST 9: Validate Empty Version
        [Fact]
        public void VersionSelector_IsValidVersion_Empty_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("");

            // Assert
            Assert.False(result);
        }

        /// TEST 10: Validate Whitespace Version
        [Fact]
        public void VersionSelector_IsValidVersion_Whitespace_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act
            bool result = selector.IsValidVersion("   ");

            // Assert
            Assert.False(result);
        }

        /// TEST 11: Execute - Happy Path
        /// User selects valid version, configuration updated
        [Fact]
        public void VersionSelector_Execute_Valid_Version_Returns_True()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            bool result = selector.Execute(config, "2024", handler);

            // Assert
            Assert.True(result);
            Assert.Equal("2024", config.version);
            Assert.Null(config.errorResult); // No error
        }

        /// TEST 12: Execute - Invalid Version
        /// User selects invalid version, error set
        [Fact]
        public void VersionSelector_Execute_Invalid_Version_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            bool result = selector.Execute(config, "2025", handler);

            // Assert
            Assert.False(result);
            Assert.Null(config.version); // Not updated
            Assert.NotNull(config.errorResult); // Error set
            Assert.Equal("OFF-CONFIG-001", config.errorResult.code);
        }

        /// TEST 13: Execute - Null Version
        [Fact]
        public void VersionSelector_Execute_Null_Version_Returns_False()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            bool result = selector.Execute(config, null, handler);

            // Assert
            Assert.False(result);
            Assert.Null(config.version);
            Assert.NotNull(config.errorResult);
            Assert.Equal("OFF-CONFIG-001", config.errorResult.code);
        }

        /// TEST 14: Execute - Each Valid Version
        [Theory]
        [InlineData("2024")]
        [InlineData("2021")]
        [InlineData("2019")]
        public void VersionSelector_Execute_All_Valid_Versions(string version)
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            bool result = selector.Execute(config, version, handler);

            // Assert
            Assert.True(result);
            Assert.Equal(version, config.version);
            Assert.Null(config.errorResult);
        }

        /// TEST 15: Execute - State Not Updated
        /// Version selection does not change state (state machine handles it)
        [Fact]
        public void VersionSelector_Execute_Does_Not_Change_State()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();
            var initialState = config.state;

            // Act
            selector.Execute(config, "2024", handler);

            // Assert
            Assert.Equal(initialState, config.state); // State unchanged
        }

        /// TEST 16: Execute - Error Message Set
        /// When error occurs, user-facing message is clear
        [Fact]
        public void VersionSelector_Execute_Error_Has_Clear_Message()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            selector.Execute(config, "2025", handler);

            // Assert
            Assert.NotNull(config.errorResult.message);
            Assert.NotEmpty(config.errorResult.message);
            Assert.DoesNotContain("2025", config.errorResult.message); // No technical details in message
        }

        /// TEST 17: Execute - Technical Details Set
        /// Error object includes technical details for IT support
        [Fact]
        public void VersionSelector_Execute_Error_Has_Technical_Details()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act
            selector.Execute(config, "2025", handler);

            // Assert
            Assert.NotNull(config.errorResult.technicalDetails);
            Assert.NotEmpty(config.errorResult.technicalDetails);
        }

        /// TEST 18: Multiple Executions
        /// Can execute multiple times (idempotent)
        [Fact]
        public void VersionSelector_Multiple_Executions()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var handler = new ErrorHandler();

            // Act - First execution
            selector.Execute(config, "2024", handler);
            Assert.Equal("2024", config.version);

            // Act - Second execution (overwrites)
            selector.Execute(config, "2021", handler);

            // Assert
            Assert.Equal("2021", config.version); // Updated
        }

        /// TEST 19: Case Sensitivity
        /// Version input should be case-sensitive (exact match)
        [Fact]
        public void VersionSelector_IsValidVersion_Case_Sensitive()
        {
            // Arrange
            var selector = new VersionSelector();

            // Act & Assert
            Assert.True(selector.IsValidVersion("2024"));
            Assert.False(selector.IsValidVersion("2024 ")); // With space
            Assert.False(selector.IsValidVersion(" 2024")); // Leading space
        }

        /// TEST 20: Complete UC-001 Workflow
        /// Full version selection workflow
        [Fact]
        public void VersionSelector_Complete_UC001_Workflow()
        {
            // Arrange
            var selector = new VersionSelector();
            var config = new Configuration();
            var stateMachine = new OfficeAutomatorStateMachine();
            var handler = new ErrorHandler();

            // Pre-condition: INIT state
            Assert.Equal("INIT", stateMachine.GetCurrentState());
            Assert.Null(config.version);

            // Act 1: Transition to SELECT_VERSION
            bool transitioned = stateMachine.TransitionTo("SELECT_VERSION");
            Assert.True(transitioned);

            // Act 2: User selects version
            bool selected = selector.Execute(config, "2024", handler);
            Assert.True(selected);

            // Assert: Version selected, ready for next UC
            Assert.Equal("2024", config.version);
            Assert.Null(config.errorResult);
            Assert.Equal("SELECT_VERSION", stateMachine.GetCurrentState());
        }
    }
}
