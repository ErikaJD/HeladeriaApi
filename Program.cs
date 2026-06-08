using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// CORRECCIÓN AQUÍ: Agrega soporte para evitar bucles de referencia usando el serializador nativo
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DE LA BASE DE DATOS FORZADA DESDE CONFIGURACIÓN
// CONFIGURACIÓN DE LA BASE DE DATOS HARDCODED PARA EVITAR LOCALHOST
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Forzamos la cadena real interna de Railway directamente aquí
    var connectionString = "Host=postgres.railway.internal;Port=5432;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

// 🔥 MIGRACIÓN SEGURA
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HeladeriaContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}

app.Run();