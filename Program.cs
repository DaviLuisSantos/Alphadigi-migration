using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using FirebirdSql.EntityFrameworkCore.Firebird;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Alphadigi_migration;
using Carter;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContextFirebird>(options =>
    options.UseFirebird("Server=localhost;Database=D:\\AcessoLinear\\Dados\\BANCODEDADOS.fdb;User=SYSDBA;Password=masterkey;"));

// Register AppDbContextSqlite
builder.Services.AddDbContext<AppDbContextSqlite>(options =>
    options.UseSqlite("Data Source=database.db"));

// Register VeiculoService
builder.Services.AddScoped<VeiculoService>();

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
    serverOptions.ListenAnyIP(3333); // Escuta em todos os endereços IP na porta 5000
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // Escuta em todos os endereços IP na porta 5001 com HTTPS
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextSqlite>();
    dbContext.Database.EnsureCreated();
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
