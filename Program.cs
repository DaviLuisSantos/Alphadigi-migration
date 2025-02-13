using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using FirebirdSql.EntityFrameworkCore.Firebird;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Alphadigi_migration;
using Carter;
using System;
using FirebirdSql.Data.FirebirdClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContextFirebird>(options =>
    options.UseFirebird("Server=127.0.0.1;Database=D:\\AcessoLinear\\Dados\\BANCODEDADOS.fdb;User=SYSDBA;Password=masterkey;Pooling=true"));

// Register AppDbContextSqlite
builder.Services.AddDbContext<AppDbContextSqlite>(options =>
    options.UseSqlite("Data Source=database.db"));

// Register VeiculoService
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IAlphadigiService,AlphadigiService>();
builder.Services.AddScoped<IAlphadigiHearthBeatService,AlphadigiHearthBeatService>();

builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(3333); // Escuta em todos os endere�os IP na porta 5000
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // Escuta em todos os endere�os IP na porta 5001 com HTTPS
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextSqlite>();
    dbContext.Database.EnsureCreated();
}

using (var connection = new FbConnection("User=SYSDBA;Password=masterkey;Database=D:\\AcessoLinear\\Dados\\BANCODEDADOS.fdb;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=UTF8;Pooling=true"))
{
    await connection.OpenAsync();
    Console.WriteLine("Conex�o bem-sucedida!");
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapCarter();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
