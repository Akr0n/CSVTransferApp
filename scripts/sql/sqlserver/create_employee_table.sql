-- T-SQL script for Microsoft SQL Server
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.employee', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.employee (
        id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        first_name NVARCHAR(50) NOT NULL,
        last_name NVARCHAR(50) NOT NULL,
        email NVARCHAR(100) NOT NULL,
        hire_date DATE NOT NULL,
        salary DECIMAL(10,2) NOT NULL,
        department NVARCHAR(50),
        is_active BIT CONSTRAINT DF_employee_is_active DEFAULT ((1)),
        created_at DATETIME2 CONSTRAINT DF_employee_created DEFAULT (GETDATE()),
        updated_at DATETIME2 CONSTRAINT DF_employee_updated DEFAULT (GETDATE()),
        CONSTRAINT UQ_employee_email UNIQUE (email)
    );
END;
GO
-- Sample data
INSERT INTO dbo.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('John', 'Doe', 'john.doe@company.com', '2023-01-15', 75000.00, 'IT'),
('Jane', 'Smith', 'jane.smith@company.com', '2023-02-01', 82000.00, 'HR'),
('Bob', 'Johnson', 'bob.johnson@company.com', '2023-03-10', 65000.00, 'Marketing'),
('Alice', 'Williams', 'alice.williams@company.com', '2023-04-20', 71000.00, 'IT'),
('Charlie', 'Brown', 'charlie.brown@company.com', '2023-05-05', 68000.00, 'Finance');
GO
