using DeviceManager.Business.Implementations;
using DeviceManager.Business.Interfaces;
using DeviceManager.Repository.Implementations;
using DeviceManager.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManager.API
{
    public static class ServiceExtensions
    {
        public static void ConfigureCORS(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }
        public static void ConfigureDIRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
        }
        public static void ConfigureDIServices(this IServiceCollection services)
        {
            #region Services Configuration

            services.AddTransient<IServiceHelper, ServiceHelper>();
            services.AddTransient<IUserService, UserService>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDeviceService, DeviceService>();
            
            #endregion
        }
    }
}
