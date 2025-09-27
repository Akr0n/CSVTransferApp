#!/bin/bash
TABLE=$1
DB_CONNECTION=${2:-"Oracle"}
SFTP_CONNECTION=${3:-"MainServer"}
QUERY=$4

ARGS="transfer --table $TABLE --db-connection $DB_CONNECTION --sftp-connection $SFTP_CONNECTION"

if [ ! -z "$QUERY" ]; then
    ARGS="$ARGS --query \"$QUERY\""
fi

dotnet CSVTransferApp.Console.dll $ARGS
