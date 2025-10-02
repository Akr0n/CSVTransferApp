# CSVTransferApp Installation Script
# Run as Administrator

param(
    [Parameter(Mandatory=$true)]
    [string]$InstallPath = "C:\CSVTransferApp"
)

Write-Host "Installing CSVTransferApp to $InstallPath..." -ForegroundColor Green

# Create installation directory
if (!(Test-Path $InstallPath)) {
    New-Item -Path $InstallPath -ItemType Directory -Force
    Write-Host "Created installation directory: $InstallPath"
}

# Copy application files
$PublishPath = Split-Path $PSScriptRoot -Parent | Join-Path -ChildPath "publish"
Copy-Item -Path "$PublishPath\*" -Destination $InstallPath -Recurse -Force
Write-Host "Application files copied successfully"

# Set permissions
icacls $InstallPath /grant "Everyone:(OI)(CI)F" /T
Write-Host "Permissions set successfully"

# Create desktop shortcut (optional)
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$Home\Desktop\CSVTransferApp.lnk")
$Shortcut.TargetPath = "$InstallPath\CSVTransferApp.exe"
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.Save()
Write-Host "Desktop shortcut created"

Write-Host "Installation completed successfully!" -ForegroundColor Green
Write-Host "You can now run: $InstallPath\CSVTransferApp.exe" -ForegroundColor Yellow

# Test installation
Write-Host "Testing installation..." -ForegroundColor Blue
& "$InstallPath\CSVTransferApp.exe" --help