using Alphadigi_migration.Factories;
using Alphadigi_migration.Models;
using Alphadigi_migration.Repositories;
using Alphadigi_migration.Services;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddScoped<IAlphadigiRepository, AlphadigiRepository>();
            services.AddScoped<IAlphadigiService, AlphadigiService>();

            services.AddScoped<IVeiculoService, VeiculoService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IAlphadigiHearthBeatService, AlphadigiHearthBeatService>();
            services.AddScoped<IAlphadigiPlateService, AlphadigiPlateService>();

            services.AddScoped<IUnidadeService, UnidadeService>();
            services.AddScoped<MonitorAcessoLinear>();
            services.AddScoped<UdpBroadcastService>();

            services.AddScoped<IAccessHandlerFactory, AccessHandlerFactory>();
            services.AddScoped<IVeiculoAccessProcessor, VeiculoAccessProcessor>();
            services.AddScoped<IPlacaLidaService, PlacaLidaService>();

            services.AddScoped<AcessoService>();
            services.AddScoped<DisplayService>();
            services.AddScoped<CondominioService>();

            services.AddSingleton<IHeartbeatFactory, HeartbeatFactory>();

            services.AddScoped<MensagemDisplayService>();
            services.AddScoped<MensagemDisplayRepository>();

            services.AddAutoMapper(typeof(MappingProfile));

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
}
