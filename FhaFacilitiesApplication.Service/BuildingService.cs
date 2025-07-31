#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class BuildingService : IBuildingService
    {
        #region Declaration
        private readonly IBuildingRepository _buildingRepository;
        #endregion

        #region Constructor
        public BuildingService(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository ?? throw new ArgumentNullException(nameof(buildingRepository));
        }
        #endregion

        public async Task<List<BuildingModel>> GetBuildingsAsync(Guid campusGUID, bool onlyBuildings)
        {
            return await _buildingRepository.GetBuildingsAsync(campusGUID, onlyBuildings);
        }

        public async Task<ToasterModel> AddBuildingAsync(BuildingModel buildingModel)
        {
            buildingModel.UniqueGUID = Guid.NewGuid();
            var result = await _buildingRepository.AddBuildingAsync(buildingModel);
            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Building added successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Created
                };
            }
            if (result == -1)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Building already exists.",
                    Type = ToasterType.warning.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to add building.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public async Task<BuildingModel?> GetBuildingByUniqueIdAsync(string buildingUniqueId)
        {
            return await _buildingRepository.GetBuildingByUniqueIdAsync(buildingUniqueId);
        }

        public async Task<ToasterModel> UpdateBuildingAsync(BuildingModel buildingModel)
        {
            var insertId = await _buildingRepository.AddBuildingAsync(buildingModel);
            if (insertId > 0)
            {
                _ = _buildingRepository.DeleteBuildingByUniqueGuidAsync(buildingModel);
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Building updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            if (insertId == -1)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = $"A Building with the ID {buildingModel.BuildingID} already exists.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to update building.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public async Task<ToasterModel> DeleteBuildingByUniqueGuidAsync(BuildingModel buildingModel)
        {
            buildingModel.LastSavedBy = "LastSavedBy";
            buildingModel.LastSavedDate = DateTime.UtcNow;
            var result = await _buildingRepository.DeleteBuildingByUniqueGuidAsync(buildingModel);

            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Building deleted successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to delete building.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }

        public async Task<BuildingModel?> CheckBuildingExistsAsync(string buildingId)
        {
            return await _buildingRepository.CheckBuildingExistsAsync(buildingId);
        }




        public async Task<BuildingModel?> GetBuildingByCampusAndBuildingAsync(Guid campusGuid, Guid buildingGuid)
        {
            return await _buildingRepository.GetBuildingByCampusAndBuildingAsync(campusGuid, buildingGuid);
        }
    }
}
