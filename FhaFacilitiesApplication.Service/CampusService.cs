#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
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

        public async Task<List<CampusModel>> GetAllAsync()
        {
            return await _campusRepository.GetAllAsync();
        }

        public async Task<ToasterModel> AddCampusAsync(CampusModel requestModel)
        {
            requestModel.UniqueGUID = Guid.NewGuid();
            var result = await _campusRepository.AddCampusAsync(requestModel);

            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Campus added successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }

            if (result == -1)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Campus already exists.",
                    Type = ToasterType.warning.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }

            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to add campus.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public Task<CampusModel?> GetCampusByUniqueID(string UniqueID)
        {
            return _campusRepository.GetCampusByUniqueID(UniqueID);
        }

        public async Task<ToasterModel> UpdateCampusByIdAsync(CampusModel requestModel)
        {

            var insertId = await _campusRepository.AddCampusAsync(requestModel);
            if (insertId > 0)
            {
                _ = _campusRepository.SoftDeleteCampusByIdAsync(requestModel.UniqueID, requestModel.UniqueGUID, requestModel.CreatedBy);
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Campus updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else if (insertId == -1)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = $"A Campus with the ID {requestModel.CampusID} already exists.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to update campus.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public async Task<ToasterModel> DeleteCampusByIdAsync(int UniqueID, Guid UniqueGUID)
        {
            var result = await _campusRepository.SoftDeleteCampusByIdAsync(UniqueID, UniqueGUID, "LastSavedBy");

            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Campus Deleted successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }

            else
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to delete campus.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };

            }
        }
    }
}
