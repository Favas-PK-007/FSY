#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IConduitPathRepository
    {
        Task<List<ConduitModel>> GetConduitsAsync(string campusGuid, string structureGuid, bool loadDucts);
        Task<int> CreateConduitAsync(ConduitModel requestModel);
        Task<bool> CheckConduitPathExists(ConduitModel requestModel);
        Task<int> DeleteConduitPathAsync(ConduitModel requestModel);
        Task<ConduitModel?> GetConduitByUniqueGuid(Guid conduitGuid);
        Task<ConduitModel?> GetConduitByUniqueId(int conduitId);
    }
}
