using System.Data;
using System.Text;

namespace CSVTransferApp.Core.Extensions;

public static class DataTableExtensions
{
    public static MemoryStream ToCsvStream(this DataTable dataTable, string[] customHeaders = null!)
    {
        var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
        
        // Write headers
        var headers = customHeaders ?? dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
        writer.Write(string.Join(",", headers.Select(EscapeCsvValue)));
        writer.Write("\n");
        
        // Write data rows
        foreach (DataRow row in dataTable.Rows)
        {
            var values = row.ItemArray.Select(field => EscapeCsvValue(field?.ToString() ?? ""));
            writer.Write(string.Join(",", values));
            writer.Write("\n");
        }
        
        writer.Flush();
        stream.Position = 0;
        return stream;
    }

    private static string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
        
        return value;
    }

    public static Dictionary<string, Type> GetColumnTypes(this DataTable dataTable)
    {
        return dataTable.Columns.Cast<DataColumn>()
            .ToDictionary(col => col.ColumnName, col => col.DataType);
    }
}
