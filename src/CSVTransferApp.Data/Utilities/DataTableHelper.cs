using System.Data;

namespace CSVTransferApp.Data.Utilities;

public static class DataTableHelper
{
    public static DataTable CreateEmptyTable(IEnumerable<string> columnNames)
    {
        var table = new DataTable();
        foreach (var columnName in columnNames)
        {
            table.Columns.Add(columnName, typeof(string));
        }
        return table;
    }

    public static void OptimizeDataTable(DataTable dataTable)
    {
        // Set capacity to improve performance
        if (dataTable.Rows.Count > 1000)
        {
            dataTable.MinimumCapacity = dataTable.Rows.Count;
        }

        // Optimize column data types
        foreach (DataColumn column in dataTable.Columns)
        {
            if (column.DataType == typeof(string))
            {
                var maxLength = GetMaxStringLength(dataTable, column.ColumnName);
                if (maxLength > 0 && maxLength <= 8000)
                {
                    column.MaxLength = maxLength;
                }
            }
        }
    }

    private static int GetMaxStringLength(DataTable table, string columnName)
    {
        return table.AsEnumerable()
            .Where(row => row[columnName] != DBNull.Value)
            .Select(row => row[columnName]?.ToString()?.Length ?? 0)
            .DefaultIfEmpty(0)
            .Max();
    }

    public static DataTable FilterRows(DataTable source, Func<DataRow, bool> predicate)
    {
        var filtered = source.Clone();
        var rows = source.AsEnumerable().Where(predicate);
        
        foreach (var row in rows)
        {
            filtered.ImportRow(row);
        }
        
        return filtered;
    }

    public static void LogTableInfo(DataTable table, Action<string> logger)
    {
        logger($"DataTable: {table.TableName}, Rows: {table.Rows.Count}, Columns: {table.Columns.Count}");
        foreach (DataColumn column in table.Columns)
        {
            logger($"  Column: {column.ColumnName}, Type: {column.DataType.Name}, MaxLength: {column.MaxLength}");
        }
    }
}
