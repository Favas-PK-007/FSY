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
    public interface IConduitPathService
    {
        Task<List<ConduitModel>> GetConduitsAsync(string campusGuid, string structureGuid, bool loadDucts);
        Task<ToasterModel> CreateConduitAsync(ConduitModel requestModel);
        Task<ToasterModel> UpdateConduitPathAsync(ConduitModel requestModel);
        Task<ConduitModel?> GetConduitByUniqueGuid(Guid conduitGuid);
        Task<ConduitModel?> GetConduitByUniqueId(int conduitId);
        Task<ToasterModel> DeleteConduitPathAsync(ConduitModel requestModel);
    }
}
