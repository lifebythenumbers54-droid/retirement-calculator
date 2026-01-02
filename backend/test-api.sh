#!/bin/bash

# Test script for Retirement Calculator API

echo "=========================================="
echo "Retirement Calculator API Test Script"
echo "=========================================="
echo ""

# Navigate to the API directory
cd "$(dirname "$0")/RetirementCalculator.API"

echo "1. Restoring NuGet packages..."
dotnet restore

if [ $? -ne 0 ]; then
    echo "Error: Failed to restore packages"
    exit 1
fi

echo ""
echo "2. Building the project..."
dotnet build

if [ $? -ne 0 ]; then
    echo "Error: Build failed"
    exit 1
fi

echo ""
echo "3. Running the API..."
echo "   The API will start at http://localhost:5000"
echo "   Swagger UI: http://localhost:5000/swagger"
echo "   Health endpoint: http://localhost:5000/api/health"
echo ""
echo "   Press Ctrl+C to stop the server"
echo ""

dotnet run
