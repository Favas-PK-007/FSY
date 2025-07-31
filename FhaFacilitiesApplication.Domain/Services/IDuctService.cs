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
    public interface IDuctService
    {
        Task<List<DuctModel>> GetDuctsAsync(Guid conduitGuid, bool loadSubDucts);
        Task<int> CreateDuctAsync(DuctModel requestModel);
        Task<DuctModel?> GetDuctByGuidAsync(Guid ductGuid, bool subDucts, string mode);
        Task<ToasterModel<Guid>> SaveDuctAsync(DuctModel requestModel);
        Task<List<MaterialTypeModel>> GetDuctTypesAsync(bool addEdit,string materialType, Guid parentGuid, Guid templateGuid);
        Task<ToasterModel<Guid>> UpdateDuctAsync(DuctModel requestModel);
        Task<ToasterModel> DeleteDuctAsync(DuctModel model);
    }
}
