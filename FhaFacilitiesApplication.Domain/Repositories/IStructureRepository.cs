#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IStructureRepository
    {
        Task<List<StructureModel>> GetStructureByCampusAndBuildingAsync(string campusGuid, string buildingGuid);
        Task<int> CreateStructureAsync(StructureModel requestModel);
        Task<List<MeterialModel>> GetStructureModelsByTypeAsync(string meterialType, Guid parentGuid, Guid TemplateGuid, bool isAddEdit);
        Task<List<StructureModel>> GetStructureIdsAsync(Guid campusGuid, Guid buildingGuid, bool loadBuilding, bool loadAllStructures);
        Task<bool> IsExistingStructureIdAsync(Guid campusGuid, string structureId);

        Task<StructureModel?> GetStructureAsync(Guid uniqueGuid);
        Task<StructureModel?> GetStructureByIdAsync(int id);
        Task<int> DeleteStructureByUniqueGuidAsync(StructureModel structureModel);
        Task<StructureModel?> CheckStructureExistsAsync(string UniqueID);
    }
}
