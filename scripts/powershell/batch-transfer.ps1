param(
    [Parameter(Mandatory=$true)]
    [string]$BatchFile,
    
    [string]$ConfigFile = "appsettings.json",
    [switch]$Verbose
)

Write-Host "CSV Transfer Application - Batch Transfer" -ForegroundColor Green
Write-Host "Batch File: $BatchFile" -ForegroundColor Yellow

if (-not (Test-Path $BatchFile)) {
    Write-Error "Batch file not found: $BatchFile"
    exit 1
}

if (-not (Test-Path $ConfigFile)) {
    Write-Warning "Config file not found: $ConfigFile. Using default configuration."
}

$startTime = Get-Date
Write-Host "Starting batch transfer at $startTime" -ForegroundColor Cyan

try {
    $arguments = @("batch", "--file", $BatchFile)
    
    if ($Verbose) {
        $arguments += "--verbose"
    }
    
    Write-Host "Executing: dotnet CSVTransferApp.Console.dll $($arguments -join ' ')" -ForegroundColor Gray
    
    $exitCode = & dotnet CSVTransferApp.Console.dll @arguments
    
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    if ($exitCode -eq 0) {
        Write-Host "Batch transfer completed successfully in $($duration.ToString('hh\:mm\:ss'))" -ForegroundColor Green
    } else {
        Write-Error "Batch transfer failed with exit code: $exitCode"
    }
    
    exit $exitCode
}
catch {
    Write-Error "Error executing batch transfer: $($_.Exception.Message)"
    exit 1
}
