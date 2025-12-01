using Alphadigi_migration.Api;
using Alphadigi_migration.Api.Extensions;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Domain.Options;
using Alphadigi_migration.Infrastructure.Data;
using Carter;
using IniParser;
using IniParser.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();


builder.Services.Configure<PlateComparisonSettings>(
    builder.Configuration.GetSection("PlateComparisonSettings")
);

//builder.Services.AddCarter(configurator: c =>
//    c.WithResponseNegotiator<SystemTextJsonResponseNegotiator>()
//);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

if (host != "localhost" && host != "Localhost")
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<RequestTimingMiddleware>();

app.UseCors("AllowAllOrigins");

//app.MapCarter();
app.MapControllers();

app.Run();