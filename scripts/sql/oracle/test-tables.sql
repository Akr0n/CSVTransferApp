-- Oracle Test Tables for CSV Transfer Application

-- Drop tables if they exist (for clean setup)
DROP TABLE order_details CASCADE CONSTRAINTS;
DROP TABLE orders CASCADE CONSTRAINTS;
DROP TABLE products CASCADE CONSTRAINTS;
DROP TABLE categories CASCADE CONSTRAINTS;
DROP TABLE suppliers CASCADE CONSTRAINTS;
DROP TABLE employees CASCADE CONSTRAINTS;
DROP TABLE departments CASCADE CONSTRAINTS;
DROP TABLE customers CASCADE CONSTRAINTS;

-- Departments table
CREATE TABLE departments (
    department_id NUMBER(10) PRIMARY KEY,
    department_name VARCHAR2(50) NOT NULL,
    manager_id NUMBER(10),
    location VARCHAR2(100),
    created_date DATE DEFAULT SYSDATE
);

-- Employees table
CREATE TABLE employees (
    employee_id NUMBER(10) PRIMARY KEY,
    first_name VARCHAR2(50) NOT NULL,
    last_name VARCHAR2(50) NOT NULL,
    email VARCHAR2(100) UNIQUE NOT NULL,
    phone VARCHAR2(20),
    hire_date DATE NOT NULL,
    salary NUMBER(10,2),
    department_id NUMBER(10),
    manager_id NUMBER(10),
    active VARCHAR2(1) DEFAULT 'Y' CHECK (active IN ('Y', 'N')),
    created_date DATE DEFAULT SYSDATE,
    FOREIGN KEY (department_id) REFERENCES departments(department_id),
    FOREIGN KEY (manager_id) REFERENCES employees(employee_id)
);

-- Customers table
CREATE TABLE customers (
    customer_id VARCHAR2(10) PRIMARY KEY,
    company_name VARCHAR2(100) NOT NULL,
    contact_name VARCHAR2(50),
    contact_title VARCHAR2(50),
    address VARCHAR2(100),
    city VARCHAR2(50),
    region VARCHAR2(50),
    postal_code VARCHAR2(20),
    country VARCHAR2(50),
    phone VARCHAR2(20),
    fax VARCHAR2(20),
    created_date DATE DEFAULT SYSDATE
);

-- Categories table
CREATE TABLE categories (
    category_id NUMBER(10) PRIMARY KEY,
    category_name VARCHAR2(50) NOT NULL,
    description VARCHAR2(500),
    created_date DATE DEFAULT SYSDATE
);

-- Suppliers table
CREATE TABLE suppliers (
    supplier_id NUMBER(10) PRIMARY KEY,
    company_name VARCHAR2(100) NOT NULL,
    contact_name VARCHAR2(50),
    contact_title VARCHAR2(50),
    address VARCHAR2(100),
    city VARCHAR2(50),
    region VARCHAR2(50),
    postal_code VARCHAR2(20),
    country VARCHAR2(50),
    phone VARCHAR2(20),
    fax VARCHAR2(20),
    homepage VARCHAR2(200),
    created_date DATE DEFAULT SYSDATE
);

-- Products table
CREATE TABLE products (
    product_id NUMBER(10) PRIMARY KEY,
    product_name VARCHAR2(100) NOT NULL,
    supplier_id NUMBER(10),
    category_id NUMBER(10),
    quantity_per_unit VARCHAR2(50),
    unit_price NUMBER(10,2) DEFAULT 0,
    units_in_stock NUMBER(10) DEFAULT 0,
    units_on_order NUMBER(10) DEFAULT 0,
    reorder_level NUMBER(10) DEFAULT 0,
    discontinued NUMBER(1) DEFAULT 0 CHECK (discontinued IN (0, 1)),
    created_date DATE DEFAULT SYSDATE,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id),
    FOREIGN KEY (category_id) REFERENCES categories(category_id)
);

-- Orders table
CREATE TABLE orders (
    order_id NUMBER(10) PRIMARY KEY,
    customer_id VARCHAR2(10) NOT NULL,
    employee_id NUMBER(10),
    order_date DATE NOT NULL,
    required_date DATE,
    shipped_date DATE,
    ship_via NUMBER(10),
    freight NUMBER(10,2) DEFAULT 0,
    ship_name VARCHAR2(100),
    ship_address VARCHAR2(100),
    ship_city VARCHAR2(50),
    ship_region VARCHAR2(50),
    ship_postal_code VARCHAR2(20),
    ship_country VARCHAR2(50),
    created_date DATE DEFAULT SYSDATE,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Order Details table
CREATE TABLE order_details (
    order_id NUMBER(10),
    product_id NUMBER(10),
    unit_price NUMBER(10,2) NOT NULL,
    quantity NUMBER(10) NOT NULL,
    discount NUMBER(5,2) DEFAULT 0,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

-- Create sequences for auto-increment
CREATE SEQUENCE dept_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE emp_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE cat_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE sup_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE prod_seq START WITH 1 INCREMENT BY 1;
CREATE SEQUENCE order_seq START WITH 1 INCREMENT BY 1;

-- Insert sample data
-- Departments
INSERT INTO departments VALUES (dept_seq.NEXTVAL, 'IT', NULL, 'Building A', SYSDATE);
INSERT INTO departments VALUES (dept_seq.NEXTVAL, 'Sales', NULL, 'Building B', SYSDATE);
INSERT INTO departments VALUES (dept_seq.NEXTVAL, 'HR', NULL, 'Building A', SYSDATE);
INSERT INTO departments VALUES (dept_seq.NEXTVAL, 'Finance', NULL, 'Building C', SYSDATE);

-- Employees
INSERT INTO employees VALUES (emp_seq.NEXTVAL, 'John', 'Doe', 'john.doe@company.com', '555-1234', DATE '2023-01-15', 75000, 1, NULL, 'Y', SYSDATE);
INSERT INTO employees VALUES (emp_seq.NEXTVAL, 'Jane', 'Smith', 'jane.smith@company.com', '555-5678', DATE '2023-03-22', 82000, 2, NULL, 'Y', SYSDATE);
INSERT INTO employees VALUES (emp_seq.NEXTVAL, 'Mike', 'Johnson', 'mike.johnson@company.com', '555-9012', DATE '2022-11-08', 68000, 1, 1, 'Y', SYSDATE);
INSERT INTO employees VALUES (emp_seq.NEXTVAL, 'Sarah', 'Williams', 'sarah.williams@company.com', '555-3456', DATE '2023-05-10', 71000, 3, NULL, 'Y', SYSDATE);
INSERT INTO employees VALUES (emp_seq.NEXTVAL, 'Robert', 'Brown', 'robert.brown@company.com', '555-7890', DATE '2023-02-28', 89000, 4, NULL, 'Y', SYSDATE);

-- Categories
INSERT INTO categories VALUES (cat_seq.NEXTVAL, 'Electronics', 'Electronic devices and components', SYSDATE);
INSERT INTO categories VALUES (cat_seq.NEXTVAL, 'Office Supplies', 'Office and business supplies', SYSDATE);
INSERT INTO categories VALUES (cat_seq.NEXTVAL, 'Furniture', 'Office and home furniture', SYSDATE);
INSERT INTO categories VALUES (cat_seq.NEXTVAL, 'Software', 'Software applications and licenses', SYSDATE);

-- Suppliers
INSERT INTO suppliers VALUES (sup_seq.NEXTVAL, 'Tech Corp', 'Alice Cooper', 'Sales Manager', '123 Tech St', 'Tech City', 'TC', '12345', 'USA', '555-1111', '555-1112', 'www.techcorp.com', SYSDATE);
INSERT INTO suppliers VALUES (sup_seq.NEXTVAL, 'Office Plus', 'Bob Wilson', 'Account Manager', '456 Office Ave', 'Business City', 'BC', '67890', 'USA', '555-2222', '555-2223', 'www.officeplus.com', SYSDATE);
INSERT INTO suppliers VALUES (sup_seq.NEXTVAL, 'Furniture World', 'Carol Davis', 'Sales Rep', '789 Furniture Blvd', 'Home City', 'HC', '11111', 'USA', '555-3333', '555-3334', 'www.furnitureworld.com', SYSDATE);

-- Products
INSERT INTO products VALUES (prod_seq.NEXTVAL, 'Laptop Pro 15', 1, 1, '1 unit', 1299.99, 25, 0, 5, 0, SYSDATE);
INSERT INTO products VALUES (prod_seq.NEXTVAL, 'Wireless Mouse', 1, 1, '1 unit', 29.99, 150, 50, 20, 0, SYSDATE);
INSERT INTO products VALUES (prod_seq.NEXTVAL, 'Office Chair Executive', 3, 3, '1 unit', 299.99, 12, 0, 3, 0, SYSDATE);
INSERT INTO products VALUES (prod_seq.NEXTVAL, 'Printer Paper A4', 2, 2, '500 sheets', 12.99, 200, 100, 50, 0, SYSDATE);
INSERT INTO products VALUES (prod_seq.NEXTVAL, 'Project Management Software', 1, 4, '1 license', 199.99, 0, 10, 0, 0, SYSDATE);

-- Customers
INSERT INTO customers VALUES ('CUST01', 'ABC Corporation', 'David Miller', 'CEO', '100 Business St', 'Metro City', 'MC', '22222', 'USA', '555-4444', '555-4445', SYSDATE);
INSERT INTO customers VALUES ('CUST02', 'XYZ Enterprises', 'Emma Johnson', 'Procurement Manager', '200 Commerce Ave', 'Trade City', 'TC', '33333', 'USA', '555-5555', '555-5556', SYSDATE);
INSERT INTO customers VALUES ('CUST03', 'Global Solutions Inc', 'Frank Anderson', 'IT Director', '300 Solution Blvd', 'Innovation City', 'IC', '44444', 'USA', '555-6666', '555-6667', SYSDATE);

-- Orders
INSERT INTO orders VALUES (order_seq.NEXTVAL, 'CUST01', 2, DATE '2025-09-01', DATE '2025-09-15', DATE '2025-09-10', 1, 25.50, 'ABC Corporation', '100 Business St', 'Metro City', 'MC', '22222', 'USA', SYSDATE);
INSERT INTO orders VALUES (order_seq.NEXTVAL, 'CUST02', 2, DATE '2025-09-15', DATE '2025-09-30', NULL, 2, 15.75, 'XYZ Enterprises', '200 Commerce Ave', 'Trade City', 'TC', '33333', 'USA', SYSDATE);
INSERT INTO orders VALUES (order_seq.NEXTVAL, 'CUST03', 1, DATE '2025-09-20', DATE '2025-10-05', NULL, 1, 35.25, 'Global Solutions Inc', '300 Solution Blvd', 'Innovation City', 'IC', '44444', 'USA', SYSDATE);

-- Order Details
INSERT INTO order_details VALUES (1, 1, 1299.99, 2, 0.05);
INSERT INTO order_details VALUES (1, 2, 29.99, 5, 0.00);
INSERT INTO order_details VALUES (2, 3, 299.99, 1, 0.00);
INSERT INTO order_details VALUES (2, 4, 12.99, 10, 0.10);
INSERT INTO order_details VALUES (3, 5, 199.99, 3, 0.00);
INSERT INTO order_details VALUES (3, 1, 1299.99, 1, 0.00);

COMMIT;

-- Create indexes for better performance
CREATE INDEX idx_emp_dept ON employees(department_id);
CREATE INDEX idx_emp_manager ON employees(manager_id);
CREATE INDEX idx_prod_cat ON products(category_id);
CREATE INDEX idx_prod_sup ON products(supplier_id);
CREATE INDEX idx_order_cust ON orders(customer_id);
CREATE INDEX idx_order_emp ON orders(employee_id);
CREATE INDEX idx_order_date ON orders(order_date);

-- Show table counts
SELECT 'Departments' as table_name, COUNT(*) as row_count FROM departments
UNION ALL
SELECT 'Employees', COUNT(*) FROM employees
UNION ALL
SELECT 'Customers', COUNT(*) FROM customers
UNION ALL
SELECT 'Categories', COUNT(*) FROM categories
UNION ALL
SELECT 'Suppliers', COUNT(*) FROM suppliers
UNION ALL
SELECT 'Products', COUNT(*) FROM products
UNION ALL
SELECT 'Orders', COUNT(*) FROM orders
UNION ALL
SELECT 'Order Details', COUNT(*) FROM order_details;
