using Alphadigi_migration.Application.Mapping;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Application.Services;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Factories;
using Alphadigi_migration.Infrastructure.Repositories;


namespace Alphadigi_migration.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IAlphadigiRepository, AlphadigiRepository>();
        services.AddScoped<IAlphadigiService, AlphadigiService>();
        services.AddScoped<AcessoRepository>();

        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IAreaService, AreaService>();
        services.AddScoped<IAlphadigiHearthBeatService, AlphadigiHearthBeatService>();
        services.AddScoped<IAlphadigiPlateService, AlphadigiPlateService>();

        services.AddScoped<IUnidadeService, UnidadeService>();

        services.AddScoped<IMonitorAcessoLinear, MonitorAcessoLinear>();

        services.AddScoped<Application.Service.UdpBroadcastService>();

       
        services.AddScoped<IMensagemDisplayRepository, MensagemDisplayRepository>();

        services.AddScoped<IMensagemDisplayService, MensagemDisplayService>();

        services.AddScoped<IVeiculoAccessProcessor, VeiculoAccessProcessor>();
      
     
        services.AddScoped<IPlacaLidaRepository, PlacaLidaRepository>();
        services.AddScoped<IPlacaLidaService, PlacaLidaService>();

        services.AddScoped<IAreaRepository, AreaRepository>();

        services.AddScoped<IUnidadeRepository, UnidadeRepository>();

        services.AddScoped<Application.Services.IAccessHandlerFactory, Application.Services.AccessHandlerFactory>();
    
        services.AddScoped<IPlacaLidaService, PlacaLidaService>();

        services.AddScoped<IVeiculoRepository, VeiculoRepository>();


        services.AddScoped<IAcessoRepository, AcessoRepository>();
        services.AddScoped<ICondominioRepository, CondominioRepository>();
        services.AddScoped<IAlphadigiRepository, AlphadigiRepository>();
      



        services.AddScoped<AcessoService>();
        services.AddScoped<DisplayService>();
        services.AddScoped<CondominioService>();

        services.AddSingleton<IHeartbeatFactory, HeartbeatFactory>();

        services.AddScoped<MensagemDisplayService>();
        services.AddScoped<MensagemDisplayRepository>();

        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        services.AddRazorPages();
        services.AddHttpClient();
        services.AddOpenApi();

        return services;
    }
}
