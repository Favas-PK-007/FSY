#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface ICampusRepository
    {
        Task<List<CampusModel>> GetCampusListAsync();
        Task<string> AddCampusAsync(CampusModel requestModel);
    }
}
