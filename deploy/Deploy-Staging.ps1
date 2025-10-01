# Deployment script for staging environment

param(
    [string]$TargetEnvironment = "staging",
    [string]$ConfigPath = "./config",
    [string]$PublishPath = "./src/CSVTransferApp.Console/bin/Release/net9.0/publish"
)

# Verify environment variables
$requiredVars = @(
    "ORACLE_PASSWORD",
    "SQLSERVER_PASSWORD",
    "POSTGRES_PASSWORD",
    "SFTP_PASSWORD",
    "ENCRYPTION_KEY"
)

foreach ($var in $requiredVars) {
    if (-not [Environment]::GetEnvironmentVariable($var)) {
        Write-Error "Missing required environment variable: $var"
        exit 1
    }
}

# Create deployment directory
$deploymentPath = "./deploy/$TargetEnvironment"
New-Item -ItemType Directory -Force -Path $deploymentPath

# Copy application files
Copy-Item -Path "$PublishPath/*" -Destination $deploymentPath -Recurse -Force

# Copy configuration files
Copy-Item -Path "$ConfigPath/appsettings.$TargetEnvironment.json" -Destination "$deploymentPath/appsettings.json" -Force
Copy-Item -Path "$ConfigPath/database-connections.json" -Destination "$deploymentPath/" -Force
Copy-Item -Path "$ConfigPath/sftp-connections.json" -Destination "$deploymentPath/" -Force
Copy-Item -Path "$ConfigPath/header-overrides" -Destination "$deploymentPath/config/" -Recurse -Force

# Create logs directory
New-Item -ItemType Directory -Force -Path "$deploymentPath/logs"

Write-Host "Deployment package created at: $deploymentPath"
Write-Host "Verify the configuration files and deploy to $TargetEnvironment server"