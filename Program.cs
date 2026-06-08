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
builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Solo dejamos Npgsql. Tus etiquetas [Column] y [Table] se encargan del resto.
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