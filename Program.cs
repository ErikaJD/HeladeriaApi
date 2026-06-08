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

// 2. CONEXIÓN DINÁMICA (CON RECOVERY AUTOMÁTICO DE RAILWAY)
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Railway inyecta automáticamente la variable "DATABASE_URL" en formato de URL de Postgres.
    // Si existe, la usamos directamente porque Railway la mantiene viva y autorizada internamente.
    var railwayEnvUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string connectionString;

    if (!string.IsNullOrEmpty(railwayEnvUrl))
    {
        // Convertimos el formato postgres://user:pass@host:port/db al formato que entiende Npgsql
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
        // Respaldo interno por si acaso
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
        Console.WriteLine("¡Conexión dinámica configurada con éxito!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();