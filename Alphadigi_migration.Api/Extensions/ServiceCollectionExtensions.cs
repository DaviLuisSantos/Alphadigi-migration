using Alphadigi_migration.Api.Factories;
using Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;
using Alphadigi_migration.Application.Handlers.CommandHandlers.Veiculo;
using Alphadigi_migration.Application.Handlers.CommandHandlers.Alphadigi;
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
        //services.AddScoped<IAlphaDigiCommunicationService, AlphaDigiCommunicationService>();

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
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkwMTIxNjAwIiwiaWF0IjoiMTc1ODY0ODgzOSIsImFjY291bnRfaWQiOiIwMTk5NzdhMmJjMWY3NGMzOWY1YzUxNzMxMzQ5OTM0NyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazV2dDdiY3B5czQwMHRlZTE1emN6ajRzIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.VexWDxvxPmUEndH5t-vAtJB9B5wvi4s6QCHsShgd5PZX-x8MoUuLMclK0t5DnmhoWCXmkszk4BNZyB1SMpqmV9rwao5n7AK05W5jEDhDJwPLMC524pVPyP53YHcpyr5NXuXSs1zTo_n02Vb1mlk9NjIw7RiaHzqrI3V7_xjGOSuyRy0_WoAO0rcS_XknY_qSwxnLqlaqt-w2VlRvqGcu-c6EQ9Py-iHRyb6tkF8816cXeXqtxoKCsWHDGdzSn94tv7k8IzRgMt6EYLzAdgh9HBqLF7JjNLDiYf8h-6uVbF740KMBHxQFAJ7VaqYGW2JdqUYowFqaxjvD1y35cSpuUQ";
            // Registra handlers do assembly atual (API)
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            // Registra handlers do assembly da Application usando o tipo ProcessPlateCommandHandler
            cfg.RegisterServicesFromAssembly(typeof(ProcessPlateCommandHandler).Assembly);
        });





        return services;
    }


}
