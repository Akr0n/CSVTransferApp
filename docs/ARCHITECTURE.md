# Architettura del Sistema

Questa documentazione descrive l'architettura tecnica di CSV Transfer Application, i pattern architetturali utilizzati e le decisioni di design.

## Panoramica Architetturale

CSV Transfer Application è costruita seguendo i principi della **Clean Architecture** di Robert C. Martin, garantendo:

- 🎯 **Separazione delle responsabilità**
- 🔄 **Dipendenze invertite**
- 🧪 **Testabilità completa**
- 🔧 **Manutenibilità elevata**
- 📈 **Scalabilità orizzontale**

graph TB
CLI[Console CLI] --> App[Application La
er] App --> Services[Service
Layer] Services --> Data[Data A
cess Layer] Services --> Infra[Infra
tructure Layer] Data
App -.-> Core[Core Domain]
Services -.-> Core
Data -.-> Core
Infra -.-> Core

style Core fill:#f9f,stroke:#333,stroke-width:4px
style CLI fill:#bbf,stroke:#333,stroke-width:2px
style DB fill:#bfb,stroke:#333,stroke-width:2px  
style SFTP fill:#fbf,stroke:#333,stroke-width:2px

CSVTransferApp.Core/
├── Models/
│ ├── TransferJob.cs # Modello principale del job di trasferimento
│ ├── HeaderOverride.cs # Configurazione override intestazioni
│ ├── DatabaseConnectionConfig.cs # Configurazione connessioni DB
│ ├── SftpConnectionConfig.cs # Configurazione connessioni SFTP
│ └── ProcessingResult.cs # Risultato dell'elaborazione
├── Interfaces/
│ ├── IDatabaseService.cs # Contratto servizio database
│ ├── ISftpService.cs # Contratto servizio SFTP
│ ├── ICsvProcessingService.cs # Contratto elaborazione CSV
│ ├── ILoggerService.cs # Contratto logging
│ └── IConfigurationService.cs # Contratto configurazione
├── Exceptions/
│ ├── DatabaseConnectionException.cs
│ ├── SftpConnectionException.cs
│ └── CsvProcessingException.cs
├── Extensions/
│ ├── DataTableExtensions.cs # Extension per DataTable
│ ├── StringExtensions.cs # Extension per stringhe
│ └── ConfigurationExtensions.cs # Extension per configurazione
└── Constants/
├── DatabaseProviders.cs # Costanti provider database
├── LoggingConstants.cs # Costanti logging
└── ConfigurationKeys.cs # Chiavi configurazione

**Principi del Core:**
- ✅ **Zero dipendenze esterne**
- ✅ **Solo logica di business pura**
- ✅ **Interfacce per tutti i servizi esterni**
- ✅ **Immutabilità dove possibile**

### 2. Data Access Layer (CSVTransferApp.Data)

Gestisce l'accesso ai database utilizzando il **Repository Pattern** e **Factory Pattern**:

CSVTransferApp.Data/
├── Services/
│ ├── DatabaseService.cs # Implementazione IDatabaseService
│ ├── OracleDatabaseProvider.cs # Provider specifico Oracle
│ ├── SqlServerDatabaseProvider.cs # Provider specifico SQL Server
│ └── PostgreSqlDatabaseProvider.cs # Provider specifico PostgreSQL
├── Factories/
│ ├── ConnectionFactory.cs # Factory per connessioni DB
│ └── DataAdapterFactory.cs # Factory per data adapter
├── Configuration/
│ └── DatabaseConnectionManager.cs # Gestione pool connessioni
└── Utilities/
├── QueryBuilder.cs # Builder per query SQL
└── DataTableHelper.cs # Helper per DataTable

**Pattern Implementati:**
- 🏭 **Factory Pattern** - Per creazione connessioni specifiche per provider
- 🗃️ **Repository Pattern** - Per astrazione accesso dati
- 🔗 **Connection Pooling** - Per ottimizzazione performance
- 🔄 **Retry Pattern** - Per resilienza network

### 3. Services Layer (CSVTransferApp.Services)

Contiene la logica di business e orchestrazione:

CSVTransferApp.Services/
├── CsvProcessingService.cs # Servizio principale elaborazione
├── SftpService.cs # Servizio trasferimento SFTP
├── LoggingService.cs # Servizio logging strutturato
├── ConfigurationService.cs # Servizio gestione configurazione
├── FileHeaderService.cs # Servizio override intestazioni
└── JobManagerService.cs # Gestione job paralleli

**Responsabilità:**
- 📊 **Orchestrazione** del flusso di elaborazione
- 🔄 **Trasformazione** dati da database a CSV
- 📁 **Upload SFTP** con retry automatici
- 📝 **Logging** strutturato e tracciabilità
- ⚡ **Elaborazione parallela** multi-thread

### 4. Infrastructure Layer (CSVTransferApp.Infrastructure)

Servizi infrastrutturali trasversali:

CSVTransferApp.Infrastructure/
├── Logging/
│ ├── FileLogger.cs # Logger su file
│ ├── ConsoleLogger.cs # Logger console
│ └── LoggerFactory.cs # Factory logger
├── Configuration/
│ ├── AppSettingsReader.cs # Lettura appsettings
│ └── EnvironmentVariableReader.cs # Lettura variabili ambiente
├── FileSystem/
│ ├── FileSystemService.cs # Servizio filesystem
│ └── DirectoryManager.cs # Gestione directory
└── Security/
├── CredentialManager.cs # Gestione credenziali
└── EncryptionService.cs # Servizi crittografia

### 5. Console Application (CSVTransferApp.Console)

Punto di ingresso e interfaccia utente:

CSVTransferApp.Console/
├── Program.cs # Entry point
├── Application.cs # Applicazione principale
├── Commands/
│ ├── ICommand.cs # Interfaccia comandi
│ ├── TransferCommand.cs # Comando transfer
│ ├── BatchCommand.cs # Comando batch
│ ├── TestConnectionCommand.cs # Comando test
│ └── HelpCommand.cs # Comando help
├── Parsers/
│ ├── CommandLineParser.cs # Parser argomenti CLI
│ └── ArgumentValidator.cs # Validazione argomenti
└── DependencyInjection/
└── ServiceCollectionExtensions.cs # Configurazione DI

## Pattern Architetturali

### 1. Dependency Injection

Utilizziamo il container DI nativo di .NET:

public static class ServiceCollectionExtensions
{
public static IServiceCollection AddCsvTransferServices(
this IServiceCollection services,
IConfiguration configuration)
{
// Core services
services.AddSingleton<IConfigurationService, ConfigurationService>();
services.AddSingleton<ILoggerService, LoggingService>();
    // Data services - Scoped per gestione connessioni
    services.AddScoped<ConnectionFactory>();
    services.AddScoped<IDatabaseService, DatabaseService>();
    // Business services
    services.AddTransient<ICsvProcessingService, CsvProcessingService>();
    services.AddTransient<ISftpService, SftpService>();
    
    return services;
}
}

**Lifetimes utilizzati:**
- 🔧 **Singleton** - Servizi stateless (logging, configurazione)
- 🔄 **Scoped** - Servizi con stato per richiesta (database connections)
- ⚡ **Transient** - Servizi leggeri per operazione (processing, SFTP)

### 2. Factory Pattern

Per gestire multiple implementazioni database:

public class ConnectionFactory
{
private readonly Dictionary<string, IDatabaseProvider> _providers;
public ConnectionFactory()
{
    _providers = new Dictionary<string, IDatabaseProvider>
    {
        { DatabaseProviders.Oracle, new OracleDatabaseProvider() },
        { DatabaseProviders.SqlServer, new SqlServerDatabaseProvider() },
        { DatabaseProviders.PostgreSQL, new PostgreSqlDatabaseProvider() }
    };
}

public IDbConnection CreateConnection(DatabaseConnectionConfig config)
{
    if (!_providers.TryGetValue(config.Provider, out var provider))
        throw new NotSupportedException($"Provider '{config.Provider}' not supported");

    return provider.CreateConnection(config);
}
}

### 3. Command Pattern

Per l'interfaccia CLI:

public interface ICommand
{
Task<int> ExecuteAsync(Dictionary<string, string> arguments);
}

public class TransferCommand : ICommand
{
public async Task<int> ExecuteAsync(Dictionary<string, string> arguments)
{
// Validazione argomenti
// Esecuzione trasferimento
// Gestione errori e logging
return exitCode;
}
}

### 4. Strategy Pattern

Per gestione diversi provider database:

public interface IDatabaseProvider
{
string ProviderName { get; }
IDbConnection CreateConnection(DatabaseConnectionConfig config);
Task<List<string>> GetTablesAsync(IDbConnection connection);
}

// Implementazioni specifiche per Oracle, SQL Server, PostgreSQL

## Flusso di Elaborazione

### Diagramma di Sequenza

sequenceDiagram
participant CLI as Console CLI
participant App as Application
participant CSV as CsvProcessingService
participant DB as DatabaseService
participant SFTP as SftpService
participant Log as LoggerService

CLI->>App: transfer --table employees
App->>CSV: ProcessJobAsync(job)
CSV->>Log: LogInformation("Starting transfer")

CSV->>DB: ExecuteQueryAsync(connection, query)
DB->>DB: Create connection via Factory
DB->>CSV: Return DataTable with results

CSV->>CSV: Convert DataTable to CSV stream
CSV->>CSV: Apply header overrides

CSV->>SFTP: UploadFileAsync(connection, stream, filename)
SFTP->>SFTP: Connect with retry logic
SFTP->>CSV: Upload successful

CSV->>Log: LogInformation("Transfer completed")
CSV->>App: Return ProcessingResult
App->>CLI: Return exit code

### Gestione Errori

graph TD
Start[Inizio Elaborazione] --> Validate[Validazione Parametri]
Validate -->|OK| DBConnect[Connessione Database]
Validate -->|Error| LogError[Log Error] --> Exit[Exit Code 1]

DBConnect -->|Success| Execute[Esecuzione Query]
DBConnect -->|Retry| DBRetry[Retry Connection]
DBConnect -->|Failed| LogError

DBRetry -->|Success| Execute
DBRetry -->|Max Retries| LogError

Execute -->|Success| Convert[Conversione CSV]
Execute -->|Error| LogError

Convert -->|Success| SFTPConnect[Connessione SFTP]
Convert -->|Error| LogError

SFTPConnect -->|Success| Upload[Upload File]
SFTPConnect -->|Retry| SFTPRetry[Retry SFTP]
SFTPConnect -->|Failed| LogError

SFTPRetry -->|Success| Upload
SFTPRetry -->|Max Retries| LogError

Upload -->|Success| Success[Log Success] --> Exit0[Exit Code 0]
Upload -->|Error| LogError

## Gestione della Concorrenza

### Thread Safety

L'applicazione è progettata per essere thread-safe:

public class JobManagerService
{
private readonly SemaphoreSlim _concurrentJobsSemaphore;
private readonly ConcurrentDictionary<string, ProcessingResult> _activeJobs;

public JobManagerService(int maxConcurrentJobs)
{
    _concurrentJobsSemaphore = new SemaphoreSlim(maxConcurrentJobs);
    _activeJobs = new ConcurrentDictionary<string, ProcessingResult>();
}

public async Task<ProcessingResult> SubmitJobAsync(TransferJob job)
{
    await _concurrentJobsSemaphore.WaitAsync();
    try
    {
        // Elaborazione sicura multi-thread
    }
    finally
    {
        _concurrentJobsSemaphore.Release();
    }
}
}

### Controllo Risorse

- 🎛️ **Connection Pooling** - Riutilizzo connessioni database
- 🔒 **Semaphore** - Limitazione job concorrenti
- 💾 **Memory Management** - Streaming per file grandi
- ⏱️ **Timeout** - Gestione timeout connessioni

## Estensibilità

### Aggiungere Nuovo Database Provider

1. **Implementare IDatabaseProvider:**

public class MySqlDatabaseProvider : IDatabaseProvider
{
public string ProviderName => "MySQL";

public IDbConnection CreateConnection(DatabaseConnectionConfig config)
{
    return new MySqlConnection(config.ConnectionString);
}

// Implementare altri metodi...
}

2. **Registrare nel Factory:**

public class ConnectionFactory
{
public ConnectionFactory()
{
_providers = new Dictionary<string, IDatabaseProvider>
{
// Provider esistenti...
{ "MySQL", new MySqlDatabaseProvider() }
};
}
}

### Aggiungere Nuovo Comando CLI

1. **Implementare ICommand:**

public class BackupCommand : ICommand
{
public async Task<int> ExecuteAsync(Dictionary<string, string> arguments)
{
// Logica comando backup
return 0;
}
}

2. **Registrare in Application:**

_commands = new Dictionary<string, ICommand>
{
// Comandi esistenti...
{ "backup", new BackupCommand(_services) }
};

## Performance e Scalabilità

### Ottimizzazioni Implementate

**Database:**
- 🗃️ **Connection Pooling** - Riutilizzo connessioni
- 📊 **Streaming ResultSet** - Per dataset grandi
- 🔄 **Bulk Operations** - Quando supportate dal provider
- ⏱️ **Timeout Configurabili** - Per query long-running

**Memory Management:**
- 💾 **Streaming CSV Generation** - Evita caricamento in memoria
- 🗑️ **Automatic Disposal** - Pattern using per rilascio risorse
- 📈 **Configurable Batch Size** - Controllo utilizzo memoria
- 🧹 **Garbage Collection Hints** - Per elaborazioni intensive

**Network:**
- 🔄 **Connection Reuse** - SFTP connection pooling
- 📦 **Compression** - Opzionale per file grandi
- 🔁 **Retry Logic** - Con backoff esponenziale
- ⏰ **Configurable Timeouts** - Adattabili all'environment

### Metriche di Performance

public class ProcessingResult
{
public int RecordsProcessed { get; set; }
public long FileSizeBytes { get; set; }
public TimeSpan ProcessingTime { get; set; }
public TimeSpan DatabaseQueryTime { get; set; }
public TimeSpan CsvGenerationTime { get; set; }
public TimeSpan SftpUploadTime { get; set; }

// Metriche calcolate
public double RecordsPerSecond => 
    ProcessingTime.TotalSeconds > 0 ? 
    RecordsProcessed / ProcessingTime.TotalSeconds : 0;
    
public double ThroughputMBps => 
    ProcessingTime.TotalSeconds > 0 ? 
    (FileSizeBytes / 1024.0 / 1024.0) / ProcessingTime.TotalSeconds : 0;
}

## Sicurezza

### Implementazioni di Sicurezza

**Credenziali:**
- 🔐 **Encryption at Rest** - Password crittografate in configurazione
- 🌍 **Environment Variables** - Per credenziali sensibili
- 🔑 **Key Management** - Chiavi crittografia derivate dal sistema
- 🔒 **Secure Storage** - Utilizzo credential manager OS

**Network:**
- 🛡️ **TLS/SSL** - Connessioni crittografate obbligatorie
- ✅ **Host Verification** - Verifica certificati SFTP
- 🌐 **IP Whitelisting** - Restrizioni IP configurabili
- 🚫 **Host Blacklisting** - Blocco host sospetti

**Audit:**
- 📝 **Comprehensive Logging** - Tutti gli accessi registrati
- 👤 **User Activity Tracking** - Chi, cosa, quando
- 🔍 **Security Event Monitoring** - Tentativi di accesso falliti
- 📊 **Compliance Reporting** - Report per audit

## Testing Strategy

### Piramide dei Test

         / \
        /   \
       / UI  \
      /_______\
     /         \
    /Integration\
   /_____________\
  /               \
 /   Unit Tests    \
/___________________\

**Unit Tests (Base):**
- ✅ Ogni metodo pubblico testato
- ✅ Mock di tutte le dipendenze esterne
- ✅ Test dei casi limite e errori
- ✅ Coverage > 80%

**Integration Tests (Medio):**
- ✅ Test con database reali (TestContainers)
- ✅ Test SFTP con server di test
- ✅ Test configurazione completa
- ✅ Test performance su dataset reali

**End-to-End Tests (Vertice):**
- ✅ Scenari completi utente
- ✅ Test batch con file reali
- ✅ Test failure e recovery
- ✅ Test in ambienti simil-produzione

### Test Architecture

// Unit Test Example
[Fact]
public async Task ProcessJobAsync_WithValidJob_ShouldReturnSuccess()
{
// Arrange
var mockDb = new Mock<IDatabaseService>();
var mockSftp = new Mock<ISftpService>();
var service = new CsvProcessingService(mockDb.Object, mockSftp.Object);
// Act
var result = await service.ProcessJobAsync(validJob);

// Assert
Assert.True(result.IsSuccess);
mockDb.Verify(x => x.ExecuteQueryAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

}

// Integration Test Example
[Fact]
public async Task DatabaseService_WithRealOracle_ShouldConnect()
{
// Arrange - Using TestContainers
await using var container = new OracleContainer();
await container.StartAsync();
var connectionString = container.GetConnectionString();
var service = new DatabaseService(connectionString);

// Act & Assert
var canConnect = await service.TestConnectionAsync();
Assert.True(canConnect);
}

## Monitoring e Observability

### Structured Logging

public class CsvProcessingService
{
private readonly ILogger<CsvProcessingService> _logger;

public async Task<ProcessingResult> ProcessJobAsync(TransferJob job)
{
    using var scope = _logger.BeginScope(new Dictionary<string, object>
    {
        ["JobId"] = job.Id,
        ["TableName"] = job.TableName,
        ["DatabaseConnection"] = job.DatabaseConnection
    });
    
    _logger.LogInformation("Starting job processing for {TableName}", job.TableName);
    
    try
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Processing...
        
        _logger.LogInformation("Job completed successfully in {ElapsedMs}ms. Records: {RecordCount}, Size: {FileSizeMB}MB", 
            stopwatch.ElapsedMilliseconds, 
            result.RecordsProcessed, 
            result.FileSizeBytes / 1024.0 / 1024.0);
            
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Job processing failed for {TableName}", job.TableName);
        throw;
    }
}
}

### Health Checks

public class DatabaseHealthCheck : IHealthCheck
{
private readonly IDatabaseService _databaseService;

public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context, 
    CancellationToken cancellationToken = default)
{
    try
    {
        var canConnect = await _databaseService.TestConnectionAsync("Default");
        return canConnect 
            ? HealthCheckResult.Healthy("Database connection successful")
            : HealthCheckResult.Unhealthy("Database connection failed");
    }
    catch (Exception ex)
    {
        return HealthCheckResult.Unhealthy("Database check exception", ex);
    }
}
}

### Metrics Collection

public class MetricsCollector
{
private static readonly Counter JobsProcessed = Metrics
.CreateCounter("csvapp_jobs_total", "Total jobs processed", new[] { "status", "table" });

private static readonly Histogram ProcessingDuration = Metrics
    .CreateHistogram("csvapp_processing_duration_seconds", "Time spent processing jobs");
    
private static readonly Gauge ActiveJobs = Metrics
    .CreateGauge("csvapp_active_jobs", "Number of currently active jobs");

public void RecordJobCompletion(string tableName, bool success, TimeSpan duration)
{
    JobsProcessed.WithLabels(success ? "success" : "failed", tableName).Inc();
    ProcessingDuration.Observe(duration.TotalSeconds);
}
}