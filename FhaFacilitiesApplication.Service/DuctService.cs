#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class DuctService : IDuctService
    {
        #region Declarations
        private readonly IDuctRepository _ductRepository;
        private readonly IStructureService _structureService;
        private readonly IMeterialService _materialService;

        #endregion

        #region Constructor
        public DuctService(IDuctRepository ductRepository, IStructureService structureService, 
            IMeterialService meterialService)
        {
            _ductRepository = ductRepository;
            _structureService = structureService;
            _materialService = meterialService;
        }
        #endregion


        public async Task<List<DuctModel>> GetDuctsAsync(Guid conduitGuid, bool loadSubDucts)
        {
            return await _ductRepository.GetDuctsAsync(conduitGuid, loadSubDucts);
        }

        public async Task<int> CreateDuctAsync(DuctModel requestModel)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null.");
            }
            // Validate the request model here if needed
            return await _ductRepository.CreateDuctAsync(requestModel);

        }

        public async Task<DuctModel?> GetDuctByGuidAsync(Guid ductGuid, bool includeSubDucts, string mode)
        {
            // Fetch the main duct by its GUID
            var duct = await _ductRepository.GetDuctByGuidAsync(ductGuid);
            if (duct == null)
                return null;

            if (mode?.ToLower() == "delete")
            {
                return new DuctModel
                {
                    UniqueID = duct.UniqueID,
                    UniqueGUID = duct.UniqueGUID,
                    TypeGUID = duct.TypeGUID,
                    StructureGUID = duct.StructureGUID,
                    ConduitGUID = duct.ConduitGUID,
                    DuctID = duct.DuctID,
                    DuctID_B = duct.DuctID_B,
                    Comments = duct.Comments,
                    LatestRev = duct.LatestRev,
                    CreatedBy = duct.CreatedBy,
                    CreatedDate = duct.CreatedDate,
                    LastSavedBy = duct.LastSavedBy,
                    LastSavedDate = duct.LastSavedDate
                };
            }

            // Optionally load sub-ducts
            if (includeSubDucts)
            {
                duct.SubDucts = await _ductRepository.GetDuctsAsync(ductGuid, false);
            }

            // Load associated structure details
            duct.Structure = await _structureService.GetStructureAsync(duct.StructureGUID);

            // Load associated material details
            duct.Material = await _materialService.GetMeterialsAsync(duct.UniqueGUID, duct.TypeGUID, false, false);

            return duct;
        }

        public async Task<ToasterModel<Guid>> SaveDuctAsync(DuctModel ductModel)
        {
            var affectedRows = await _ductRepository.CreateDuctAsync(ductModel);

            if (affectedRows > 0)
            {
                var selectedDuctType = await _materialService.GetMaterialByGuidAsync(ductModel.TypeGUID);
                if (selectedDuctType == null)
                {
                    return new ToasterModel<Guid>
                    {
                        IsError = true,
                        Message = "Invalid duct type selected.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }

                var material = new MeterialModel
                {
                    UniqueID = ductModel.UniqueID,
                    UniqueGUID = ductModel.MeterialGuid,
                    TemplateGUID = ductModel.TypeGUID,
                    ParentGUID = ductModel.UniqueGUID,
                    Comments = selectedDuctType.Comments,
                    CreatedBy = ductModel.CreatedBy,
                    LastSavedBy = ductModel.LastSavedBy,
                    MaterialType = selectedDuctType.MaterialType,
                    MaterialID = selectedDuctType.MaterialID,
                    ManufacturerID = selectedDuctType.ManufacturerID,
                    ModelID = selectedDuctType.ModelID,

                };

                var saved = await _materialService.CreateMaterialAsync(material);

                if (saved <= 0)
                {
                    return new ToasterModel<Guid>
                    {
                        IsError = true,
                        Message = "Failed to create duct.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }

                return new ToasterModel<Guid>
                {
                    IsError = false,
                    Message = "Duct added successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Response = ductModel.UniqueGUID
                };
            }
            else
            {
                return new ToasterModel<Guid>
                {
                    IsError = true,
                    Message = "Failed to add duct.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
        }

        public async Task<List<MaterialTypeModel>> GetDuctTypesAsync(bool addEdit, string materialType, Guid parentGuid, Guid templateGuid)
        {
            var ductTypes = await _materialService.GetStructureModels(addEdit, materialType, parentGuid, templateGuid);

            var selectList = ductTypes.Select(x => new MaterialTypeModel
            {
                Text = x.ModelID,            // Display text
                Value = x.UniqueGUID        // Option value
            }).ToList();

            return selectList;
        }


        public async Task<ToasterModel<Guid>> UpdateDuctAsync(DuctModel requestModel)
        {
            var affectedRows = await DeleteDuctAsync(requestModel);
            if (!affectedRows.IsError)
            {
               var duct = await SaveDuctAsync(requestModel);
                if (!duct.IsError)
                {
                    return new ToasterModel<Guid>
                    {
                        IsError = false,
                        Message = "Duct updated successfully.",
                        Type = ToasterType.success.ToString(),
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                return new ToasterModel<Guid>
                {
                    IsError = true,
                    Message = "Duct updated failed.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            return new ToasterModel<Guid>
            {
                IsError = true,
                Message = "Failed to update duct.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public async Task<ToasterModel> DeleteDuctAsync(DuctModel ductModel)
        {
            var isDeleted = await _ductRepository.DeleteDuctAsync(ductModel);

            if (!isDeleted)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to delete duct.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var materialModel = new MeterialModel
            {
                ParentGUID = ductModel.UniqueGUID,
                LastSavedBy = ductModel.LastSavedBy
            };

            var deletedMaterialCount = await _materialService.DeleteMaterialAsync(materialModel);

            if (deletedMaterialCount <= 0)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to delete subduct.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            return new ToasterModel
            {
                IsError = false,
                Message = "Duct deleted successfully.",
                Type = ToasterType.success.ToString(),
                StatusCode = HttpStatusCode.OK
            };
        }


        //public async Task<List<DuctModel>> GetCableDuctBindingAsync(Guid cableGuid, Guid structureGuid)
        //{
        //    var result = new List<DuctModel>();

        //    if (cableGuid == Guid.Empty)
        //        return result;

        //    var cable = await _cableRepository.GetCableByGuidAsync(cableGuid, null);
        //    if (cable == null)
        //        return result;

        //    var structureGuidStr = structureGuid != Guid.Empty ? structureGuid.ToString() : Guid.Empty.ToString();
        //    var conduits = await _conduitPathRepository.GetConduitsAsync(cable.CampusGUID.ToString(), structureGuidStr, true);

        //    foreach (var conduit in conduits)
        //    {
        //        if (conduit.Ducts == null)
        //            continue;

        //        foreach (var duct in conduit.Ducts)
        //        {
        //            var isDuctSelected = duct.UniqueGUID == cable.DuctGUID;

        //            // If duct has subducts, check if any match
        //            if (duct.SubDucts != null && duct.SubDucts.Count > 0)
        //            {
        //                foreach (var subduct in duct.SubDucts)
        //                {
        //                    if (subduct.UniqueGUID == cable.DuctGUID)
        //                    {
        //                        subduct.LatestRev = true; // mark selected
        //                        isDuctSelected = false;
        //                    }
        //                }
        //            }

        //            duct.LatestRev = isDuctSelected; // Mark only if not a subduct match
        //            result.Add(duct);
        //        }
        //    }

        //    return result;
        //}
    }
}
