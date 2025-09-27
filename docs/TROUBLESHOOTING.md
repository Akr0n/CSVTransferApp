# Risoluzione Problemi

Questa guida contiene soluzioni ai problemi più comuni che potresti incontrare utilizzando CSV Transfer Application.

## Problemi di Installazione

### ❌ Errore: "dotnet command not found"

**Sintomi:**

bash: dotnet: command not found

**Cause:**
- .NET SDK non installato
- .NET SDK non nel PATH

**Soluzioni:**

**Windows:**

Verifica installazione
dotnet --version

Se non trovato, installa via winget
winget install Microsoft.DotNet.SDK.9

Oppure scarica da https://dotnet.microsoft.com/download

**Linux (Ubuntu/Debian):**

Aggiorna package index
sudo apt update

Installa .NET SDK
sudo apt install dotnet-sdk-9.0

Verifica installazione
dotnet --version

Se ancora non trovato, aggiungi al PATH
export PATH=$PATH:/usr/share/dotnet
echo 'export PATH=$PATH:/usr/share/dotnet' >> ~/.bashrc
source ~/.bashrc

**Linux (CentOS/RHEL):**

Installa repository Microsoft
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

Installa .NET SDK
sudo yum install dotnet-sdk-9.0

### ❌ Errore: "Could not load file or assembly"

**Sintomi:**

Could not load file or assembly 'System.Data.SqlClient, Version=4.8.0.0'

**Soluzioni:**

Ripristina dipendenze NuGet
dotnet restore

Pulisci e rebuilda
dotnet clean
dotnet build

Se persiste, verifica target framework
In tutti i .csproj deve essere <TargetFramework>net9.0</TargetFramework>

### ❌ Errore: "The specified framework 'Microsoft.NETCore.App', version 'x.x.x' was not found"

**Soluzioni:**

Lista runtime installati
dotnet --list-runtimes

Lista SDK installati
dotnet --list-sdks

Se mancante, installa il runtime necessario
dotnet --install-additional-deps-runtime

## Problemi di Database

### ❌ Oracle: "ORA-12154: TNS:could not resolve the connect identifier specified"

**Sintomi:**
- Connessione Oracle fallisce
- Errore TNS

**Soluzioni:**

**1. Verifica Connection String:**

{
"ConnectionString": "Data Source=hostname:1521/servicename;User Id=username;Password=password;"
}

**2. Verifica TNS Names (se utilizzato):**

Verifica ORACLE_HOME
echo $ORACLE_HOME

Verifica tnsnames.ora
cat $ORACLE_HOME/network/admin/tnsnames.ora

Test connessione con sqlplus
sqlplus username/password@hostname:1521/servicename

**3. Usa connessione diretta senza TNS:**

{
"ConnectionString": "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=hostname)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=servicename)));User Id=username;Password=password;"
}

### ❌ SQL Server: "Login failed for user"

**Sintomi:**

Microsoft.Data.SqlClient.SqlException: Login failed for user 'username'

**Soluzioni:**

**1. Verifica credenziali:**

Test manuale connessione
sqlcmd -S servername -U username -P password -d database

**2. Verifica autenticazione SQL:**

-- Su SQL Server Management Studio
-- Verifica che sia abilitata autenticazione mista
SELECT SERVERPROPERTY('IsIntegratedSecurityOnly')
-- Deve restituire 0 per autenticazione mista

**3. Connection string con Integrated Security:**

{
"ConnectionString": "Server=servername;Database=database;Integrated Security=true;TrustServerCertificate=true;"
}

**4. Verifica firewall:**

Windows - Abilita porta SQL Server
netsh advfirewall firewall add rule name="SQL Server" dir=in action=allow protocol=TCP localport=1433

### ❌ PostgreSQL: "FATAL: password authentication failed"

**Sintomi:**

Npgsql.PostgresException: FATAL: password authentication failed for user "username"

**Soluzioni:**

**1. Verifica credenziali:**

Test connessione
psql -h hostname -U username -d database -W

**2. Verifica pg_hba.conf:**

Trova file configurazione
sudo -u postgres psql -c "SHOW hba_file;"

Verifica regole autenticazione
sudo cat /etc/postgresql/14/main/pg_hba.conf

Assicurati che ci sia una riga tipo:
host all all 0.0.0.0/0 md5

**3. Connection string con SSL:**

{
"ConnectionString": "Host=hostname;Database=database;Username=username;Password=password;SSL Mode=Require;"
}

### ❌ Errore: "Timeout expired. The timeout period elapsed prior to completion"

**Sintomi:**

- Query molto lente
- Timeout durante l'estrazione dati

**Soluzioni:**

**1. Aumenta timeout:**

{
"DatabaseConnections": {
"YourConnection": {
"ConnectionTimeout": 120,
"CommandTimeout": 1800
}
}
}

**2. Ottimizza query:**

-- Usa LIMIT/TOP per test
SELECT TOP 1000 * FROM large_table

-- Verifica indici
EXPLAIN PLAN FOR SELECT * FROM your_table WHERE your_column = 'value';

**3. Usa batch processing:**

{
"Processing": {
"BatchSize": 1000,
"UseStreamingExport": true
}
}

## Problemi SFTP

### ❌ Errore: "Authentication failed"

**Sintomi:**

Renci.SshNet.Common.SshAuthenticationException: Permission denied (publickey,password)

**Soluzioni:**

**1. Verifica credenziali:**

Test manuale SFTP
sftp -P 22 username@hostname

**2. Verifica configurazione:**

{
"SftpConnections": {
"TestConnection": {
"Host": "hostname",
"Port": 22,
"Username": "correct_username",
"Password": "correct_password"
}
}
}

**3. Usa chiave privata:**

{
"SftpConnections": {
"KeyBasedConnection": {
"Host": "hostname",
"Port": 22,
"Username": "username",
"PrivateKeyPath": "/path/to/private/key",
"Passphrase": "key_passphrase"
}
}
}

### ❌ Errore: "Host key verification failed"

**Sintomi:**

The server's host key does not match

**Soluzioni:**

**1. Disabilita verifica (solo per test):**

{
"SftpConnections": {
"TestConnection": {
"VerifyHostKey": false
}
}
}

**2. Aggiungi host key:**

Ottieni host key
ssh-keyscan -p 22 hostname

Aggiungi a known_hosts
ssh-keyscan -p 22 hostname >> ~/.ssh/known_hosts

**3. Specifica fingerprint:**

{
"SftpConnections": {
"SecureConnection": {
"VerifyHostKey": true,
"HostKeyFingerprint": "SHA256:your_fingerprint_here"
}
}
}

### ❌ Errore: "Connection timed out"

**Sintomi:**
- Connessioni SFTP lente o che falliscono
- Timeout durante upload

**Soluzioni:**

**1. Aumenta timeout:**

{
"SftpConnections": {
"SlowConnection": {
"ConnectionTimeout": 120,
"KeepAliveInterval": 30
}
}
}

**2. Verifica rete:**

Test connettività
telnet hostname 22

Test latenza
ping hostname

Verifica MTU
ping -M do -s 1472 hostname

**3. Usa compressione:**

{
"SftpConnections": {
"CompressedConnection": {
"UseCompression": true,
"CompressionLevel": 6
}
}
}

## Problemi di Elaborazione

### ❌ Errore: "Out of Memory Exception"

**Sintomi:**

System.OutOfMemoryException: Exception of type 'System.OutOfMemoryException' was thrown

**Soluzioni:**

**1. Riduce batch size:**

{
"Processing": {
"BatchSize": 1000,
"UseStreamingExport": true,
"StreamingBufferSize": 32768
}
}

**2. Abilita ottimizzazioni memoria:**

{
"Processing": {
"EnableMemoryOptimization": true,
"MemoryThreshold": 536870912,
"MaxConcurrentFiles": 2
}
}

**3. Monitora utilizzo memoria:**

Windows
Get-Process -Name "CSVTransferApp*" | Select-Object ProcessName,WorkingSet

Linux
top -p $(pgrep -f CSVTransferApp)

### ❌ Errore: "CSV header override not found"

**Sintomi:**
- Warning sui header override mancanti
- Header di default utilizzati

**Soluzioni:**

**1. Verifica percorso:**

{
"Processing": {
"HeaderOverridePath": "./config/header-overrides"
}
}

**2. Crea file override:**

Crea directory
mkdir -p config/header-overrides

Crea file per tabella
cat > config/header-overrides/employees.json << 'EOF'
{
"TableName": "employees",
"ColumnMappings": {
"emp_id": "Employee ID",
"first_name": "First Name",
"last_name": "Last Name"
}
}
EOF

**3. Verifica permessi:**

Linux
chmod 644 config/header-overrides/*.json
ls -la config/header-overrides/

### ❌ Errore: "Invalid CSV characters"

**Sintomi:**
- CSV malformati
- Caratteri speciali non gestiti

**Soluzioni:**

**1. Configura encoding:**

{
"Processing": {
"FileEncoding": "UTF-8",
"CsvDelimiter": ",",
"CsvQuoteChar": """,
"CsvEscapeChar": """
}
}

**2. Gestione caratteri speciali:**

{
"Processing": {
"EscapeSpecialCharacters": true,
"RemoveControlCharacters": true,
"NormalizeLineEndings": true
}
}

## Problemi di Performance

### ❌ Performance Lente

**Sintomi:**
- Elaborazione molto lenta
- Utilizzo CPU/memoria alto

**Soluzioni di Diagnosi:**

**1. Abilita logging dettagliato:**

{
"Logging": {
"LogLevel": {
"CSVTransferApp": "Debug"
}
}
}

**2. Monitor performance:**

Genera report performance
dotnet run transfer --table test_table --verbose --performance-report

Analizza log
grep "Processing time" logs/app-*.log

**Soluzioni di Ottimizzazione:**

**1. Ottimizza concorrenza:**

{
"Processing": {
"MaxConcurrentConnections": 10,
"MaxConcurrentFiles": 20,
"EnableParallelProcessing": true
}
}

**2. Ottimizza database:**

{
"DatabaseConnections": {
"OptimizedConnection": {
"MaxPoolSize": 200,
"CommandTimeout": 300,
"EnableBulkCopy": true
}
}
}

**3. Ottimizza rete:**

{
"SftpConnections": {
"FastConnection": {
"UseCompression": true,
"BufferSize": 65536,
"EnableConnectionReuse": true
}
}
}

## Problemi di Logging

### ❌ Log Non Generati

**Sintomi:**
- File di log vuoti o inesistenti
- Nessun output nei log

**Soluzioni:**

**1. Verifica configurazione:**

{
"Logging": {
"LogLevel": {
"Default": "Information"
},
"File": {
"Path": "./logs/app-.log"
}
}
}

**2. Verifica permessi directory:**

Crea directory log
mkdir -p logs

Imposta permessi
chmod 755 logs

Windows - verifica permessi
icacls logs /grant %USERNAME%:(OI)(CI)F

**3. Test logging:**

Test diretto
dotnet run test --verbose

Verifica file creati
ls -la logs/

### ❌ Log File Troppo Grandi

**Sintomi:**
- File di log di dimensioni eccessive
- Spazio disco esaurito

**Soluzioni:**

**1. Configura rotazione:**

{
"Logging": {
"File": {
"MaxSizeInMB": 10,
"MaxFiles": 5,
"RollingInterval": "Day"
}
}
}

**2. Implementa cleanup automatico:**

Script cleanup Linux
#!/bin/bash
find ./logs -name ".log" -mtime +30 -delete
find ./logs -name ".log" -size +100M -delete

**3. Riduci verbosità:**

{
"Logging": {
"LogLevel": {
"Default": "Warning",
"CSVTransferApp": "Information"
}
}
}

## Diagnostics e Debugging

### Tools di Diagnosi

**1. Test Completo Sistema:**

Esegui tutti i test
dotnet run test --comprehensive

Test con timeout esteso
dotnet run test --timeout 300

Test solo componenti specifici
dotnet run test --component database
dotnet run test --component sftp

**2. Validazione Configurazione:**

Valida configurazione
dotnet run validate-config

Verifica variabili ambiente
dotnet run validate-config --check-environment

Test con configurazione specifica
dotnet run validate-config --config appsettings.Production.json

**3. Report Salute Sistema:**

Genera health report
dotnet run health-check

Report dettagliato
dotnet run health-check --detailed

Export metriche
dotnet run metrics --export ./metrics.json

### Raccolta Informazioni per Supporto

Quando contatti il supporto, includi:

**1. Informazioni Sistema:**

Windows
systeminfo | findstr /B /C:"OS Name" /C:"OS Version"
dotnet --info

Linux
uname -a
lsb_release -a
dotnet --info

**2. Log Rilevanti:**

Ultimi 100 righe log applicazione
tail -n 100 logs/app-$(date +%Y%m%d).log

Log errori
grep -i "error|exception|failed" logs/app-*.log | tail -20

**3. Configurazione (Sanificata):**

Rimuovi password dalla configurazione prima di condividerla
cat appsettings.json | sed 's/"Password":"[^"]"/"Password":"**"/g'

**4. Output Comando con Errore:**

Esegui comando con verbose output
dotnet run transfer --table problematic_table --verbose > debug_output.txt 2>&1

## FAQ - Domande Frequenti

### Q: Posso usare l'applicazione senza SFTP?

**A:** Attualmente no, ma puoi:
- Configurare SFTP locale (docker container)
- Modificare codice per save su filesystem locale
- Utilizzare SFTP su localhost

### Q: Supporta database MySQL?

**A:** Non nativamente, ma può essere aggiunto:
1. Installa package MySQL
2. Implementa `MySqlDatabaseProvider`
3. Registra nel `ConnectionFactory`

### Q: Come gestire file CSV molto grandi?

**A:** Usa queste configurazioni:

{
"Processing": {
"UseStreamingExport": true,
"BatchSize": 10000,
"EnableCompression": true
}
}

### Q: Posso schedulare l'applicazione?

**A:** Sì:
- **Windows:** Task Scheduler
- **Linux:** Cron jobs
- **Docker:** Con orchestrator come Kubernetes

### Q: Come monitorare l'applicazione in produzione?

**A:** Utilizza:
- Log strutturati (Serilog)
- Health checks HTTP
- Metriche Prometheus
- Alert su log di errore

---

**🆘 Se il problema persiste:**

- 🐛 [Apri un Issue](https://github.com/yourusername/CSVTransferApp/issues)
- 💬 [GitHub Discussions](https://github.com/yourusername/CSVTransferApp/discussions) 
- 📧 Email: support@yourcompany.com
- 📞 Supporto: +39 XXX XXX XXXX

**📖 Prossimo: [API Reference](API.md)**