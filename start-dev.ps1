# Retirement Calculator - Development Startup Script
# This script builds and starts both the backend (.NET) and frontend (React) servers

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Retirement Calculator - Starting Dev Environment" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Stop any existing servers first
Write-Host "Stopping any existing servers..." -ForegroundColor Yellow
& "$PSScriptRoot\stop-dev.ps1"
Start-Sleep -Seconds 2
Write-Host ""

# Clean the backend bin/obj folders to avoid file lock issues
Write-Host "Cleaning backend build artifacts..." -ForegroundColor Yellow
if (Test-Path "backend\RetirementCalculator.API\bin") {
    Remove-Item "backend\RetirementCalculator.API\bin" -Recurse -Force -ErrorAction SilentlyContinue
}
if (Test-Path "backend\RetirementCalculator.API\obj") {
    Remove-Item "backend\RetirementCalculator.API\obj" -Recurse -Force -ErrorAction SilentlyContinue
}
Write-Host "Build artifacts cleaned." -ForegroundColor Green
Write-Host ""

# Build the backend
Write-Host "Building .NET Backend..." -ForegroundColor Green
Push-Location backend\RetirementCalculator.API
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Backend build failed!" -ForegroundColor Red
    Pop-Location
    exit 1
}
Pop-Location
Write-Host "Backend build successful!" -ForegroundColor Green
Write-Host ""

# Install frontend dependencies if needed
if (-not (Test-Path "frontend\node_modules")) {
    Write-Host "Installing frontend dependencies..." -ForegroundColor Yellow
    Push-Location frontend
    npm install
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Frontend dependency installation failed!" -ForegroundColor Red
        Pop-Location
        exit 1
    }
    Pop-Location
    Write-Host "Frontend dependencies installed!" -ForegroundColor Green
    Write-Host ""
}

# Start the backend server in a new window
Write-Host "Starting .NET Backend on http://localhost:5000..." -ForegroundColor Green
$backendJob = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\backend\RetirementCalculator.API'; Write-Host 'Backend Server Running on http://localhost:5000' -ForegroundColor Cyan; Write-Host 'Press Ctrl+C to stop the server' -ForegroundColor Yellow; Write-Host ''; dotnet run" -PassThru
Write-Host "Backend server starting in new window (PID: $($backendJob.Id))..." -ForegroundColor Green
Write-Host ""

# Wait a bit for backend to start
Write-Host "Waiting for backend to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Write-Host ""

# Start the frontend server in a new window
Write-Host "Starting React Frontend on http://localhost:3000..." -ForegroundColor Green
$frontendJob = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\frontend'; Write-Host 'Frontend Server Running on http://localhost:3000' -ForegroundColor Cyan; Write-Host 'Press Ctrl+C to stop the server' -ForegroundColor Yellow; Write-Host ''; npm run dev" -PassThru
Write-Host "Frontend server starting in new window (PID: $($frontendJob.Id))..." -ForegroundColor Green
Write-Host ""

# Wait for frontend to start
Write-Host "Waiting for frontend to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 5
Write-Host ""

# Display final information
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Development Environment Started!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backend API:  " -NoNewline -ForegroundColor White
Write-Host "http://localhost:5000" -ForegroundColor Cyan
Write-Host "Swagger UI:   " -NoNewline -ForegroundColor White
Write-Host "http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Frontend App: " -NoNewline -ForegroundColor White
Write-Host "http://localhost:3000" -ForegroundColor Cyan
Write-Host ""
Write-Host "Both servers are running in separate windows." -ForegroundColor Yellow
Write-Host "Close those windows (or press Ctrl+C in them) to stop the servers." -ForegroundColor Yellow
Write-Host "Or run stop-dev.ps1 to stop all servers." -ForegroundColor Yellow
Write-Host ""
Write-Host "Opening browser..." -ForegroundColor Green
Start-Sleep -Seconds 2

# Open the frontend in default browser
Start-Process "http://localhost:3000"

Write-Host ""
Write-Host "Press any key to exit this script (servers will continue running)..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
