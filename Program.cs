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

// 2. CADENA DE PRODUCCIÓN FIJA (Eliminamos por completo el localhost)
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // Usamos directamente tu dominio proxy externo verificado con el puerto público
    var connectionString = "Host=autorack.proxy.rlwy.net;Port=15822;Database=railway;Username=postgres;Password=kHtburGXECttprHpPdvkImCHliTrtFYG;Include Error Detail=true;";

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
        Console.WriteLine("¡Conexión forzada exitosa!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB migration error: " + ex.Message);
    }
}
//actualizacion
app.Run();