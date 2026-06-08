using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de Controladores con IgnoreCycles para evitar el bucle infinito
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esta línea rompe el bucle infinito ignorando las referencias circulares
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // Mantiene los nombres de las propiedades en camelCase en el JSON (bueno para el frontend)
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Configuración de la Base de Datos PostgreSQL
// 2. Configuración de la Base de Datos PostgreSQL
builder.Services.AddDbContext<HeladeriaContext>(options =>
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString);
}));

var app = builder.Build();

// 3. Configuración del Pipeline de la aplicación
app.UseSwagger();
app.UseSwaggerUI();

// Muestra el error real en Swagger si algo falla en Render
app.UseDeveloperExceptionPage();

// app.UseHttpsRedirection(); // Comentado para evitar problemas en Render
app.UseAuthorization();

app.MapControllers();

app.Run();