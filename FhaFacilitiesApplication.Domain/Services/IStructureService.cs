#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IStructureService
    {
        Task<List<StructureModel>> GetStructureByCampusAndBuildingAsync(string campusGuid, string buildingGuid);
        Task<ToasterModel> CreateStructureAsync(StructureModel requestModel);
        Task<ToasterModel> UpdateStructureAsync(StructureModel requestModel);
        Task<List<MeterialModel>> GetStructureModelsByTypeAsync(string meterialType, Guid parentGuid, Guid TemplateGuid, bool isAddEdit);
        Task<StructureModel?> GetStructureAsync(Guid uniqueGuid);
        Task<List<StructureModel>> GetStructureIdsAsync(Guid campusGuid, Guid buildingGuid, bool loadBuilding, bool loadAllStructures);
        Task<StructureModel?> GetStructureByIdAsync(int id);
        Task<ToasterModel> DeleteStructureByUniqueGuidAsync(StructureModel buildingModel);
        Task<StructureModel?> CheckStructureExistsAsync(string buildingId);
    }
}
