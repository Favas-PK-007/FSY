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
using System.Web.WebPages.Html;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class MeterialService : IMeterialService
    {
        #region Declarations
        private readonly IMeterialRepository _meterialRepository;
        private readonly IMaterialDetailService _materialDetailService;
        private readonly IComponentService _componentService;
        #endregion



        #region Constructor
        public MeterialService(IMeterialRepository meterialRepository, IMaterialDetailService materialDetailService, IComponentService componentService)
        {
            _meterialRepository = meterialRepository;
            _materialDetailService = materialDetailService;
            _componentService = componentService;
        }
        #endregion

        public async Task<MeterialModel> GetMeterialsAsync(Guid parentGuid, Guid templateGuid, bool loadDetails, bool loadComponents)
        {
            return await _meterialRepository.GetMeterialsAsync(parentGuid, templateGuid, loadDetails, loadComponents) ?? new MeterialModel();
        }

        public async Task<int> SaveNewDuctMaterialAsync(Guid uniqueGuid, Guid parentGuid, bool loadData, bool loadDetails, bool loadComponents)
        {
            var templateMeterial = await _meterialRepository.GetTemplateMeterialAsync(uniqueGuid, parentGuid);

            //if (loadData)
            //{
            //    var typeGuidToUse = templateMeterial.TemplateGUID == Guid.Empty.ToString()
            //                        ? templateMeterial.UniqueGUID : templateMeterial.TemplateGUID;

            //    var loadDataDetails = await _materialDetailService.GetMaterialDetailsAsync(templateMeterial.MaterialType,templateMeterial.UniqueGUID, Guid.Empty.ToString());
            //}

            //  Create new material based on template
            var newMaterial = new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                // ParentGUID = ductGuid,
                // TemplateGUID = typeGuid,
                MaterialType = templateMeterial.MaterialType,
                MaterialID = templateMeterial.MaterialID,
                ManufacturerID = templateMeterial.ManufacturerID,
                ModelID = templateMeterial.ModelID,
                Comments = templateMeterial.Comments,
                CreatedBy = "createdBy",
                CreatedDate = DateTime.UtcNow,
                LastSavedBy = "createdBy",
                LastSavedDate = DateTime.UtcNow,
                LatestRev = true
            };


            return await _meterialRepository.SaveNewDuctMaterialAsync(newMaterial);
        }

        public async Task<List<MeterialModel>> GetStructureModels(bool addEdit, string materialType, Guid parentGuid, Guid templateGuid)
        {
            return await _meterialRepository.GetStructureModels(addEdit, materialType, parentGuid, templateGuid);
        }

        public async Task<int> CreateMaterialAsync(MeterialModel materialModel)
        {
            return await _meterialRepository.SaveNewDuctMaterialAsync(materialModel);
        }

        public async Task<MeterialModel?> GetMaterialByGuidAsync(Guid uniqueGuid)
        {
            return await _meterialRepository.GetMaterialByGuidAsync(uniqueGuid);
        }

        public async Task<MeterialModel?> GetMaterialByParentGuidAsync(Guid parentGuid)
        {
            return await _meterialRepository.GetMaterialByParentGuidAsync(parentGuid);
        }

        public async Task<List<MeterialModel>> GetSubDuctsAsync(Guid parentGuid, Guid templateGuid)
        {
            return await _meterialRepository.GetSubDuctsAsync(parentGuid, templateGuid);
        }

        public async Task<int> DeleteMaterialAsync(MeterialModel model)
        {
            return await _meterialRepository.DeleteMaterialAsync(model);

        }

        public async Task<List<MeterialModel>> GetComponentModelDropdownAsync(string materialType, Guid modelGuid, Guid templateGuid)
        {
            return await GetStructureModels(true, materialType, modelGuid, templateGuid);
        }

        //public async Task<List<ComponentTreeNode>> GetComponentTreeAsync(Guid parentGuid)
        //{
        //    var rootMaterial = await _meterialRepository.GetMaterialByGuidAsync(parentGuid);

        //    if (rootMaterial == null)
        //        return new List<ComponentTreeNode>();

        //    await LoadComponentHierarchyAsync(rootMaterial); // Load children recursively

        //    var rootNodes = new List<ComponentTreeNode>();

        //    foreach (var tray in rootMaterial.Components)
        //    {
        //        var trayNode = new ComponentTreeNode
        //        {
        //            ComponentId = tray.UniqueGUID,
        //            ComponentName = tray.ToString(),
        //            Children = tray.Components.Select(module => new ComponentTreeNode
        //            {
        //                ComponentId = module.UniqueGUID,
        //                ComponentName = module.ToString(),
        //            }).ToList()
        //        };

        //        rootNodes.Add(trayNode);
        //    }

        //    return rootNodes;
        //}


        public async Task<MeterialModel> GetEquipmentModelAsync(Guid parentGuid, Guid templateGuid)
        {
            var data = await _meterialRepository.GetTemplateMeterialAsync(parentGuid, templateGuid);

            return data;

        }

        public async Task<List<string>> GetMaterialTypesAsync()
        {
            return await _meterialRepository.GetMaterialTypesAsync();
        }

        public async Task<List<DropdownModel>> GetParentMaterialDropdownAsync(string materialType)
        {
            var parentMaterials = await _meterialRepository.GetParentMaterialsAsync(materialType);
            var result = new List<DropdownModel>();

            foreach (var m in parentMaterials)
            {
                string label;

                if (string.IsNullOrEmpty(m.ParentGUID.ToString()) || m.ParentGUID.ToString() == Guid.Empty.ToString())
                {
                    label = m.ToString();
                }
                else
                {
                    label = m.ToString();
                }

                result.Add(new DropdownModel
                {
                    Value = m.UniqueGUID,
                    Text = label
                });
            }

            return result;
        }

        public async Task<List<DropdownModel>> GetChildMaterialsAsync(Guid parentGuid, string materialType)
        {
            var modelIdList = await _meterialRepository.GetChildMaterialsAsync(parentGuid, materialType);
            var result = new List<DropdownModel>();
            foreach (var m in modelIdList)
            {
                result.Add(new DropdownModel
                {
                    Value = m.UniqueGUID,
                    Text = m.ToString()
                });
            }
            return result;
        }


        public async Task<List<string>> GetManufacturersAsync(string materialType)
        {
            return await _meterialRepository.GetMaterialManufacturersAsync(materialType);
        }

        public async Task<List<string>> GetDetailHeadersAsync(string materialType)
        {
            return await _meterialRepository.GetMaterialDetailHeadersAsync(materialType);
        }

        public async Task<bool> CheckIfMaterialExistsAsync(MeterialModel model)
        {
            return await _meterialRepository.CheckMaterialExistAsync(model.ModelID, model.MaterialType, model.ManufacturerID);
        }

        public async Task<ToasterModel> SaveMaterialAsync(MeterialModel model)
        {
            try
            {
                var insertResult = await _meterialRepository.SaveNewDuctMaterialAsync(model);

                if (insertResult <= 0)
                {
                    return new ToasterModel
                    {
                        IsError = true,
                        Message = "Failed to save material.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                // Now try saving material details
                var detailResult = await _materialDetailService.SaveMaterialDetailsAsync(model);

                if (detailResult <= 0)
                {
                    return new ToasterModel
                    {
                        IsError = true,
                        Message = "Material saved, but failed to save material details.",
                        Type = ToasterType.warning.ToString(),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

                return new ToasterModel
                {
                    IsError = false,
                    Message = "New material saved successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ToasterModel> UpdateMaterialAsync(MeterialModel model)
        {
            try
            {
                var updateResult = await _meterialRepository.UpdateMaterialAsync(model);
                if (!updateResult)
                {
                    return new ToasterModel
                    {
                        IsError = true,
                        Message = "Failed to update material.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }

               var newMaterial = await _meterialRepository.SaveNewDuctMaterialAsync(model);

                // Now try updating material details
                var detailResult = await _materialDetailService.SaveMaterialDetailsAsync(model);
                if (detailResult <= 0)
                {
                    return new ToasterModel
                    {
                        IsError = true,
                        Message = "Material updated, but failed to save material details.",
                        Type = ToasterType.warning.ToString(),
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Material updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
