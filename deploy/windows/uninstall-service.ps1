param(
    [string]$ServiceName = "CSVTransferApp"
)

Write-Output "Removing Windows service $ServiceName"
sc.exe stop $ServiceName | Out-Null
sc.exe delete $ServiceName | Out-Null
Write-Output "Service $ServiceName removed."
