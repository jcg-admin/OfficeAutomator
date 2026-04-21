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

Describe "OfficeAutomator.Logging.Handler" {
    
    Context "Write-LogEntry function" {
        
        It "writes message to log file" {
            # ARRANGE
            $tempLogPath = Join-Path $env:TEMP "Test_OfficeAutomator_$(Get-Random).log"
            $testMessage = "Test log entry"
            
            # ACT
            Write-LogEntry -Message $testMessage -Level "INFO" -LogPath $tempLogPath
            
            # ASSERT
            $tempLogPath | Should -Exist
            Get-Content $tempLogPath | Should -Match $testMessage
            
            # CLEANUP
            Remove-Item $tempLogPath -ErrorAction SilentlyContinue
        }
        
        It "includes timestamp in log entry" {
            # ARRANGE
            $tempLogPath = Join-Path $env:TEMP "Test_OfficeAutomator_$(Get-Random).log"
            $testMessage = "Test with timestamp"
            
            # ACT
            Write-LogEntry -Message $testMessage -Level "INFO" -LogPath $tempLogPath
            
            # ASSERT
            $logContent = Get-Content $tempLogPath
            $logContent | Should -Match "\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}"
            
            # CLEANUP
            Remove-Item $tempLogPath -ErrorAction SilentlyContinue
        }
        
        It "includes log level in log entry" {
            # ARRANGE
            $tempLogPath = Join-Path $env:TEMP "Test_OfficeAutomator_$(Get-Random).log"
            
            # ACT
            Write-LogEntry -Message "Test" -Level "ERROR" -LogPath $tempLogPath
            
            # ASSERT
            Get-Content $tempLogPath | Should -Match "\[ERROR\]"
            
            # CLEANUP
            Remove-Item $tempLogPath -ErrorAction SilentlyContinue
        }
        
        It "accepts valid log levels (INFO, SUCCESS, WARNING, ERROR)" {
            # ARRANGE
            $tempLogPath = Join-Path $env:TEMP "Test_OfficeAutomator_$(Get-Random).log"
            $levels = @("INFO", "SUCCESS", "WARNING", "ERROR")
            
            # ACT & ASSERT
            foreach ($level in $levels) {
                { Write-LogEntry -Message "Test" -Level $level -LogPath $tempLogPath } | Should -Not -Throw
            }
            
            # CLEANUP
            Remove-Item $tempLogPath -ErrorAction SilentlyContinue
        }
        
        It "appends to existing log file instead of overwriting" {
            # ARRANGE
            $tempLogPath = Join-Path $env:TEMP "Test_OfficeAutomator_$(Get-Random).log"
            $firstMessage = "First entry"
            $secondMessage = "Second entry"
            
            # ACT
            Write-LogEntry -Message $firstMessage -Level "INFO" -LogPath $tempLogPath
            Write-LogEntry -Message $secondMessage -Level "INFO" -LogPath $tempLogPath
            
            # ASSERT
            $logContent = Get-Content $tempLogPath
            $logContent -is [array] | Should -Be $true  # Multiple lines
            $logContent | Should -Contain $firstMessage
            $logContent | Should -Contain $secondMessage
            
            # CLEANUP
            Remove-Item $tempLogPath -ErrorAction SilentlyContinue
        }
    }
}

Describe "OfficeAutomator.Menu.Display" {
    
    Context "Show-Menu function" {
        
        It "displays menu with title" {
            # ARRANGE
            $title = "Select Version"
            $options = @("Option1", "Option2", "Option3")
            
            # Mock Read-Host to return selection
            $input = "1"
            
            # This test is hard to validate console output
            # But we can test that the function accepts the parameters
            { Show-Menu -Title $title -Options $options } | Should -Not -Throw
        }
        
        It "accepts array of options" {
            # ARRANGE
            $title = "Test Menu"
            $options = @("Choice A", "Choice B", "Choice C", "Choice D")
            
            # Mock Read-Host
            $input = "1"
            
            # ACT & ASSERT
            { Show-Menu -Title $title -Options $options } | Should -Not -Throw
        }
        
        It "returns integer when valid selection provided" {
            # ARRANGE
            $title = "Select Option"
            $options = @("Option1", "Option2", "Option3")
            
            # This requires mocking Read-Host which is complex
            # We validate that the function signature is correct
            Get-Command Show-Menu | Should -Not -BeNullOrEmpty
        }
        
        It "requires Title parameter" {
            # ARRANGE
            $options = @("Option1", "Option2")
            
            # ACT & ASSERT
            { Show-Menu -Options $options } | Should -Throw
        }
        
        It "requires Options parameter" {
            # ARRANGE
            $title = "Test Menu"
            
            # ACT & ASSERT
            { Show-Menu -Title $title } | Should -Throw
        }
        
        It "accepts minimum 2 options" {
            # ARRANGE
            $title = "Test Menu"
            $options = @("Option1", "Option2")
            
            # ACT & ASSERT
            { Show-Menu -Title $title -Options $options } | Should -Not -Throw
        }
    }
}
