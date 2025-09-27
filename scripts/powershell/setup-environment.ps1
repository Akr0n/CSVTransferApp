param(
    [string]$Environment = "Development",
    [string]$ConfigPath = "./config",
    [switch]$Force
)

Write-Host "CSV Transfer Application - Environment Setup" -ForegroundColor Green
Write-Host "Environment: $Environment" -ForegroundColor Yellow

# Create directory structure
$directories = @(
    "./logs",
    "./logs/application",
    "./logs/transfers",
    "./logs/errors",
    "./config",
    "./config/header-overrides",
    "./temp"
)

Write-Host "`nCreating directory structure..." -ForegroundColor Cyan
foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "  Created: $dir" -ForegroundColor Green
    } else {
        Write-Host "  Exists: $dir" -ForegroundColor Gray
    }
}

# Copy configuration templates
$configFiles = @{
    "appsettings.$Environment.json" = "./config/appsettings.$Environment.json.template"
    "database-connections.json" = "./config/database-connections.json.template"
    "sftp-connections.json" = "./config/sftp-connections.json.template"
}

Write-Host "`nSetting up configuration files..." -ForegroundColor Cyan
foreach ($configFile in $configFiles.Keys) {
    $targetPath = $configFile
    $templatePath = $configFiles[$configFile]
    
    if ((Test-Path $targetPath) -and -not $Force) {
        Write-Host "  Skipped: $targetPath (already exists)" -ForegroundColor Yellow
    } else {
        if (Test-Path $templatePath) {
            Copy-Item $templatePath $targetPath -Force
            Write-Host "  Created: $targetPath" -ForegroundColor Green
        } else {
            # Create basic template
            $basicConfig = @{
                "Logging" = @{
                    "LogLevel" = @{
                        "Default" = "Information"
                    }
                }
                "DatabaseConnections" = @{}
                "SftpConnections" = @{}
                "Processing" = @{
                    "MaxConcurrentConnections" = 5
                    "MaxConcurrentFiles" = 10
                    "HeaderOverridePath" = "./config/header-overrides"
                    "LogPath" = "./logs"
                }
            }
            
            $basicConfig | ConvertTo-Json -Depth 10 | Set-Content $targetPath
            Write-Host "  Created: $targetPath (basic template)" -ForegroundColor Green
        }
    }
}

# Set permissions (Windows)
if ($IsWindows -or $env:OS -eq "Windows_NT") {
    Write-Host "`nSetting permissions..." -ForegroundColor Cyan
    try {
        icacls "./logs" /grant "${env:USERNAME}:(OI)(CI)F" /T | Out-Null
        icacls "./config" /grant "${env:USERNAME}:(OI)(CI)F" /T | Out-Null
        Write-Host "  Permissions set successfully" -ForegroundColor Green
    } catch {
        Write-Warning "Could not set permissions: $($_.Exception.Message)"
    }
}

# Create sample header override
$sampleOverride = @{
    "ColumnMappings" = @{
        "emp_id" = "Employee ID"
        "first_name" = "First Name"
        "last_name" = "Last Name"
        "hire_date" = "Hire Date"
    }
}

$overridePath = "./config/header-overrides/employees.json"
if (-not (Test-Path $overridePath) -or $Force) {
    $sampleOverride | ConvertTo-Json -Depth 5 | Set-Content $overridePath
    Write-Host "  Created sample header override: $overridePath" -ForegroundColor Green
}

Write-Host "`nEnvironment setup completed successfully!" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Edit configuration files in ./config/" -ForegroundColor White
Write-Host "  2. Set up database and SFTP connections" -ForegroundColor White
Write-Host "  3. Test connections: ./test-connections.ps1" -ForegroundColor White
Write-Host "  4. Run your first transfer!" -ForegroundColor White
Write-Host "  5. Review logs in ./logs/" -ForegroundColor White