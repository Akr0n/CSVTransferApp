param(
    [string]$Table,
    [string]$DbConnection = "Oracle",
    [string]$SftpConnection = "MainServer",
    [string]$Query
)

$args = @("transfer", "--table", $Table, "--db-connection", $DbConnection, "--sftp-connection", $SftpConnection)

if ($Query) {
    $args += @("--query", $Query)
}

& dotnet CSVTransferApp.Console.dll @args
