using Alphadigi_migration.Api.Factories;
using Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;
using Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;
using Alphadigi_migration.Application.Mapping;
using Alphadigi_migration.Application.Queries.Veiculo;
using Alphadigi_migration.Application.Service;
using Alphadigi_migration.Domain.Interfaces;
using Alphadigi_migration.Infrastructure.Repositories;
using MediatR;
using System.Reflection;


namespace Alphadigi_migration.Api.Extensions;

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
       // services.AddScoped<IAlphadigiPlateService, AlphadigiPlateService>();

        services.AddScoped<IUnidadeService, UnidadeService>();

        services.AddScoped<IMonitorAcessoLinear, MonitorAcessoLinear>();

        services.AddScoped<UdpBroadcastService>();

        services.AddScoped<IUdpBroadcastService, UdpBroadcastService>();


        services.AddScoped<IMensagemDisplayRepository, MensagemDisplayRepository>();

        services.AddScoped<IMensagemDisplayService, MensagemDisplayService>();

        services.AddScoped<IVeiculoAccessProcessor, VeiculoAccessProcessor>();

        services.AddScoped<IVisitanteRepository, VisitanteRepository>();

       
        services.AddScoped<IVisitaSaiuSemControleRepository, VisitaSaiuSemControleRepository>();
        services.AddScoped<IPlacaLidaRepository, PlacaLidaRepository>();
        services.AddScoped<IPlacaLidaService, PlacaLidaService>();

        services.AddScoped<IAreaRepository, AreaRepository>();

        services.AddScoped<IUnidadeRepository, UnidadeRepository>();

        services.AddScoped<IAccessHandlerFactory, AccessHandlerFactory>();
    
        services.AddScoped<IPlacaLidaService, PlacaLidaService>();

        services.AddScoped<IVeiculoRepository, VeiculoRepository>();


        services.AddScoped<IAcessoRepository, AcessoRepository>();
        services.AddScoped<ICondominioRepository, CondominioRepository>();
        services.AddScoped<IAlphadigiRepository, AlphadigiRepository>();


        services.AddTransient<IDisplayProtocolService, DisplayProtocolService>();

        services.AddScoped<ICameraRepository, CameraRepository>();
        services.AddScoped<IAcessoRepository, AcessoRepository>();
        services.AddScoped<IAlphadigiRepository, AlphadigiRepository>();
        services.AddScoped<IAreaRepository, AreaRepository>();
        services.AddScoped<ICondominioRepository, CondominioRepository>();
        services.AddScoped<IMensagemDisplayRepository, MensagemDisplayRepository>();
        services.AddScoped<IPlacaLidaRepository, PlacaLidaRepository>();
        services.AddScoped<IUnidadeRepository, UnidadeRepository>();
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();

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

       

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.Load("Alphadigi_migration.Application"));
        });

      

       

        return services;
    }

    
}
