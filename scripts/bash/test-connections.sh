#!/bin/bash

# CSV Transfer Application - Connection Test Script

# Default values
CONFIG_FILE="appsettings.json"
DATABASE_ONLY=false
SFTP_ONLY=false
VERBOSE=false

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Function to display usage
usage() {
    echo -e "${GREEN}CSV Transfer Application - Connection Test${NC}"
    echo ""
    echo "Usage: $0 [-c <config-file>] [--database-only] [--sftp-only] [-v]"
    echo ""
    echo "Options:"
    echo "  -c, --config <file>   Configuration file (default: appsettings.json)"
    echo "  --database-only       Test only database connections"
    echo "  --sftp-only          Test only SFTP connections"
    echo "  -v, --verbose        Enable verbose output"
    echo "  -h, --help          Show this help message"
    echo ""
    echo "Example:"
    echo "  $0 -c production.json --database-only -v"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--config)
            CONFIG_FILE="$2"
            shift 2
            ;;
        --database-only)
            DATABASE_ONLY=true
            shift
            ;;
        --sftp-only)
            SFTP_ONLY=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            echo -e "${RED}Error: Unknown option $1${NC}"
            usage
            exit 1
            ;;
    esac
done

if [[ ! -f "$CONFIG_FILE" ]]; then
    echo -e "${RED}Error: Config file not found: $CONFIG_FILE${NC}"
    exit 1
fi

echo -e "${GREEN}CSV Transfer Application - Connection Test${NC}"

SUCCESS_COUNT=0
FAILED_COUNT=0
FAILED_CONNECTIONS=()

# Test database connections
if [[ "$SFTP_ONLY" != true ]]; then
    echo -e "\n${YELLOW}Testing Database Connections...${NC}"
    
    # Extract database connection names from config
    DB_CONNECTIONS=$(jq -r '.DatabaseConnections | keys[]' "$CONFIG_FILE" 2>/dev/null)
    
    if [[ -n "$DB_CONNECTIONS" ]]; then
        while IFS= read -r connection; do
            echo -n "  Testing $connection..."
            
            ARGS=("test" "--type" "database" "--connection" "$connection")
            if [[ "$VERBOSE" == true ]]; then
                ARGS+=("--verbose")
            fi
            
            if dotnet CSVTransferApp.Console.dll "${ARGS[@]}" >/dev/null 2>&1; then
                echo -e " ${GREEN}OK${NC}"
                ((SUCCESS_COUNT++))
            else
                echo -e " ${RED}FAILED${NC}"
                ((FAILED_COUNT++))
                FAILED_CONNECTIONS+=("Database: $connection")
            fi
        done <<< "$DB_CONNECTIONS"
    fi
fi

# Test SFTP connections
if [[ "$DATABASE_ONLY" != true ]]; then
    echo -e "\n${YELLOW}Testing SFTP Connections...${NC}"
    
    # Extract SFTP connection names from config
    SFTP_CONNECTIONS=$(jq -r '.SftpConnections | keys[]' "$CONFIG_FILE" 2>/dev/null)
    
    if [[ -n "$SFTP_CONNECTIONS" ]]; then
        while IFS= read -r connection; do
            echo -n "  Testing $connection..."
            
            ARGS=("test" "--type" "sftp" "--connection" "$connection")
            if [[ "$VERBOSE" == true ]]; then
                ARGS+=("--verbose")
            fi
            
            if dotnet CSVTransferApp.Console.dll "${ARGS[@]}" >/dev/null 2>&1; then
                echo -e " ${GREEN}OK${NC}"
                ((SUCCESS_COUNT++))
            else
                echo -e " ${RED}FAILED${NC}"
                ((FAILED_COUNT++))
                FAILED_CONNECTIONS+=("SFTP: $connection")
            fi
        done <<< "$SFTP_CONNECTIONS"
    fi
fi

# Summary
echo -e "\n${CYAN}Connection Test Summary:${NC}"
echo -e "  ${GREEN}Successful: $SUCCESS_COUNT${NC}"
echo -e "  ${RED}Failed: $FAILED_COUNT${NC}"

if [[ $FAILED_COUNT -gt 0 ]]; then
    echo -e "\n${RED}Failed Connections:${NC}"
    for failed in "${FAILED_CONNECTIONS[@]}"; do
        echo -e "  ${RED}- $failed${NC}"
    done
    exit 1
fi

echo -e "\n${GREEN}All connections tested successfully!${NC}"
exit 0
