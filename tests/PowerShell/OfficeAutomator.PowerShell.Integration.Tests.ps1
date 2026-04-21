param(
    [string]$ConfigPath = "$PSScriptRoot\pester.configuration.psd1"
)

# Import Pester configuration
$config = if (Test-Path $ConfigPath) {
    Import-PowerShellDataFile $ConfigPath
} else {
    @{}
}

Describe "OfficeAutomator.CoreDll.Loader" {
    
    Context "Load-OfficeAutomatorCoreDll function" {
        
        It "loads DLL successfully when file exists" {
            # ARRANGE
            $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
            
            # Verify test setup
            $dllPath | Should -Exist
            
            # ACT
            $result = Load-OfficeAutomatorCoreDll -DllPath $dllPath
            
            # ASSERT
            $result | Should -Be $true
        }
        
        It "throws error when DLL file not found" {
            # ARRANGE
            $invalidPath = ".\nonexistent\path\fake.dll"
            
            # ACT & ASSERT
            { Load-OfficeAutomatorCoreDll -DllPath $invalidPath } | Should -Throw
        }
        
        It "verifies OfficeAutomator.Core.Models.Configuration class exists after load" {
            # ARRANGE
            $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
            
            # ACT
            $result = Load-OfficeAutomatorCoreDll -DllPath $dllPath
            
            # ASSERT
            $result | Should -Be $true
            { [OfficeAutomator.Core.Models.Configuration] } | Should -Not -Throw
        }
        
        It "verifies ConfigValidator class exists after load" {
            # ARRANGE
            $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
            
            # ACT
            $result = Load-OfficeAutomatorCoreDll -DllPath $dllPath
            
            # ASSERT
            $result | Should -Be $true
            { [OfficeAutomator.Core.Validation.ConfigValidator] } | Should -Not -Throw
        }
        
        It "throws when DLL is not valid .NET assembly" {
            # ARRANGE
            $invalidDllPath = ".\tests\PowerShell\pester.configuration.psd1"  # Not a DLL
            
            # ACT & ASSERT
            { Load-OfficeAutomatorCoreDll -DllPath $invalidDllPath } | Should -Throw
        }
        
        It "handles exception gracefully with descriptive error message" {
            # ARRANGE
            $invalidPath = ".\fake.dll"
            
            # ACT & ASSERT
            $errorMessage = $null
            try {
                Load-OfficeAutomatorCoreDll -DllPath $invalidPath
            }
            catch {
                $errorMessage = $_.Exception.Message
            }
            
            $errorMessage | Should -Not -BeNullOrEmpty
        }
    }
}

Describe "OfficeAutomator.Validation.Environment" {
    
    Context "Test-AdminRights function" {
        
        It "returns true if running as administrator" {
            # ACT
            $result = Test-AdminRights
            
            # ASSERT - This test assumes it's being run with admin privileges
            if ([System.Security.Principal.WindowsIdentity]::GetCurrent().Groups -contains "S-1-5-32-544") {
                $result | Should -Be $true
            }
        }
        
        It "returns boolean value" {
            # ACT
            $result = Test-AdminRights
            
            # ASSERT
            $result | Should -BeOfType [System.Boolean]
        }
    }
    
    Context "Test-CoreDllExists function" {
        
        It "returns true when DLL file exists" {
            # ARRANGE
            $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
            
            # Skip test if DLL doesn't exist (test environment issue)
            if (-not (Test-Path $dllPath)) {
                Set-ItResult -Skipped -Because "DLL not found in test environment"
                return
            }
            
            # ACT
            $result = Test-CoreDllExists -DllPath $dllPath
            
            # ASSERT
            $result | Should -Be $true
        }
        
        It "returns false when DLL file does not exist" {
            # ARRANGE
            $invalidPath = ".\nonexistent\path\fake.dll"
            
            # ACT
            $result = Test-CoreDllExists -DllPath $invalidPath
            
            # ASSERT
            $result | Should -Be $false
        }
        
        It "returns boolean value" {
            # ARRANGE
            $invalidPath = ".\fake.dll"
            
            # ACT
            $result = Test-CoreDllExists -DllPath $invalidPath
            
            # ASSERT
            $result | Should -BeOfType [System.Boolean]
        }
    }
    
    Context "Test-DotNetRuntime function" {
        
        It "returns true if .NET 8.0+ installed" {
            # ACT
            $result = Test-DotNetRuntime
            
            # ASSERT - Check if .NET 8.0 is available
            $frameworkDesc = [System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription
            if ($frameworkDesc -match "8\.|9\.") {
                $result | Should -Be $true
            }
        }
        
        It "returns boolean value" {
            # ACT
            $result = Test-DotNetRuntime
            
            # ASSERT
            $result | Should -BeOfType [System.Boolean]
        }
        
        It "detects .NET runtime correctly" {
            # ACT
            $result = Test-DotNetRuntime
            
            # Get actual framework version
            $frameworkDesc = [System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription
            
            # ASSERT
            if ($frameworkDesc -match "Core 8\.|Core 9\.") {
                $result | Should -Be $true
            } else {
                $result | Should -Be $false
            }
        }
    }
    
    Context "Test-PrerequisitesMet function" {
        
        It "returns true when all prerequisites met" {
            # ARRANGE
            $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
            
            # Skip if running without admin or if DLL missing
            if (-not ([System.Security.Principal.WindowsIdentity]::GetCurrent().Groups -contains "S-1-5-32-544")) {
                Set-ItResult -Skipped -Because "Not running with admin privileges"
                return
            }
            
            if (-not (Test-Path $dllPath)) {
                Set-ItResult -Skipped -Because "DLL not found in test environment"
                return
            }
            
            # ACT
            $result = Test-PrerequisitesMet -DllPath $dllPath
            
            # ASSERT
            $result | Should -Be $true
        }
        
        It "returns false when DLL path invalid" {
            # ARRANGE
            $invalidPath = ".\fake.dll"
            
            # ACT
            $result = Test-PrerequisitesMet -DllPath $invalidPath
            
            # ASSERT
            $result | Should -Be $false
        }
        
        It "returns boolean value" {
            # ARRANGE
            $invalidPath = ".\fake.dll"
            
            # ACT
            $result = Test-PrerequisitesMet -DllPath $invalidPath
            
            # ASSERT
            $result | Should -BeOfType [System.Boolean]
        }
    }
}
