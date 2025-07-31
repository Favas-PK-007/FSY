#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class StructureService : IStructureService
    {
        #region Declaration
        private readonly IStructureRepository _structureRepository;
        private readonly IMeterialService _meterialService;
        #endregion

        #region Constructor
        public StructureService(IStructureRepository structureRepository, IMeterialService meterialService)
        {
            _structureRepository = structureRepository ?? throw new ArgumentNullException(nameof(structureRepository));
            _meterialService = meterialService;
        }
        #endregion

        public async Task<List<StructureModel>> GetStructureByCampusAndBuildingAsync(string campusGuid, string buildingGuid)
        {
            return await _structureRepository.GetStructureByCampusAndBuildingAsync(campusGuid, buildingGuid);
        }

        public async Task<ToasterModel> CreateStructureAsync(StructureModel requestModel)
        {
            requestModel.UniqueGUID = Guid.NewGuid();
            var isExists = await _structureRepository.IsExistingStructureIdAsync(requestModel.CampusGUID, requestModel.StructureID);
            if (isExists)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = $"Structure ID {requestModel.StructureID} already exists.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            var selectedStructureModel = await _meterialService.GetMaterialByGuidAsync(requestModel.TypeGUID);
            if (selectedStructureModel != null)
            {
                var model = new MeterialModel
                {
                    UniqueGUID = Guid.NewGuid(),
                    ModelID = selectedStructureModel.ModelID,
                    MaterialID = selectedStructureModel.MaterialID,
                    LatestRev = selectedStructureModel.LatestRev,
                    MaterialType = selectedStructureModel.MaterialType,
                    ManufacturerID = selectedStructureModel.ManufacturerID,
                    ParentGUID = requestModel.UniqueGUID,
                    TemplateGUID = requestModel.TypeGUID,
                    Comments = selectedStructureModel.Comments,
                    CreatedBy = "",
                    CreatedDate = DateTime.Now,
                    LastSavedBy = "",
                    LastSavedDate = DateTime.Now,
                };
                await _meterialService.CreateMaterialAsync(model);
            }
            var result = await _structureRepository.CreateStructureAsync(requestModel);
            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = $"Structure ID  {requestModel.StructureID} added successfully",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }

            return new ToasterModel
            {
                IsError = true,
                Message = $"Failed to add structure ID {requestModel.StructureID} .",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }

        public async Task<ToasterModel> UpdateStructureAsync(StructureModel structureModel)
        {
            var insertId = await _structureRepository.CreateStructureAsync(structureModel);
            if (insertId > 0)
            {
                _ = _structureRepository.DeleteStructureByUniqueGuidAsync(structureModel);
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Structure updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else if (insertId == -1)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = $"A Structure with the ID {structureModel.StructureID} already exists.",
                    Type = ToasterType.warning.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to update Structure.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public async Task<List<MeterialModel>> GetStructureModelsByTypeAsync(string meterialType, Guid parentGuid, Guid templateGuid, bool isAddEdit)
        {
            return await _structureRepository.GetStructureModelsByTypeAsync(meterialType, parentGuid, templateGuid, isAddEdit);
        }

        public async Task<StructureModel?> GetStructureAsync(Guid uniqueGuid)
        {
            return await _structureRepository.GetStructureAsync(uniqueGuid);
        }

        public async Task<List<StructureModel>> GetStructureIdsAsync(Guid campusGuid, Guid buildingGuid, bool loadBuilding, bool loadAllStructures)
        {
            return await _structureRepository.GetStructureIdsAsync(campusGuid, buildingGuid, loadBuilding, loadAllStructures);
        }

        public async Task<StructureModel?> GetStructureByIdAsync(int id)
        {
            return await _structureRepository.GetStructureByIdAsync(id);
        }

        public async Task<ToasterModel> DeleteStructureByUniqueGuidAsync(StructureModel buildingModel)
        {
            buildingModel.LastSavedBy = "LastSavedBy";
            buildingModel.LastSavedDate = DateTime.UtcNow;
            var result = await _structureRepository.DeleteStructureByUniqueGuidAsync(buildingModel);

            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Structure deleted successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to delete Structure.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }

        public async Task<StructureModel?> CheckStructureExistsAsync(string buildingId)
        {
            return await _structureRepository.CheckStructureExistsAsync(buildingId);
        }

    }
}
