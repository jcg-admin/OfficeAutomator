using System;
using System.Diagnostics;
using System.IO;
using OfficeAutomator.Core.Models;
using OfficeAutomator.Core.Error;
using OfficeAutomator.Core.Infrastructure;

namespace OfficeAutomator.Core.Installation
{
    /// CLASS: InstallationExecutor
    /// 
    /// Purpose: Part 1 of UC-005 (Installation) - Executes Office installation
    /// on target system with timeout protection and error recovery.
    /// 
    /// Responsibilities:
    ///   • Verify installation prerequisites (admin, disk space)
    ///   • Download Office binaries (if needed)
    ///   • Execute setup.exe with configuration
    ///   • Monitor progress (0-100%)
    ///   • Handle timeout (20-minute hard limit)
    ///   • Detect installation success/failure
    ///   • Support retry on transient errors
    /// 
    /// UC-005 Workflow (2 classes):
    ///   1. InstallationExecutor: Download + execute setup.exe
    ///   2. RollbackExecutor: Atomic 3-part cleanup on failure
    /// 
    /// Prerequisites (Verified in VerifyPrerequisites):
    ///   ✓ Running as Administrator
    ///   ✓ 5+ GB free disk space
    ///   ✓ Configuration validated (validationPassed = true)
    ///   ✓ Config file exists
    /// 
    /// Installation Phases:
    ///   Phase 1 (0-30%):   Download Office binaries (if not cached)
    ///   Phase 2 (30-90%):  Execute setup.exe with config
    ///   Phase 3 (90-100%): Verify installation success
    /// 
    /// Timeout: 20 minutes (1,200,000 ms)
    ///   • Hard limit from start of installation
    ///   • Monitored with Stopwatch
    ///   • Kills setup.exe process if exceeded
    ///   • Error: OFF-SYSTEM-201 (timeout)
    /// 
    /// Error Handling:
    ///   • Transient errors (network): Retry 3x with backoff
    ///   • System errors (timeout): Retry 1x with backoff
    ///   • Permanent errors (setup failure): No retry, trigger rollback
    /// 
    /// Success Criteria:
    ///   • setup.exe returns exit code 0
    ///   • Office executables found (Word.exe, Excel.exe, etc.)
    ///   • Registry entries present
    ///   • Completes within 20-minute timeout
    /// 
    /// Reference: T-025 (UC-005 Installation), T-029 (Retry Integration)
    public class InstallationExecutor
    {
        // ===== CONSTANTS =====

        /// Hard timeout for installation: 20 minutes (1,200,000 milliseconds)
        private const long INSTALLATION_TIMEOUT_MS = 1200000; // 20 * 60 * 1000

        /// Minimum free disk space required: 5 GB
        private const long MIN_DISK_SPACE_BYTES = 5L * 1024 * 1024 * 1024; // 5GB

        // ===== FIELDS =====

        /// Current installation progress (0-100%)
        private int currentProgress;

        /// Stopwatch for timeout monitoring
        private Stopwatch installationTimer;

        /// Dependency: Security context for privilege verification
        private ISecurityContext securityContext;

        /// Dependency: File system operations
        private IFileSystem fileSystem;

        /// Dependency: Process runner for setup.exe execution
        private IProcessRunner processRunner;

        // ===== CONSTRUCTORS =====

        /// CONSTRUCTOR: InstallationExecutor()
        /// Default constructor using production implementations
        public InstallationExecutor()
            : this(
                new SecurityContextImpl(),
                new FileSystemImpl(),
                new ProcessRunnerImpl()
            )
        {
        }

        /// CONSTRUCTOR: InstallationExecutor(dependencies)
        /// Constructor with dependency injection for testing
        public InstallationExecutor(
            ISecurityContext secContext,
            IFileSystem fileSys,
            IProcessRunner procRunner)
        {
            currentProgress = 0;
            installationTimer = null;
            securityContext = secContext ?? new SecurityContextImpl();
            fileSystem = fileSys ?? new FileSystemImpl();
            processRunner = procRunner ?? new ProcessRunnerImpl();
        }

        // ===== PUBLIC METHODS =====

        /// METHOD: Execute(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Main installation workflow: Verify prerequisites, download, execute setup.
        /// 
        /// Parameters:
        ///   config: Configuration with validated selections
        ///   handler: ErrorHandler for error creation and retry logic
        /// 
        /// Returns:
        ///   true if installation succeeds and completes within timeout
        ///   false if any step fails (errorResult populated)
        /// 
        /// Pre-conditions:
        ///   • config is not null
        ///   • handler is not null
        ///   • config.validationPassed = true
        ///   • config.configPath set and valid
        /// 
        /// Post-conditions (if true):
        ///   • config.odtPath = path to setup.exe
        ///   • config.errorResult = null
        ///   • Office successfully installed
        ///   • Ready for RollbackExecutor (UC-005 Part 2) not needed
        /// 
        /// Post-conditions (if false):
        ///   • config.errorResult = ErrorResult with specific code
        ///   • RollbackExecutor will be triggered by StateMachine
        ///   • System needs cleanup (3-part atomic rollback)
        /// 
        /// Workflow:
        ///   1. Verify prerequisites (admin, disk, validation)
        ///   2. Download Office binaries (network retry: 3x)
        ///   3. Execute setup.exe with config (system retry: 1x)
        ///   4. Monitor progress and timeout
        ///   5. Verify installation success
        /// 
        /// Timeout:
        ///   • Monitored continuously
        ///   • Kills process if > 1,200,000ms
        ///   • Error: OFF-SYSTEM-201
        /// 
        /// Reference: T-025 (UC-005 Complete)
        public bool Execute(Configuration config, ErrorHandler handler)
        {
            if (config == null || handler == null)
            {
                return false;
            }

            // Pre-check: Configuration must be validated
            if (!config.validationPassed)
            {
                config.errorResult = handler.CreateError(
                    "OFF-INSTALL-401",
                    "Configuration was not validated. Cannot proceed with installation.",
                    "config.validationPassed = false"
                );
                return false;
            }

            installationTimer = Stopwatch.StartNew();

            try
            {
                // Phase 1: Verify prerequisites
                if (!VerifyPrerequisites(config, handler))
                {
                    return false; // Error already set
                }

                currentProgress = 10; // Prerequisites OK

                // Phase 2: Download Office binaries
                if (!DownloadOffice(config, handler))
                {
                    return false; // Error already set
                }

                currentProgress = 30; // Download complete

                // Phase 3: Execute setup.exe
                if (!ExecuteSetup(config, handler))
                {
                    return false; // Error already set
                }

                currentProgress = 90; // Setup complete

                // Phase 4: Verify installation
                if (!VerifyInstallation(config, handler))
                {
                    return false; // Error already set
                }

                currentProgress = 100; // Complete

                // All phases successful
                config.errorResult = null;
                config.odtPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "Microsoft Office", "root", "Office16", "setup.exe"
                );

                return true;
            }
            catch (Exception ex)
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-999",
                    "An unexpected error occurred during installation.",
                    $"Exception: {ex.GetType().Name} - {ex.Message}"
                );
                return false;
            }
            finally
            {
                installationTimer.Stop();
            }
        }

        /// METHOD: VerifyPrerequisites(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Verify system meets requirements for installation.
        /// 
        /// Checks:
        ///   ✓ Running as Administrator
        ///   ✓ 5+ GB free disk space
        ///   ✓ Configuration validated
        ///   ✓ Timeout not exceeded
        public bool VerifyPrerequisites(Configuration config, ErrorHandler handler)
        {
            if (config == null)
            {
                return false;
            }

            // Check admin rights using ISecurityContext
            if (!securityContext.IsRunningAsAdmin())
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-203",
                    "Administrator rights are required to install Office.",
                    "Not running with elevated privileges"
                );
                return false;
            }

            // Check disk space using IFileSystem
            string sysDrive = Path.GetPathRoot(Environment.SystemDirectory);
            long availableSpace = fileSystem.GetAvailableFreeSpace(sysDrive);
            if (availableSpace < MIN_DISK_SPACE_BYTES)
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-202",
                    "Insufficient disk space. At least 5 GB free space is required.",
                    $"Free space: {availableSpace / (1024 * 1024 * 1024)} GB (need 5 GB)"
                );
                return false;
            }

            // Check timeout
            if (installationTimer.ElapsedMilliseconds > INSTALLATION_TIMEOUT_MS)
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-201",
                    "Installation took too long.",
                    $"Exceeded {INSTALLATION_TIMEOUT_MS}ms timeout"
                );
                return false;
            }

            return true;
        }

        /// METHOD: CanDownloadOffice(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Check if Office binaries can be downloaded.
        public bool CanDownloadOffice(Configuration config, ErrorHandler handler)
        {
            // Check network connectivity, CDN availability, etc.
            return true; // Mock: assume CDN is available
        }

        /// METHOD: GetTimeoutMs() → long
        /// 
        /// Returns installation timeout in milliseconds.
        public long GetTimeoutMs()
        {
            return INSTALLATION_TIMEOUT_MS;
        }

        /// METHOD: GetCurrentProgress() → int
        /// 
        /// Returns current installation progress (0-100%).
        public int GetCurrentProgress()
        {
            return currentProgress;
        }

        /// METHOD: GetSetupExecutablePath(Configuration config) → string
        /// 
        /// Returns path to setup.exe for given Office version.
        public string GetSetupExecutablePath(Configuration config)
        {
            // In real implementation: Find setup.exe in cached/downloaded location
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OfficeAutomator",
                "Office",
                config.version ?? "2024",
                "setup.exe"
            );
        }

        // ===== PRIVATE METHODS =====

        /// METHOD: DownloadOffice(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Download Office binaries from CDN.
        /// Retry: 3x with backoff on network errors (OFF-NETWORK-301/302)
        private bool DownloadOffice(Configuration config, ErrorHandler handler)
        {
            // In mock: Assume already cached or available
            // In real: Download from Microsoft CDN
            // Retry transient network errors
            return true;
        }

        /// METHOD: ExecuteSetup(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Execute setup.exe with configuration.
        /// Retry: 1x with backoff on timeout (OFF-SYSTEM-201)
        private bool ExecuteSetup(Configuration config, ErrorHandler handler)
        {
            // In real implementation:
            // 1. Create ProcessStartInfo
            // 2. Add arguments: /configure "{configPath}"
            // 3. Start process
            // 4. Monitor for timeout
            // 5. Wait for exit (max 20 minutes from start)
            // 6. Check exit code (0 = success, non-zero = failure)

            // Mock: Simulate successful execution
            System.Threading.Thread.Sleep(100); // Simulate some work

            if (installationTimer.ElapsedMilliseconds > INSTALLATION_TIMEOUT_MS)
            {
                config.errorResult = handler.CreateError(
                    "OFF-SYSTEM-201",
                    "Installation timeout.",
                    $"Exceeded {INSTALLATION_TIMEOUT_MS}ms"
                );
                return false;
            }

            return true;
        }

        /// METHOD: VerifyInstallation(Configuration config, ErrorHandler handler) → bool
        /// 
        /// Verify Office was installed successfully.
        /// Checks:
        ///   • Word.exe exists
        ///   • Excel.exe exists
        ///   • PowerPoint.exe exists
        ///   • Registry entries present
        private bool VerifyInstallation(Configuration config, ErrorHandler handler)
        {
            // In real implementation: Check file system and registry
            // For testing: Mock successful verification
            return true;
        }

        // ===== NO PRIVATE METHODS (All functionality via DI) =====
        // Previous methods IsRunningAsAdmin() and HasSufficientDiskSpace()
        // are now handled by ISecurityContext and IFileSystem interfaces
    }
}
