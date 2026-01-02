# Backend Architecture

## Technology Stack
- **.NET 8.0** - Latest LTS version of .NET
- **ASP.NET Core Web API** - RESTful API framework
- **Swagger/OpenAPI** - API documentation and testing

## Project Structure

```
backend/
└── RetirementCalculator.API/
    ├── Controllers/          # API endpoint controllers
    │   └── HealthController.cs
    ├── Models/              # Data models and DTOs
    ├── Services/            # Business logic services
    ├── Data/               # Data access layer
    ├── Properties/         # Launch settings
    │   └── launchSettings.json
    ├── Program.cs          # Application entry point and configuration
    ├── appsettings.json    # Application configuration
    └── RetirementCalculator.API.csproj
```

## Key Components

### Program.cs
- Application configuration and startup
- Dependency injection setup
- Middleware pipeline configuration
- CORS policy configuration

### Controllers/
API endpoint controllers that handle HTTP requests and responses.

**Current Controllers:**
- `HealthController` - Health check and ping endpoints

**Future Controllers:**
- `RetirementController` - Retirement calculation endpoints
- Add additional controllers as needed

### Models/
Data models, DTOs (Data Transfer Objects), and request/response models.

**Planned Models:**
- `RetirementInputModel` - User input for calculations
- `RetirementResultModel` - Calculation results
- `SavingsProjectionModel` - Year-by-year projection data

### Services/
Business logic layer that implements calculation and processing logic.

**Planned Services:**
- `IRetirementCalculationService` - Core retirement calculations
- `IInflationService` - Inflation adjustments
- `IProjectionService` - Multi-year projections

### Data/
Data access layer for any persistent storage needs.

**Potential Use:**
- Database context (if using Entity Framework)
- Repository pattern implementations
- Data seeding

## Configuration

### CORS Policy
Configured in `Program.cs` to allow requests from:
- `http://localhost:5173` (Vite development server)

To add additional origins, modify the CORS policy:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://other-origin.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
```

### Application URLs
Configured in `Properties/launchSettings.json`:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Environment Variables
- `ASPNETCORE_ENVIRONMENT` - Set to "Development" by default

## API Endpoints

### Current Endpoints

#### Health Check
- **GET** `/api/health`
  - Returns: API health status, message, and timestamp
  - Use: Verify API is running

- **GET** `/api/health/ping`
  - Returns: Simple "pong" message
  - Use: Quick connectivity test

### Planned Endpoints

#### Retirement Calculations
- **POST** `/api/retirement/calculate`
  - Request: Retirement input parameters
  - Response: Calculated retirement results

- **POST** `/api/retirement/projection`
  - Request: Retirement input parameters
  - Response: Year-by-year projection data

## Development Workflow

### Adding a New Endpoint

1. **Create a Model** (if needed)
   ```csharp
   // Models/RetirementInputModel.cs
   public class RetirementInputModel
   {
       public decimal CurrentAge { get; set; }
       public decimal RetirementAge { get; set; }
       // ... additional properties
   }
   ```

2. **Create a Service** (if business logic is needed)
   ```csharp
   // Services/IRetirementCalculationService.cs
   public interface IRetirementCalculationService
   {
       RetirementResultModel Calculate(RetirementInputModel input);
   }

   // Services/RetirementCalculationService.cs
   public class RetirementCalculationService : IRetirementCalculationService
   {
       public RetirementResultModel Calculate(RetirementInputModel input)
       {
           // Implementation
       }
   }
   ```

3. **Register the Service** in `Program.cs`
   ```csharp
   builder.Services.AddScoped<IRetirementCalculationService, RetirementCalculationService>();
   ```

4. **Create a Controller**
   ```csharp
   // Controllers/RetirementController.cs
   [ApiController]
   [Route("api/[controller]")]
   public class RetirementController : ControllerBase
   {
       private readonly IRetirementCalculationService _calculationService;

       public RetirementController(IRetirementCalculationService calculationService)
       {
           _calculationService = calculationService;
       }

       [HttpPost("calculate")]
       public IActionResult Calculate([FromBody] RetirementInputModel input)
       {
           var result = _calculationService.Calculate(input);
           return Ok(result);
       }
   }
   ```

## Testing

### Manual Testing
- Use Swagger UI at `http://localhost:5000/swagger`
- Use curl or Postman for API requests
- Test scripts provided in `test-api.bat` and `test-api.sh`

### Unit Testing (Future)
Consider adding:
- `RetirementCalculator.API.Tests` project
- xUnit or NUnit framework
- Moq for mocking dependencies

## Dependencies

### Current NuGet Packages
- `Microsoft.AspNetCore.OpenApi` (8.0.0) - OpenAPI support
- `Swashbuckle.AspNetCore` (6.5.0) - Swagger UI and documentation

### Potential Future Packages
- `Microsoft.EntityFrameworkCore` - If database access is needed
- `FluentValidation.AspNetCore` - Input validation
- `AutoMapper` - Object-to-object mapping
- `Serilog` - Advanced logging

## Security Considerations

### Current Setup
- HTTPS redirection enabled
- CORS policy restricts origins
- Development environment only

### Production Considerations
- Add authentication/authorization (JWT, OAuth)
- Implement rate limiting
- Add input validation
- Configure production CORS policy
- Enable HTTPS only
- Add security headers
- Implement logging and monitoring

## Next Steps

1. Implement retirement calculation models
2. Create calculation service with business logic
3. Add retirement controller endpoints
4. Test with frontend integration
5. Add input validation
6. Implement error handling
7. Add comprehensive logging
8. Create unit tests
