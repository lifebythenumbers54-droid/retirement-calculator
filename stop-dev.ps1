# Retirement Calculator - Stop Development Servers Script
# This script stops all running backend and frontend servers

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Retirement Calculator - Stopping Dev Servers" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kill any RetirementCalculator.API processes
Write-Host "Stopping backend servers..." -ForegroundColor Yellow
$backendProcesses = Get-Process | Where-Object { $_.ProcessName -eq "RetirementCalculator.API" }
if ($backendProcesses) {
    $backendProcesses | ForEach-Object {
        Write-Host "  Killing RetirementCalculator.API (PID: $($_.Id))" -ForegroundColor Yellow
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "Backend servers stopped." -ForegroundColor Green
} else {
    Write-Host "No backend servers found running." -ForegroundColor Gray
}
Write-Host ""

# Kill any dotnet processes running from the backend directory
Write-Host "Cleaning up dotnet processes..." -ForegroundColor Yellow
$dotnetProcesses = Get-Process dotnet -ErrorAction SilentlyContinue | Where-Object {
    $_.Path -like "*RetirementCalculator*"
}
if ($dotnetProcesses) {
    $dotnetProcesses | ForEach-Object {
        Write-Host "  Killing dotnet (PID: $($_.Id))" -ForegroundColor Yellow
        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
    }
    Write-Host "Dotnet processes cleaned up." -ForegroundColor Green
} else {
    Write-Host "No dotnet processes found." -ForegroundColor Gray
}
Write-Host ""

# Find and kill node processes running on port 3000
Write-Host "Stopping frontend servers..." -ForegroundColor Yellow
$port3000 = Get-NetTCPConnection -LocalPort 3000 -ErrorAction SilentlyContinue
if ($port3000) {
    $port3000 | ForEach-Object {
        $processId = $_.OwningProcess
        Write-Host "  Killing process on port 3000 (PID: $processId)" -ForegroundColor Yellow
        Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
    }
    Write-Host "Frontend servers stopped." -ForegroundColor Green
} else {
    Write-Host "No frontend servers found running on port 3000." -ForegroundColor Gray
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "All development servers stopped!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You can now run start-dev.ps1 to restart the servers." -ForegroundColor Yellow
