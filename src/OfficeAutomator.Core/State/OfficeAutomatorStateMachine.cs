using System;
using System.Collections.Generic;

namespace OfficeAutomator.Core.State
{
    /// CLASS: OfficeAutomatorStateMachine
    /// 
    /// Purpose: Orchestrates the workflow through 11 distinct states, managing
    /// valid state transitions and preventing invalid state changes.
    /// 
    /// Responsibilities:
    ///   • Maintain current state
    ///   • Validate transitions before allowing state changes
    ///   • Prevent invalid transitions (e.g., INIT → INSTALLING)
    ///   • Support both success path and error/recovery paths
    ///   • Provide query methods for state information
    /// 
    /// States (11 total):
    ///   1. INIT - Initial state, application starting
    ///   2. SELECT_VERSION - User selecting Office version (2024/2021/2019)
    ///   3. SELECT_LANGUAGE - User selecting language(s) (en-US/es-MX)
    ///   4. SELECT_APPS - User selecting apps to exclude
    ///   5. GENERATE_CONFIG - Config.xml generation (internal transition)
    ///   6. VALIDATE - 8-step validation of selections
    ///   7. INSTALL_READY - Confirmation UI before installation
    ///   8. INSTALLING - Office installation in progress
    ///   9. INSTALL_COMPLETE - Installation succeeded (terminal state)
    ///   10. INSTALL_FAILED - Installation failed, triggering rollback
    ///   11. ROLLED_BACK - Rollback completed, ready for retry
    /// 
    /// Transitions (Valid paths):
    ///   Success Path:
    ///     INIT → SELECT_VERSION → SELECT_LANGUAGE → SELECT_APPS
    ///     → GENERATE_CONFIG → VALIDATE → INSTALL_READY
    ///     → INSTALLING → INSTALL_COMPLETE (terminal)
    /// 
    ///   Error Path:
    ///     INSTALLING → INSTALL_FAILED (any UC failure)
    ///     INSTALL_FAILED → ROLLED_BACK (after 3-part rollback)
    /// 
    ///   Recovery Path:
    ///     ROLLED_BACK → INIT (user retries)
    /// 
    /// Transition Rules:
    ///   • Only valid transitions allowed (documented above)
    ///   • Cannot skip states (must follow sequence)
    ///   • Cannot go backwards (except ROLLED_BACK → INIT)
    ///   • Terminal state (INSTALL_COMPLETE) cannot transition
    ///   • Cannot transition to same state
    /// 
    /// Reference: T-019 (State Management Design), T-026 (Integration & E2E)
    public class OfficeAutomatorStateMachine
    {
        // ===== FIELDS =====

        /// Current state in the workflow
        private string currentState;

        /// Valid transitions dictionary (from state → list of allowed to states)
        private readonly Dictionary<string, HashSet<string>> validTransitions;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: OfficeAutomatorStateMachine()
        /// 
        /// Initializes state machine in INIT state with all valid transitions defined.
        /// 
        /// Post-condition:
        ///   • currentState = "INIT"
        ///   • All 11 states defined
        ///   • All valid transitions configured
        public OfficeAutomatorStateMachine()
        {
            currentState = "INIT";
            validTransitions = InitializeValidTransitions();
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: TransitionTo(string newState) → bool
        /// 
        /// Attempts to transition to the specified state.
        /// Validates transition before changing state.
        /// 
        /// Parameters:
        ///   newState: Target state (must be one of the 11 valid states)
        /// 
        /// Returns:
        ///   true if transition succeeded, false if transition invalid
        /// 
        /// Pre-condition:
        ///   • newState is not null
        ///   • currentState is valid
        /// 
        /// Post-condition (if true):
        ///   • currentState = newState
        /// 
        /// Post-condition (if false):
        ///   • currentState unchanged
        /// 
        /// Error handling:
        ///   • Invalid transitions silently return false (no exception)
        ///   • Allows graceful error handling in calling code
        /// 
        /// Example:
        ///   if (stateMachine.TransitionTo("SELECT_VERSION")) {
        ///       // State changed successfully
        ///   } else {
        ///       // Transition invalid, state unchanged
        ///   }
        /// 
        /// Reference: T-026 (State Transitions)
        public bool TransitionTo(string newState)
        {
            if (string.IsNullOrWhiteSpace(newState))
            {
                return false; // Invalid state name
            }

            // Check if transition is valid
            if (!IsValidTransition(currentState, newState))
            {
                return false; // Transition not allowed
            }

            // Transition is valid - change state
            currentState = newState;
            return true;
        }

        /// METHOD: IsValidTransition(string from, string to) → bool
        /// 
        /// Checks if a transition from one state to another is valid.
        /// Does not change state - purely informational.
        /// 
        /// Parameters:
        ///   from: Current state
        ///   to: Target state
        /// 
        /// Returns:
        ///   true if transition allowed, false if not allowed
        /// 
        /// Pre-condition:
        ///   • from and to are not null
        /// 
        /// Post-condition:
        ///   • No state changes (read-only query)
        /// 
        /// Example:
        ///   if (stateMachine.IsValidTransition("INIT", "SELECT_VERSION")) {
        ///       // Transition is valid
        ///   }
        /// 
        /// Reference: T-019 (Valid Transitions)
        public bool IsValidTransition(string from, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return false;
            }

            if (from == to)
            {
                return false; // Cannot transition to same state
            }

            if (!validTransitions.ContainsKey(from))
            {
                return false; // Invalid from state
            }

            return validTransitions[from].Contains(to);
        }

        /// METHOD: GetCurrentState() → string
        /// 
        /// Returns the current state without changing it.
        /// 
        /// Returns:
        ///   String representing current state (one of 11 valid states)
        /// 
        /// Post-condition:
        ///   • No state change
        /// 
        /// Example:
        ///   string current = stateMachine.GetCurrentState();
        ///   if (current == "INSTALL_COMPLETE") {
        ///       // Installation succeeded
        ///   }
        public string GetCurrentState()
        {
            return currentState;
        }

        /// METHOD: IsTerminalState(string state) → bool
        ///
        /// Checks if the state is terminal (no transitions possible).
        /// Used to determine if workflow is complete or failed.
        /// 
        /// Parameters:
        ///   state: State to check
        /// 
        /// Returns:
        ///   true if state is terminal, false otherwise
        /// 
        /// Terminal states:
        ///   • INSTALL_COMPLETE (success - workflow done)
        /// 
        /// Non-terminal states that can recover:
        ///   • ROLLED_BACK (can retry)
        /// 
        /// Example:
        ///   if (stateMachine.IsTerminalState(current)) {
        ///       // Workflow complete, show summary
        ///   }
        public bool IsTerminalState(string state)
        {
            // INSTALL_COMPLETE is the only true terminal state
            return state == "INSTALL_COMPLETE";
        }

        /// METHOD: IsErrorState(string state) → bool
        /// 
        /// Checks if the state indicates an error condition.
        /// Used to determine if error handling/rollback is needed.
        /// 
        /// Parameters:
        ///   state: State to check
        /// 
        /// Returns:
        ///   true if state is error-related, false otherwise
        /// 
        /// Error states:
        ///   • INSTALL_FAILED (installation failed, rollback needed)
        ///   • ROLLED_BACK (recovery state after failed installation)
        /// 
        /// Example:
        ///   if (stateMachine.IsErrorState(current)) {
        ///       // Show error message, offer retry
        ///   }
        public bool IsErrorState(string state)
        {
            return state == "INSTALL_FAILED" || state == "ROLLED_BACK";
        }

        // ===== PRIVATE METHODS =====

        /// METHOD: InitializeValidTransitions() → Dictionary<string, HashSet<string>>
        /// 
        /// Defines all valid transitions between states.
        /// Called during construction.
        /// 
        /// Returns:
        ///   Dictionary mapping from state → set of allowed to states
        /// 
        /// Transitions defined:
        ///   INIT → SELECT_VERSION
        ///   SELECT_VERSION → SELECT_LANGUAGE
        ///   SELECT_LANGUAGE → SELECT_APPS
        ///   SELECT_APPS → GENERATE_CONFIG
        ///   GENERATE_CONFIG → VALIDATE
        ///   VALIDATE → INSTALL_READY
        ///   INSTALL_READY → INSTALLING
        ///   INSTALLING → INSTALL_COMPLETE | INSTALL_FAILED
        ///   INSTALL_FAILED → ROLLED_BACK
        ///   ROLLED_BACK → INIT (retry) | (terminal)
        /// 
        /// Reference: T-019 (State Machine Design), T-026 (All Transitions)
        private Dictionary<string, HashSet<string>> InitializeValidTransitions()
        {
            var transitions = new Dictionary<string, HashSet<string>>
            {
                // Success path (UC-001 through UC-005)
                { "INIT", new HashSet<string> { "SELECT_VERSION" } },
                { "SELECT_VERSION", new HashSet<string> { "SELECT_LANGUAGE" } },
                { "SELECT_LANGUAGE", new HashSet<string> { "SELECT_APPS" } },
                { "SELECT_APPS", new HashSet<string> { "GENERATE_CONFIG" } },
                { "GENERATE_CONFIG", new HashSet<string> { "VALIDATE" } },
                { "VALIDATE", new HashSet<string> { "INSTALL_READY" } },
                { "INSTALL_READY", new HashSet<string> { "INSTALLING" } },

                // Success completion
                { "INSTALLING", new HashSet<string> { "INSTALL_COMPLETE", "INSTALL_FAILED" } },

                // Success terminal (no transitions)
                { "INSTALL_COMPLETE", new HashSet<string> { } },

                // Error recovery
                { "INSTALL_FAILED", new HashSet<string> { "ROLLED_BACK" } },

                // Recovery (can retry)
                { "ROLLED_BACK", new HashSet<string> { "INIT" } },
            };

            return transitions;
        }
    }
}
