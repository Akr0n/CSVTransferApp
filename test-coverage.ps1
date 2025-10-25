# Test Script per verificare coverage e test
Write-Host "🔍 Testing code coverage configuration..." -ForegroundColor Cyan

# Verifica che la solution contenga i progetti di test
Write-Host "📋 Checking solution file..." -ForegroundColor Yellow
$solutionContent = Get-Content "CSVTransferApp.sln" -Raw
if ($solutionContent -match "CSVTransferApp\..*\.Tests") {
    Write-Host "✅ Test projects found in solution" -ForegroundColor Green
} else {
    Write-Host "❌ Test projects NOT found in solution" -ForegroundColor Red
}

# Lista progetti di test
Write-Host "📁 Listing test projects..." -ForegroundColor Yellow
$testProjects = Get-ChildItem -Path . -Recurse -Include "*Tests.csproj"
Write-Host "Found $($testProjects.Count) test projects:" -ForegroundColor Cyan
foreach ($project in $testProjects) {
    Write-Host "  - $($project.FullName)" -ForegroundColor White
}

# Test build della solution
Write-Host "🏗️ Building solution..." -ForegroundColor Yellow
try {
    dotnet build CSVTransferApp.sln --configuration Release --verbosity minimal
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Build failed: $($_)" -ForegroundColor Red
    exit 1
}

# Test esecuzione con coverage
Write-Host "🧪 Running tests with coverage..." -ForegroundColor Yellow
try {
    # Esegui test con coverage
    dotnet-coverage collect "dotnet test CSVTransferApp.sln --configuration Release --no-build --verbosity normal" -f xml -o "test-coverage.xml"
    
    # Verifica risultati
    if (Test-Path "test-coverage.xml") {
        $fileSize = (Get-Item "test-coverage.xml").Length
        Write-Host "✅ Coverage file created successfully (Size: $fileSize bytes)" -ForegroundColor Green
        
        # Mostra un preview del contenuto
        $content = Get-Content "test-coverage.xml" -TotalCount 10
        Write-Host "📄 Coverage file preview:" -ForegroundColor Cyan
        $content | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
    } else {
        Write-Host "❌ Coverage file not created" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Test execution failed: $($_)" -ForegroundColor Red
}

Write-Host "🏁 Test completed!" -ForegroundColor Cyan