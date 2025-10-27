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

-- Sample data
INSERT INTO public.employee (first_name, last_name, email, hire_date, salary, department) VALUES
('John', 'Doe', 'john.doe@company.com', '2023-01-15', 75000.00, 'IT'),
('Jane', 'Smith', 'jane.smith@company.com', '2023-02-01', 82000.00, 'HR'),
('Bob', 'Johnson', 'bob.johnson@company.com', '2023-03-10', 65000.00, 'Marketing'),
('Alice', 'Williams', 'alice.williams@company.com', '2023-04-20', 71000.00, 'IT'),
('Charlie', 'Brown', 'charlie.brown@company.com', '2023-05-05', 68000.00, 'Finance');
