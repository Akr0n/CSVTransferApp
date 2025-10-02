namespace CSVTransferApp.Core.Constants;

public static class DatabaseProviders
{
    public const string Oracle = "Oracle.ManagedDataAccess.Client";
    public const string SqlServer = "Microsoft.Data.SqlClient";
    public const string PostgreSQL = "Npgsql";
    
    public static readonly string[] SupportedProviders = { Oracle, SqlServer, PostgreSQL };
    
    public static bool IsSupported(string provider) => SupportedProviders.Contains(provider);
}
