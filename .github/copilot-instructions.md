# Copilot Instructions for CSVTransferApp

## Project Overview
CSVTransferApp is a .NET 9.0 application that facilitates automated CSV data transfers between databases and SFTP servers. It follows Clean Architecture principles with distinct layers (Core, Data, Services, Infrastructure, Console UI). The application handles concurrent transfers using connection pooling and resource throttling via semaphores.

## Key Architecture Points
- **Core Layer** (`CSVTransferApp.Core/`): Contains domain models, interfaces, and business logic with zero external dependencies
- **Data Layer** (`CSVTransferApp.Data/`): Implements database access using Repository and Factory patterns
- **Services Layer** (`CSVTransferApp.Services/`): Contains business logic and orchestration
- **Infrastructure Layer** (`CSVTransferApp.Infrastructure/`): Cross-cutting concerns (logging, security, config)
- **Console UI** (`CSVTransferApp.Console/`): Command-line interface using Command pattern

## Developer Workflows

### Building & Testing
```powershell
# Build solution
dotnet build CSVTransferApp.sln

# Run tests (including integration tests with TestContainers)
dotnet test

# Publish console app
dotnet publish src/CSVTransferApp.Console/CSVTransferApp.Console.csproj
```

### Configuration Files
- `config/database-connections.json`: Database connection strings 
- `config/sftp-connections.json`: SFTP server configurations
- `config/header-overrides/*.json`: CSV header mapping files

## Project-Specific Patterns

### Dependency Injection
- **Singleton**: Stateless services (logging, configuration)
- **Scoped**: Services with request state (database connections)
- **Transient**: Lightweight operation services (processing, SFTP)

### Database Access
- Use `DatabaseService` through `IDatabaseService` interface
- Connection pooling using `ConcurrentDictionary<string, IDbConnection>`
- Provider-specific implementations via Strategy pattern (Oracle, SQL Server, PostgreSQL)
- Connection strings and providers configured in `database-connections.json`

### Concurrency Management
- Resource throttling via `SemaphoreSlim` for connections and files
- Configure limits in `appsettings.json` under `Processing:MaxConcurrentConnections` and `Processing:MaxConcurrentFiles`
- Connection pooling ensures efficient database connection reuse
- Async/await patterns throughout for non-blocking operations

### Error Handling
- **Custom Exceptions**:
  - Domain-specific exceptions in `Core/Exceptions/` (DatabaseConnectionException, SftpConnectionException, CsvProcessingException)
  - Always include context details and inner exception

- **Structured Logging**:
  - Use `LoggingService` with semantic logging patterns
  - Include correlation IDs and contextual data in log scopes
  - Log levels: Information for operations, Warning for recoverable issues, Error for failures
  - Example:
  ```csharp
  using var scope = _logger.BeginScope(new Dictionary<string, object>
  {
      ["JobId"] = job.Id,
      ["TableName"] = job.TableName
  });
  _logger.LogInformation("Starting job processing for {TableName}", job.TableName);
  ```

- **Retry Patterns**:
  - Network operations (database, SFTP) implement exponential backoff
  - Configurable retry counts and delays in appsettings.json
  - Circuit breaker pattern for external service calls

- **Error Recovery**:
  - Automatic cleanup of partial files on failure
  - Transaction rollback for database operations
  - Detailed error codes and solutions in API documentation

- **Monitoring & Alerts**:
  - Structured error logging to files with rotation
  - Error metrics collection via metrics service
  - Email notifications for critical failures (configurable)
  - Log file paths: `logs/{Category}/app-.log`

### Testing
- Unit tests with mocked dependencies
- Integration tests using TestContainers
- E2E tests covering full workflows

## Common Operations

### Adding New Database Provider
1. Create provider class implementing `IDatabaseProvider`
2. Add to `DatabaseProviders.cs` constants
3. Update `ConnectionFactory.cs`
4. Add integration tests

### CSV Processing Pipeline
```csharp
// Single job processing
var job = new TransferJob {
    TableName = "employees",
    DatabaseConnection = "OracleDB",
    SftpConnection = "MainServer",
    Query = "SELECT * FROM employees"
};
await csvProcessingService.ProcessJobAsync(job);

// Batch job processing with concurrency control
var jobs = new List<TransferJob> {
    new() { TableName = "employees", ... },
    new() { TableName = "orders", ... }
};
// Jobs will be processed concurrently up to configured limits
await csvProcessingService.ProcessJobsAsync(jobs);
```

## References
- Architecture details: `docs/ARCHITECTURE.md`
- API documentation: `docs/API.md`
- Configuration guide: `docs/CONFIGURATION.md`

**For AI Agents:**
- Follow Clean Architecture principles - keep Core layer free of external dependencies
- Use interfaces defined in Core for all service implementations
- Add integration tests for database/SFTP interactions
- Follow existing error handling and logging patterns
- Update documentation when modifying public interfaces
