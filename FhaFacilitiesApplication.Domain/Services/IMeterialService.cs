#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IMeterialService
    {
        Task<MeterialModel> GetMeterialsAsync(Guid parentGuid, Guid templateGuid, bool loadDetails, bool loadComponents);
        Task<int> SaveNewDuctMaterialAsync(Guid uniqueGuid, Guid parentGuid, bool loadData, bool loadDetails, bool loadComponents);
        Task<List<MeterialModel>> GetStructureModels(bool addEdit, string materialType, Guid parentGuid, Guid templateGuid);
        Task<int> CreateMaterialAsync(MeterialModel materialModel);
        Task<MeterialModel?> GetMaterialByGuidAsync(Guid uniqueGuid);
        Task<MeterialModel?> GetMaterialByParentGuidAsync(Guid parentGuid);
        Task<List<MeterialModel>> GetSubDuctsAsync(Guid parentGuid, Guid templateGuid);
        Task<int> DeleteMaterialAsync(MeterialModel model);
        Task<List<MeterialModel>> GetComponentModelDropdownAsync(string materialType, Guid modelGuid, Guid templateGuid);
        Task<MeterialModel> GetEquipmentModelAsync(Guid parentGuid, Guid templateGuid);
        Task<List<string>> GetMaterialTypesAsync();
        Task<List<DropdownModel>> GetParentMaterialDropdownAsync(string materialType);
        Task<List<DropdownModel>> GetChildMaterialsAsync(Guid parentGuid, string materialType);
        Task<List<string>> GetManufacturersAsync(string materialType);
        Task<List<string>> GetDetailHeadersAsync(string materialType);
        Task<ToasterModel> SaveMaterialAsync(MeterialModel mode);
        Task<bool> CheckIfMaterialExistsAsync(MeterialModel mode);
        Task<ToasterModel> UpdateMaterialAsync(MeterialModel model);
    }
}
