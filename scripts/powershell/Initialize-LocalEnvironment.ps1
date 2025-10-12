# Script per inizializzare l'ambiente locale di test

# Verifica che Docker sia in esecuzione
$maxAttempts = 12
$attempts = 0
$dockerRunning = $false

Write-Host "Verifico che Docker sia pronto..."
do {
    try {
        $null = docker ps
        $dockerRunning = $true
        break
    }
    catch {
        $attempts++
        if ($attempts -lt $maxAttempts) {
            Write-Host "In attesa che Docker sia pronto... ($attempts/$maxAttempts)"
            Start-Sleep -Seconds 5
        }
        else {
            Write-Error "Docker non risponde dopo 60 secondi. Verifica che Docker Desktop sia in esecuzione."
            exit 1
        }
    }
} while ($attempts -lt $maxAttempts)

# Crea le directory necessarie
$directories = @(
    "./logs",
    "./config/header-overrides",
    "./deploy/local"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force
        Write-Host "Directory creata: $dir"
    }
}

# Avvia i container Docker
Write-Host "Avvio dei container Docker..."
docker-compose -f docker-compose.local.yml up -d

# Attendi che i container siano pronti
Write-Host "Attendi l'inizializzazione dei database..."
Start-Sleep -Seconds 30

# Copia la configurazione locale
Copy-Item -Path "./config/appsettings.Local.json" -Destination "./deploy/local/appsettings.json" -Force
Copy-Item -Path "./config/database-connections.json" -Destination "./deploy/local/" -Force
Copy-Item -Path "./config/sftp-connections.json" -Destination "./deploy/local/" -Force
Copy-Item -Path "./config/header-overrides/*" -Destination "./deploy/local/config/header-overrides/" -Force

# Build e pubblicazione dell'applicazione
Write-Host "Build dell'applicazione..."
dotnet publish src/CSVTransferApp.Console/CSVTransferApp.Console.csproj -c Release -o ./deploy/local

# Inizializzazione dei database
Write-Host "Inizializzazione dei database..."

# PostgreSQL
docker exec csvtransferapp-postgres-1 psql -U csvapp -d csvapp_db -c "
CREATE TABLE IF NOT EXISTS employees (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    email VARCHAR(200)
);
INSERT INTO employees (first_name, last_name, email) VALUES 
    ('John', 'Doe', 'john@example.com'),
    ('Jane', 'Smith', 'jane@example.com')
ON CONFLICT DO NOTHING;"

# SQL Server
docker exec csvtransferapp-sqlserver-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$env:SQLSERVER_SA_PASSWORD" -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'CsvTransfer')
BEGIN
    CREATE DATABASE CsvTransfer;
END
GO
USE CsvTransfer;
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
    CREATE TABLE Products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100),
        Price DECIMAL(18,2),
        Category NVARCHAR(50)
    );
    INSERT INTO Products (Name, Price, Category) VALUES 
        ('Product A', 99.99, 'Electronics'),
        ('Product B', 149.99, 'Electronics');
END"

Write-Host "Ambiente locale inizializzato con successo!"
Write-Host "Puoi ora eseguire l'applicazione da: ./deploy/local/CSVTransferApp.Console.exe"