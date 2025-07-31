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
    public interface ISpliceService
    {
        Task<List<SpliceModel>> GetSpliceByCampusAndStructureAsync(Guid campusGuid, Guid structureGuid, bool loadStructure);
        Task<List<SpliceModel>> GetSplicesAsync(Guid campusGuid);
        Task<SpliceModel?> GetSpliceByGuidAsync(Guid uniqueGuid, string? action);
        Task<ToasterModel> CreateSpliceAsync(SpliceModel spliceModel);
        Task<ToasterModel> UpdateSpliceAsync(SpliceModel model);
        Task<ToasterModel> DeleteSpliceAsync(SpliceModel model);
    }
}
