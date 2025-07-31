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
            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IStructureRepository, StructureRepository>();
            services.AddScoped<IConduitPathRepository, ConduitPathRepository>();
            services.AddScoped<IDuctRepository, DuctRepository>();
            services.AddScoped<IMeterialRepository, MeterialRepository>();
            services.AddScoped<IMaterialDetailRepository, MaterialDetailRepository>();
            services.AddScoped<ICableRepository, CableRepository>();
            services.AddScoped<IFiberRepository, FiberRepository>();
            services.AddScoped<ICircuitRepository, CircuitRepository>();
            services.AddScoped<ISpliceRepository, SpliceRepository>();
            services.AddScoped<IComponentRepository, ComponentRepository>();
            services.AddScoped<IPortRepository, PortRepository>();
            services.AddScoped<IConnectionRepository, ConnectionRepository>();
            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            return services;
        }
    }
}
