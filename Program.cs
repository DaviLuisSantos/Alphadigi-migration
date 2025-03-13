using Microsoft.EntityFrameworkCore;
using FirebirdSql.EntityFrameworkCore.Firebird;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Carter;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using Carter.Response;
using Carter.ResponseNegotiators.SystemTextJson;
using Alphadigi_migration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter(configurator: c =>
    c.WithResponseNegotiator<SystemTextJsonResponseNegotiator>()
);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

// Get connection strings from configuration
var firebirdConnectionString = builder.Configuration.GetConnectionString("FirebirdConnection");
var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection");

// Register the Firebird context
builder.Services.AddDbContext<AppDbContextFirebird>(options =>
    options.UseFirebird(firebirdConnectionString));
// Register the SQLite context
builder.Services.AddDbContext<AppDbContextSqlite>(options =>
    options.UseSqlite(sqliteConnectionString));

// Registre os serviços
builder.Services.AddScoped<IAlphadigiService, AlphadigiService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAlphadigiHearthBeatService, AlphadigiHearthBeatService>();
builder.Services.AddScoped<IAlphadigiPlateService, AlphadigiPlateService>();

builder.Services.AddScoped<IUnidadeService, UnidadeService>();
builder.Services.AddScoped<MonitorAcessoLinear>();
builder.Services.AddScoped<UdpBroadcastService>();

// Registre os handlers
builder.Services.AddScoped<VisitaAccessHandler>();
builder.Services.AddScoped<SaidaSempreAbreAccessHandler>();
builder.Services.AddScoped<ControlaVagaAccessHandler>();
builder.Services.AddScoped<NaoControlaVagaAccessHandler>();

// Registre a fábrica de handlers
builder.Services.AddScoped<IAccessHandlerFactory, AccessHandlerFactory>();
builder.Services.AddScoped<IVeiculoAccessProcessor, VeiculoAccessProcessor>();
builder.Services.AddScoped<PlacaLidaService>();

builder.Services.AddScoped<AcessoService>();
builder.Services.AddScoped<DisplayService>();

// Configure CORS
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

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(3332); // Escuta em todos os endereços IP na porta 3332
    //serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // Escuta em todos os endereços IP na porta 5001 com HTTPS
});

var app = builder.Build();

// Configure o banco de dados SQLite
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextSqlite>();
    dbContext.Database.EnsureCreated();

    var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();
    await areaService.SyncAreas();
    var alphadigiService = scope.ServiceProvider.GetRequiredService<IAlphadigiService>();
    await alphadigiService.SyncAlphadigi();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<RequestTimingMiddleware>();

app.UseCors("AllowAllOrigins");

app.MapCarter();

 app.Run();