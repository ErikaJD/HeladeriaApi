using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE CONTROLADORES
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. CONEXIÓN INTELIGENTE Y BLINDADA
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    var railwayEnvUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string connectionString;

    if (!string.IsNullOrEmpty(railwayEnvUrl))
    {
        // Si la variable ya viene en formato tradicional "Host=...", la usamos directo
        if (railwayEnvUrl.Contains("Host=") || railwayEnvUrl.Contains("Server="))
        {
            connectionString = railwayEnvUrl;
        }
        else if (railwayEnvUrl.StartsWith("postgres://"))
        {
            // Si viene en formato URL "postgres://...", la parseamos de forma segura
            var uri = new Uri(railwayEnvUrl);
            var userInfo = uri.UserInfo.Split(':');
            var user = userInfo[0];
            var password = userInfo[1];
            var host = uri.Host;
            var port = uri.Port;
            var database = uri.AbsolutePath.TrimStart('/');

            connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};Include Error Detail=true;";
        }
        else
        {
            // Respaldo por si acaso
            connectionString = "Host=postgres.railway.internal;Port=5432;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";
        }
    }
    else
    {
        // Respaldo local
        connectionString = "Host=postgres.railway.internal;Port=5432;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";
    }

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// 3. MIGRACIÓN AUTOMÁTICA AL ARRANCAR
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HeladeriaContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("¡Conexión inteligente establecida con éxito!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();