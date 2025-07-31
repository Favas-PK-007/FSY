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
    public class CableService : ICableService
    {
        #region Declarations

        private readonly ICableRepository _cableRepository;
        private readonly IMeterialService _materialService;
        private readonly IConnectionRepository _connectionRepository;
        #endregion


        #region Constructor
        public CableService(ICableRepository cableRepository, IMeterialService materialService, IConnectionRepository connectionRepository)
        {
            _cableRepository = cableRepository;
            _materialService = materialService;
            _connectionRepository = connectionRepository;
        }
        #endregion

        public async Task<List<CableModel>> GetAllCablesAsync(Guid campusGuid, Guid spliceGuid, bool loadFibers)
        {
            return await _cableRepository.GetAllCablesAsync(campusGuid, spliceGuid, loadFibers);
        }

        public async Task<CableModel?> GetCableByGuidAsync(Guid cableGuid, string? action)
        {
            return await _cableRepository.GetCableByGuidAsync(cableGuid, action);
        }

        public async Task<List<CableTypeModel>> GetCableTypesAsync()
        {
            var cableTypes = new List<CableTypeModel>
            {
                new CableTypeModel { Text = "Outdoor", Value = "Outdoor" },
                new CableTypeModel { Text = "Indoor", Value = "Indoor" },
                new CableTypeModel { Text = "Transition", Value = "Transition" },
                new CableTypeModel { Text = "Stub", Value = "Stub" },
                new CableTypeModel { Text = "Patch", Value = "Patch" }
            };

            return await Task.FromResult(cableTypes);
        }

        public async Task<List<CableModel>> GetCableModelsAsync()
        {
            var cableModels = await _materialService.GetStructureModels(true, "Cable", new Guid(), new Guid());
            var cableModelList = cableModels.Select(cm => new CableModel
            {
                Text = cm.ToString(),
                Value = cm.UniqueGUID

            }).ToList();
            return cableModelList;
        }

        public async Task<ToasterModel> SaveCableAsync(CableModel cableModel)
        {
            var isExists = await _cableRepository.IsCableExistsAsync(cableModel.CableID, cableModel.CampusGUID);
            if (isExists)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = $"Cable ID {cableModel.CableID} already exists.",
                    Type = ToasterType.info.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            // Get cable model (material) data from existing catalog
            var cableModelData = await _materialService.GetMaterialByGuidAsync(cableModel.TypeGUID);
            if (cableModelData == null)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Invalid cable model selected.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            // Create a new material record linked to this cable
            var model = new MeterialModel
            {
                UniqueID = 0,
                UniqueGUID = Guid.NewGuid(),
                ParentGUID = cableModel.UniqueGUID,
                TemplateGUID = cableModel.TypeGUID,
                ManufacturerID = cableModelData.ManufacturerID,
                ModelID = cableModelData.ModelID,
                MaterialType = cableModelData.MaterialType,
                MaterialID = cableModelData.MaterialID,
                Comments = cableModel.Comments,
                LastSavedBy = cableModel.LastSavedBy,
                CreatedBy = cableModel.CreatedBy,
                LatestRev = true,
            };

            await _materialService.CreateMaterialAsync(model);

            var result = await _cableRepository.SaveCableAsync(cableModel);

            return result > 0
                ? new ToasterModel
                {
                    IsError = false,
                    Message = "Cable saved successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                }
                : new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to save cable.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
        }

        public async Task<ToasterModel> UpdateCableAsync(CableModel cableModel)
        {
            var cableMaterial = await _materialService.GetMaterialByGuidAsync(cableModel.TypeGUID);

            var cableMaterialModel = new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                ParentGUID = cableModel.UniqueGUID,
                TemplateGUID = cableModel.TypeGUID,
                ManufacturerID = cableMaterial?.ManufacturerID ?? string.Empty,
                ModelID = cableMaterial?.ModelID ?? string.Empty,
                MaterialType = cableMaterial?.MaterialType ?? string.Empty,
                MaterialID = cableMaterial?.MaterialID ?? string.Empty,
                Comments = cableModel.Comments,
                LastSavedBy = cableModel.LastSavedBy,
                CreatedBy = cableModel.CreatedBy,
                LatestRev = true
            };

            await _materialService.CreateMaterialAsync(cableMaterialModel);

            var result = await _cableRepository.UpdateCableAsync(cableModel);
            if (result > 0)
            {
                await _cableRepository.SaveCableAsync(cableModel);

                return new ToasterModel
                {
                    IsError = false,
                    Message = "Cable updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to update cable.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<List<FiberModel>> GenerateNewFiberAsync(CableModel cableModel, string cableType, int numOfBuffers, int quantityPerBuffer, int fibersPerRibbon, string fiberType, string userID)
        {
            return await _cableRepository.GenerateNewFiberAsync(cableModel, cableType, numOfBuffers, quantityPerBuffer, fibersPerRibbon, fiberType, userID);
        }

        public async Task<List<FiberModel>> GetFiberInCablesAsync(CableModel cableModel)
        {
            return await _cableRepository.GetFiberInCablesAsync(cableModel);
        }

        public async Task<ToasterModel> DeleteCableAsync(CableModel cableModel)
        {
            var result = await _cableRepository.UpdateCableAsync(cableModel);
            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Cable deleted successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            else
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to delete cable.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<(CableModel Cable, List<Guid> AvailableFiberGuids)> GetAvailableFibersForCableAsync(Guid cableGuid, Guid spliceGuid)
        {
            var UNPATCHED_GUID = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var EQUIPMENT_GUID = Guid.Parse("22222222-2222-2222-2222-222222222222");

            // Step 1: Fetch cable details from repository
            var cable = await _cableRepository.GetCableByGuidAsync(cableGuid, null);

            // Step 2: If cable is invalid, or represents a virtual port/equipment, return empty
            if (cable == null || cableGuid == UNPATCHED_GUID || cableGuid == EQUIPMENT_GUID)
                return (new CableModel(), new List<Guid>());

            // Step 3: Get all fibers associated with this cable
            var allFibers = await _cableRepository.GetFiberInCablesAsync(cable);

            // Assign fibers to cable model (even if empty)
            cable.Fiber = allFibers ?? new List<FiberModel>();

            // Step 4: Fetch existing fiber connections for this splice
            var connections = await _connectionRepository.GetConnectionsAsync(spliceGuid);

            // Step 5: Build a list of already-used fiber GUIDs
            var usedFiberGuids = connections
                .Where(c => c.LatestRev) // Only consider latest revision
                .SelectMany(c => new[] { c.FiberA_GUID, c.FiberB_GUID }) // Include both ends
                .Where(g => g.HasValue) // Exclude nulls
                .Select(g => g.Value)
                .ToList();

            // Step 6: Identify only the available (unused) fibers in the cable
            var availableFiberGuids = cable.Fiber
                .Where(f => !usedFiberGuids.Contains(f.UniqueGUID))
                .Select(f => f.UniqueGUID)
                .ToList();

            // Step 7: Return the full cable model + list of available fiber GUIDs
            return (cable, availableFiberGuids);
        }
    }
}
