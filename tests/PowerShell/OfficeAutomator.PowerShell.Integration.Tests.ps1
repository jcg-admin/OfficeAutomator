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
