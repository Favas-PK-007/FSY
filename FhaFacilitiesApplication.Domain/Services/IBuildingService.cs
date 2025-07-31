#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IBuildingService
    {
        Task<List<BuildingModel>> GetBuildingsAsync(Guid campusGUID, bool onlyBuildings);
        Task<ToasterModel> AddBuildingAsync(BuildingModel buildingModel);
        Task<BuildingModel?> GetBuildingByUniqueIdAsync(string buildingUniqueId);
        Task<ToasterModel> UpdateBuildingAsync(BuildingModel buildingModel);
        Task<ToasterModel> DeleteBuildingByUniqueGuidAsync(BuildingModel buildingModel);
        Task<BuildingModel?> CheckBuildingExistsAsync(string buildingId);

        Task<BuildingModel?> GetBuildingByCampusAndBuildingAsync(Guid campusGuid, Guid buildingGuid);
    }
}
