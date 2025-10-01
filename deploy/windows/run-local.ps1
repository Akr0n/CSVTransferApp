param(
    [string]$ExePath = "$PSScriptRoot\..\..\src\CSVTransferApp.Console\bin\Release\net9.0\publish\CSVTransferApp.exe"
)

if (-not (Test-Path $ExePath)) {
    Write-Error "Executable not found at $ExePath. Please publish first."
    exit 1
}

Write-Output "Running $ExePath"
& $ExePath
