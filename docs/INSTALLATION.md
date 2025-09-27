# Guida di Installazione

Questa guida ti accompagnerà passo-passo nell'installazione e configurazione iniziale di CSV Transfer Application.

## Prerequisiti

### 1. .NET 9.0 SDK

**Windows:**

Scarica da https://dotnet.microsoft.com/download/dotnet/9.0
Oppure usa winget
winget install Microsoft.DotNet.SDK.9

Verifica installazione
dotnet --version

**Linux (Ubuntu/Debian):**

Aggiungi il repository Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

Installa .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

Verifica installazione
dotnet --version

**Linux (CentOS/RHEL):**

Aggiungi il repository Microsoft
sudo rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm

Installa .NET SDK
sudo yum install dotnet-sdk-9.0

Verifica installazione
dotnet --version

### 2. Git

**Windows:**

winget install Git.Git

**Linux:**

Ubuntu/Debian
sudo apt-get install git

CentOS/RHEL
sudo yum install git

### 3. Database Client (Opzionale ma Consigliato)

**Oracle Client:**
- Scarica Oracle Instant Client da [Oracle Website](https://www.oracle.com/database/technologies/instant-client.html)
- Estrai in una cartella e aggiungi al PATH

**SQL Server Client:**
- Windows: Automaticamente disponibile
- Linux: Installa `mssql-tools`

**PostgreSQL Client:**

Ubuntu/Debian
sudo apt-get install postgresql-client

CentOS/RHEL
sudo yum install postgresql

Windows
winget install PostgreSQL.PostgreSQL

## Installazione dell'Applicazione

### Metodo 1: Clone da Repository

1. Clone del repository
git clone https://github.com/yourusername/CSVTransferApp.git
cd CSVTransferApp

2. Restore delle dipendenze
dotnet restore

3. Build dell'applicazione
dotnet build --configuration Release

4. Test del build
dotnet run --project src/CSVTransferApp.Console/CSVTransferApp.Console.csproj -- help

### Metodo 2: Download Release

1. Scarica l'ultima release da GitHub
wget https://github.com/yourusername/CSVTransferApp/releases/latest/download/CSVTransferApp-linux-x64.tar.gz

2. Estrai l'archivio
tar -xzf CSVTransferApp-linux-x64.tar.gz
cd CSVTransferApp

3. Rendi eseguibile (Linux)
chmod +x CSVTransferApp.Console

4. Test dell'applicazione
./CSVTransferApp.Console help

## Setup dell'Ambiente

### Automatico (Consigliato)

**Windows:**

Naviga nella directory del progetto
cd CSVTransferApp

Esegui lo script di setup
.\scripts\powershell\setup-environment.ps1 -Environment Development

Per produzione
.\scripts\powershell\setup-environment.ps1 -Environment Production

**Linux:**

Naviga nella directory del progetto
cd CSVTransferApp

Rendi eseguibile lo script
chmod +x scripts/bash/setup-environment.sh

Esegui lo script di setup
./scripts/bash/setup-environment.sh -e Development

Per produzione
./scripts/bash/setup-environment.sh -e Production

### Manuale

Se preferisci configurare manualmente:

1. Crea le directory necessarie
mkdir -p logs/{application,transfers,errors}
mkdir -p config/header-overrides
mkdir -p temp

2. Copia i file di configurazione
cp config/appsettings.json.template appsettings.json
cp config/database-connections.json.template config/database-connections.json
cp config/sftp-connections.json.template config/sftp-connections.json

3. Imposta i permessi (Linux)
chmod 755 logs config temp
chmod 644 config/*.json

## Configurazione Iniziale

### 1. Database Connections

Modifica `appsettings.json`:

{
"DatabaseConnections": {
"Oracle": {
"Provider": "Oracle.EntityFrameworkCore",
"ConnectionString": "Data Source=YOUR_HOST:1521/XE;User Id=YOUR_USER;Password=YOUR_PASSWORD;",
"IsEnabled": true
},
"SqlServer": {
"Provider": "Microsoft.EntityFrameworkCore.SqlServer",
"ConnectionString": "Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=true;",
"IsEnabled": true
},
"PostgreSQL": {
"Provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
"ConnectionString": "Host=YOUR_HOST;Database=YOUR_DB;Username=YOUR_USER;Password=YOUR_PASSWORD;",
"IsEnabled": true
}
}
}

### 2. SFTP Connections

Aggiungi le tue connessioni SFTP:

{
"SftpConnections": {
"MainServer": {
"Host": "your-sftp-server.com",
"Port": 22,
"Username": "your-username",
"Password": "your-password",
"RemotePath": "/upload/csv",
"IsEnabled": true
}
}
}

### 3. Test delle Connessioni

Test di tutte le connessioni
dotnet run test

Test solo database
dotnet run test --database-only

Test solo SFTP
dotnet run test --sftp-only

## Verifica dell'Installazione

### Test Completo

1. Verifica che l'applicazione si avvii
dotnet run help

2. Test delle connessioni
dotnet run test

3. Trasferimento di test (se hai dati)
dotnet run transfer --table employees --db-connection Oracle --sftp-connection MainServer

4. Controlla i log
ls -la logs/
cat logs/application/app-$(date +%Y%m%d).log

### Output Atteso

Se tutto è configurato correttamente, dovresti vedere:

CSV Transfer Application

Usage: CSVTransferApp <command> [options]

Commands:
transfer Transfer a single table to CSV and upload via SFTP
batch Process multiple transfers from a batch file
test Test database and SFTP connections
help Show this help message

## Installazione come Servizio

### Windows Service

1. Installa come Windows Service (richiede privilegi admin)
sc create "CSV Transfer Service" binPath="C:\path\to\CSVTransferApp.Console.exe service"

2. Avvia il servizio
sc start "CSV Transfer Service"

3. Configura l'avvio automatico
sc config "CSV Transfer Service" start=auto

### Linux Systemd Service

1. Crea il file di servizio
sudo nano /etc/systemd/system/csvtransferapp.service

Contenuto del file:

[Unit]
Description=CSV Transfer Application
After=network.target

[Service]
Type=simple
User=csvapp
WorkingDirectory=/opt/csvtransferapp
ExecStart=/usr/bin/dotnet /opt/csvtransferapp/CSVTransferApp.Console.dll service
Restart=always
RestartSec=10
SyslogIdentifier=csvtransferapp
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target

undefined

2. Abilita e avvia il servizio
sudo systemctl daemon-reload
sudo systemctl enable csvtransferapp
sudo systemctl start csvtransferapp

3. Verifica lo stato
sudo systemctl status csvtransferapp

## Configurazione dell'Ambiente di Produzione

### Sicurezza

1. **Crittografia delle Password:**

Usa variabili d'ambiente per le password sensibili
export ORACLE_PASSWORD="your-secure-password"
export SFTP_PASSWORD="your-sftp-password"

2. **Permessi File:**

Imposta permessi restrittivi sui file di configurazione
chmod 600 config/.json
chown csvapp:csvapp config/.json

3. **Firewall:**

Apri solo le porte necessarie
sudo ufw allow out 1521/tcp # Oracle
sudo ufw allow out 1433/tcp # SQL Server
sudo ufw allow out 5432/tcp # PostgreSQL
sudo ufw allow out 22/tcp # SFTP

### Monitoraggio

1. Configura rotazione log
sudo nano /etc/logrotate.d/csvtransferapp

undefined

/opt/csvtransferapp/logs/*.log {
daily
rotate 30
compress
delaycompress
missingok
notifempty
copytruncate
}

undefined

2. Configura monitoraggio spazio disco
df -h /opt/csvtransferapp/logs

## Risoluzione Problemi Comuni

### Errore: "dotnet command not found"

**Soluzione:**

Aggiungi .NET al PATH
export PATH=$PATH:/usr/share/dotnet
echo 'export PATH=$PATH:/usr/share/dotnet' >> ~/.bashrc

### Errore: "Oracle Client not found"

**Soluzione:**

Installa Oracle Instant Client
wget https://download.oracle.com/otn_software/linux/instantclient/1923000/instantclient-basic-linux.x64-19.23.0.0.0dbru.zip
unzip instantclient-basic-linux.x64-19.23.0.0.0dbru.zip
sudo mv instantclient_19_23 /opt/oracle/
export LD_LIBRARY_PATH=/opt/oracle/instantclient_19_23:$LD_LIBRARY_PATH

### Errori di Connessione SFTP

**Soluzione:**

Test manuale SFTP
sftp -P 22 username@your-sftp-server.com

Verifica chiavi SSH
ssh-keygen -t rsa -b 4096 -C "csvtransferapp@yourcompany.com"
ssh-copy-id username@your-sftp-server.com

## Prossimi Passi

✅ Installazione completata!

Ora puoi procedere con:

1. 📖 [**Configurazione Dettagliata**](CONFIGURATION.md) - Setup avanzato
2. 🚀 [**Esempi di Utilizzo**](EXAMPLES.md) - Casi d'uso pratici
3. 🔧 [**Risoluzione Problemi**](TROUBLESHOOTING.md) - Se qualcosa non funziona

## Supporto

Se incontri problemi durante l'installazione:

- 🐛 [Apri un Issue](https://github.com/yourusername/CSVTransferApp/issues)
- 📧 Email: support@yourcompany.com
- 💬 [Discussions](https://github.com/yourusername/CSVTransferApp/discussions)