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
    public interface ISpliceRepository
    {
        Task<List<SpliceModel>> GetSpliceByCampusAndStructureAsync(Guid campusGuid, Guid structureGuid, bool loadStructure);
        Task<SpliceModel?> GetSpliceByGuidAsync(Guid uniqueGuid);
        Task<bool> IsSpliceExistsAsync(string spliceId, Guid campusGuid);
        Task<int> CreateSpliceAsync(SpliceModel model);
        Task<int> UpdateSpliceAsync(SpliceModel model);

    }
}
