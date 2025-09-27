#!/bin/bash

# CSV Transfer Application - Batch Transfer Script

# Default values
BATCH_FILE=""
CONFIG_FILE="appsettings.json"
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
    echo -e "${GREEN}CSV Transfer Application - Batch Transfer${NC}"
    echo ""
    echo "Usage: $0 -f <batch-file> [-c <config-file>] [-v]"
    echo ""
    echo "Options:"
    echo "  -f, --file <file>     Batch file path (required)"
    echo "  -c, --config <file>   Configuration file (default: appsettings.json)"
    echo "  -v, --verbose         Enable verbose output"
    echo "  -h, --help           Show this help message"
    echo ""
    echo "Example:"
    echo "  $0 -f batch-jobs.json -c production.json -v"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -f|--file)
            BATCH_FILE="$2"
            shift 2
            ;;
        -c|--config)
            CONFIG_FILE="$2"
            shift 2
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

# Validate required parameters
if [[ -z "$BATCH_FILE" ]]; then
    echo -e "${RED}Error: Batch file is required${NC}"
    usage
    exit 1
fi

if [[ ! -f "$BATCH_FILE" ]]; then
    echo -e "${RED}Error: Batch file not found: $BATCH_FILE${NC}"
    exit 1
fi

if [[ ! -f "$CONFIG_FILE" ]]; then
    echo -e "${YELLOW}Warning: Config file not found: $CONFIG_FILE. Using default configuration.${NC}"
fi

echo -e "${GREEN}CSV Transfer Application - Batch Transfer${NC}"
echo -e "${YELLOW}Batch File: $BATCH_FILE${NC}"

START_TIME=$(date +%s)
echo -e "${CYAN}Starting batch transfer at $(date)${NC}"

# Build arguments
ARGS=("batch" "--file" "$BATCH_FILE")

if [[ "$VERBOSE" == true ]]; then
    ARGS+=("--verbose")
fi

# Execute the application
echo -e "${GRAY}Executing: dotnet CSVTransferApp.Console.dll ${ARGS[*]}${NC}"

dotnet CSVTransferApp.Console.dll "${ARGS[@]}"
EXIT_CODE=$?

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))

if [[ $EXIT_CODE -eq 0 ]]; then
    echo -e "${GREEN}Batch transfer completed successfully in ${DURATION}s${NC}"
else
    echo -e "${RED}Batch transfer failed with exit code: $EXIT_CODE${NC}"
fi

exit $EXIT_CODE
