#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class CampusService : ICampusService
    {
        #region Declarations
        private readonly ICampusRepository _campusRepository;
        #endregion

        #region Constructor
        public CampusService(ICampusRepository campusRepository)
        {
            _campusRepository = campusRepository ?? throw new ArgumentNullException(nameof(campusRepository));
        }
        #endregion

        public async Task<List<CampusModel>> GetCampusListAsync()
        {
            return await _campusRepository.GetCampusListAsync();
        }

        public async Task<string> AddCampusAsync(CampusModel requestModel)
        {
            return await _campusRepository.AddCampusAsync(requestModel);
        }

    }
}
