using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Carter;
using Carter.ResponseNegotiators.SystemTextJson;
using Alphadigi_migration;
using Alphadigi_migration.Models;

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

// Registre os servi�os
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

// Registre a f�brica de handlers
builder.Services.AddScoped<IAccessHandlerFactory, AccessHandlerFactory>();
builder.Services.AddScoped<IVeiculoAccessProcessor, VeiculoAccessProcessor>();
builder.Services.AddScoped<PlacaLidaService>();

builder.Services.AddScoped<AcessoService>();
builder.Services.AddScoped<DisplayService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));


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
// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var kestrelConfig = builder.Configuration.GetSection("Kestrel");
    serverOptions.Configure(kestrelConfig);
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