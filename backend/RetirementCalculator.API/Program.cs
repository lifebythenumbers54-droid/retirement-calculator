using RetirementCalculator.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Historical Data Service as singleton (load once at startup)
builder.Services.AddSingleton<IHistoricalDataService, HistoricalDataService>();

// Register Withdrawal Calculation Service as scoped
builder.Services.AddScoped<IWithdrawalCalculationService, WithdrawalCalculationService>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowViteFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
