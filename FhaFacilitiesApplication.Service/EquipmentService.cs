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

namespace FhaFacilitiesApplication.Service
{
    public class EquipmentService : IEquipmentService
    {
        #region Declarations

        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMeterialRepository _materialRepository;
        #endregion


        #region Constructor

        public EquipmentService(IEquipmentRepository equipmentRepository, IMeterialRepository materialRepository)
        {
            _equipmentRepository = equipmentRepository;
            _materialRepository = materialRepository;
        }
        #endregion

        public async Task<Dictionary<int, EquipmentModel>> GetInstalledEquipmentAsync(Guid structureGuid, Guid spliceGuid)
        {
            return await _equipmentRepository.GetInstalledEquipmentAsync(structureGuid, spliceGuid);
        }

        public async Task<List<string>> GetEquipmentTypesAsync()
        {
            return await _equipmentRepository.GetEquipmentTypesAsync();
        }

        public async Task<List<MeterialModel>> GetEquipmentModelAsync(string equipmentType)
        {
            return await _equipmentRepository.GetEquipmentModelAsync(equipmentType);
        }

        public async Task<Dictionary<int, string>> GetAvailableEquipmentPortAsync(Guid equimentStructureGuid, Guid equipmentSpliceGuid, Guid equipmentTypeGuid, string selectedEquipmentld)
        {
            return await _equipmentRepository.GetAvailableEquipmentPortAsync(equimentStructureGuid, equipmentSpliceGuid, equipmentTypeGuid,
                selectedEquipmentld);
        }

        public async Task<bool> IsEquipmentExistAsync(string equipmentId, Guid equipmentStructureGuid, Guid equipmentSpliceGuid)
        {
            return await _equipmentRepository.IsEquipmentExistAsync(equipmentId, equipmentStructureGuid, equipmentSpliceGuid);
        }

        public async Task<ToasterModel> SaveNewEquipmentMaterialAsync(EquipmentModel equipmentModel)
        {
            var result = new ToasterModel();
            var materialData = await _materialRepository.GetMaterialByGuidAsync(equipmentModel.TypeGUID);
            var equipmentMaterialModel = new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                ParentGUID = equipmentModel.UniqueGUID,
                TemplateGUID = equipmentModel.TypeGUID,
                MaterialType = materialData.MaterialType,
                MaterialID = materialData.MaterialID,
                ManufacturerID = materialData.ManufacturerID,
                ModelID = materialData.ModelID,
                Comments = materialData.Comments,
                CreatedBy = equipmentModel.CreatedBy,
                LastSavedBy = equipmentModel.LastSavedBy,
            };

            var inserted = await _materialRepository.SaveNewDuctMaterialAsync(equipmentMaterialModel);

            if (inserted > 0)
            {
                result.IsError = false;
                result.Message = "Equipment Material saved successfully.";
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Type = ToasterType.success.ToString();
            }
            else
            {
                result.IsError = true;
                result.Message = "Failed to save Equipment Material.";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Type = ToasterType.fail.ToString();
            }
            return result;
        }

        public async Task<ToasterModel> SaveEquipmentPortsAsync(bool newEquipment, EquipmentModel model)
        {
            if (!newEquipment)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "New equipment flag must be true.",
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Type = ToasterType.fail.ToString()
                };
            }

            var equipmentId = await _equipmentRepository.SaveEquipmentAsync(model);
            if (equipmentId <= 0)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to save Equipment.",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Type = ToasterType.fail.ToString()
                };
            }

            var equipmentModelData = await _equipmentRepository.GetEquipmentModelAsync(model.EquipmentType);

            foreach (var detailDict in equipmentModelData.Select(x => x.Details))
            {
                foreach (var kvp in detailDict)
                {
                    if (kvp.Key.Length > 3 && kvp.Key.Substring(0, 4).Equals("Port", StringComparison.OrdinalIgnoreCase))
                    {
                        var portId = $"{kvp.Value} ({kvp.Key})";

                        if (!string.Equals(portId, model.PortID, StringComparison.OrdinalIgnoreCase))
                        {
                            var newModel = new EquipmentModel
                            {
                                EquipmentType = model.EquipmentType,
                                UniqueGUID = model.UniqueGUID,
                                UniqueID = model.UniqueID,
                                StructureGUID = model.StructureGUID,
                                SpliceGUID = model.SpliceGUID,
                                LastSavedBy = model.LastSavedBy,
                                Comments = model.Comments,
                                PortID = portId,
                                PortGUID = Guid.NewGuid(),
                                FiberA_GUID = Guid.Empty,
                                LatestRev = true
                            };

                            await _equipmentRepository.SaveEquipmentAsync(newModel);
                        }
                    }
                }
            }

            return new ToasterModel
            {
                IsError = false,
                Message = "Equipment saved successfully.",
                StatusCode = System.Net.HttpStatusCode.OK,
                Type = ToasterType.success.ToString()
            };
        }

        public async Task<EquipmentModel?> GerEquipmentByPortAndGuid(Guid portGuid, Guid uniqueGuid)
        {
            return await _equipmentRepository.GerEquipmentByPortAndGuid(portGuid, uniqueGuid);
        }

        public async Task<EquipmentModel?> GetEquipmentAsync(Guid uniqueGuid, Guid portGuid)
        {
            return await _equipmentRepository.GetEquipmentAsync(uniqueGuid, portGuid);
        }

    }
}
