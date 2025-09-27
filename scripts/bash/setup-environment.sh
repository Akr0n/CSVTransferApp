#!/bin/bash

# CSV Transfer Application - Environment Setup Script

# Default values
ENVIRONMENT="Development"
CONFIG_PATH="./config"
FORCE=false

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

# Function to display usage
usage() {
    echo -e "${GREEN}CSV Transfer Application - Environment Setup${NC}"
    echo ""
    echo "Usage: $0 [-e <environment>] [-p <config-path>] [--force]"
    echo ""
    echo "Options:"
    echo "  -e, --environment <env>   Environment name (default: Development)"
    echo "  -p, --path <path>        Configuration path (default: ./config)"
    echo "  --force                  Overwrite existing files"
    echo "  -h, --help              Show this help message"
    echo ""
    echo "Example:"
    echo "  $0 -e Production -p /opt/csvapp/config --force"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -e|--environment)
            ENVIRONMENT="$2"
            shift 2
            ;;
        -p|--path)
            CONFIG_PATH="$2"
            shift 2
            ;;
        --force)
            FORCE=true
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

echo -e "${GREEN}CSV Transfer Application - Environment Setup${NC}"
echo -e "${YELLOW}Environment: $ENVIRONMENT${NC}"

# Create directory structure
DIRECTORIES=(
    "./logs"
    "./logs/application"
    "./logs/transfers"
    "./logs/errors"
    "$CONFIG_PATH"
    "$CONFIG_PATH/header-overrides"
    "./temp"
)

echo -e "\n${CYAN}Creating directory structure...${NC}"
for dir in "${DIRECTORIES[@]}"; do
    if [[ ! -d "$dir" ]]; then
        mkdir -p "$dir"
        echo -e "  ${GREEN}Created: $dir${NC}"
    else
        echo -e "  ${GRAY}Exists: $dir${NC}"
    fi
done

# Create configuration files
echo -e "\n${CYAN}Setting up configuration files...${NC}"

# Basic configuration template
CONFIG_CONTENT=$(cat << EOF
{
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "DatabaseConnections": {
    "Oracle": {
      "Provider": "Oracle.EntityFrameworkCore",
      "ConnectionString": "Data Source=localhost:1521/XE;User Id=user;Password=password;"
    },
    "SqlServer": {
      "Provider": "Microsoft.EntityFrameworkCore.SqlServer",
      "ConnectionString": "Server=localhost;Database=database;Integrated Security=true;"
    },
    "PostgreSQL": {
      "Provider": "Npgsql.EntityFrameworkCore.PostgreSQL",
      "ConnectionString": "Host=localhost;Database=database;Username=user;Password=password"
    }
  },
  "SftpConnections": {
    "MainServer": {
      "Host": "sftp.example.com",
      "Port": 22,
      "Username": "user",
      "Password": "password",
      "RemotePath": "/upload"
    }
  },
  "Processing": {
    "MaxConcurrentConnections": 5,
    "MaxConcurrentFiles": 10,
    "HeaderOverridePath": "$CONFIG_PATH/header-overrides",
    "LogPath": "./logs"
  }
}
EOF
)

CONFIG_FILE="appsettings.$ENVIRONMENT.json"
if [[ ! -f "$CONFIG_FILE" || "$FORCE" == true ]]; then
    echo "$CONFIG_CONTENT" > "$CONFIG_FILE"
    echo -e "  ${GREEN}Created: $CONFIG_FILE${NC}"
else
    echo -e "  ${YELLOW}Skipped: $CONFIG_FILE (already exists)${NC}"
fi

# Sample header override
HEADER_OVERRIDE=$(cat << EOF
{
  "ColumnMappings": {
    "emp_id": "Employee ID",
    "first_name": "First Name",
    "last_name": "Last Name",
    "hire_date": "Hire Date"
  }
}
EOF
)

OVERRIDE_FILE="$CONFIG_PATH/header-overrides/employees.json"
if [[ ! -f "$OVERRIDE_FILE" || "$FORCE" == true ]]; then
    echo "$HEADER_OVERRIDE" > "$OVERRIDE_FILE"
    echo -e "  ${GREEN}Created sample header override: $OVERRIDE_FILE${NC}"
fi

# Set permissions
echo -e "\n${CYAN}Setting permissions...${NC}"
chmod -R 755 "./logs" "$CONFIG_PATH" "./temp" 2>/dev/null || true
echo -e "  ${GREEN}Permissions set successfully${NC}"

echo -e "\n${GREEN}Environment setup completed successfully!${NC}"
echo -e "${CYAN}Next steps:${NC}"
echo -e "  ${NC}1. Edit configuration files in $CONFIG_PATH/${NC}"
echo -e "  ${NC}2. Set up database and SFTP connections${NC}"
echo -e "  ${NC}3. Test connections: ./test-connections.sh${NC}"
echo -e "  ${NC}4. Run your first transfer!${NC}"
