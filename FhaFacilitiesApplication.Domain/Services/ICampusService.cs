#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface ICampusService
    {
        Task<List<CampusModel>> GetAllAsync();
        Task<CampusModel?> GetCampusByUniqueID(string UniqueID);
        Task<ToasterModel> AddCampusAsync(CampusModel requestModel);
        Task<ToasterModel> UpdateCampusByIdAsync(CampusModel requestModel);
        Task<ToasterModel> DeleteCampusByIdAsync(int UniqueID, Guid UniqueGUID);
    }
}
