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
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("NLog.config")
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // ── Vault ──────────────────────────────────────────────────────────────
    var vaultUrl = builder.Configuration["Vault__Url"]!;
    var vaultToken = builder.Configuration["Vault__Token"]!;

    var vaultClient = new VaultClient(new VaultClientSettings(vaultUrl, new TokenAuthMethodInfo(vaultToken)));
    var vaultSecrets = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
        path: "personaltrainer",
        mountPoint: "secret");

    var secrets = vaultSecrets.Data.Data;
    var connectionString = secrets["Mongo__ConnectionString"].ToString()!;
    var databaseName = secrets["Mongo__DatabaseName"].ToString()!;
    var jwtSecret = secrets["Jwt__Secret"].ToString()!;
    var jwtIssuer = secrets["Jwt__Issuer"].ToString()!;
    var jwtAudience = secrets["Jwt__Audience"].ToString()!;

    // ── MongoDB ────────────────────────────────────────────────────────────
    builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));
    builder.Services.AddSingleton(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(databaseName);
    });

    // ── JWT Authentication ─────────────────────────────────────────────────
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["JwtToken"];
                    return Task.CompletedTask;
                }
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

    // ── Middleware rækkefølge ──────────────────────────────────────────────
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapRazorPages();
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