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
{
    // Cambiamos esto para que use directamente la URL de producción en Render
    var connectionUrl = "postgresql://heladeria_u:1SVQiPPnU721pWOA2NKUXYZkGCj0CNHu@dpg-d8j2her7uimc73b9kk50-a.virginia-postgres.render.com/heladeria_chy4\r\n";

    // .NET necesita convertir el formato postgres:// a un formato entendible por Npgsql
    var databaseUri = new Uri(connectionUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    var connectionString = $"Host={databaseUri.Host};" +
                           $"Port={databaseUri.Port};" +
                           $"Username={userInfo[0]};" +
                           $"Password={userInfo[1]};" +
                           $"Database={databaseUri.LocalPath.TrimStart('/')};" +
                           $"SslMode=Require;" +
                           $"Trust Server Certificate=True;";

    options.UseNpgsql(connectionString);
});

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