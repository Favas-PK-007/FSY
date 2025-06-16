using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Storage;

namespace FhaFacilitiesApplication.Startup
{
    public static class AppRepositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Register application repositories here
            services.AddScoped<ICampusRepository, CampusRepository>();
            return services;
        }
    }
}
