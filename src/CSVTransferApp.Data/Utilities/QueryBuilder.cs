using System.Text;

namespace CSVTransferApp.Data.Utilities;

public class QueryBuilder
{
    public static string BuildSelectAllQuery(string tableName)
    {
        return $"SELECT * FROM {EscapeIdentifier(tableName)}";
    }

    public static string BuildCountQuery(string tableName, string? whereClause = null)
    {
        var query = $"SELECT COUNT(*) FROM {EscapeIdentifier(tableName)}";
        
        if (!string.IsNullOrWhiteSpace(whereClause))
            query += $" WHERE {whereClause}";
            
        return query;
    }

    public static string BuildSelectQuery(string tableName, IEnumerable<string> columns, 
        string? whereClause = null, string? orderBy = null, int? limit = null)
    {
        var escapedColumns = columns.Select(EscapeIdentifier);
        var query = new StringBuilder($"SELECT {string.Join(", ", escapedColumns)} FROM {EscapeIdentifier(tableName)}");
        
        if (!string.IsNullOrWhiteSpace(whereClause))
            query.Append($" WHERE {whereClause}");
            
        if (!string.IsNullOrWhiteSpace(orderBy))
            query.Append($" ORDER BY {orderBy}");
            
        if (limit.HasValue)
            query.Append($" LIMIT {limit.Value}");
            
        return query.ToString();
    }

    private static string EscapeIdentifier(string identifier)
    {
        // Basic escaping - in production, use provider-specific escaping
        return identifier.Contains(' ') || identifier.Contains('-') 
            ? $"[{identifier}]" 
            : identifier;
    }

    public static string BuildParameterizedQuery(string baseQuery, Dictionary<string, object> parameters)
    {
        var query = baseQuery;
        foreach (var param in parameters)
        {
            query = query.Replace($"@{param.Key}", param.Value?.ToString() ?? "NULL");
        }
        return query;
    }
}
