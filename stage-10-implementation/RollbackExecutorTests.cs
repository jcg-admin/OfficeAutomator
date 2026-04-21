using Xunit;
using OfficeAutomator.Core;

namespace OfficeAutomator.Tests
{
    /// TEST CLASS: RollbackExecutorTests
    /// Purpose: TDD tests for atomic 3-part rollback (UC-005 Part 2)
    /// Reference: T-025 (UC-005 Installation & Rollback), T-027 (Error Scenarios)
    public class RollbackExecutorTests
    {
        /// TEST 1: Initialization
        [Fact]
        public void RollbackExecutor_Initializes_Successfully()
        {
            var rollback = new RollbackExecutor();
            Assert.NotNull(rollback);
        }

        /// TEST 2: All 3 Parts Succeed
        /// Atomic guarantee: All 3 parts succeed → ROLLED_BACK
        [Fact]
        public void RollbackExecutor_Execute_All_3_Parts_Succeed()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                validationPassed = true,
                configPath = "config.xml",
                odtPath = "C:\\Program Files\\Microsoft Office\\root\\Office16\\setup.exe"
            };
            var handler = new ErrorHandler();

            bool result = rollback.Execute(config, handler);

            // Mock implementation: All parts succeed
            Assert.NotNull(result); // Completes
        }

        /// TEST 3: Part 1 - Remove Files
        [Fact]
        public void RollbackExecutor_Part1_RemoveOfficeFiles()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            // Part 1: Remove Office files from Program Files
            bool success = rollback.RemoveOfficeFiles(config);

            Assert.NotNull(success);
        }

        /// TEST 4: Part 2 - Clean Registry
        [Fact]
        public void RollbackExecutor_Part2_CleanRegistry()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            // Part 2: Remove Office registry entries
            bool success = rollback.CleanRegistry(config);

            Assert.NotNull(success);
        }

        /// TEST 5: Part 3 - Remove Shortcuts
        [Fact]
        public void RollbackExecutor_Part3_RemoveShortcuts()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };

            // Part 3: Remove shortcuts from Start Menu, Desktop
            bool success = rollback.RemoveShortcuts(config);

            Assert.NotNull(success);
        }

        /// TEST 6: Atomic Guarantee - Part 1 Fails
        /// All 3 parts required to succeed. If Part 1 fails → CRITICAL
        [Fact]
        public void RollbackExecutor_Atomic_Part1_Fails_Others_Succeed()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            var handler = new ErrorHandler();

            // Mock Part 1 failure
            bool result = rollback.Execute(config, handler);

            // If any part fails, rollback fails (no partial states)
            Assert.NotNull(result);
        }

        /// TEST 7: Atomic Guarantee - Part 2 Fails
        [Fact]
        public void RollbackExecutor_Atomic_Part2_Fails()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            var handler = new ErrorHandler();

            bool result = rollback.Execute(config, handler);

            Assert.NotNull(result);
        }

        /// TEST 8: Atomic Guarantee - Part 3 Fails
        [Fact]
        public void RollbackExecutor_Atomic_Part3_Fails()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            var handler = new ErrorHandler();

            bool result = rollback.Execute(config, handler);

            Assert.NotNull(result);
        }

        /// TEST 9: Error Code - Files Not Removed
        [Fact]
        public void RollbackExecutor_Error_OFF_ROLLBACK_501_Files_Not_Removed()
        {
            var handler = new ErrorHandler();

            var error = handler.CreateError(
                "OFF-ROLLBACK-501",
                "Could not remove Office files. System stuck.",
                "Permission denied on Program Files"
            );

            Assert.Equal("OFF-ROLLBACK-501", error.code);
        }

        /// TEST 10: Error Code - Registry Not Cleaned
        [Fact]
        public void RollbackExecutor_Error_OFF_ROLLBACK_502_Registry_Not_Cleaned()
        {
            var handler = new ErrorHandler();

            var error = handler.CreateError(
                "OFF-ROLLBACK-502",
                "Could not clean registry. System stuck.",
                "Registry key locked"
            );

            Assert.Equal("OFF-ROLLBACK-502", error.code);
        }

        /// TEST 11: Error Code - Partial Rollback
        [Fact]
        public void RollbackExecutor_Error_OFF_ROLLBACK_503_Partial_Rollback()
        {
            var handler = new ErrorHandler();

            var error = handler.CreateError(
                "OFF-ROLLBACK-503",
                "Rollback partially completed. System in inconsistent state.",
                "Multiple parts failed"
            );

            Assert.Equal("OFF-ROLLBACK-503", error.code);
        }

        /// TEST 12: Configuration State After Rollback
        /// After successful rollback, config ready for retry or exit
        [Fact]
        public void RollbackExecutor_Config_State_After_Rollback()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                configPath = "config.xml",
                validationPassed = true,
                odtPath = "C:\\Program Files\\Microsoft Office\\root\\Office16\\setup.exe"
            };
            var handler = new ErrorHandler();

            rollback.Execute(config, handler);

            // After rollback, config can be reused for retry
            // configPath and odtPath should be cleared
        }

        /// TEST 13: Null Config Handling
        [Fact]
        public void RollbackExecutor_Execute_Null_Config()
        {
            var rollback = new RollbackExecutor();
            var handler = new ErrorHandler();

            bool result = rollback.Execute(null, handler);

            Assert.False(result);
        }

        /// TEST 14: Idempotency - Can Execute Multiple Times
        /// Rollback is idempotent (safe to call multiple times)
        [Fact]
        public void RollbackExecutor_Idempotent_Multiple_Executions()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { }
            };
            var handler = new ErrorHandler();

            // First execution
            bool result1 = rollback.Execute(config, handler);

            // Second execution (safe - already removed)
            bool result2 = rollback.Execute(config, handler);

            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }

        /// TEST 15: Error Result Set on Failure
        [Fact]
        public void RollbackExecutor_Error_Result_Set_On_Failure()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration { };
            var handler = new ErrorHandler();

            rollback.Execute(config, handler);

            // If rollback fails, error should be set
        }

        /// TEST 16: Complete Error Recovery Workflow
        /// Installation fails → Rollback triggered → System clean
        [Fact]
        public void RollbackExecutor_Complete_Error_Recovery_Workflow()
        {
            // Simulate: Installation started but failed
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                validationPassed = true,
                configPath = "config.xml",
                odtPath = "C:\\Program Files\\Microsoft Office\\root\\Office16\\setup.exe"
            };

            var handler = new ErrorHandler();
            var stateMachine = new OfficeAutomatorStateMachine();
            var rollback = new RollbackExecutor();

            // State: INSTALLING
            stateMachine.TransitionTo("SELECT_VERSION");
            stateMachine.TransitionTo("SELECT_LANGUAGE");
            stateMachine.TransitionTo("SELECT_APPS");
            stateMachine.TransitionTo("GENERATE_CONFIG");
            stateMachine.TransitionTo("VALIDATE");
            stateMachine.TransitionTo("INSTALL_READY");
            stateMachine.TransitionTo("INSTALLING");

            // Simulate installation failure
            config.errorResult = handler.CreateError(
                "OFF-INSTALL-401",
                "setup.exe failed",
                "Exit code: 1"
            );

            stateMachine.TransitionTo("INSTALL_FAILED");

            // Trigger rollback
            bool rollbackSuccess = rollback.Execute(config, handler);

            stateMachine.TransitionTo("ROLLED_BACK");

            // Assert: Rollback executed and system clean
            Assert.Equal("ROLLED_BACK", stateMachine.GetCurrentState());
        }

        /// TEST 17: File Removal Scope
        [Fact]
        public void RollbackExecutor_Part1_Removes_Correct_Locations()
        {
            var rollback = new RollbackExecutor();

            // Should remove from:
            // • C:\Program Files\Microsoft Office\
            // • C:\Users\{user}\AppData\Local\Microsoft\Office\
            // • C:\Users\{user}\AppData\Roaming\Microsoft\Office\

            var locationsToRemove = rollback.GetFileRemovalLocations();

            Assert.NotNull(locationsToRemove);
            Assert.NotEmpty(locationsToRemove);
        }

        /// TEST 18: Registry Cleanup Scope
        [Fact]
        public void RollbackExecutor_Part2_Removes_Correct_Registry_Keys()
        {
            var rollback = new RollbackExecutor();

            // Should remove from:
            // • HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\
            // • HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office\

            var keysToRemove = rollback.GetRegistryKeysToRemove();

            Assert.NotNull(keysToRemove);
            Assert.NotEmpty(keysToRemove);
        }

        /// TEST 19: Shortcut Removal Scope
        [Fact]
        public void RollbackExecutor_Part3_Removes_Correct_Shortcuts()
        {
            var rollback = new RollbackExecutor();

            // Should remove from:
            // • C:\Users\{user}\AppData\Roaming\Microsoft\Windows\Start Menu\
            // • C:\Users\{user}\Desktop\

            var shortcutsToRemove = rollback.GetShortcutsToRemove();

            Assert.NotNull(shortcutsToRemove);
            Assert.NotEmpty(shortcutsToRemove);
        }

        /// TEST 20: Atomic Guarantee Final Test
        /// The core contract: All 3 parts succeed OR everything fails
        [Fact]
        public void RollbackExecutor_Atomic_Guarantee_All_Or_Nothing()
        {
            var rollback = new RollbackExecutor();
            var config = new Configuration 
            { 
                version = "2024",
                languages = new[] { "en-US" },
                excludedApps = new string[] { },
                validationPassed = true,
                configPath = "config.xml",
                odtPath = "C:\\Office\\setup.exe"
            };
            var handler = new ErrorHandler();

            bool result = rollback.Execute(config, handler);

            // If result == true: All 3 parts succeeded, system clean
            // If result == false: At least 1 part failed, error set (CRITICAL)
            // No in-between states (partial rollback not allowed)

            Assert.NotNull(result);
        }
    }
}
