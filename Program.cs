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

// 2. CONEXIÓN PRIVADA INTERNA RECOMENDADA POR RAILWAY (PUERTO 5432)
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Usamos el host interno directo de la red privada de tu proyecto
    var connectionString = "Host=postgres.railway.internal;Port=5432;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";

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
        Console.WriteLine("¡Base de datos interna conectada con éxito!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();