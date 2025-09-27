param(
    [string]$ConfigFile = "appsettings.json",
    [switch]$DatabaseOnly,
    [switch]$SftpOnly,
    [switch]$Verbose
)

Write-Host "CSV Transfer Application - Connection Test" -ForegroundColor Green

if (-not (Test-Path $ConfigFile)) {
    Write-Error "Config file not found: $ConfigFile"
    exit 1
}

try {
    # Read configuration
    $config = Get-Content $ConfigFile | ConvertFrom-Json
    
    $testResults = @()
    
    # Test database connections
    if (-not $SftpOnly) {
        Write-Host "`nTesting Database Connections..." -ForegroundColor Yellow
        
        if ($config.DatabaseConnections) {
            foreach ($connectionName in $config.DatabaseConnections.PSObject.Properties.Name) {
                Write-Host "  Testing $connectionName..." -NoNewline
                
                $arguments = @("test", "--type", "database", "--connection", $connectionName)
                if ($Verbose) { $arguments += "--verbose" }
                
                $result = & dotnet CSVTransferApp.Console.dll @arguments
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host " OK" -ForegroundColor Green
                    $testResults += @{ Type = "Database"; Name = $connectionName; Status = "Success" }
                } else {
                    Write-Host " FAILED" -ForegroundColor Red
                    $testResults += @{ Type = "Database"; Name = $connectionName; Status = "Failed" }
                }
            }
        }
    }
    
    # Test SFTP connections
    if (-not $DatabaseOnly) {
        Write-Host "`nTesting SFTP Connections..." -ForegroundColor Yellow
        
        if ($config.SftpConnections) {
            foreach ($connectionName in $config.SftpConnections.PSObject.Properties.Name) {
                Write-Host "  Testing $connectionName..." -NoNewline
                
                $arguments = @("test", "--type", "sftp", "--connection", $connectionName)
                if ($Verbose) { $arguments += "--verbose" }
                
                $result = & dotnet CSVTransferApp.Console.dll @arguments
                
                if ($LASTEXITCODE -eq 0) {
                    Write-Host " OK" -ForegroundColor Green
                    $testResults += @{ Type = "SFTP"; Name = $connectionName; Status = "Success" }
                } else {
                    Write-Host " FAILED" -ForegroundColor Red
                    $testResults += @{ Type = "SFTP"; Name = $connectionName; Status = "Failed" }
                }
            }
        }
    }
    
    # Summary
    Write-Host "`nConnection Test Summary:" -ForegroundColor Cyan
    $successCount = ($testResults | Where-Object { $_.Status -eq "Success" }).Count
    $failedCount = ($testResults | Where-Object { $_.Status -eq "Failed" }).Count
    
    Write-Host "  Successful: $successCount" -ForegroundColor Green
    Write-Host "  Failed: $failedCount" -ForegroundColor Red
    
    if ($failedCount -gt 0) {
        Write-Host "`nFailed Connections:" -ForegroundColor Red
        $testResults | Where-Object { $_.Status -eq "Failed" } | ForEach-Object {
            Write-Host "  - $($_.Type): $($_.Name)" -ForegroundColor Red
        }
        exit 1
    }
    
    Write-Host "`nAll connections tested successfully!" -ForegroundColor Green
    exit 0
}
catch {
    Write-Error "Error testing connections: $($_.Exception.Message)"
    exit 1
}
