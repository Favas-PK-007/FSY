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
    public interface IMeterialRepository
    {
        Task<MeterialModel?> GetMeterialsAsync(Guid parentGuid, Guid templateGuid, bool loadDetails, bool loadComponents);
        Task<MeterialModel?> GetTemplateMeterialAsync(Guid uniqueGuid, Guid parentGuid);
        Task<int> SaveNewDuctMaterialAsync(MeterialModel model);
        Task<List<MeterialModel>> GetStructureModels(bool addEdit, string materialType, Guid parentGuid, Guid templateGuid);
        Task<MeterialModel?> GetMaterialByGuidAsync(Guid uniqueGuid);
        Task<MeterialModel?> GetMaterialByParentGuidAsync(Guid parentGuid);
        Task<List<MeterialModel>> GetSubDuctsAsync(Guid parentGuid, Guid templateGuid);
        Task<bool> BulkInsertMaterialsAsync(List<MeterialModel> materials);
        Task<int> DeleteMaterialAsync(MeterialModel model);
        Task<List<string>> GetMaterialTypesAsync();
        Task<List<MeterialModel>> GetParentMaterialsAsync(string materialType);
        Task<List<MeterialModel>> GetChildMaterialsAsync(Guid parentGuid, string materialType);
        Task<List<string>> GetMaterialManufacturersAsync(string materialType);
        Task<List<string>> GetMaterialDetailHeadersAsync(string materialType);
        Task<bool> CheckMaterialExistAsync(string modelId, string materialType, string manufacturerId);
        Task<bool> UpdateMaterialAsync(MeterialModel model);

    }
}
