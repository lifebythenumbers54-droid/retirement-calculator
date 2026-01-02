using RetirementCalculator.API.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Retirement Calculator API",
        Version = "v1",
        Description = "API for calculating safe retirement withdrawal rates based on historical market data with tax-optimized strategies",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Retirement Calculator",
        }
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add response compression for better performance
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/json" });
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register Historical Data Service as singleton (load once at startup)
builder.Services.AddSingleton<IHistoricalDataService, HistoricalDataService>();

// Register Tax Calculation Service as scoped
builder.Services.AddScoped<ITaxCalculationService, TaxCalculationService>();

// Register Early Retirement Service as scoped
builder.Services.AddScoped<IEarlyRetirementService, EarlyRetirementService>();

// Register Roth Conversion Service as scoped
builder.Services.AddScoped<IRothConversionService, RothConversionService>();

// Register Withdrawal Calculation Service as scoped
builder.Services.AddScoped<IWithdrawalCalculationService, WithdrawalCalculationService>();

// Register Allocation Analysis Service as scoped
builder.Services.AddScoped<IAllocationAnalysisService, AllocationAnalysisService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Add global exception handling middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

        if (exceptionHandlerFeature != null)
        {
            logger.LogError(exceptionHandlerFeature.Error, "Unhandled exception occurred");

            await context.Response.WriteAsJsonAsync(new
            {
                message = "An unexpected error occurred. Please try again later.",
                error = app.Environment.IsDevelopment() ? exceptionHandlerFeature.Error.Message : null
            });
        }
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable response compression
app.UseResponseCompression();

// Enable CORS
app.UseCors("AllowViteFrontend");

app.UseAuthorization();

app.MapControllers();

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Retirement Calculator API started successfully");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);

app.Run();
