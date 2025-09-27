using System.Data;
using CSVTransferApp.Core.Models;

namespace CSVTransferApp.Core.Tests.TestData;

public static class SampleData
{
    public static List<TransferJob> CreateTransferJobs()
    {
        return new List<TransferJob>
        {
            new()
            {
                TableName = "employees",
                DatabaseConnection = "Oracle",
                SftpConnection = "MainServer",
                Query = "SELECT * FROM employees",
                RequestTime = DateTime.UtcNow.AddMinutes(-5)
            },
            new()
            {
                TableName = "products",
                DatabaseConnection = "SqlServer",
                SftpConnection = "BackupServer",
                Query = "SELECT id, name, price FROM products WHERE active = 1",
                RequestTime = DateTime.UtcNow.AddMinutes(-3)
            },
            new()
            {
                TableName = "orders",
                DatabaseConnection = "PostgreSQL",
                SftpConnection = "MainServer",
                Query = "SELECT * FROM orders WHERE order_date >= '2025-01-01'",
                RequestTime = DateTime.UtcNow.AddMinutes(-1)
            }
        };
    }

    public static DataTable CreateEmployeesDataTable()
    {
        var table = new DataTable("employees");
        table.Columns.Add("emp_id", typeof(int));
        table.Columns.Add("first_name", typeof(string));
        table.Columns.Add("last_name", typeof(string));
        table.Columns.Add("email", typeof(string));
        table.Columns.Add("hire_date", typeof(DateTime));
        table.Columns.Add("salary", typeof(decimal));

        table.Rows.Add(1, "John", "Doe", "john.doe@company.com", new DateTime(2023, 1, 15), 75000m);
        table.Rows.Add(2, "Jane", "Smith", "jane.smith@company.com", new DateTime(2023, 3, 22), 82000m);
        table.Rows.Add(3, "Mike", "Johnson", "mike.johnson@company.com", new DateTime(2022, 11, 8), 68000m);

        return table;
    }

    public static DataTable CreateProductsDataTable()
    {
        var table = new DataTable("products");
        table.Columns.Add("product_id", typeof(int));
        table.Columns.Add("name", typeof(string));
        table.Columns.Add("category", typeof(string));
        table.Columns.Add("price", typeof(decimal));
        table.Columns.Add("active", typeof(bool));

        table.Rows.Add(1, "Laptop Pro 15", "Electronics", 1299.99m, true);
        table.Rows.Add(2, "Wireless Mouse", "Electronics", 29.99m, true);
        table.Rows.Add(3, "Office Chair", "Furniture", 199.99m, false);

        return table;
    }

    public static Dictionary<string, string> CreateHeaderMappings()
    {
        return new Dictionary<string, string>
        {
            { "emp_id", "Employee ID" },
            { "first_name", "First Name" },
            { "last_name", "Last Name" },
            { "hire_date", "Hire Date" },
            { "product_id", "Product ID" }
        };
    }
}
