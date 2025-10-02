-- Oracle: Prima creare lo schema/user
CREATE USER CSVAPP IDENTIFIED BY csvapp123
    DEFAULT TABLESPACE USERS
    TEMPORARY TABLESPACE TEMP
    QUOTA UNLIMITED ON USERS;

-- Concedere i permessi necessari
GRANT CREATE SESSION TO CSVAPP;
GRANT CREATE TABLE TO CSVAPP;
GRANT CREATE SEQUENCE TO CSVAPP;
GRANT CREATE TRIGGER TO CSVAPP;
GRANT CREATE PROCEDURE TO CSVAPP;
/

-- PostgreSQL (schema public)
CREATE TABLE IF NOT EXISTS public.employee (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    hire_date DATE NOT NULL,
    salary DECIMAL(10,2) NOT NULL,
    department VARCHAR(50),
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- SQL Server (schema dbo)
USE CSVTransferDB_Dev;
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[employee]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.employee (
        id INT IDENTITY(1,1) PRIMARY KEY,
        first_name NVARCHAR(50) NOT NULL,
        last_name NVARCHAR(50) NOT NULL,
        email NVARCHAR(100) NOT NULL UNIQUE,
        hire_date DATE NOT NULL,
        salary DECIMAL(10,2) NOT NULL,
        department NVARCHAR(50),
        is_active BIT DEFAULT 1,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );
END;

-- Oracle (schema CSVAPP)
-- Connettersi come utente CSVAPP per creare la tabella
CREATE TABLE CSVAPP.employee (
    id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name VARCHAR2(50) NOT NULL,
    last_name VARCHAR2(50) NOT NULL,
    email VARCHAR2(100) NOT NULL UNIQUE,
    hire_date DATE NOT NULL,
    salary NUMBER(10,2) NOT NULL,
    department VARCHAR2(50),
    is_active NUMBER(1) DEFAULT 1,
    created_at TIMESTAMP DEFAULT SYSTIMESTAMP,
    updated_at TIMESTAMP DEFAULT SYSTIMESTAMP
);
/

-- Sample data (PostgreSQL & SQL Server syntax)
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('John', 'Doe', 'john.doe@company.com', '2023-01-15', 75000.00, 'IT'),
('Jane', 'Smith', 'jane.smith@company.com', '2023-02-01', 82000.00, 'HR'),
('Bob', 'Johnson', 'bob.johnson@company.com', '2023-03-10', 65000.00, 'Marketing'),
('Alice', 'Williams', 'alice.williams@company.com', '2023-04-20', 71000.00, 'IT'),
('Charlie', 'Brown', 'charlie.brown@company.com', '2023-05-05', 68000.00, 'Finance');

-- Oracle sample data
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('John', 'Doe', 'john.doe@company.com', DATE '2023-01-15', 75000.00, 'IT');
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Jane', 'Smith', 'jane.smith@company.com', DATE '2023-02-01', 82000.00, 'HR');
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Bob', 'Johnson', 'bob.johnson@company.com', DATE '2023-03-10', 65000.00, 'Marketing');
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Alice', 'Williams', 'alice.williams@company.com', DATE '2023-04-20', 71000.00, 'IT');
INSERT INTO employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Charlie', 'Brown', 'charlie.brown@company.com', DATE '2023-05-05', 68000.00, 'Finance');