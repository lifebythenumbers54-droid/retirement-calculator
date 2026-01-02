@echo off
REM Test script for Retirement Calculator API (Windows)

echo ==========================================
echo Retirement Calculator API Test Script
echo ==========================================
echo.

REM Navigate to the API directory
cd /d "%~dp0RetirementCalculator.API"

echo 1. Restoring NuGet packages...
dotnet restore

if %errorlevel% neq 0 (
    echo Error: Failed to restore packages
    exit /b 1
)

echo.
echo 2. Building the project...
dotnet build

if %errorlevel% neq 0 (
    echo Error: Build failed
    exit /b 1
)

echo.
echo 3. Running the API...
echo    The API will start at http://localhost:5000
echo    Swagger UI: http://localhost:5000/swagger
echo    Health endpoint: http://localhost:5000/api/health
echo.
echo    Press Ctrl+C to stop the server
echo.

dotnet run
