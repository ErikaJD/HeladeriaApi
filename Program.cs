using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using EFCore.NamingConventions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACIÓN DE LA BASE DE DATOS MODIFICADA PARA POSTGRES EN LA NUBE
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    // 1. Primero intenta leer la variable de entorno de Railway; si no existe, usa la local de appsettings
    var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                           ?? builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention(); // 2. Convierte automáticamente Mayúsculas a minúsculas_con_guion
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection();

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
// Conexión corregida para producción en la heladería

app.Run();