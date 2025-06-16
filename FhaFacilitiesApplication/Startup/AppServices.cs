using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Service;

namespace FhaFacilitiesApplication.Startup
{
    public static class AppServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Register application services here
            services.AddScoped<ICampusService, CampusService>();
            return services;
        }
    }
}
