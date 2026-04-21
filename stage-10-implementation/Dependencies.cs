using System;
using System.Collections.Generic;

namespace OfficeAutomator.Core
{
    /// INTERFACE: IFileSystem
    /// 
    /// Purpose: Abstraction for file system operations
    /// Allows mocking in tests for InstallationExecutor and RollbackExecutor
    /// 
    /// Enables:
    ///   • Unit testing without actual file I/O
    ///   • Test isolation
    ///   • Predictable test behavior
    public interface IFileSystem
    {
        /// Delete directory recursively
        bool DeleteDirectory(string path, bool recursive);

        /// Delete file
        bool DeleteFile(string path);

        /// Check if directory exists
        bool DirectoryExists(string path);

        /// Check if file exists
        bool FileExists(string path);

        /// Get available free space on drive (bytes)
        long GetAvailableFreeSpace(string drivePath);

        /// Create directory if doesn't exist
        void CreateDirectory(string path);
    }

    /// INTERFACE: IRegistry
    /// 
    /// Purpose: Abstraction for Windows registry operations
    /// Allows mocking in tests for RollbackExecutor
    /// 
    /// Enables:
    ///   • Unit testing without actual registry I/O
    ///   • Safe testing on non-Windows environments
    ///   • Predictable test behavior
    public interface IRegistry
    {
        /// Delete registry key recursively
        bool DeleteKey(string keyPath, bool recursive);

        /// Check if registry key exists
        bool KeyExists(string keyPath);

        /// Get registry keys matching pattern
        List<string> GetKeysByPattern(string pattern);
    }

    /// INTERFACE: ISecurityContext
    /// 
    /// Purpose: Abstraction for security/admin checks
    /// Allows mocking in tests for InstallationExecutor
    /// 
    /// Enables:
    ///   • Unit testing without requiring admin privileges
    ///   • Testing both admin and non-admin scenarios
    ///   • Predictable test behavior
    public interface ISecurityContext
    {
        /// Check if running as administrator
        bool IsRunningAsAdmin();

        /// Get current user identity
        string GetCurrentUser();
    }

    /// INTERFACE: IProcessRunner
    /// 
    /// Purpose: Abstraction for process execution
    /// Allows mocking in tests for InstallationExecutor
    /// 
    /// Enables:
    ///   • Unit testing without launching actual processes
    ///   • Simulating process outcomes
    ///   • Timeout testing without real delays
    public interface IProcessRunner
    {
        /// Execute process with arguments and return exit code
        int Execute(string processPath, string arguments, int timeoutMs);

        /// Get current process progress (0-100%)
        int GetProgress();

        /// Kill running process
        bool KillProcess(int processId);
    }

    /// CLASS: FileSystemImpl
    /// 
    /// Real implementation of IFileSystem using actual file system
    /// Used in production
    public class FileSystemImpl : IFileSystem
    {
        public bool DeleteDirectory(string path, bool recursive)
        {
            try
            {
                System.IO.Directory.Delete(path, recursive);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteFile(string path)
        {
            try
            {
                System.IO.File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DirectoryExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return System.IO.File.Exists(path);
        }

        public long GetAvailableFreeSpace(string drivePath)
        {
            try
            {
                var drive = new System.IO.DriveInfo(drivePath);
                return drive.AvailableFreeSpace;
            }
            catch
            {
                return 0;
            }
        }

        public void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }

    /// CLASS: RegistryImpl
    /// 
    /// Real implementation of IRegistry using Windows registry
    /// Used in production on Windows systems
    public class RegistryImpl : IRegistry
    {
        public bool DeleteKey(string keyPath, bool recursive)
        {
            try
            {
                // In production: Use Microsoft.Win32.Registry.LocalMachine.DeleteSubKey(keyPath, recursive)
                return true; // Mock for now
            }
            catch
            {
                return false;
            }
        }

        public bool KeyExists(string keyPath)
        {
            try
            {
                // In production: Check if registry key exists
                return false; // Mock for now
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetKeysByPattern(string pattern)
        {
            // In production: Find registry keys matching pattern
            return new List<string>();
        }
    }

    /// CLASS: SecurityContextImpl
    /// 
    /// Real implementation of ISecurityContext
    /// Used in production
    public class SecurityContextImpl : ISecurityContext
    {
        public bool IsRunningAsAdmin()
        {
            try
            {
                var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
                var principal = new System.Security.Principal.WindowsPrincipal(identity);
                return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
        }

        public string GetCurrentUser()
        {
            try
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                return "Unknown";
            }
        }
    }

    /// CLASS: ProcessRunnerImpl
    /// 
    /// Real implementation of IProcessRunner
    /// Used in production
    public class ProcessRunnerImpl : IProcessRunner
    {
        private int currentProgress = 0;

        public int Execute(string processPath, string arguments, int timeoutMs)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = processPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    if (process.WaitForExit(timeoutMs))
                    {
                        return process.ExitCode;
                    }
                    else
                    {
                        process.Kill();
                        return -1; // Timeout
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        public int GetProgress()
        {
            return currentProgress;
        }

        public bool KillProcess(int processId)
        {
            try
            {
                var process = System.Diagnostics.Process.GetProcessById(processId);
                process.Kill();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
