param(
    [string]$InstallPath = "$PSScriptRoot\..\..\src\CSVTransferApp.Console\bin\Release\net9.0\publish",
    [string]$ServiceName = "CSVTransferApp"
)

Write-Output "Installing $ServiceName as Windows service from $InstallPath"

$exe = Join-Path $InstallPath "CSVTransferApp.exe"
if (-not (Test-Path $exe)) {
    Write-Error "Executable not found at $exe. Please publish first."
    exit 1
}

# Try using sc.exe to create a simple service
$display = "$ServiceName Service"
sc.exe create $ServiceName binPath= `"$exe`" start= auto DisplayName= `"$display`" | Out-Null
sc.exe description $ServiceName "CSV Transfer Application service" | Out-Null

Write-Output "Service $ServiceName installed. Use 'sc start $ServiceName' to run."
