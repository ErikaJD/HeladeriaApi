using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE CONTROLADORES + EVITAR BUCLES INFINITOS DE JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. CONFIGURACIÓN DINÁMICA DE LA BASE DE DATOS (RAILWAY / LOCAL)
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Buscamos la variable en el entorno o en el config de .NET (Railway la mete como ConnectionStrings o variable pura)
    var connectionString = builder.Configuration["DATABASE_URL"]
                           ?? Environment.GetEnvironmentVariable("DATABASE_URL");

    // Si no se encuentra de ninguna forma en Railway, recurrimos al appsettings de desarrollo
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Si la URL viene con "postgresql://", la parseamos para Npgsql
        if (connectionString.StartsWith("postgres"))
        {
            var databaseUri = new Uri(connectionString);
            var userInfo = databaseUri.UserInfo.Split(':');

            var host = databaseUri.Host;
            var port = databaseUri.Port;
            var database = databaseUri.AbsolutePath.TrimStart('/');
            var username = userInfo[0];
            var password = userInfo[1];

            connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true;";
        }

        options.UseNpgsql(connectionString);
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// 4. 🔥 MIGRACIÓN AUTOMÁTICA AL ARRANCAR
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HeladeriaContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("¡Base de datos conectada y migrada con éxito!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();