#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IBuildingRepository
    {
        Task<List<BuildingModel>> GetBuildingsAsync(Guid campusGUID, bool onlyBuildings);
        Task<int> AddBuildingAsync(BuildingModel buildingModel);
        Task<BuildingModel?> GetBuildingByUniqueIdAsync(string buildingUniqueId);
        Task<int> DeleteBuildingByUniqueGuidAsync(BuildingModel buildingModel);
        Task<BuildingModel?> CheckBuildingExistsAsync(string buildingId);




        Task<BuildingModel?> GetBuildingByCampusAndBuildingAsync(Guid campusGuid, Guid buildingGuid);


    }
}
