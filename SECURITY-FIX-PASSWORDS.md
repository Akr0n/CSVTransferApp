# Security Fix: Password Remediation

## Overview
This document describes the security fixes applied to remove hardcoded passwords from configuration files.

## Files Modified

### 1. Docker Compose (`docker-compose.local.yml`)
**Issues Fixed:**
- PostgreSQL password: `localdev123` → `${POSTGRES_PASSWORD:-localdev123}`
- SQL Server password: `LocalDev123!` → `${SQLSERVER_SA_PASSWORD:-LocalDev123!}`
- Oracle password: `localdev123` → `${ORACLE_PWD:-localdev123}`
- Updated healthchecks to use environment variables

### 2. Database Connections (`config/database-connections.json`)
**Issues Fixed:**
- Oracle Development: `localdev123` → `${ORACLE_PASSWORD}`
- SQL Server Development: `LocalDev123!` → `${SQLSERVER_PASSWORD}`
- PostgreSQL Development: `localdev123` → `${POSTGRESQL_PASSWORD}`

### 3. Application Settings (`config/appsettings.Local.json`)
**Issues Fixed:**
- Oracle connection string password
- SQL Server connection string password
- PostgreSQL connection string password
- SFTP connection password
- Encryption key

### 4. Console App Settings (`src/CSVTransferApp.Console/appsettings.json`)
**Issues Fixed:**
- All database connection string passwords
- All SFTP connection passwords and passphrases

### 5. PowerShell Script (`scripts/powershell/Initialize-LocalEnvironment.ps1`)
**Issues Fixed:**
- SQL Server command password: hardcoded → `$env:SQLSERVER_SA_PASSWORD`

### 6. Documentation (`docs/CONFIGURATION.md`)
**Issues Fixed:**
- Replaced all example passwords with environment variable placeholders
- Updated all connection string examples
- Updated SFTP configuration examples

## Environment Variables Added

### Database Passwords
- `ORACLE_PASSWORD` - Oracle development password
- `SQLSERVER_PASSWORD` - SQL Server development password  
- `POSTGRESQL_PASSWORD` - PostgreSQL development password
- `ORACLE_PROD_PASSWORD` - Oracle production password
- `ORACLE_RO_PASSWORD` - Oracle read-only password
- `SQLSERVER_PROD_PASSWORD` - SQL Server production password
- `SQLSERVER_RO_PASSWORD` - SQL Server read-only password
- `POSTGRESQL_PROD_PASSWORD` - PostgreSQL production password
- `POSTGRESQL_CLUSTER_PASSWORD` - PostgreSQL cluster password

### Docker Environment
- `POSTGRES_PASSWORD` - PostgreSQL container password
- `SQLSERVER_SA_PASSWORD` - SQL Server container SA password
- `ORACLE_PWD` - Oracle container password

### SFTP Passwords
- `SFTP_PASSWORD` - Generic SFTP password
- `SFTP_LOCALTEST_PASSWORD` - Local test SFTP password
- `SFTP_MAINSERVER_PASSWORD` - Main server SFTP password
- `SFTP_BACKUPSERVER_PASSWORD` - Backup server SFTP password
- `SFTP_BACKUPSERVER_PASSPHRASE` - Backup server key passphrase
- `SFTP_TESTSERVER_PASSWORD` - Test server SFTP password
- `SFTP_SECURE_PASSWORD` - Secure SFTP password
- `SFTP_PRIMARY_PASSWORD` - Primary SFTP password
- `SFTP_BACKUP_PASSWORD` - Backup SFTP password

### Security
- `ENCRYPTION_KEY` - Application encryption key

## Files Created/Updated

1. **`.env.example`** - Template for environment variables
2. **`.env.local`** - Local development environment file (with default values)
3. **`.gitignore`** - Updated to exclude environment files

## Security Benefits

1. **No hardcoded passwords** in version control
2. **Environment-specific configuration** support
3. **Production-ready** security practices
4. **Easy password rotation** without code changes
5. **Audit trail** for configuration changes

## Next Steps for Production

1. Create `.env` file with production passwords
2. Use Azure Key Vault or similar for password management
3. Implement password rotation policies
4. Monitor for any remaining hardcoded secrets

## Local Development Setup

1. Copy `.env.local` to `.env` if needed
2. Update passwords in `.env` as required
3. Run `docker-compose -f docker-compose.local.yml up -d`
4. Environment variables will be automatically loaded

## Important Notes

- ⚠️ **Never commit `.env` files to version control**
- ✅ `.env.example` is safe to commit (contains no real passwords)
- ✅ `.env.local` contains default development values only
- 🔒 Use proper secrets management in production environments