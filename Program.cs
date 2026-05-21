
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NLog;
using NLog.Web;
using System.Text;
using FitLife.PersonalTrainer.API.Repositories;
using FitLife.PersonalTrainer.API.Services;
using Scalar.AspNetCore;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    var connectionString = builder.Configuration["Mongo__ConnectionString"]!;
    var databaseName = builder.Configuration["Mongo__DatabaseName"]!;

    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
    builder.Services.AddSingleton(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(databaseName);
    });

    var jwtSecret = builder.Configuration["Jwt__Secret"]!;
    var jwtIssuer = builder.Configuration["Jwt__Issuer"]!;
    var jwtAudience = builder.Configuration["Jwt__Audience"]!;

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

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("/tmp/dataprotection-keys"));

    builder.Services.AddAntiforgery(options =>
    {
        options.Cookie.Path = "/";
        options.Cookie.Name = ".AspNetCore.Antiforgery";
    });

    builder.Services.AddRazorPages(options =>
    {
        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
    });

    builder.Services.AddOpenApi();

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    });

    builder.Services.AddMemoryCache();
    builder.Services.AddControllers();

    builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
    builder.Services.AddScoped<ITrainingPlanRepository, TrainingPlanRepository>();
    builder.Services.AddScoped<INutritionPlanRepository, NutritionPlanRepository>();

    builder.Services.AddScoped<ITrainerService, TrainerService>();
    builder.Services.AddScoped<ITrainingPlanService, TrainingPlanService>();
    builder.Services.AddScoped<INutritionPlanService, NutritionPlanService>();

    var app = builder.Build();

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
}
catch (Exception ex)
{
    logger.Fatal(ex, "PersonalTrainer service failed to start");
    throw;
}
finally
{
    LogManager.Shutdown();
}
