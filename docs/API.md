# API Reference

Documentazione completa delle interfacce, classi e metodi di CSV Transfer Application.

## Core Interfaces

### IDatabaseService

Interfaccia principale per l'accesso ai database.

namespace CSVTransferApp.Core.Interfaces;

public interface IDatabaseService
{
Task<DataTable> ExecuteQueryAsync(string connectionName, string query);
Task<bool> TestConnectionAsync(string connectionName);
Task<List<string>> GetTablesAsync(string connectionName);
Task<List<string>> GetColumnsAsync(string connectionName, string tableName);
void Dispose();
}

#### ExecuteQueryAsync

Esegue una query SQL e restituisce i risultati come DataTable.

**Parametri:**
- `connectionName` (string): Nome della connessione configurata
- `query` (string): Query SQL da eseguire

**Ritorna:** `Task<DataTable>` - Risultati della query

**Eccezioni:**
- `DatabaseConnectionException` - Errore di connessione
- `SqlException` - Errore SQL
- `TimeoutException` - Timeout query

**Esempio:**

var data = await databaseService.ExecuteQueryAsync("Oracle", "SELECT * FROM employees");
Console.WriteLine($"Records retrieved: {data.Rows.Count}");

#### TestConnectionAsync

Verifica la connettività al database.

**Parametri:**
- `connectionName` (string): Nome della connessione da testare

**Ritorna:** `Task<bool>` - True se connessione riuscita

**Esempio:**

var isConnected = await databaseService.TestConnectionAsync("SqlServer");
if (isConnected)
{
Console.WriteLine("Database connection successful");
}

#### GetTablesAsync

Ottiene l'elenco delle tabelle disponibili.

**Parametri:**
- `connectionName` (string): Nome della connessione

**Ritorna:** `Task<List<string>>` - Lista nomi tabelle

**Esempio:**

var tables = await databaseService.GetTablesAsync("PostgreSQL");
foreach (var table in tables)
{
Console.WriteLine($"Table: {table}");
}

#### GetColumnsAsync

Ottiene l'elenco delle colonne di una tabella.

**Parametri:**
- `connectionName` (string): Nome della connessione
- `tableName` (string): Nome della tabella

**Ritorna:** `Task<List<string>>` - Lista nomi colonne

---

### ISftpService

Interfaccia per operazioni SFTP.

namespace CSVTransferApp.Core.Interfaces;

public interface ISftpService
{
Task UploadFileAsync(string connectionName, Stream fileStream, string fileName);
Task<bool> TestConnectionAsync(string connectionName);
Task<bool> FileExistsAsync(string connectionName, string fileName);
Task DeleteFileAsync(string connectionName, string fileName);
void Dispose();
}

#### UploadFileAsync

Carica un file via SFTP.

**Parametri:**
- `connectionName` (string): Nome connessione SFTP
- `fileStream` (Stream): Stream del file da caricare
- `fileName` (string): Nome file di destinazione

**Ritorna:** `Task` - Operazione asincrona

**Eccezioni:**
- `SftpConnectionException` - Errore connessione SFTP
- `UnauthorizedAccessException` - Errore autorizzazione
- `IOException` - Errore I/O

**Esempio:**

using var fileStream = File.OpenRead("data.csv");
await sftpService.UploadFileAsync("MainServer", fileStream, "employees.csv");

---

### ICsvProcessingService

Servizio principale per l'elaborazione CSV.

---

### ICsvProcessingService

Servizio principale per l'elaborazione CSV.

---

### ICsvProcessingService

Servizio principale per l'elaborazione CSV.

namespace CSVTransferApp.Core.Interfaces;

public interface ICsvProcessingService
{
Task<ProcessingResult> ProcessJobAsync(TransferJob job);
Task<List<ProcessingResult>> ProcessJobsAsync(IEnumerable<TransferJob> jobs);
}

#### ProcessJobAsync

Elabora un singolo job di trasferimento.

**Parametri:**
- `job` (TransferJob): Definizione del job da elaborare

**Ritorna:** `Task<ProcessingResult>` - Risultato dell'elaborazione

**Esempio:**

var job = new TransferJob
{
TableName = "employees",
DatabaseConnection = "Oracle",
SftpConnection = "MainServer",
Query = "SELECT * FROM employees WHERE active = 1"
};

var result = await csvProcessingService.ProcessJobAsync(job);
if (result.IsSuccess)
{
Console.WriteLine($"Processed {result.RecordsProcessed} records");
}

---

## Core Models

### TransferJob

Modello che rappresenta un job di trasferimento.

namespace CSVTransferApp.Core.Models;

public class TransferJob
{
public string TableName { get; set; } = string.Empty;
public string DatabaseConnection { get; set; } = string.Empty;
public string SftpConnection { get; set; } = string.Empty;
public string Query { get; set; } = string.Empty;
public DateTime RequestTime { get; set; } = DateTime.UtcNow;
}

**Proprietà:**

| Nome | Tipo | Descrizione | Obbligatorio |
|------|------|-------------|--------------|
| TableName | string | Nome della tabella/file CSV | ✅ |
| DatabaseConnection | string | Nome connessione database | ✅ |
| SftpConnection | string | Nome connessione SFTP | ✅ |
| Query | string | Query SQL da eseguire | ✅ |
| RequestTime | DateTime | Timestamp richiesta | ❌ (auto) |

**Esempio:**

var job = new TransferJob
{
TableName = "products",
DatabaseConnection = "SqlServer",
SftpConnection = "BackupServer",
Query = "SELECT product_id, name, price FROM products WHERE active = 1"
};

---

### ProcessingResult

Risultato dell'elaborazione di un job.

namespace CSVTransferApp.Core.Models;

public class ProcessingResult
{
public string TableName { get; set; } = string.Empty;
public bool IsSuccess { get; set; }
public string? ErrorMessage { get; set; }
public int RecordsProcessed { get; set; }
public long FileSizeBytes { get; set; }
public TimeSpan ProcessingTime { get; set; }
public DateTime StartTime { get; set; }
public DateTime EndTime { get; set; }
public string LogFileName { get; set; } = string.Empty;
public List<string> Warnings { get; set; } = new();
}

**Proprietà:**

| Nome | Tipo | Descrizione |
|------|------|-------------|
| TableName | string | Nome tabella elaborata |
| IsSuccess | bool | Successo dell'operazione |
| ErrorMessage | string? | Messaggio di errore se fallita |
| RecordsProcessed | int | Numero record elaborati |
| FileSizeBytes | long | Dimensione file CSV generato |
| ProcessingTime | TimeSpan | Tempo totale elaborazione |
| StartTime | DateTime | Inizio elaborazione |
| EndTime | DateTime | Fine elaborazione |
| LogFileName | string | Nome file di log |
| Warnings | List<string> | Lista warning non bloccanti |

---

### DatabaseConnectionConfig

Configurazione connessione database.

namespace CSVTransferApp.Core.Models;

public class DatabaseConnectionConfig
{
public string Name { get; set; } = string.Empty;
public string Provider { get; set; } = string.Empty;
public string ConnectionString { get; set; } = string.Empty;
public bool IsEnabled { get; set; } = true;
public int MaxPoolSize { get; set; } = 100;
public int ConnectionTimeout { get; set; } = 30;
public int CommandTimeout { get; set; } = 300;
}

**Provider Supportati:**
- `Oracle.EntityFrameworkCore`
- `Microsoft.EntityFrameworkCore.SqlServer`
- `Npgsql.EntityFrameworkCore.PostgreSQL`

---

### SftpConnectionConfig

Configurazione connessione SFTP.

namespace CSVTransferApp.Core.Models;

public class SftpConnectionConfig
{
public string Name { get; set; } = string.Empty;
public string Host { get; set; } = string.Empty;
public int Port { get; set; } = 22;
public string Username { get; set; } = string.Empty;
public string? Password { get; set; }
public string? PrivateKeyPath { get; set; }
public string? Passphrase { get; set; }
public string RemotePath { get; set; } = "/";
public int ConnectionTimeout { get; set; } = 30;
public bool IsEnabled { get; set; } = true;
}

---

## Extension Methods

### DataTableExtensions

Estensioni per la classe DataTable.

namespace CSVTransferApp.Core.Extensions;

public static class DataTableExtensions
{
public static MemoryStream ToCsvStream(this DataTable dataTable, string[] customHeaders = null!);
public static Dictionary<string, Type> GetColumnTypes(this DataTable dataTable);
}

#### ToCsvStream

Converte un DataTable in stream CSV.

**Parametri:**
- `customHeaders` (string[]?): Header personalizzati opzionali

**Ritorna:** `MemoryStream` - Stream CSV

**Esempio:**

var dataTable = GetDataFromDatabase();
var customHeaders = new[] { "ID", "Nome", "Email" };
using var csvStream = dataTable.ToCsvStream(customHeaders);

---

### StringExtensions

Estensioni per stringhe.

namespace CSVTransferApp.Core.Extensions;

public static class StringExtensions
{
public static bool IsNullOrWhiteSpace(this string? value);
public static string ToSafeFileName(this string fileName);
public static string ReplaceTokens(this string template, Dictionary<string, string> tokens);
public static string TruncateWithEllipsis(this string value, int maxLength);
}

#### ToSafeFileName

Rimuove caratteri non validi dai nomi file.

**Esempio:**

var unsafeName = "my|file<name>.csv";
var safeName = unsafeName.ToSafeFileName(); // "myfilename.csv"

#### ReplaceTokens

Sostituisce token in un template con valori.

**Esempio:**

var template = "Hello {name}, today is {date}";
var tokens = new Dictionary<string, string>
{
{ "name", "John" },
{ "date", "2025-09-27" }
};
var result = template.ReplaceTokens(tokens); // "Hello John, today is 2025-09-27"

---

## Services Implementation

### CsvProcessingService

Implementazione principale del servizio di elaborazione CSV.

namespace CSVTransferApp.Services;

public class CsvProcessingService : ICsvProcessingService
{
public CsvProcessingService(
IDatabaseService databaseService,
ISftpService sftpService,
ILoggerService loggerService,
IConfigurationService configurationService);

public async Task<ProcessingResult> ProcessJobAsync(TransferJob job);
public async Task<List<ProcessingResult>> ProcessJobsAsync(IEnumerable<TransferJob> jobs);
}

**Costruttore:**
- `databaseService` - Servizio accesso database
- `sftpService` - Servizio SFTP
- `loggerService` - Servizio logging
- `configurationService` - Servizio configurazione

---

### DatabaseService

Implementazione servizio database.

namespace CSVTransferApp.Data.Services;

public class DatabaseService : IDatabaseService
{
public DatabaseService(
IConfigurationService configurationService,
ILoggerService loggerService,
ConnectionFactory connectionFactory);
// Implementa tutti i metodi di IDatabaseService
}

---

### SftpService

Implementazione servizio SFTP.

namespace CSVTransferApp.Services;

public class SftpService : ISftpService
{
public SftpService(
IConfigurationService configurationService,
ILoggerService loggerService);
// Implementa tutti i metodi di ISftpService
}

---

## Command Line Interface

### Commands

#### TransferCommand

Comando per trasferimento singolo.

**Sintassi:**

dotnet run transfer --table <table_name> --db-connection <db_conn> --sftp-connection <sftp_conn> [--query <sql_query>]

**Parametri:**
- `--table` (obbligatorio): Nome tabella
- `--db-connection` (obbligatorio): Nome connessione database
- `--sftp-connection` (obbligatorio): Nome connessione SFTP  
- `--query` (opzionale): Query SQL personalizzata

**Esempi:**

Transfer base
dotnet run transfer --table employees --db-connection Oracle --sftp-connection MainServer

Transfer con query personalizzata
dotnet run transfer --table employees --db-connection Oracle --sftp-connection MainServer --query "SELECT id, name, email FROM employees WHERE active = 1"

#### BatchCommand

Comando per elaborazione batch.

**Sintassi:**

dotnet run batch --file <batch_file_path>

**Parametri:**
- `--file` (obbligatorio): Percorso file JSON batch

**Formato file batch:**
[
{
"TableName": "employees",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT * FROM employees"
},
{
"TableName": "products",
"DatabaseConnection": "SqlServer",
"SftpConnection": "BackupServer",
"Query": "SELECT * FROM products WHERE active = 1"
}
]

#### TestConnectionCommand

Comando per test connessioni.

**Sintassi:**

dotnet run test [--type <database|sftp>] [--connection <connection_name>] [--verbose]

**Parametri:**
- `--type` (opzionale): Tipo connessioni da testare
- `--connection` (opzionale): Nome connessione specifica
- `--verbose` (opzionale): Output dettagliato

**Esempi:**

Test tutte le connessioni
dotnet run test

Test solo database
dotnet run test --type database

Test connessione specifica
dotnet run test --type database --connection Oracle --verbose

---

## Configuration Schema

### appsettings.json Schema

{
"$schema": "http://json-schema.org/draft-07/schema#",
"type": "object",
"properties": {
"Logging": {
"type": "object",
"properties": {
"LogLevel": {
"type": "object",
"properties": {
"Default": { "type": "string", "enum": ["Trace", "Debug", "Information", "Warning", "Error", "Critical"] }
}
}
}
},
"DatabaseConnections": {
"type": "object",
"patternProperties": {
"^[a-zA-Z0-9_-]+$": {
"type": "object",
"properties": {
"Provider": {
"type": "string",
"enum": ["Oracle.EntityFrameworkCore", "Microsoft.EntityFrameworkCore.SqlServer", "Npgsql.EntityFrameworkCore.PostgreSQL"]
},
"ConnectionString": { "type": "string" },
"ConnectionTimeout": { "type": "integer", "minimum": 1, "maximum": 300 },
"CommandTimeout": { "type": "integer", "minimum": 1, "maximum": 3600 },
"MaxPoolSize": { "type": "integer", "minimum": 1, "maximum": 1000 },
"IsEnabled": { "type": "boolean" }
},
"required": ["Provider", "ConnectionString"]
}
}
},
"SftpConnections": {
"type": "object",
"patternProperties": {
"^[a-zA-Z0-9_-]+$": {
"type": "object",
"properties": {
"Host": { "type": "string", "format": "hostname" },
"Port": { "type": "integer", "minimum": 1, "maximum": 65535 },
"Username": { "type": "string" },
"Password": { "type": "string" },
"PrivateKeyPath": { "type": "string" },
"Passphrase": { "type": "string" },
"RemotePath": { "type": "string" },
"ConnectionTimeout": { "type": "integer", "minimum": 1, "maximum": 300 },
"IsEnabled": { "type": "boolean" }
},
"required": ["Host", "Username"]
}
}
},
"Processing": {
"type": "object",
"properties": {
"MaxConcurrentConnections": { "type": "integer", "minimum": 1, "maximum": 100 },
"MaxConcurrentFiles": { "type": "integer", "minimum": 1, "maximum": 100 },
"MaxConcurrentJobs": { "type": "integer", "minimum": 1, "maximum": 100 },
"HeaderOverridePath": { "type": "string" },
"LogPath": { "type": "string" },
"TempPath": { "type": "string" },
"BatchSize": { "type": "integer", "minimum": 1, "maximum": 100000 },
"EnableCompression": { "type": "boolean" },
"CompressionLevel": { "type": "integer", "minimum": 1, "maximum": 9 }
}
}
},
"required": ["DatabaseConnections", "SftpConnections"]
}

---

## Error Codes

### Database Error Codes

| Codice | Descrizione | Soluzione |
|--------|-------------|-----------|
| DB001 | Connessione fallita | Verifica connection string |
| DB002 | Timeout query | Aumenta CommandTimeout |
| DB003 | Provider non supportato | Usa provider supportati |
| DB004 | Credenziali non valide | Verifica username/password |
| DB005 | Tabella non trovata | Verifica nome tabella |

### SFTP Error Codes

| Codice | Descrizione | Soluzione |
|--------|-------------|-----------|
| SFTP001 | Autenticazione fallita | Verifica credenziali |
| SFTP002 | Host non raggiungibile | Verifica host e porta |
| SFTP003 | Percorso non trovato | Verifica RemotePath |
| SFTP004 | Permessi insufficienti | Verifica permessi utente |
| SFTP005 | Timeout connessione | Aumenta ConnectionTimeout |

### Processing Error Codes

| Codice | Descrizione | Soluzione |
|--------|-------------|-----------|
| PROC001 | Out of memory | Riduci BatchSize |
| PROC002 | File non generato | Verifica permessi directory |
| PROC003 | Header override non trovato | Crea file override |
| PROC004 | CSV malformato | Verifica caratteri speciali |
| PROC005 | Job timeout | Aumenta timeout processing |

---

## Performance Metrics

### Metriche Disponibili

public class PerformanceMetrics
{
public int JobsProcessed { get; set; }
public int JobsSuccessful { get; set; }
public int JobsFailed { get; set; }
public long TotalRecordsProcessed { get; set; }
public long TotalBytesTransferred { get; set; }
public TimeSpan TotalProcessingTime { get; set; }
public double AverageRecordsPerSecond { get; set; }
public double AverageThroughputMBps { get; set; }
}

### Accesso alle Metriche

/ Via servizio
var metrics = metricsService.GetCurrentMetrics();

// Via health check
var health = await healthCheckService.CheckHealthAsync();

// Via endpoint (se abilitato)
// GET http://localhost:5000/metrics

---

**📖 Torna a: [Documentazione Principale](README.md)**