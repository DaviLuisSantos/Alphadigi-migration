using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;
using Alphadigi_migration.Services;
using Alphadigi_migration.Repositories;
using Alphadigi_migration.Interfaces;
using Carter;
using Carter.ResponseNegotiators.SystemTextJson;
using Alphadigi_migration;
using Serilog;
using Serilog.Filters;
using IniParser;
using IniParser.Model;
using Alphadigi_migration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCarter(configurator: c =>
    c.WithResponseNegotiator<SystemTextJsonResponseNegotiator>()
);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.log",
                  rollingInterval: RollingInterval.Day,
                  retainedFileCountLimit: 7,
                  outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
    .Filter.ByExcluding(Matching.FromSource("Microsoft.EntityFrameworkCore"))
    //.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore"))
    .Filter.ByExcluding(Matching.FromSource("Microsoft.Hosting"))
    .CreateLogger();

var parser = new FileIniDataParser();
IniData data = parser.ReadFile(builder.Configuration.GetConnectionString("ConfigConnection"));
string host = data["BD"]["host"];

if (host != "localhost" &&  host != "Localhost")
{
    throw new InvalidOperationException("The server can only be started on localhost.");
}

var end = data["BD"]["end"];

var firebirdConnectionString = $"Server=127.0.0.1;Database={end};{builder.Configuration.GetConnectionString("FirebirdConnection")}";

var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnection");

builder.Services.AddDbContext<AppDbContextFirebird>(options =>
    options.UseFirebird(firebirdConnectionString));

builder.Services.AddDbContext<AppDbContextSqlite>(options =>
    options.UseSqlite(sqliteConnectionString));

builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddSingleton(builder.Configuration);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var kestrelConfig = builder.Configuration.GetSection("Kestrel");
    serverOptions.Configure(kestrelConfig);
});

builder.Host.UseSerilog();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContextSqlite>();
    dbContext.Database.EnsureCreated();

    var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();
    await areaService.SyncAreas();
    var alphadigiService = scope.ServiceProvider.GetRequiredService<IAlphadigiService>();
    await alphadigiService.SyncAlphadigi();
    var condominioService = scope.ServiceProvider.GetRequiredService<CondominioService>();
    await condominioService.SyncCondominio();

    var schedulerService = new DailyTaskSchedulerService(() =>
    {
        alphadigiService.UpdateStage("DELETE").Wait();
    });

}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<RequestTimingMiddleware>();

app.UseCors("AllowAllOrigins");

app.MapCarter();

app.Run();