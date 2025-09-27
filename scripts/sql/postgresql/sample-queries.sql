-- PostgreSQL Sample Queries for CSV Transfer Application

-- Basic employee export query
SELECT 
    employee_id as "ID",
    first_name as "First Name",
    last_name as "Last Name",
    email as "Email",
    hire_date as "Hire Date",
    salary as "Salary",
    department_name as "Department"
FROM employees e
JOIN departments d ON e.department_id = d.department_id
WHERE e.active = true
ORDER BY e.employee_id;

-- Product catalog with categories
SELECT 
    p.product_id as "Product ID",
    p.product_name as "Product Name",
    c.category_name as "Category",
    p.unit_price as "Price",
    p.units_in_stock as "Stock",
    CASE 
        WHEN p.discontinued = false THEN 'Active'
        ELSE 'Discontinued'
    END as "Status",
    p.created_date as "Created Date"
FROM products p
JOIN categories c ON p.category_id = c.category_id
ORDER BY c.category_name, p.product_name;

-- Monthly sales report
SELECT 
    TO_CHAR(o.order_date, 'YYYY-MM') as "Month",
    COUNT(o.order_id) as "Total Orders",
    SUM(od.quantity * od.unit_price) as "Total Revenue",
    AVG(od.quantity * od.unit_price) as "Avg Order Value",
    COUNT(DISTINCT o.customer_id) as "Unique Customers"
FROM orders o
JOIN order_details od ON o.order_id = od.order_id
WHERE o.order_date >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY TO_CHAR(o.order_date, 'YYYY-MM')
ORDER BY TO_CHAR(o.order_date, 'YYYY-MM');

-- Customer analysis with geographic data
SELECT 
    c.customer_id as "Customer ID",
    c.company_name as "Company",
    c.contact_name as "Contact",
    c.city as "City",
    c.country as "Country",
    COUNT(o.order_id) as "Total Orders",
    SUM(od.quantity * od.unit_price) as "Total Spent",
    MAX(o.order_date) as "Last Order Date"
FROM customers c
LEFT JOIN orders o ON c.customer_id = o.customer_id
LEFT JOIN order_details od ON o.order_id = od.order_id
GROUP BY c.customer_id, c.company_name, c.contact_name, c.city, c.country
HAVING COUNT(o.order_id) > 0
ORDER BY SUM(od.quantity * od.unit_price) DESC NULLS LAST;

-- Inventory levels with reorder alerts
SELECT 
    p.product_id as "Product ID",
    p.product_name as "Product Name",
    c.category_name as "Category",
    p.units_in_stock as "Current Stock",
    p.reorder_level as "Reorder Level",
    CASE 
        WHEN p.units_in_stock <= p.reorder_level THEN 'REORDER NOW'
        WHEN p.units_in_stock <= (p.reorder_level * 1.5) THEN 'LOW STOCK'
        ELSE 'OK'
    END as "Stock Status",
    s.company_name as "Supplier"
FROM products p
JOIN categories c ON p.category_id = c.category_id
JOIN suppliers s ON p.supplier_id = s.supplier_id
WHERE p.discontinued = false
ORDER BY 
    CASE 
        WHEN p.units_in_stock <= p.reorder_level THEN 1
        WHEN p.units_in_stock <= (p.reorder_level * 1.5) THEN 2
        ELSE 3
    END, p.product_name;

-- Employee performance metrics
SELECT 
    e.employee_id as "Employee ID",
    CONCAT(e.first_name, ' ', e.last_name) as "Full Name",
    d.department_name as "Department",
    COUNT(o.order_id) as "Orders Processed",
    SUM(od.quantity * od.unit_price) as "Revenue Generated",
    ROUND(AVG(od.quantity * od.unit_price), 2) as "Avg Order Value",
    TO_CHAR(MIN(o.order_date), 'YYYY-MM-DD') as "First Order",
    TO_CHAR(MAX(o.order_date), 'YYYY-MM-DD') as "Last Order"
FROM employees e
JOIN departments d ON e.department_id = d.department_id
LEFT JOIN orders o ON e.employee_id = o.employee_id
LEFT JOIN order_details od ON o.order_id = od.order_id
WHERE e.active = true
    AND o.order_date >= CURRENT_DATE - INTERVAL '12 months'
GROUP BY e.employee_id, e.first_name, e.last_name, d.department_name
ORDER BY SUM(od.quantity * od.unit_price) DESC NULLS LAST;

-- Top selling products with analytics
SELECT 
    p.product_name as "Product Name",
    c.category_name as "Category",
    SUM(od.quantity) as "Units Sold",
    SUM(od.quantity * od.unit_price) as "Total Revenue",
    AVG(od.unit_price) as "Avg Unit Price",
    COUNT(DISTINCT o.customer_id) as "Unique Customers",
    RANK() OVER (ORDER BY SUM(od.quantity * od.unit_price) DESC) as "Revenue Rank"
FROM products p
JOIN categories c ON p.category_id = c.category_id
JOIN order_details od ON p.product_id = od.product_id
JOIN orders o ON od.order_id = o.order_id
WHERE o.order_date >= CURRENT_DATE - INTERVAL '6 months'
GROUP BY p.product_id, p.product_name, c.category_name
ORDER BY "Total Revenue" DESC;
