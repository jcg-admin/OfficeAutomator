@{
    Run = @{
        Path = '.\tests\PowerShell\'
        PassThru = $true
        TestExtension = '.Tests.ps1'
        Exit = $true
    }
    
    Filter = @{
        Tag = @()
        ExcludeTag = @()
    }
    
    TestResult = @{
        Enabled = $true
        OutputFormat = 'NUnitXml'
        OutputPath = '.\test-results.xml'
    }
    
    CodeCoverage = @{
        Enabled = $true
        Path = '.\scripts\functions\*'
        OutputFormat = 'JaCoCo'
        OutputPath = '.\coverage.xml'
        CoveragePercentTarget = 90
    }
    
    Should = @{
        ErrorAction = 'Continue'
    }
    
    Debug = @{
        ShowNavigationMarkers = $false
        WriteVSCodeMarker = $false
    }
}
