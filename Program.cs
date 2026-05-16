using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
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

// ── 4. Data Protection ────────────────────────────────────────────────────
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/tmp/dataprotection-keys"));

// ── 5. Antiforgery ────────────────────────────────────────────────────────
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Path = "/";
    options.Cookie.Name = ".AspNetCore.Antiforgery";
});

// ── 6. Razor pages ────────────────────────────────────────────────────────
builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});

// ── 7. OpenAPI ────────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

// ── 8. API Versionering ───────────────────────────────────────────────────
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// ── 9. Cache ──────────────────────────────────────────────────────────────
builder.Services.AddMemoryCache();

// ── 10. Controllers ───────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── 11. Repositories ──────────────────────────────────────────────────────
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();
builder.Services.AddScoped<INutritionPlanRepository, NutritionPlanRepository>();

// ── 12. Services ──────────────────────────────────────────────────────────
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<ITrainingPlanService, TrainingPlanService>();
builder.Services.AddScoped<INutritionPlanService, NutritionPlanService>();

var app = builder.Build();

// ── 13. Middleware pipeline ───────────────────────────────────────────────
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.Use(async (context, next) =>
{
    var prefix = context.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();
    if (!string.IsNullOrEmpty(prefix))
    {
        context.Request.PathBase = new PathString(prefix);
    }
    await next();
});

app.UsePathBase("/personaltrainer");

app.MapOpenApi();
app.MapScalarApiReference();
app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();