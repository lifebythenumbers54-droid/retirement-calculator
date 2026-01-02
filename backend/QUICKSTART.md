# Quick Start Guide - Retirement Calculator API

## Prerequisites
- .NET 8.0 SDK or later installed
- Command line or terminal access

## Quick Start

### Option 1: Using the Test Script (Recommended)

**Windows:**
```bash
cd backend
test-api.bat
```

**Linux/Mac:**
```bash
cd backend
chmod +x test-api.sh
./test-api.sh
```

### Option 2: Manual Setup

1. Navigate to the API directory:
```bash
cd backend/RetirementCalculator.API
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## Verify the API is Running

Once the API starts, you should see output similar to:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
```

### Test Endpoints

Open your browser or use curl to test:

1. **Health Check:**
   - URL: http://localhost:5000/api/health
   - Expected: JSON with status "Healthy"

2. **Ping:**
   - URL: http://localhost:5000/api/health/ping
   - Expected: JSON with message "pong"

3. **Swagger UI:**
   - URL: http://localhost:5000/swagger
   - Interactive API documentation

### Example curl commands:

```bash
# Health check
curl http://localhost:5000/api/health

# Ping
curl http://localhost:5000/api/health/ping
```

## Stopping the API

Press `Ctrl+C` in the terminal where the API is running.

## Next Steps

The API is now ready to accept controller and service implementations. The folder structure is in place:
- `Controllers/` - Add new API controllers here
- `Models/` - Add data models and DTOs here
- `Services/` - Add business logic services here
- `Data/` - Add data access layer components here

## Troubleshooting

### Port Already in Use
If port 5000 or 5001 is already in use, you can change the port in:
`Properties/launchSettings.json`

### CORS Issues
The API is configured to accept requests from `http://localhost:5173` (Vite default).
To add more origins, edit the CORS policy in `Program.cs`.

### Build Errors
Make sure you have .NET 8.0 SDK installed:
```bash
dotnet --version
```
