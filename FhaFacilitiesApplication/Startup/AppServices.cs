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
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IStructureService, StructureService>();
            services.AddScoped<IConduitPathService, ConduitPathService>();
            services.AddScoped<IDuctService, DuctService>();
            services.AddScoped<IMeterialService, MeterialService>();
            services.AddScoped<IMaterialDetailService, MaterialDetailService>();
            services.AddScoped<ICableService, CableService>();
            services.AddScoped<IFiberService, FiberService>();
            services.AddScoped<ICircuitService, CircuitService>();
            services.AddScoped<ISpliceService, SpliceService>();
            services.AddScoped<IComponentService, ComponentService>();
            services.AddScoped<IPortService, PortService>();
            services.AddScoped<IConnectionService, ConnectionService>();
            services.AddScoped<IEquipmentService, EquipmentService>();
            services.AddScoped<IReportService, ReportService>();
            return services;
        }
    }
}
