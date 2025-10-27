-- Oracle schema objects
-- NOTE: Do NOT commit real passwords. If you need to create the CSVAPP user, run as a DBA and set a secure password.
-- Example (uncomment and replace <secure_password>):
-- CREATE USER CSVAPP IDENTIFIED BY <secure_password>
--     DEFAULT TABLESPACE USERS
--     TEMPORARY TABLESPACE TEMP
--     QUOTA UNLIMITED ON USERS;
-- GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE, CREATE TRIGGER, CREATE PROCEDURE TO CSVAPP;

-- Table (connect as CSVAPP)
CREATE TABLE CSVAPP.employee (
    id NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name VARCHAR2(50) NOT NULL,
    last_name  VARCHAR2(50) NOT NULL,
    email      VARCHAR2(100) NOT NULL UNIQUE,
    hire_date  DATE NOT NULL,
    salary     NUMBER(10,2) NOT NULL,
    department VARCHAR2(50),
    is_active  NUMBER(1) DEFAULT 1,
    created_at TIMESTAMP DEFAULT SYSTIMESTAMP,
    updated_at TIMESTAMP DEFAULT SYSTIMESTAMP
);

-- Sample data
INSERT INTO CSVAPP.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('John', 'Doe', 'john.doe@company.com', DATE '2023-01-15', 75000.00, 'IT');
INSERT INTO CSVAPP.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Jane', 'Smith', 'jane.smith@company.com', DATE '2023-02-01', 82000.00, 'HR');
INSERT INTO CSVAPP.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Bob', 'Johnson', 'bob.johnson@company.com', DATE '2023-03-10', 65000.00, 'Marketing');
INSERT INTO CSVAPP.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Alice', 'Williams', 'alice.williams@company.com', DATE '2023-04-20', 71000.00, 'IT');
INSERT INTO CSVAPP.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('Charlie', 'Brown', 'charlie.brown@company.com', DATE '2023-05-05', 68000.00, 'Finance');
