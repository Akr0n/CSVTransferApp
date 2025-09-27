# CSV Transfer Application - Documentazione

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![License](https://img.shields.io/badge/license-MIT-green)
![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux-lightgrey)

## Panoramica

CSV Transfer Application è un'applicazione .NET Core multi-piattaforma progettata per esportare dati da database Oracle, SQL Server e PostgreSQL in formato CSV e trasferirli automaticamente via SFTP.

### Caratteristiche Principali

- 🎯 **Multi-Database**: Supporto nativo per Oracle, SQL Server e PostgreSQL
- 🚀 **Multi-Threading**: Elaborazione parallela di multiple connessioni e file
- 🔐 **Sicurezza**: Crittografia delle credenziali e connessioni SFTP sicure
- 📝 **Logging Avanzato**: Sistema di logging strutturato con file separati per componente
- 🎛️ **Personalizzazione**: Override delle intestazioni CSV tramite file JSON
- 🖥️ **Cross-Platform**: Funziona su Windows e Linux
- 📋 **CLI Friendly**: Interfaccia a riga di comando per automazione
- 🔄 **Automazione**: Supporto per script PowerShell e Bash

### Architettura

L'applicazione segue i principi della **Clean Architecture** con separazione chiara delle responsabilità:

┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│ Console UI │────│ Services │────│ Data Access │
└─────────────────┘ └─────────────────┘ └─────────────────┘
│ │ │
└───────────────────────┼───────────────────────┘
│
┌─────────────────┐
│ Core Models │
└─────────────────┘


## Indice Documentazione

- 📦 [**Installazione**](INSTALLATION.md) - Guida completa all'installazione
- ⚙️ [**Configurazione**](CONFIGURATION.md) - Setup database, SFTP e parametri
- 🚀 [**Esempi di Utilizzo**](EXAMPLES.md) - Esempi pratici e casi d'uso
- 🏗️ [**Architettura**](ARCHITECTURE.md) - Dettagli tecnici dell'architettura
- 🔧 [**Risoluzione Problemi**](TROUBLESHOOTING.md) - Soluzioni ai problemi comuni
- 📖 [**API Reference**](API.md) - Documentazione delle interfacce

## Quick Start

### 1. Installazione

Clone del repository
git clone https://github.com/yourusername/CSVTransferApp.git
cd CSVTransferApp

Build dell'applicazione
dotnet build

Setup dell'ambiente
./scripts/powershell/setup-environment.ps1 # Windows
./scripts/bash/setup-environment.sh # Linux


### 2. Configurazione Base

Modifica `appsettings.json`:

{
"DatabaseConnections": {
"Oracle": {
"ConnectionString": "Data Source=localhost:1521/XE;User Id=user;Password=pass;"
}
},
"SftpConnections": {
"MainServer": {
"Host": "sftp.example.com",
"Username": "user",
"Password": "pass",
"RemotePath": "/upload"
}
}
}


### 3. Primo Trasferimento

Test delle connessioni
dotnet run test

Trasferimento singola tabella
dotnet run transfer --table employees --db-connection Oracle --sftp-connection MainServer

Trasferimento batch
dotnet run batch --file batch-jobs.json


## Requisiti di Sistema

### Software

- **.NET 9.0 SDK** o superiore
- **Sistema Operativo**: Windows 10+ o Linux (Ubuntu 18.04+, CentOS 7+)
- **Database Client**: Oracle Client, SQL Server Client, o PostgreSQL Client

### Hardware Consigliato

- **CPU**: 2+ core
- **RAM**: 4 GB minimo, 8 GB consigliato
- **Spazio Disco**: 1 GB per l'applicazione + spazio per log e file temporanei
- **Rete**: Connessione stabile per database e SFTP

## Struttura del Progetto

CSVTransferApp/
├── src/ # Codice sorgente
│ ├── CSVTransferApp.Core/ # Modelli e interfacce
│ ├── CSVTransferApp.Data/ # Accesso ai dati
│ ├── CSVTransferApp.Services/ # Logica di business
│ ├── CSVTransferApp.Infrastructure/ # Servizi infrastrutturali
│ └── CSVTransferApp.Console/ # Applicazione console
├── tests/ # Test unitari e integrazione
├── scripts/ # Script di automazione
├── config/ # File di configurazione
├── docs/ # Documentazione
└── logs/ # File di log


## Supporto e Contributi

- 🐛 **Bug Report**: [GitHub Issues](https://github.com/yourusername/CSVTransferApp/issues)
- 💡 **Feature Request**: [GitHub Discussions](https://github.com/yourusername/CSVTransferApp/discussions)
- 📧 **Email**: support@yourcompany.com

## Licenza

Questo progetto è rilasciato sotto licenza MIT. Vedi il file [LICENSE](../LICENSE) per i dettagli.

---

**📚 Inizia con la [Guida di Installazione](INSTALLATION.md) per configurare l'applicazione.**