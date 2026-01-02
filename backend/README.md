# Retirement Calculator API

## Overview
.NET Core Web API backend for the Retirement Calculator application.

## Project Structure
```
backend/
└── RetirementCalculator.API/
    ├── Controllers/      # API endpoint controllers
    ├── Models/          # Data models and DTOs
    ├── Services/        # Business logic services
    ├── Data/           # Data access layer
    └── Properties/     # Launch settings and configuration
```

## Getting Started

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code (optional)

### Running the API

1. Navigate to the API directory:
```bash
cd backend/RetirementCalculator.API
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

## Configuration

### CORS
The API is configured to accept requests from:
- http://localhost:5173 (Vite development server)

### Available Endpoints

#### Health Check
- `GET /api/health` - Returns API health status
- `GET /api/health/ping` - Simple ping endpoint

## Development

### Adding New Controllers
1. Create a new controller class in the `Controllers/` directory
2. Inherit from `ControllerBase`
3. Add the `[ApiController]` and `[Route("api/[controller]")]` attributes

### Adding Services
1. Create service interface in `Services/` directory
2. Implement the service
3. Register in `Program.cs` using dependency injection

## Build and Deploy

### Build
```bash
dotnet build
```

### Publish
```bash
dotnet publish -c Release -o out
```
