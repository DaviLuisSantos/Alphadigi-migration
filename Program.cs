using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using FirebirdSql.EntityFrameworkCore.Firebird;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Carter;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using Carter.Response;
using Carter.ResponseNegotiators.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter(configurator: c =>
    c.WithResponseNegotiator<SystemTextJsonResponseNegotiator>()
);

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
builder.Services.AddSingleton<IResponseNegotiator, SystemTextJsonResponseNegotiator>();
builder.Services.AddScoped<IAlphadigiService, AlphadigiService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAlphadigiHearthBeatService, AlphadigiHearthBeatService>();
builder.Services.AddScoped<IAlphadigiPlateService, AlphadigiPlateService>();

builder.Services.AddScoped<UnidadeService>();
builder.Services.AddScoped<MonitorAcessoLinear>();
builder.Services.AddScoped<UdpBroadcastService>();
builder.Services.AddScoped<AccessHandlerFactory>(); 

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
    serverOptions.ListenAnyIP(3332); // Escuta em todos os endereços IP na porta 5000
    serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // Escuta em todos os endereços IP na porta 5001 com HTTPS
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextSqlite>();
    dbContext.Database.EnsureCreated();

    var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();
     await areaService.SyncAreas();
}

using (var connection = new FbConnection("User=SYSDBA;Password=masterkey;Database=D:\\AcessoLinear\\Dados\\BANCODEDADOS.fdb;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=UTF8;Pooling=true"))
{
    await connection.OpenAsync();
    Console.WriteLine("Conexão bem-sucedida!");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();

app.MapCarter();

app.Run();