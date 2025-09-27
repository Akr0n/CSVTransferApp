# Guida alla Configurazione

Questa guida copre tutti gli aspetti di configurazione di CSV Transfer Application, dalle impostazioni di base a quelle avanzate per ambienti di produzione.

## Panoramica dei File di Configurazione

config/
├── appsettings.json # Configurazione principale
├── appsettings.Development.json # Override per sviluppo
├── appsettings.Production.json # Override per produzione
├── appsettings.Test.json # Override per test
├── database-connections.json # Template connessioni DB
├── sftp-connections.json # Template connessioni SFTP
├── logging.json # Configurazione logging
└── header-overrides/ # Override intestazioni CSV
├── employees.json
├── products.json
└── orders.json

## Configurazione Database

### Struttura Base

{
"DatabaseConnections": {
"NomeConnessione": {
"Provider": "Provider.Name",
"ConnectionString": "stringa di connessione",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true
}
}
}

### Oracle Database

{
"DatabaseConnections": {
"OracleMain": {
"Provider": "Oracle.EntityFrameworkCore",
"ConnectionString": "Data Source=oracle-server:1521/ORCL;User Id=csvapp;Password=password123;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true
},
"OracleReporting": {
"Provider": "Oracle.EntityFrameworkCore",
"ConnectionString": "Data Source=oracle-rep:1521/REPORTING;User Id=csvapp_ro;Password=password123;",
"ConnectionTimeout": 60,
"CommandTimeout": 600,
"MaxPoolSize": 50,
"IsEnabled": true
}
}
}

**Parametri Avanzati Oracle:**

{
"OracleAdvanced": {
"Provider": "Oracle.EntityFrameworkCore",
"ConnectionString": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=oracle-cluster)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=PROD)));User Id=csvapp;Password=password123;Connection Timeout=30;Pooling=true;Min Pool Size=5;Max Pool Size=100;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true,
"Features": {
"EnableBulkCopy": true,
"ArrayBindCount": 1000,
"FetchSize": 65536
}
}
}

### SQL Server

{
"DatabaseConnections": {
"SqlServerMain": {
"Provider": "Microsoft.EntityFrameworkCore.SqlServer",
"ConnectionString": "Server=sql-server;Database=ProductionDB;User Id=csvapp;Password=password123;TrustServerCertificate=true;Encrypt=true;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true
},
"SqlServerReadOnly": {
"Provider": "Microsoft.EntityFrameworkCore.SqlServer",
"ConnectionString": "Server=sql-server;Database=ProductionDB;User Id=csvapp_ro;Password=password123;TrustServerCertificate=true;Encrypt=true;ApplicationIntent=ReadOnly;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 200,
"IsEnabled": true
}
}
}

**Con Windows Authentication:**

{
"SqlServerWinAuth": {
"Provider": "Microsoft.EntityFrameworkCore.SqlServer",
"ConnectionString": "Server=sql-server;Database=ProductionDB;Integrated Security=true;TrustServerCertificate=true;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"IsEnabled": true
}
}

### PostgreSQL

{
"DatabaseConnections": {
"PostgreSQLMain": {
"Provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
"ConnectionString": "Host=postgres-server;Port=5432;Database=production;Username=csvapp;Password=password123;SSL Mode=Require;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true
},
"PostgreSQLCluster": {
"Provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
"ConnectionString": "Host=pg-primary,pg-secondary;Port=5432;Database=production;Username=csvapp;Password=password123;Target Session Attributes=read-write;",
"ConnectionTimeout": 30,
"CommandTimeout": 300,
"MaxPoolSize": 100,
"IsEnabled": true
}
}
}

## Configurazione SFTP

### Autenticazione con Password

{
"SftpConnections": {
"MainServer": {
"Host": "sftp.company.com",
"Port": 22,
"Username": "csvtransfer",
"Password": "secure_password_123",
"RemotePath": "/upload/csv",
"ConnectionTimeout": 30,
"IsEnabled": true,
"UseCompression": true,
"KeepAlive": 60
}
}
}

### Autenticazione con Chiave Privata

{
"SftpConnections": {
"SecureServer": {
"Host": "secure-sftp.company.com",
"Port": 22,
"Username": "csvtransfer",
"PrivateKeyPath": "/etc/csvapp/keys/sftp_key.pem",
"Passphrase": "key_passphrase_123",
"RemotePath": "/secure/uploads",
"ConnectionTimeout": 30,
"IsEnabled": true,
"VerifyHostKey": true,
"HostKeyFingerprint": "SHA256:abc123def456..."
}
}
}

### Configurazione con Failover

{
"SftpConnections": {
"PrimaryServer": {
"Host": "sftp-primary.company.com",
"Port": 22,
"Username": "csvtransfer",
"Password": "password123",
"RemotePath": "/primary/uploads",
"ConnectionTimeout": 30,
"IsEnabled": true,
"BackupOnFailure": true,
"BackupServer": "BackupServer"
},
"BackupServer": {
"Host": "sftp-backup.company.com",
"Port": 22,
"Username": "csvtransfer",
"Password": "password123",
"RemotePath": "/backup/uploads",
"ConnectionTimeout": 30,
"IsEnabled": true
}
}
}

## Configurazione Elaborazione

### Impostazioni di Base

{
"Processing": {
"MaxConcurrentConnections": 5,
"MaxConcurrentFiles": 10,
"MaxConcurrentJobs": 8,
"HeaderOverridePath": "./config/header-overrides",
"LogPath": "./logs",
"TempPath": "./temp"
}
}

### Impostazioni Avanzate

{
"Processing": {
"MaxConcurrentConnections": 10,
"MaxConcurrentFiles": 20,
"MaxConcurrentJobs": 15,
"HeaderOverridePath": "./config/header-overrides",
"LogPath": "./logs",
"TempPath": "./temp",
"BatchSize": 5000,
"EnableCompression": true,
"CompressionLevel": 6,
"FileEncoding": "UTF-8",
"CsvDelimiter": ",",
"CsvQuoteChar": """,
"CsvEscapeChar": """,
"IncludeHeaders": true,
"MaxFileSize": 500,
"RetryCount": 3,
"RetryDelay": 5000,
"EnableProgressReporting": true,
"ProgressReportInterval": 1000
}
}

### Ottimizzazioni per Grandi Volumi

{
"Processing": {
"MaxConcurrentConnections": 20,
"MaxConcurrentFiles": 50,
"BatchSize": 10000,
"EnableCompression": true,
"CompressionLevel": 9,
"UseStreamingExport": true,
"StreamingBufferSize": 65536,
"EnableParallelProcessing": true,
"ChunkSize": 100000,
"MemoryThreshold": 1073741824,
"EnableMemoryOptimization": true
}
}

## Override delle Intestazioni CSV

### Struttura del File Override

{
"TableName": "nome_tabella",
"Description": "Descrizione del mapping",
"LastModified": "2025-09-27T15:00:00Z",
"ColumnMappings": {
"nome_colonna_db": "Nome Friendly per CSV",
"employee_id": "Employee ID",
"first_name": "First Name"
},
"DataTypes": {
"Employee ID": "Integer",
"First Name": "Text"
},
"ValidationRules": {
"Employee ID": {
"Required": true,
"Type": "Integer",
"MinValue": 1
}
}
}

### Esempio Completo: employees.json

{
"TableName": "employees",
"Description": "Employee data export with localized headers",
"LastModified": "2025-09-27T15:00:00Z",
"ColumnMappings": {
"emp_id": "ID Dipendente",
"employee_id": "ID Dipendente",
"first_name": "Nome",
"last_name": "Cognome",
"email": "Email",
"phone": "Telefono",
"hire_date": "Data Assunzione",
"salary": "Stipendio",
"department_name": "Reparto",
"active": "Attivo"
},
"DataTypes": {
"ID Dipendente": "Integer",
"Nome": "Text",
"Cognome": "Text",
"Email": "Email",
"Telefono": "Phone",
"Data Assunzione": "Date",
"Stipendio": "Currency",
"Reparto": "Text",
"Attivo": "Boolean"
},
"FormatRules": {
"Stipendio": {
"Format": "Currency",
"CurrencySymbol": "€",
"DecimalPlaces": 2
},
"Data Assunzione": {
"Format": "Date",
"DateFormat": "dd/MM/yyyy"
},
"Attivo": {
"Format": "Boolean",
"TrueValue": "Sì",
"FalseValue": "No"
}
},
"ValidationRules": {
"ID Dipendente": {
"Required": true,
"Type": "Integer",
"MinValue": 1
},
"Email": {
"Required": true,
"Type": "Email",
"MaxLength": 100
},
"Stipendio": {
"Required": false,
"Type": "Decimal",
"MinValue": 0,
"MaxValue": 999999.99
}
}
}

## Configurazione del Logging

### Configurazione Base

{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft": "Warning",
"CSVTransferApp": "Debug"
},
"Console": {
"IncludeScopes": true,
"TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff"
},
"File": {
"Path": "./logs/app-.log",
"MaxSizeInMB": 10,
"MaxFiles": 5
}
}
}

### Configurazione Avanzata

{
"Logging": {
"LogLevel": {
"Default": "Information",
"Microsoft": "Warning",
"Microsoft.Hosting.Lifetime": "Information",
"CSVTransferApp": "Debug",
"CSVTransferApp.Data": "Trace",
"CSVTransferApp.Services.SftpService": "Debug"
},
"Console": {
"IncludeScopes": true,
"TimestampFormat": "yyyy-MM-dd HH:mm:ss.fff",
"UseUtc": true,
"Colors": {
"Information": "White",
"Warning": "Yellow",
"Error": "Red",
"Critical": "Magenta"
}
},
"File": {
"Path": "./logs/{Category}/app-.log",
"RollingInterval": "Day",
"MaxSizeInMB": 100,
"MaxFiles": 30,
"Format": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}"
},
"Serilog": {
"Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
"MinimumLevel": "Debug",
"WriteTo": [
{"Name": "Console"},
{
"Name": "File",
"Args": {
"path": "./logs/csvapp-.log",
"rollingInterval": "Day"
}
}
],
"Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"]
}
}
}

## Configurazione di Sicurezza

### Impostazioni Base

{
"Security": {
"EncryptPasswords": true,
"RequireSecureConnections": true,
"AllowedHosts": [
"sftp.company.com",
"backup-sftp.company.com"
],
"MaxLoginAttempts": 3,
"LockoutDuration": 300
}
}

### Configurazione Avanzata

{
"Security": {
"EncryptPasswords": true,
"RequireSecureConnections": true,
"PasswordEncryptionKey": "${ENCRYPTION_KEY}",
"AllowedHosts": [
"sftp.company.com",
"backup-sftp.company.com"
],
"BlockedHosts": [
"suspicious-server.com"
],
"MaxLoginAttempts": 3,
"LockoutDuration": 900,
"EnableAuditLogging": true,
"AuditLogPath": "./logs/audit",
"RequireTwoFactorAuth": false,
"SessionTimeout": 3600,
"EnableIPWhitelist": true,
"AllowedIPRanges": [
"192.168.1.0/24",
"10.0.0.0/8"
]
}
}

## Variabili d'Ambiente

### Configurazione delle Variabili

**Windows (.env file):**

Database Passwords
ORACLE_PASSWORD=super_secure_oracle_password
SQLSERVER_PASSWORD=super_secure_sqlserver_password
POSTGRES_PASSWORD=super_secure_postgres_password

SFTP Credentials
SFTP_PRIMARY_PASSWORD=secure_sftp_password
SFTP_SECONDARY_PASSWORD=secure_sftp_backup_password
SFTP_PRIMARY_KEY_PASSPHRASE=private_key_passphrase

Security
ENCRYPTION_KEY=your_32_character_encryption_key_here

Environment
ASPNETCORE_ENVIRONMENT=Production
CSV_APP_LOG_LEVEL=Information
CSV_APP_MAX_CONNECTIONS=10

**Linux (.bashrc o .profile):**

Database Passwords
export ORACLE_PASSWORD="super_secure_oracle_password"
export SQLSERVER_PASSWORD="super_secure_sqlserver_password"
export POSTGRES_PASSWORD="super_secure_postgres_password"

SFTP Credentials
export SFTP_PRIMARY_PASSWORD="secure_sftp_password"
export SFTP_SECONDARY_PASSWORD="secure_sftp_backup_password"
export SFTP_PRIMARY_KEY_PASSPHRASE="private_key_passphrase"

Security
export ENCRYPTION_KEY="your_32_character_encryption_key_here"

Environment
export ASPNETCORE_ENVIRONMENT="Production"
export CSV_APP_LOG_LEVEL="Information"
export CSV_APP_MAX_CONNECTIONS="10"

### Uso nel File di Configurazione

{
"DatabaseConnections": {
"Oracle": {
"ConnectionString": "Data Source=oracle-server:1521/ORCL;User Id=csvapp;Password=${ORACLE_PASSWORD};"
}
},
"SftpConnections": {
"MainServer": {
"Password": "${SFTP_PRIMARY_PASSWORD}",
"Passphrase": "${SFTP_PRIMARY_KEY_PASSPHRASE}"
}
}
}

## Configurazione per Ambienti Specifici

### Development

{
"Logging": {
"LogLevel": {
"Default": "Debug",
"CSVTransferApp": "Trace"
}
},
"Processing": {
"MaxConcurrentConnections": 2,
"MaxConcurrentFiles": 3,
"BatchSize": 100,
"RetryCount": 1
},
"Security": {
"EncryptPasswords": false,
"RequireSecureConnections": false
}
}

### Production

{
"Logging": {
"LogLevel": {
"Default": "Warning",
"CSVTransferApp": "Information"
}
},
"Processing": {
"MaxConcurrentConnections": 20,
"MaxConcurrentFiles": 50,
"BatchSize": 10000,
"RetryCount": 5
},
"Security": {
"EncryptPasswords": true,
"RequireSecureConnections": true,
"EnableAuditLogging": true
}
}

## Validazione della Configurazione

### Test della Configurazione

Valida tutta la configurazione
dotnet run validate-config

Valida solo le connessioni database
dotnet run validate-config --database-only

Valida solo le connessioni SFTP
dotnet run validate-config --sftp-only

Verifica le variabili d'ambiente
dotnet run validate-config --check-environment

### Output di Esempio

✅ Configuration Validation Results:

Database Connections:
✅ Oracle: Connection successful
✅ SqlServer: Connection successful
✅ PostgreSQL: Connection successful

SFTP Connections:
✅ MainServer: Connection successful
✅ BackupServer: Connection successful

Environment Variables:
✅ All required variables set
⚠️ ENCRYPTION_KEY not set (using default)

Security Settings:
✅ Password encryption enabled
✅ Secure connections required
✅ Host whitelist configured

Processing Settings:
✅ All parameters within valid ranges
ℹ️ MaxConcurrentConnections: 10 (recommended: 5-20)

## Troubleshooting Configurazione

### Problemi Comuni

**1. Errore: "Connection string invalid"**

Verifica la sintassi della stringa di connessione
dotnet run test --connection Oracle --verbose

**2. Errore: "SFTP authentication failed"**

Test manuale SFTP
sftp -P 22 username@hostname

Verifica permessi chiave privata
chmod 600 /path/to/private/key

**3. Errore: "Environment variable not found"**

Verifica variabili d'ambiente
env | grep CSV_APP
printenv ORACLE_PASSWORD

## Best Practices

### Sicurezza
- ✅ Usa sempre variabili d'ambiente per password
- ✅ Abilita crittografia delle password in produzione
- ✅ Limita i tentativi di login
- ✅ Usa chiavi SSH invece di password quando possibile

### Performance
- ✅ Regola `MaxConcurrentConnections` in base al carico del database
- ✅ Usa `BatchSize` appropriato per la dimensione dei dati
- ✅ Abilita compressione per file grandi
- ✅ Monitora l'utilizzo della memoria

### Manutenibilità
- ✅ Documenta i tuoi override delle intestazioni
- ✅ Usa nomi descrittivi per le connessioni
- ✅ Mantieni configurazioni separate per ambiente
- ✅ Versiona i tuoi file di configurazione

---

**📖 Prossimo: [Esempi di Utilizzo](EXAMPLES.md)**