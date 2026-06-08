using HeladeriaAPI.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HeladeriaContext>(options =>
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (string.IsNullOrEmpty(databaseUrl))
    {
        throw new Exception("No existe la variable DATABASE_URL en Railway.");
    }

    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    var connectionStringBuilder = new NpgsqlConnectionStringBuilder
    {
        Host = databaseUri.Host,
        Port = databaseUri.Port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = databaseUri.AbsolutePath.TrimStart('/'),
        SslMode = SslMode.Require,
        TrustServerCertificate = true,
        IncludeErrorDetail = true
    };

    options.UseNpgsql(connectionStringBuilder.ConnectionString);
});

var app = builder.Build();

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
        Console.WriteLine("Error al conectar con PostgreSQL:");
        Console.WriteLine(ex.ToString());
    }
}

app.Run();