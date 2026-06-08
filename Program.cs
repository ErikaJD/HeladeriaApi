using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE CONTROLADORES + EVITAR BUCLES INFINITOS DE JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Evita que la API truene con Error 500 si hay relaciones circulares (ej. Categoria -> Producto -> Categoria)
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. CONFIGURACIÓN DINÁMICA DE LA BASE DE DATOS (RAILWAY / LOCAL)
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Intentamos leer la variable oficial enlazada desde el panel de Railway
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(connectionString))
    {
        // Si la variable está vacía (entorno local de desarrollo), usa el appsettings.json tradicional
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Si está en Railway, parseamos la URL automáticamente (convierte postgresql:// a un formato válido para EF Core)
        var databaseUri = new Uri(connectionString);
        var userInfo = databaseUri.UserInfo.Split(':');

        var host = databaseUri.Host;
        var port = databaseUri.Port;
        var database = databaseUri.AbsolutePath.TrimStart('/');
        var username = userInfo[0];
        var password = userInfo[1];

        // Construimos la cadena de conexión limpia compatible con Npgsql
        var formattedConnectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true;";

        options.UseNpgsql(formattedConnectionString);
    }
});

var app = builder.Build();

// 3. CONFIGURACIÓN DEL PIPELINE HTTP (SWAGGER EN PRODUCCIÓN)
// Dejamos Swagger fuera de "if (app.Environment.IsDevelopment())" para que abra en Railway perfectamente
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// 4. 🔥 MIGRACIÓN AUTOMÁTICA Y SEGURA AL ARRANCAR EL CONTENEDOR
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HeladeriaContext>();
    try
    {
        db.Database.Migrate();
        Console.WriteLine("¡Migración e inicio de Base de Datos aplicados con éxito!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();