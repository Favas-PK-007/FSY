#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface ICampusService
    {
        Task<List<CampusModel>> GetCampusListAsync();
        Task<string> AddCampusAsync(CampusModel requestModel);
    }
}
