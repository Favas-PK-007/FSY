#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface ICampusRepository
    {
        Task<List<CampusModel>> GetAllAsync();
        Task<CampusModel?> GetCampusByUniqueID(string uniqueID);
        Task<int> AddCampusAsync(CampusModel requestModel);
        Task<int> SoftDeleteCampusByIdAsync(int uniqueID, Guid? uniqueGUID, string user);
    }
}
