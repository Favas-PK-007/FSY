#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class ConduitPathService : IConduitPathService
    {
        #region Declarations
        private readonly IConduitPathRepository _conduitPathRepository;
        private readonly IDuctService _ductService;
        private readonly IMeterialService _materialService;
        #endregion

        #region Constructor
        public ConduitPathService(IConduitPathRepository conduitPathRepository, IDuctService ductService, IMeterialService meterialService)
        {
            // Initialize any dependencies if needed
            _conduitPathRepository = conduitPathRepository;
            _ductService = ductService;
            _materialService = meterialService;
        }
        #endregion

        public async Task<List<ConduitModel>> GetConduitsAsync(string campusGuid, string structureGuid, bool loadDucts)
        {
            return await _conduitPathRepository.GetConduitsAsync(campusGuid, structureGuid, loadDucts);
        }

        public async Task<ToasterModel> CreateConduitAsync(ConduitModel requestModel)
        {
            var isExists = await _conduitPathRepository.CheckConduitPathExists(requestModel);

            if (isExists)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Conduit already exists.",
                    Type = ToasterType.warning.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Conflict
                };
            }

            // Save Ducts first (if any)
            if (requestModel.Ducts != null && requestModel.Ducts.Any())
            {
                foreach (var duct in requestModel.Ducts)
                {
                    if (string.IsNullOrEmpty(duct.UniqueGUID.ToString()))
                        duct.UniqueGUID = Guid.NewGuid();
                    duct.ConduitGUID = requestModel.UniqueGUID; // assign parent conduit
                    duct.CreatedBy = requestModel.CreatedBy;
                    duct.CreatedDate = DateTime.UtcNow;
                    duct.LastSavedBy = requestModel.CreatedBy;
                    duct.LastSavedDate = DateTime.UtcNow;
                    await _ductService.CreateDuctAsync(duct);


                    // Save Duct Material from TypeGUID (template)
                    await _materialService.SaveNewDuctMaterialAsync(duct.TypeGUID, Guid.NewGuid(), true, false, false);
                }
            }
            var saveConduit = await _conduitPathRepository.CreateConduitAsync(requestModel);
            if (saveConduit > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Conduit created successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Created
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to create Conduit.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }

        public async Task<ToasterModel> UpdateConduitPathAsync(ConduitModel requestModel)
        {
            var updateResult = await _conduitPathRepository.DeleteConduitPathAsync(requestModel);

            if (updateResult > 0)
            {
                var createResult = await _conduitPathRepository.CreateConduitAsync(requestModel);

                if (createResult > 0)
                {
                    return new ToasterModel
                    {
                        IsError = false,
                        Message = "Conduit updated successfully.",
                        Type = ToasterType.success.ToString(),
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }

                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to update conduit.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            else
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to update conduit.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }


        public async Task<ConduitModel?> GetConduitByUniqueGuid(Guid conduitGuid)
        {
            return await _conduitPathRepository.GetConduitByUniqueGuid(conduitGuid);
        }

        public async Task<ConduitModel?> GetConduitByUniqueId(int conduitId)
        {
            return await _conduitPathRepository.GetConduitByUniqueId(conduitId);
        }

        public async Task<ToasterModel> DeleteConduitPathAsync(ConduitModel requestModel)
        {
            var updateResult = await _conduitPathRepository.DeleteConduitPathAsync(requestModel);

            if (updateResult > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Conduit updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }

            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to update conduit.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };

        }
    }
}
