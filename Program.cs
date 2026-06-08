using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    var host = Environment.GetEnvironmentVariable("PGHOST");
    var port = Environment.GetEnvironmentVariable("PGPORT");
    var database = Environment.GetEnvironmentVariable("PGDATABASE");
    var username = Environment.GetEnvironmentVariable("PGUSER");
    var password = Environment.GetEnvironmentVariable("PGPASSWORD");

    var connectionString =
        $"Host={host};" +
        $"Port={port};" +
        $"Database={database};" +
        $"Username={username};" +
        $"Password={password};" +
        $"SSL Mode=Disable;" +
        $"Include Error Detail=true;";

    Console.WriteLine($"Conectando a {host}:{port}/{database}");

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HeladeriaContext>();

    try
    {
        context.Database.Migrate();
        Console.WriteLine("Base de datos conectada correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }
}

app.Run();