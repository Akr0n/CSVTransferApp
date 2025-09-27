# Esempi di Utilizzo

Questa guida fornisce esempi pratici e casi d'uso reali per CSV Transfer Application, dalla configurazione base agli scenari aziendali complessi.

## Esempi di Base

### 1. Primo Trasferimento Semplice

**Scenario:** Esportare la tabella dipendenti da Oracle e inviarla via SFTP.

Test preliminare delle connessioni
dotnet run test

Trasferimento singola tabella
dotnet run transfer
--table employees
--db-connection Oracle
--sftp-connection MainServer

**Output atteso:**

[2025-09-27 15:30:15] INFO: Starting transfer for table employees
[2025-09-27 15:30:16] INFO: Connected to database Oracle
[2025-09-27 15:30:17] INFO: Query executed: SELECT * FROM employees
[2025-09-27 15:30:18] INFO: Processed 1,250 records
[2025-09-27 15:30:19] INFO: CSV file generated: employees.csv (156 KB)
[2025-09-27 15:30:20] INFO: Connected to SFTP MainServer
[2025-09-27 15:30:22] INFO: File uploaded successfully: employees.csv
[2025-09-27 15:30:22] INFO: Transfer completed successfully

### 2. Trasferimento con Query Personalizzata

Esporta solo dipendenti attivi dell'ultimo anno
dotnet run transfer
--table employees
--db-connection Oracle
--sftp-connection MainServer
--query "SELECT emp_id, first_name, last_name, email, hire_date, salary FROM employees WHERE active = 1 AND hire_date >= ADD_MONTHS(SYSDATE, -12)"

### 3. Test delle Connessioni

Test completo
dotnet run test

Test specifico database
dotnet run test --type database --connection Oracle

Test specifico SFTP
dotnet run test --type sftp --connection MainServer

Test con output dettagliato
dotnet run test --verbose

## Esempi di Elaborazione Batch

### 1. File Batch Semplice

**Crea:** `batch-jobs.json`

[
{
"TableName": "employees",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT * FROM employees WHERE active = 1"
},
{
"TableName": "departments",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT * FROM departments WHERE enabled = 1"
},
{
"TableName": "products",
"DatabaseConnection": "SqlServer",
"SftpConnection": "MainServer",
"Query": "SELECT * FROM products WHERE discontinued = 0"
}
]

**Esecuzione:**

dotnet run batch --file batch-jobs.json

### 2. Batch con Schedule Giornaliero

**Crea:** `daily-export.json`

[
{
"TableName": "daily_sales",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT order_id, customer_id, order_date, total_amount, sales_rep FROM orders WHERE order_date = TRUNC(SYSDATE - 1)"
},
{
"TableName": "inventory_status",
"DatabaseConnection": "SqlServer",
"SftpConnection": "MainServer",
"Query": "SELECT product_id, product_name, current_stock, reorder_level, last_updated FROM inventory_view WHERE last_updated >= CAST(GETDATE()-1 AS DATE)"
},
{
"TableName": "customer_activity",
"DatabaseConnection": "PostgreSQL",
"SftpConnection": "MainServer",
"Query": "SELECT customer_id, login_count, last_login, page_views FROM customer_analytics WHERE activity_date = CURRENT_DATE - INTERVAL '1 day'"
}
]

**Script PowerShell per automazione:**

daily-export.ps1
param(
[string]$Environment = "Production"
)

$logFile = "logs/daily-export-$(Get-Date -Format 'yyyyMMdd').log"

Write-Output "Starting daily export at $(Get-Date)" | Tee-Object -FilePath $logFile -Append

try {

dotnet run batch --file "daily-export.json" | Tee-Object -FilePath $logFile -Append

if ($LASTEXITCODE -eq 0) {
    Write-Output "Daily export completed successfully at $(Get-Date)" | Tee-Object -FilePath $logFile -Append
    
    # Invia notifica di successo (opzionale)
    # Send-MailMessage -To "ops@company.com" -Subject "Daily Export Success" -Body "Export completed successfully"
} else {
    Write-Error "Daily export failed with exit code $LASTEXITCODE" | Tee-Object -FilePath $logFile -Append
    
    # Invia notifica di errore
    # Send-MailMessage -To "ops@company.com" -Subject "Daily Export Failed" -Body "Export failed. Check logs."
}
} catch {
Write-Error "Error during daily export: $($_.Exception.Message)" | Tee-Object -FilePath $logFile -Append
exit 1
}

## Esempi di Override Intestazioni

### 1. Override Base per Tabella Dipendenti

**Crea:** `config/header-overrides/employees.json`

{
"TableName": "employees",
"Description": "Export dipendenti con intestazioni italiane",
"ColumnMappings": {
"emp_id": "ID Dipendente",
"first_name": "Nome",
"last_name": "Cognome",
"email": "Email",
"hire_date": "Data Assunzione",
"salary": "Stipendio Annuale",
"department_name": "Reparto",
"active": "Attivo"
},
"FormatRules": {
"Stipendio Annuale": {
"Format": "Currency",
"CurrencySymbol": "€",
"DecimalPlaces": 2
},
"Data Assunzione": {
"Format": "Date",
"DateFormat": "dd/MM/yyyy"
},
"Attivo": {
"Format": "Boolean",
"TrueValue": "Sì",
"FalseValue": "No"
}
}
}

**Test:**

dotnet run transfer --table employees --db-connection Oracle --sftp-connection MainServer

**CSV Risultante:**

ID Dipendente,Nome,Cognome,Email,Data Assunzione,Stipendio Annuale,Reparto,Attivo
1001,Mario,Rossi,mario.rossi@company.com,15/01/2023,"€ 45.000,00",IT,Sì
1002,Laura,Bianchi,laura.bianchi@company.com,22/03/2023,"€ 52.000,00",Marketing,Sì

### 2. Override Avanzato per Prodotti

**Crea:** `config/header-overrides/products.json`

{
"TableName": "products",
"Description": "Export catalogo prodotti per e-commerce",
"ColumnMappings": {
"product_id": "SKU",
"product_name": "Nome Prodotto",
"category_name": "Categoria",
"unit_price": "Prezzo Unitario",
"units_in_stock": "Disponibilità",
"reorder_level": "Scorta Minima",
"discontinued": "Fuori Produzione"
},
"FormatRules": {
"Prezzo Unitario": {
"Format": "Currency",
"CurrencySymbol": "€",
"DecimalPlaces": 2
},
"Disponibilità": {
"Format": "Integer",
"ShowZeroAs": "Esaurito"
},
"Fuori Produzione": {
"Format": "Boolean",
"TrueValue": "Sì",
"FalseValue": "No"
}
},
"ConditionalFormatting": {
"Disponibilità": {
"LowStockThreshold": 10,
"LowStockValue": "Scorte Limitate"
}
}
}

## Esempi di Automazione

### 1. Script PowerShell Completo

**Crea:** `scripts/export-monthly-reports.ps1`

<#
.SYNOPSIS
Esporta report mensili automatizzati
.DESCRIPTION
Script per l'esportazione automatica di report mensili con notificazioni email
.PARAMETER Month
Mese da esportare (default: mese precedente)
.PARAMETER Year
Anno da esportare (default: anno corrente)
.EXAMPLE
.\export-monthly-reports.ps1 -Month 8 -Year 2025
#>

param(
[int]$Month = (Get-Date).AddMonths(-1).Month,
[int]$Year = (Get-Date).Year,
[string]$Environment = "Production",
[switch]$SendEmail
)

Configurazione
$reportDate = Get-Date -Year $Year -Month $Month -Day 1
$reportDateStr = $reportDate.ToString("yyyy-MM")
$logDir = "logs/monthly-reports"
$logFile = "$logDir/monthly-report-$reportDateStr.log"

Crea directory log se non esiste
if (!(Test-Path $logDir)) {
New-Item -ItemType Directory -Path $logDir -Force
}

Funzione logging
function Write-Log {
param($Message, $Level = "INFO")
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$logEntry = "[$timestamp] [$Level] $Message"
Write-Output $logEntry | Tee-Object -FilePath $logFile -Append
}

Write-Log "Starting monthly report export for $reportDateStr"

try {
# 1. Sales Report
Write-Log "Generating sales report..."
$salesBatch = @"
[
{
"TableName": "monthly_sales_$reportDateStr",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT customer_id, product_category, SUM(total_amount) as total_sales, COUNT(*) as order_count FROM orders WHERE TO_CHAR(order_date, 'YYYY-MM') = '$reportDateStr' GROUP BY customer_id, product_category"
}
]
"@

$salesBatch | Out-File -FilePath "temp/sales-batch-$reportDateStr.json" -Encoding UTF8

$result = & dotnet run batch --file "temp/sales-batch-$reportDateStr.json"
if ($LASTEXITCODE -ne 0) {
    throw "Sales report failed with exit code $LASTEXITCODE"
}
Write-Log "Sales report completed successfully"

# 2. Inventory Report
Write-Log "Generating inventory report..."
$inventoryBatch = @"
[
{
"TableName": "monthly_inventory_$reportDateStr",
"DatabaseConnection": "SqlServer",
"SftpConnection": "MainServer",
"Query": "SELECT product_id, product_name, beginning_stock, ending_stock, units_sold, reorder_alerts FROM monthly_inventory_summary WHERE report_month = '$reportDateStr'"
}
]
"@

$inventoryBatch | Out-File -FilePath "temp/inventory-batch-$reportDateStr.json" -Encoding UTF8

$result = & dotnet run batch --file "temp/inventory-batch-$reportDateStr.json"
if ($LASTEXITCODE -ne 0) {
    throw "Inventory report failed with exit code $LASTEXITCODE"
}
Write-Log "Inventory report completed successfully"

# 3. Customer Analytics
Write-Log "Generating customer analytics..."
$customerBatch = @"

[
{
"TableName": "monthly_customers_$reportDateStr",
"DatabaseConnection": "PostgreSQL",
"SftpConnection": "MainServer",
"Query": "SELECT customer_segment, COUNT(*) as customer_count, AVG(order_value) as avg_order_value, SUM(total_spent) as total_revenue FROM customer_monthly_summary WHERE report_month = '$reportDateStr' GROUP BY customer_segment"
}
]
"@

$customerBatch | Out-File -FilePath "temp/customer-batch-$reportDateStr.json" -Encoding UTF8

$result = & dotnet run batch --file "temp/customer-batch-$reportDateStr.json"
if ($LASTEXITCODE -ne 0) {
    throw "Customer analytics failed with exit code $LASTEXITCODE"
}
Write-Log "Customer analytics completed successfully"

Write-Log "All monthly reports completed successfully"

# Cleanup temp files
Remove-Item "temp/*-batch-$reportDateStr.json" -Force

# Send success email
if ($SendEmail) {
    $emailBody = @"

Monthly reports for $reportDateStr have been generated successfully.

Reports generated:

Sales Report: monthly_sales_$reportDateStr.csv

Inventory Report: monthly_inventory_$reportDateStr.csv

Customer Analytics: monthly_customers_$reportDateStr.csv

Files have been uploaded to the SFTP server.

Log file: $logFile
"@

    Send-MailMessage -To "reports@company.com" -From "csvapp@company.com" -Subject "Monthly Reports $reportDateStr - Success" -Body $emailBody -SmtpServer "smtp.company.com"
    Write-Log "Success notification sent"
}
} catch {
Write-Log "ERROR: $($_.Exception.Message)" -Level "ERROR"

# Send error email
if ($SendEmail) {
    $errorBody = @"
Monthly report generation for $reportDateStr failed.

Error: $($_.Exception.Message)

Please check the log file: $logFile

"@
    Send-MailMessage -To "ops@company.com" -From "csvapp@company.com" -Subject "Monthly Reports $reportDateStr - FAILED" -Body $errorBody -SmtpServer "smtp.company.com"
}

exit 1
}

### 2. Script Bash per Linux

**Crea:** `scripts/export-daily-reports.sh`

#!/bin/bash

export-daily-reports.sh - Export giornaliero automatizzato
set -euo pipefail

Configurazione
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
LOG_DIR="$PROJECT_DIR/logs/daily-reports"
DATE_STR=$(date +%Y%m%d)
LOG_FILE="$LOG_DIR/daily-report-$DATE_STR.log"

Colori per output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

Funzione logging
log() {
local level=$1
shift
local message="$@"
local timestamp=$(date '+%Y-%m-%d %H:%M:%S')
echo -e "[$timestamp] [$level] $message" | tee -a "$LOG_FILE"
}

log_info() { log "INFO" "$@"; }
log_warn() { log "WARN" "$@"; }
log_error() { log "ERROR" "$@"; }

Crea directory log
mkdir -p "$LOG_DIR"

log_info "Starting daily export for $(date +%Y-%m-%d)"

Change to project directory
cd "$PROJECT_DIR"

Verifica che l'applicazione sia disponibile
if ! command -v dotnet &> /dev/null; then
log_error "dotnet command not found"
exit 1
fi

if [ ! -f "CSVTransferApp.sln" ]; then
log_error "CSVTransferApp.sln not found in $PROJECT_DIR"
exit 1
fi

Funzione per inviare notifiche (richiede mailx)
send_notification() {
local subject=$1
local body=$2

if command -v mail &> /dev/null; then
    echo "$body" | mail -s "$subject" ops@company.com
    log_info "Notification sent: $subject"
else
    log_warn "mail command not available, skipping notification"
fi
}

Trap per cleanup in caso di errore
cleanup() {
local exit_code=$?
if [ $exit_code -ne 0 ]; then
log_error "Script failed with exit code $exit_code"
send_notification "Daily Export Failed" "Daily export failed on $(hostname) at $(date). Check log: $LOG_FILE"
fi
exit $exit_code
}

trap cleanup EXIT

try_export() {
local description=$1
local batch_file=$2

log_info "Starting $description..."

if dotnet run batch --file "$batch_file" >> "$LOG_FILE" 2>&1; then
    log_info "$description completed successfully"
    return 0
else
    log_error "$description failed"
    return 1
fi
}

1. Export transazioni giornaliere
log_info "Creating daily transactions batch..."
cat > "temp/daily-transactions-$DATE_STR.json" << EOF
[
{
"TableName": "daily_transactions",
"DatabaseConnection": "Oracle",
"SftpConnection": "MainServer",
"Query": "SELECT transaction_id, customer_id, transaction_date, amount, payment_method, status FROM transactions WHERE TRUNC(transaction_date) = TRUNC(SYSDATE - 1)"
}
]
EOF

try_export "Daily transactions export" "temp/daily-transactions-$DATE_STR.json"

2. Export ordini in elaborazione
log_info "Creating pending orders batch..."
cat > "temp/pending-orders-$DATE_STR.json" << EOF
[
{
"TableName": "pending_orders",
"DatabaseConnection": "SqlServer",
"SftpConnection": "MainServer",
"Query": "SELECT order_id, customer_id, order_date, total_amount, status, priority FROM orders WHERE status IN ('PENDING', 'PROCESSING') AND order_date >= DATEADD(day, -7, GETDATE())"
}
]
EOF

try_export "Pending orders export" "temp/pending-orders-$DATE_STR.json"

3. Export metriche giornaliere
log_info "Creating daily metrics batch..."
cat > "temp/daily-metrics-$DATE_STR.json" << EOF
[
{
"TableName": "daily_metrics",
"DatabaseConnection": "PostgreSQL",
"SftpConnection": "MainServer",
"Query": "SELECT metric_date, total_orders, total_revenue, avg_order_value, new_customers, returning_customers FROM daily_metrics WHERE metric_date = CURRENT_DATE - INTERVAL '1 day'"
}
]
EOF

try_export "Daily metrics export" "temp/daily-metrics-$DATE_STR.json"

Cleanup file temporanei
log_info "Cleaning up temporary files..."
rm -f temp/daily-*-$DATE_STR.json

Verifica spazio disco
DISK_USAGE=$(df -h "$PROJECT_DIR" | awk 'NR==2{print $5}' | sed 's/%//')
if [ "$DISK_USAGE" -gt 80 ]; then
log_warn "Disk usage is high: ${DISK_USAGE}%"
send_notification "High Disk Usage Warning" "Disk usage on $(hostname) is ${DISK_USAGE}%. Please check $PROJECT_DIR"
fi

Rotazione log (mantieni solo gli ultimi 30 giorni)
find "$LOG_DIR" -name "daily-report-*.log" -mtime +30 -delete

log_info "Daily export completed successfully"

Invia notifica di successo
send_notification "Daily Export Success" "Daily export completed successfully on $(hostname) at $(date)"

exit 0

**Rendi eseguibile:**

chmod +x scripts/export-daily-reports.sh

### 3. Cron Job per Linux

Modifica crontab
crontab -e

Aggiungi entry per esecuzione giornaliera alle 02:00
0 2 * * * /opt/csvtransferapp/scripts/export-daily-reports.sh >/dev/null 2>&1

Oppure con logging completo
0 2 * * * /opt/csvtransferapp/scripts/export-daily-reports.sh >> /var/log/csvtransferapp/cron.log 2>&1

### 4. Scheduled Task per Windows

**PowerShell per creare Scheduled Task:**

Crea Scheduled Task per export giornaliero
$action = New-ScheduledTaskAction -Execute "PowerShell.exe" -Argument "-File C:\CSVTransferApp\scripts\export-daily-reports.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 2:00AM
$settings = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries
$principal = New-ScheduledTaskPrincipal -UserID "SYSTEM" -LogonType ServiceAccount

Register-ScheduledTask -TaskName "CSV Transfer Daily Export" -Action $action -Trigger $trigger -Settings $settings -Principal $principal -Description "Esporta report giornalieri CSV"

## Casi d'Uso Aziendali Complessi

### 1. Sincronizzazione Multi-Database

**Scenario:** Sincronizzare dati customer tra Oracle (CRM), SQL Server (ERP) e PostgreSQL (Analytics).

**File:** `sync-customer-data.json`

[
{
"TableName": "crm_customers",
"DatabaseConnection": "OracleCRM",
"SftpConnection": "DataLake",
"Query": "SELECT customer_id, company_name, contact_name, email, phone, created_date, last_modified FROM customers WHERE last_modified >= TRUNC(SYSDATE - 1)"
},
{
"TableName": "erp_customers",
"DatabaseConnection": "SqlServerERP",
"SftpConnection": "DataLake",
"Query": "SELECT customer_code, billing_address, credit_limit, payment_terms, account_status FROM customer_accounts WHERE modified_date >= CAST(GETDATE()-1 AS DATE)"
},
{
"TableName": "analytics_customers",
"DatabaseConnection": "PostgreSQLAnalytics",
"SftpConnection": "DataLake",
"Query": "SELECT customer_id, segment, lifetime_value, churn_risk, last_order_date FROM customer_analytics WHERE updated_at >= CURRENT_DATE - INTERVAL '1 day'"
}
]

### 2. Report Finanziario End-of-Month

**File:** `financial-eom-reports.json`

[
{
"TableName": "monthly_revenue_by_region",
"DatabaseConnection": "OracleFinance",
"SftpConnection": "FinanceReports",
"Query": "SELECT region, SUM(revenue) as total_revenue, COUNT(*) as transaction_count FROM financial_transactions WHERE EXTRACT(MONTH FROM transaction_date) = EXTRACT(MONTH FROM ADD_MONTHS(SYSDATE, -1)) AND EXTRACT(YEAR FROM transaction_date) = EXTRACT(YEAR FROM SYSDATE) GROUP BY region"
},
{
"TableName": "monthly_expenses_by_department",
"DatabaseConnection": "SqlServerAccounting",
"SftpConnection": "FinanceReports",
"Query": "SELECT department, expense_category, SUM(amount) as total_expenses FROM expenses WHERE MONTH(expense_date) = MONTH(DATEADD(month, -1, GETDATE())) AND YEAR(expense_date) = YEAR(GETDATE()) GROUP BY department, expense_category"
},
{
"TableName": "monthly_profit_loss",
"DatabaseConnection": "PostgreSQLReporting",
"SftpConnection": "FinanceReports",
"Query": "SELECT account_code, account_name, debit_total, credit_total, net_balance FROM monthly_trial_balance WHERE reporting_month = DATE_TRUNC('month', CURRENT_DATE - INTERVAL '1 month')"
}
]

## Monitoraggio e Alerting

### 1. Script di Monitoraggio

**Crea:** `scripts/monitor-transfers.ps1`

Monitor delle performance transfer
$logDir = "./logs/transfers"
$today = Get-Date -Format "yyyyMMdd"

Analizza log di oggi
$todayLogs = Get-ChildItem "$logDir" -Filter "$today"

$stats = @{
TotalTransfers = 0
SuccessfulTransfers = 0
FailedTransfers = 0
TotalRecords = 0
TotalSizeMB = 0
}

foreach ($log in $todayLogs) {
$content = Get-Content $log.FullName
$stats.TotalTransfers++

if ($content | Select-String "Transfer completed successfully") {
    $stats.SuccessfulTransfers++
} else {
    $stats.FailedTransfers++
}

# Estrai statistiche
$records = $content | Select-String "Processed (\d+) records" | ForEach-Object { $_.Matches.Groups[1].Value }
$size = $content | Select-String "File size: (\d+) MB" | ForEach-Object { $_.Matches.Groups[1].Value }

if ($records) { $stats.TotalRecords += [int]$records }
if ($size) { $stats.TotalSizeMB += [int]$size }
}

Report giornaliero
Write-Output "=== Daily Transfer Report $(Get-Date -Format 'yyyy-MM-dd') ==="
Write-Output "Total Transfers: $($stats.TotalTransfers)"
Write-Output "Successful: $($stats.SuccessfulTransfers)"
Write-Output "Failed: $($stats.FailedTransfers)"
Write-Output "Success Rate: $(if($stats.TotalTransfers -gt 0){[math]::Round($stats.SuccessfulTransfers/$stats.TotalTransfers*100,2)}else{0})%"
Write-Output "Total Records: $($stats.TotalRecords)"
Write-Output "Total Size: $($stats.TotalSizeMB) MB"

Alert se tasso di errore > 10%
if ($stats.TotalTransfers -gt 0 -and ($stats.FailedTransfers / $stats.TotalTransfers) -gt 0.1) {
Write-Warning "High failure rate detected: $(($stats.FailedTransfers / $stats.TotalTransfers * 100).ToString('F2'))%"
# Invia alert email...
}