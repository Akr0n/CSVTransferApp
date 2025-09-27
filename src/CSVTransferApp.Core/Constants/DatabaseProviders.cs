namespace CSVTransferApp.Core.Constants;

public static class DatabaseProviders
{
    public const string Oracle = "Oracle.EntityFrameworkCore";
    public const string SqlServer = "Microsoft.EntityFrameworkCore.SqlServer";
    public const string PostgreSQL = "Npgsql.EntityFrameworkCore.PostgreSQL";
    
    public static readonly string[] SupportedProviders = { Oracle, SqlServer, PostgreSQL };
    
    public static bool IsSupported(string provider) => SupportedProviders.Contains(provider);
}
