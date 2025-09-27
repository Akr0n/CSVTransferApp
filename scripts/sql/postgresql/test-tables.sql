-- PostgreSQL Test Tables for CSV Transfer Application

-- Drop tables if they exist (for clean setup)
DROP TABLE IF EXISTS order_details CASCADE;
DROP TABLE IF EXISTS orders CASCADE;
DROP TABLE IF EXISTS products CASCADE;
DROP TABLE IF EXISTS categories CASCADE;
DROP TABLE IF EXISTS suppliers CASCADE;
DROP TABLE IF EXISTS employees CASCADE;
DROP TABLE IF EXISTS departments CASCADE;
DROP TABLE IF EXISTS customers CASCADE;

-- Departments table
CREATE TABLE departments (
    department_id SERIAL PRIMARY KEY,
    department_name VARCHAR(50) NOT NULL,
    manager_id INTEGER,
    location VARCHAR(100),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Employees table
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone VARCHAR(20),
    hire_date DATE NOT NULL,
    salary DECIMAL(10,2),
    department_id INTEGER,
    manager_id INTEGER,
    active BOOLEAN DEFAULT true,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (department_id) REFERENCES departments(department_id),
    FOREIGN KEY (manager_id) REFERENCES employees(employee_id)
);

-- Customers table
CREATE TABLE customers (
    customer_id VARCHAR(10) PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    contact_name VARCHAR(50),
    contact_title VARCHAR(50),
    address VARCHAR(100),
    city VARCHAR(50),
    region VARCHAR(50),
    postal_code VARCHAR(20),
    country VARCHAR(50),
    phone VARCHAR(20),
    fax VARCHAR(20),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Categories table
CREATE TABLE categories (
    category_id SERIAL PRIMARY KEY,
    category_name VARCHAR(50) NOT NULL,
    description TEXT,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Suppliers table
CREATE TABLE suppliers (
    supplier_id SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    contact_name VARCHAR(50),
    contact_title VARCHAR(50),
    address VARCHAR(100),
    city VARCHAR(50),
    region VARCHAR(50),
    postal_code VARCHAR(20),
    country VARCHAR(50),
    phone VARCHAR(20),
    fax VARCHAR(20),
    homepage VARCHAR(200),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Products table
CREATE TABLE products (
    product_id SERIAL PRIMARY KEY,
    product_name VARCHAR(100) NOT NULL,
    supplier_id INTEGER,
    category_id INTEGER,
    quantity_per_unit VARCHAR(50),
    unit_price DECIMAL(10,2) DEFAULT 0,
    units_in_stock INTEGER DEFAULT 0,
    units_on_order INTEGER DEFAULT 0,
    reorder_level INTEGER DEFAULT 0,
    discontinued BOOLEAN DEFAULT false,
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (supplier_id) REFERENCES suppliers(supplier_id),
    FOREIGN KEY (category_id) REFERENCES categories(category_id)
);

-- Orders table
CREATE TABLE orders (
    order_id SERIAL PRIMARY KEY,
    customer_id VARCHAR(10) NOT NULL,
    employee_id INTEGER,
    order_date DATE NOT NULL,
    required_date DATE,
    shipped_date DATE,
    ship_via INTEGER,
    freight DECIMAL(10,2) DEFAULT 0,
    ship_name VARCHAR(100),
    ship_address VARCHAR(100),
    ship_city VARCHAR(50),
    ship_region VARCHAR(50),
    ship_postal_code VARCHAR(20),
    ship_country VARCHAR(50),
    created_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (customer_id) REFERENCES customers(customer_id),
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Order Details table
CREATE TABLE order_details (
    order_id INTEGER,
    product_id INTEGER,
    unit_price DECIMAL(10,2) NOT NULL,
    quantity INTEGER NOT NULL,
    discount DECIMAL(5,2) DEFAULT 0,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);

-- Insert sample data
-- Departments
INSERT INTO departments (department_name, manager_id, location) VALUES 
('IT', NULL, 'Building A'),
('Sales', NULL, 'Building B'),
('HR', NULL, 'Building A'),
('Finance', NULL, 'Building C');

-- Update manager references after employees are inserted
-- Employees
INSERT INTO employees (first_name, last_name, email, phone, hire_date, salary, department_id, manager_id, active) VALUES 
('John', 'Doe', 'john.doe@company.com', '555-1234', '2023-01-15', 75000, 1, NULL, true),
('Jane', 'Smith', 'jane.smith@company.com', '555-5678', '2023-03-22', 82000, 2, NULL, true),
('Mike', 'Johnson', 'mike.johnson@company.com', '555-9012', '2022-11-08', 68000, 1, 1, true),
('Sarah', 'Williams', 'sarah.williams@company.com', '555-3456', '2023-05-10', 71000, 3, NULL, true),
('Robert', 'Brown', 'robert.brown@company.com', '555-7890', '2023-02-28', 89000, 4, NULL, true);

-- Categories
INSERT INTO categories (category_name, description) VALUES 
('Electronics', 'Electronic devices and components'),
('Office Supplies', 'Office and business supplies'),
('Furniture', 'Office and home furniture'),
('Software', 'Software applications and licenses');

-- Suppliers
INSERT INTO suppliers (company_name, contact_name, contact_title, address, city, region, postal_code, country, phone, fax, homepage) VALUES 
('Tech Corp', 'Alice Cooper', 'Sales Manager', '123 Tech St', 'Tech City', 'TC', '12345', 'USA', '555-1111', '555-1112', 'www.techcorp.com'),
('Office Plus', 'Bob Wilson', 'Account Manager', '456 Office Ave', 'Business City', 'BC', '67890', 'USA', '555-2222', '555-2223', 'www.officeplus.com'),
('Furniture World', 'Carol Davis', 'Sales Rep', '789 Furniture Blvd', 'Home City', 'HC', '11111', 'USA', '555-3333', '555-3334', 'www.furnitureworld.com');

-- Products
INSERT INTO products (product_name, supplier_id, category_id, quantity_per_unit, unit_price, units_in_stock, units_on_order, reorder_level, discontinued) VALUES 
('Laptop Pro 15', 1, 1, '1 unit', 1299.99, 25, 0, 5, false),
('Wireless Mouse', 1, 1, '1 unit', 29.99, 150, 50, 20, false),
('Office Chair Executive', 3, 3, '1 unit', 299.99, 12, 0, 3, false),
('Printer Paper A4', 2, 2, '500 sheets', 12.99, 200, 100, 50, false),
('Project Management Software', 1, 4, '1 license', 199.99, 0, 10, 0, false);

-- Customers
INSERT INTO customers VALUES 
('CUST01', 'ABC Corporation', 'David Miller', 'CEO', '100 Business St', 'Metro City', 'MC', '22222', 'USA', '555-4444', '555-4445', CURRENT_TIMESTAMP),
('CUST02', 'XYZ Enterprises', 'Emma Johnson', 'Procurement Manager', '200 Commerce Ave', 'Trade City', 'TC', '33333', 'USA', '555-5555', '555-5556', CURRENT_TIMESTAMP),
('CUST03', 'Global Solutions Inc', 'Frank Anderson', 'IT Director', '300 Solution Blvd', 'Innovation City', 'IC', '44444', 'USA', '555-6666', '555-6667', CURRENT_TIMESTAMP);

-- Orders
INSERT INTO orders (customer_id, employee_id, order_date, required_date, shipped_date, ship_via, freight, ship_name, ship_address, ship_city, ship_region, ship_postal_code, ship_country) VALUES 
('CUST01', 2, '2025-09-01', '2025-09-15', '2025-09-10', 1, 25.50, 'ABC Corporation', '100 Business St', 'Metro City', 'MC', '22222', 'USA'),
('CUST02', 2, '2025-09-15', '2025-09-30', NULL, 2, 15.75, 'XYZ Enterprises', '200 Commerce Ave', 'Trade City', 'TC', '33333', 'USA'),
('CUST03', 1, '2025-09-20', '2025-10-05', NULL, 1, 35.25, 'Global Solutions Inc', '300 Solution Blvd', 'Innovation City', 'IC', '44444', 'USA');

-- Order Details
INSERT INTO order_details VALUES 
(1, 1, 1299.99, 2, 0.05),
(1, 2, 29.99, 5, 0.00),
(2, 3, 299.99, 1, 0.00),
(2, 4, 12.99, 10, 0.10),
(3, 5, 199.99, 3, 0.00),
(3, 1, 1299.99, 1, 0.00);

-- Create indexes for better performance
CREATE INDEX idx_employees_department_id ON employees(department_id);
CREATE INDEX idx_employees_manager_id ON employees(manager_id);
CREATE INDEX idx_products_category_id ON products(category_id);
CREATE INDEX idx_products_supplier_id ON products(supplier_id);
CREATE INDEX idx_orders_customer_id ON orders(customer_id);
CREATE INDEX idx_orders_employee_id ON orders(employee_id);
CREATE INDEX idx_orders_order_date ON orders(order_date);

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
