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
// CONFIGURACIÓN DE LA BASE DE DATOS EXTERNA PARA EVITAR EL CAMBIO DE CLAVE INTERNO
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Usamos el dominio público y el puerto 5432 (o el que te dé la url externa) con tu clave
    var connectionString = "Host=autorack.proxy.rlwy.net;Port=15822;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";

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