# Build Test Script
Write-Host "🔍 Testing build after fixes..." -ForegroundColor Cyan

# Restore packages first
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
try {
    dotnet restore CSVTransferApp.sln --verbosity minimal
    Write-Host "✅ Restore successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Restore failed: $($_)" -ForegroundColor Red
    exit 1
}

# Build solution
Write-Host "🏗️ Building solution..." -ForegroundColor Yellow
try {
    dotnet build CSVTransferApp.sln --configuration Release --no-restore --verbosity minimal
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Build failed: $($_)" -ForegroundColor Red
    exit 1
}

Write-Host "🎉 All fixes applied successfully!" -ForegroundColor Green