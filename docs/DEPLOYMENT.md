# Deployment guide — Run CSVTransferApp as a service

This repository includes several artifacts to run CSVTransferApp as a long-running service:

- Docker (recommended for portability)
  - `src/CSVTransferApp.Console/Dockerfile`
  - `docker-compose.yml`

- Linux systemd unit (manual install)
  - `deploy/systemd/csvtransferapp.service`

- Windows helper scripts (install/uninstall/run-local)
  - `deploy/windows/install-service.ps1`
  - `deploy/windows/uninstall-service.ps1`
  - `deploy/windows/run-local.ps1`

Quick decisions:
- If you want portability and easy config, use Docker + volumes.
- If you want a native Linux service, use systemd and install the published binary under `/opt/csvtransferapp`.
- If you want a native Windows service, publish the binary and use the provided PowerShell script to register a Windows service.

Docker (build & run):

```powershell
# Build the image (from repo root)
docker compose build

# Run as a daemon with config and logs mounted
docker compose up -d

# View logs
docker compose logs -f csvtransfer
```

Install as systemd service (Linux):

```bash
# Example: publish and install under /opt/csvtransferapp (run as root)
DOTNET_ROOT=/usr/share/dotnet
dotnet publish src/CSVTransferApp.Console/CSVTransferApp.Console.csproj -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true -o /opt/csvtransferapp

# Copy unit file
cp deploy/systemd/csvtransferapp.service /etc/systemd/system/

# Reload and enable
systemctl daemon-reload
systemctl enable csvtransferapp.service
systemctl start csvtransferapp.service

# Check status
systemctl status csvtransferapp.service
journalctl -u csvtransferapp.service -f
```

Install as Windows service:

```powershell
# Publish first (from repo root)
dotnet publish src/CSVTransferApp.Console/CSVTransferApp.Console.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true -o .\deploy\windows\publish

# Then run the helper to register the service (run as admin)
Set-ExecutionPolicy RemoteSigned -Scope Process
.\deploy\windows\install-service.ps1 -InstallPath .\deploy\windows\publish -ServiceName CSVTransferApp

# Start service
sc start CSVTransferApp

# To remove
.\deploy\windows\uninstall-service.ps1 -ServiceName CSVTransferApp
```

Notes:
- The Dockerfile publishes a Linux self-contained binary. The image uses `runtime-deps` and runs the produced single-file executable `CSVTransferApp`.
- The `docker-compose.yml` mounts `./config` and `./logs` from the repo root into the container. Ensure `config` contains required `appsettings.*.json` and connection JSON files.
- If you change `AssemblyName` in the `.csproj` file, adjust the service scripts to match the produced executable name.

If you want, I can:
- Add a small healthcheck endpoint or file-based heartbeat the service writes to `logs/` for monitoring.
- Add a systemd timer for automatic cleanup/rotation of logs.
- Create a release artifact script to package the published install directory.

Tell me which option you'd like next.

---

Self-contained vs Framework-dependent
------------------------------------

You can publish the app in two ways:

- Self-contained single-file (recommended for callers that *don't* have .NET installed):
  - Produces a native executable per platform (Windows `.exe`, Linux binary).
  - Caller simply starts the executable and reads stdout/exit code.
  - Produced by the workflow under `deploy/.../publish/` (e.g. `deploy/windows/publish` and `deploy/linux/publish`).

- Framework-dependent (DLL):
  - Produces a smaller set of files including `CSVTransferApp.dll`.
  - Caller must run `dotnet CSVTransferApp.dll ...` on a machine with .NET installed.
  - Produced by the workflow under `deploy/.../publish-fx/` (e.g. `deploy/windows/publish-fx`).

How callers should invoke the app
--------------------------------

From C# (Process.Start) — self-contained:

```csharp
var psi = new ProcessStartInfo {
    FileName = "C:\\path\\to\\CSVTransferApp.exe",
    Arguments = "transfer --table employees --db-connection Default",
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false
};
using var p = Process.Start(psi);
string outText = await p.StandardOutput.ReadToEndAsync();
p.WaitForExit();
int code = p.ExitCode;
```

From C# (framework-dependent):

```csharp
var psi = new ProcessStartInfo {
    FileName = "dotnet",
    Arguments = "C:\\path\\to\\publish\\CSVTransferApp.dll transfer --table employees",
    RedirectStandardOutput = true,
    UseShellExecute = false
};
```

PowerShell caller (self-contained):

```powershell
& "C:\path\to\CSVTransferApp.exe" transfer --table employees
if ($LASTEXITCODE -ne 0) { Write-Error "Transfer failed" }
```

Node.js caller (child_process):

```js
const { spawn } = require('child_process');
const p = spawn('C:/path/to/CSVTransferApp.exe', ['transfer','--table','employees']);
p.stdout.on('data', d => console.log(d.toString()));
p.on('close', code => console.log('exit', code));
```

Automation & CI
----------------
The workflow now produces both variants and zips them as artifacts so you have:

- `deploy/windows/publish/` (self-contained Windows exe)
- `deploy/windows/publish-fx/` (framework-dependent DLLs)
- `deploy/linux/publish/` (self-contained Linux binary)
- `deploy/linux/publish-fx/` (framework-dependent DLLs)

Pick the artifact that matches the environment of the caller.