using System.Data;
using FluentAssertions;

namespace CSVTransferApp.Data.Tests.TestFixtures;

public static class TestDatabaseHelper
{
    public static DataTable CreateTestTable(string tableName, params (string Name, Type Type)[] columns)
    {
        var table = new DataTable(tableName);
        
        foreach (var (name, type) in columns)
        {
            table.Columns.Add(name, type);
        }

        return table;
    }

    public static void AddTestRow(DataTable table, params object[] values)
    {
        if (values.Length != table.Columns.Count)
            throw new ArgumentException("Number of values must match number of columns");

        var row = table.NewRow();
        for (int i = 0; i < values.Length; i++)
        {
            row[i] = values[i];
        }
        table.Rows.Add(row);
    }

    public static DataTable CreateEmployeeTable()
    {
        var table = CreateTestTable("employees",
            ("id", typeof(int)),
            ("name", typeof(string)),
            ("department", typeof(string)),
            ("salary", typeof(decimal)));

        AddTestRow(table, 1, "John Doe", "IT", 75000m);
        AddTestRow(table, 2, "Jane Smith", "HR", 65000m);
        AddTestRow(table, 3, "Bob Johnson", "Finance", 70000m);

        return table;
    }

    // Returns sanitized, non-sensitive mock connection strings for tests only.
    // No real credentials are embedded; providers that normally require passwords
    // are intentionally returned without a password segment.
    public static string GetMockConnectionString(string provider) => provider switch
    {
        "Oracle" => "Data Source=mock:1521/test;User Id=mock;", // no password
        "SqlServer" => "Server=mock;Database=mock;Integrated Security=true;", // no password by design
        "PostgreSQL" => "Host=mock;Database=mock;Username=mock", // no password
        _ => "mock://connection"
    };

    public static void AssertDataTableStructure(DataTable table, string expectedTableName, params string[] expectedColumns)
    {
        table.TableName.Should().Be(expectedTableName);
        table.Columns.Count.Should().Be(expectedColumns.Length);
        
        foreach (var expectedColumn in expectedColumns)
        {
            table.Columns.Contains(expectedColumn).Should().BeTrue($"Column '{expectedColumn}' should exist");
        }
    }
}
