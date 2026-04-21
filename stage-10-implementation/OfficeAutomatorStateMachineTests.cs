using Xunit;
using OfficeAutomator.Core;
using System;
using System.Collections.Generic;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: OfficeAutomatorStateMachineTests
    /// Purpose: TDD tests for state machine orchestration
    /// Reference: T-019 (State Management Design), T-026 (Integration & E2E)
    public class OfficeAutomatorStateMachineTests
    {
        /// TEST 1: Initialization
        /// State machine initializes in INIT state
        [Fact]
        public void StateMachine_Initializes_In_INIT_State()
        {
            // Arrange & Act
            var stateMachine = new OfficeAutomatorStateMachine();

            // Assert
            Assert.Equal("INIT", stateMachine.GetCurrentState());
        }

        /// TEST 2: Valid Transition - INIT to SELECT_VERSION
        /// First transition must be INIT → SELECT_VERSION
        [Fact]
        public void StateMachine_Valid_Transition_INIT_To_SELECT_VERSION()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();

            // Act
            bool result = stateMachine.TransitionTo("SELECT_VERSION");

            // Assert
            Assert.True(result);
            Assert.Equal("SELECT_VERSION", stateMachine.GetCurrentState());
        }

        /// TEST 3: Invalid Transition - INIT to INSTALLING (skip steps)
        /// Cannot jump directly from INIT to INSTALLING
        [Fact]
        public void StateMachine_Invalid_Transition_INIT_To_INSTALLING_Fails()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();

            // Act
            bool result = stateMachine.TransitionTo("INSTALLING");

            // Assert
            Assert.False(result); // Transition rejected
            Assert.Equal("INIT", stateMachine.GetCurrentState()); // State unchanged
        }

        /// TEST 4: Valid Full Sequence - Success Path
        /// INIT → SELECT_VERSION → SELECT_LANGUAGE → SELECT_APPS → VALIDATE 
        /// → INSTALL_READY → INSTALLING → INSTALL_COMPLETE
        [Fact]
        public void StateMachine_Valid_Full_Success_Sequence()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();

            // Act & Assert - Full success path
            Assert.True(stateMachine.TransitionTo("SELECT_VERSION"));
            Assert.Equal("SELECT_VERSION", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("SELECT_LANGUAGE"));
            Assert.Equal("SELECT_LANGUAGE", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("SELECT_APPS"));
            Assert.Equal("SELECT_APPS", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("GENERATE_CONFIG"));
            Assert.Equal("GENERATE_CONFIG", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("VALIDATE"));
            Assert.Equal("VALIDATE", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("INSTALL_READY"));
            Assert.Equal("INSTALL_READY", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("INSTALLING"));
            Assert.Equal("INSTALLING", stateMachine.GetCurrentState());

            Assert.True(stateMachine.TransitionTo("INSTALL_COMPLETE"));
            Assert.Equal("INSTALL_COMPLETE", stateMachine.GetCurrentState());
        }

        /// TEST 5: Valid Error Path - Installation Failed
        /// INSTALLING → INSTALL_FAILED (on error)
        [Fact]
        public void StateMachine_Valid_Error_Path_INSTALLING_To_INSTALL_FAILED()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            
            // Set up to INSTALLING state
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");
            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");

            // Act - Transition to error state
            bool result = stateMachine.TransitionTo("INSTALL_FAILED");

            // Assert
            Assert.True(result);
            Assert.Equal("INSTALL_FAILED", stateMachine.GetCurrentState());
        }

        /// TEST 6: Valid Recovery Path - Rollback
        /// INSTALL_FAILED → ROLLED_BACK (after rollback succeeds)
        [Fact]
        public void StateMachine_Valid_Recovery_Path_INSTALL_FAILED_To_ROLLED_BACK()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            
            // Simulate failure path
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");
            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");
            stateMachine.TransitionTo("INSTALL_FAILED");

            // Act - Transition to rolled back state
            bool result = stateMachine.TransitionTo("ROLLED_BACK");

            // Assert
            Assert.True(result);
            Assert.Equal("ROLLED_BACK", stateMachine.GetCurrentState());
        }

        /// TEST 7: Retry After Rollback
        /// ROLLED_BACK → INIT (user retries) or EXIT (user cancels)
        [Fact]
        public void StateMachine_Retry_After_Rollback()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            
            // Simulate full failure → rollback path
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");
            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");
            stateMachine.TransitionTo("INSTALL_FAILED");
            stateMachine.TransitionTo("ROLLED_BACK");

            // Act - User chooses to retry
            bool result = stateMachine.TransitionTo("INIT");

            // Assert - Back to INIT, can start over
            Assert.True(result);
            Assert.Equal("INIT", stateMachine.GetCurrentState());
        }

        /// TEST 8: Validation Transition - SELECT_APPS to VALIDATE
        /// After app selection, must validate before proceeding
        [Fact]
        public void StateMachine_Must_Validate_Before_Install_Ready()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            
            // Navigate to SELECT_APPS
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");

            // Act - Try to go directly to INSTALL_READY (skip VALIDATE)
            bool result = stateMachine.TransitionTo("INSTALL_READY");

            // Assert - Should fail
            Assert.False(result);
            Assert.Equal("SELECT_APPS", stateMachine.GetCurrentState()); // State unchanged
        }

        /// TEST 9: Validate Transition Check
        /// IsValidTransition should correctly identify valid/invalid transitions
        [Fact]
        public void StateMachine_IsValidTransition_Checks_Correctly()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();

            // Act & Assert - Valid transitions
            Assert.True(stateMachine.IsValidTransition("INIT", "SELECT_VERSION"));
            Assert.True(stateMachine.IsValidTransition("SELECT_VERSION", "SELECT_LANGUAGE"));
            Assert.True(stateMachine.IsValidTransition("INSTALLING", "INSTALL_COMPLETE"));

            // Act & Assert - Invalid transitions
            Assert.False(stateMachine.IsValidTransition("INIT", "INSTALLING")); // Skip too many
            Assert.False(stateMachine.IsValidTransition("INSTALL_COMPLETE", "SELECT_VERSION")); // Cannot go back
            Assert.False(stateMachine.IsValidTransition("INIT", "INSTALL_FAILED")); // Skip to error state
        }

        /// TEST 10: All 11 States Reachable
        /// Verify all 11 states can be reached through valid paths
        [Fact]
        public void StateMachine_All_11_States_Reachable()
        {
            // List of all 11 states
            var expectedStates = new List<string>
            {
                "INIT",
                "SELECT_VERSION",
                "SELECT_LANGUAGE",
                "SELECT_APPS",
                "GENERATE_CONFIG",
                "VALIDATE",
                "INSTALL_READY",
                "INSTALLING",
                "INSTALL_COMPLETE",
                "INSTALL_FAILED",
                "ROLLED_BACK"
            };

            // For each state, verify it can be reached
            foreach (var state in expectedStates)
            {
                // Arrange - Create new state machine for each test
                var stateMachine = new OfficeAutomatorStateMachine();

                // Act - Navigate to the state using valid path
                if (state == "INIT")
                {
                    // Already in INIT
                    Assert.Equal("INIT", stateMachine.GetCurrentState());
                }
                else if (state == "INSTALL_COMPLETE")
                {
                    // Success path
                    stateMachine.TransitionTo("SELECT_VERSION");
                    stateMachine.TransitionTo("SELECT_LANGUAGE");
                    stateMachine.TransitionTo("SELECT_APPS");
                    stateMachine.TransitionTo("GENERATE_CONFIG");
                    stateMachine.TransitionTo("VALIDATE");
                    stateMachine.TransitionTo("INSTALL_READY");
                    stateMachine.TransitionTo("INSTALLING");
                    stateMachine.TransitionTo("INSTALL_COMPLETE");
                }
                else if (state == "ROLLED_BACK")
                {
                    // Error path with rollback
                    stateMachine.TransitionTo("SELECT_VERSION");
                    stateMachine.TransitionTo("SELECT_LANGUAGE");
                    stateMachine.TransitionTo("SELECT_APPS");
                    stateMachine.TransitionTo("GENERATE_CONFIG");
                    stateMachine.TransitionTo("VALIDATE");
                    stateMachine.TransitionTo("INSTALL_READY");
                    stateMachine.TransitionTo("INSTALLING");
                    stateMachine.TransitionTo("INSTALL_FAILED");
                    stateMachine.TransitionTo("ROLLED_BACK");
                }
                else
                {
                    // Navigate to intermediate state
                    var path = GetPathToState(state);
                    foreach (var nextState in path)
                    {
                        stateMachine.TransitionTo(nextState);
                    }
                }

                // Assert - Verify state reached
                Assert.Equal(state, stateMachine.GetCurrentState());
            }
        }

        /// TEST 11: Double Transition (Same State)
        /// Attempting to transition to same state should fail
        [Fact]
        public void StateMachine_Double_Transition_Same_State_Fails()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            Assert.Equal("INIT", stateMachine.GetCurrentState());

            // Act - Try to transition to INIT again
            bool result = stateMachine.TransitionTo("INIT");

            // Assert
            Assert.False(result); // Cannot stay in same state
            Assert.Equal("INIT", stateMachine.GetCurrentState());
        }

        /// TEST 12: Terminal State (INSTALL_COMPLETE)
        /// INSTALL_COMPLETE is terminal - cannot transition further
        [Fact]
        public void StateMachine_Terminal_State_INSTALL_COMPLETE_Cannot_Transition()
        {
            // Arrange
            var stateMachine = new OfficeAutomatorStateMachine();
            
            // Navigate to INSTALL_COMPLETE
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");
            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");
            stateMachine.TransitionTo("INSTALL_COMPLETE");

            // Act - Try to transition from terminal state
            bool result = stateMachine.TransitionTo("SELECT_VERSION");

            // Assert
            Assert.False(result); // Cannot transition from terminal state
            Assert.Equal("INSTALL_COMPLETE", stateMachine.GetCurrentState()); // State unchanged
        }

        /// HELPER METHOD: GetPathToState
        /// Returns the sequence of states needed to reach the target state
        private List<string> GetPathToState(string targetState)
        {
            var successPath = new List<string>
            {
                "SELECT_VERSION",
                "SELECT_LANGUAGE",
                "SELECT_APPS",
                "GENERATE_CONFIG",
                "VALIDATE",
                "INSTALL_READY",
                "INSTALLING"
            };

            var result = new List<string>();
            foreach (var state in successPath)
            {
                result.Add(state);
                if (state == targetState) break;
            }

            return result;
        }
    }
}
