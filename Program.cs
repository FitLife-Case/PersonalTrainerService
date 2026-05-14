using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NLog.Web;
using System.Text;
using FitLife.PersonalTrainer.API.Repositories;
using FitLife.PersonalTrainer.API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── 1. NLog ───────────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// ── 2. MongoDB ────────────────────────────────────────────────────────────
var connectionString = builder.Configuration["Mongo:ConnectionString"]!;
var databaseName = builder.Configuration["Mongo:DatabaseName"]!;

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(databaseName);
});

// ── 3. JWT Authentication ─────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── Razor pages ──
builder.Services.AddRazorPages();

// ── 4. OpenAPI ────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

// ── 5. API Versionering ───────────────────────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ── 6. Cache ──────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();

// ── 7. Controllers ────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── 8. Repositories ───────────────────────────────────────────────────────
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();
builder.Services.AddScoped<INutritionPlanRepository, NutritionPlanRepository>();

// ── 9. Services ───────────────────────────────────────────────────────────
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<ITrainingPlanService, TrainingPlanService>();
builder.Services.AddScoped<INutritionPlanService, NutritionPlanService>();

var app = builder.Build();

// ── 10. Middleware pipeline ───────────────────────────────────────────────

// Scalar tilgængeligt i alle miljøer så det virker i Docker
app.MapOpenApi();
app.MapScalarApiReference();
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();