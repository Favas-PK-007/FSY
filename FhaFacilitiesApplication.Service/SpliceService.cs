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
    public class SpliceService : ISpliceService
    {
        #region Declaration
        private readonly ISpliceRepository _spliceRepository;
        private readonly IStructureService _structureService;
        private readonly IMeterialRepository _meterialRepository;
        #endregion


        #region Constructor
        public SpliceService(ISpliceRepository spliceRepository,IStructureService structureService, IMeterialRepository meterialRepository)
        {
            _spliceRepository = spliceRepository;
            _structureService = structureService;
            _meterialRepository = meterialRepository;
        }
        #endregion

        public async Task<List<SpliceModel>> GetSpliceByCampusAndStructureAsync(Guid campusGuid, Guid structureGuid, bool loadStructure)
        {
            return await _spliceRepository.GetSpliceByCampusAndStructureAsync(campusGuid, structureGuid, loadStructure);
        }

        public async Task<List<SpliceModel>> GetSplicesAsync(Guid campusGuid)
        {
            var spliceModels = await _spliceRepository.GetSpliceByCampusAndStructureAsync(campusGuid, Guid.Empty, false);
            if (spliceModels == null || spliceModels.Count == 0)
            {
                return new List<SpliceModel>();
            }
            var spliceModelList = spliceModels.Select(cm => new SpliceModel
            {
                UniqueID = cm.UniqueID,
                UniqueGUID = cm.UniqueGUID,
                CampusGUID = cm.CampusGUID,
                StructureGUID = cm.StructureGUID,
                SpliceType = cm.SpliceType,
                TypeGUID = cm.TypeGUID,
                SpliceID = cm.SpliceID

            }).ToList();
            return spliceModelList;

        }

        public async Task<SpliceModel?> GetSpliceByGuidAsync(Guid uniqueGuid, string? action)
        {
            var spliceData = await _spliceRepository.GetSpliceByGuidAsync(uniqueGuid);

            // Only load structure details if action is not "delete"
            if (!string.Equals(action, "delete", StringComparison.OrdinalIgnoreCase) && spliceData != null)
            {
                var structure = await _structureService.GetStructureAsync(spliceData.StructureGUID);
                spliceData.Structure = structure;
            }

            return spliceData;
        }

        public async Task<ToasterModel> CreateSpliceAsync(SpliceModel spliceModel)
        {
            var isExists = await _spliceRepository.IsSpliceExistsAsync(spliceModel.SpliceID, spliceModel.CampusGUID);
            if (isExists)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = $"Connection Enclose ID {spliceModel.SpliceID} already exists.",
                    Type = ToasterType.warning.ToString(),
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }

            var materialType = await _meterialRepository.GetMaterialByGuidAsync(spliceModel.TypeGUID);

            if (materialType == null)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Equipment Model not found in database.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }

            var materialModel = new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                MaterialType = materialType.MaterialType,
                MaterialID = materialType.MaterialID,
                ManufacturerID = materialType.ManufacturerID,
                ModelID = materialType.ModelID,
                TemplateGUID = spliceModel.TypeGUID,
                ParentGUID = spliceModel.UniqueGUID,
                Comments = materialType.Comments,
                CreatedBy = spliceModel.CreatedBy,
                CreatedDate = DateTime.Now,
                LastSavedBy = spliceModel.LastSavedBy,
                LastSavedDate = DateTime.Now,
                LatestRev = true
            };

            var result = await _meterialRepository.SaveNewDuctMaterialAsync(materialModel);

            if (result <= 0)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to save equipment model against new connection enclosure.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }

            //var bulkList = new List<MeterialModel>();

            //if (spliceModel.Components?.Any() == true)
            //{
            //    var index = 0;
            //    foreach (var comp in spliceModel.Components)
            //    {
            //        var compGuid = Guid.NewGuid();
            //        bulkList.Add(new MeterialModel
            //        {
            //            UniqueID = comp.UniqueID,
            //            UniqueGUID = Guid.NewGuid(),
            //            ParentGUID = materialModel.ParentGUID,
            //            TemplateGUID = materialModel.TemplateGUID,
            //            MaterialType = materialModel.MaterialType,
            //            MaterialID = (index + 1).ToString(),
            //            ManufacturerID = materialModel.ManufacturerID,
            //            ModelID = materialModel.ModelID,
            //            Comments = materialModel.Comments,
            //            Components= materialModel.Components,
            //            CreatedBy = spliceModel.CreatedBy,
            //            LastSavedBy = spliceModel.LastSavedBy,
            //            LastSavedDate = DateTime.UtcNow,
            //        });


            //        if (comp.Components?.Any() == true)
            //        {
            //            foreach (var sub in comp.Components)
            //            {
            //                bulkList.Add(new MeterialModel
            //                {
            //                    UniqueGUID = Guid.NewGuid(),
            //                    ParentGUID = compGuid,
            //                    TemplateGUID = sub.TemplateGUID,
            //                    MaterialType = sub.MaterialType,
            //                    MaterialID = (comp.Components.IndexOf(sub) + 1).ToString(),
            //                    ManufacturerID = sub.ManufacturerID,
            //                    ModelID = sub.ModelID,
            //                    Comments = sub.Comments,
            //                    CreatedBy = spliceModel.CreatedBy,
            //                    CreatedDate = DateTime.UtcNow,
            //                    LastSavedBy = spliceModel.LastSavedBy,
            //                    LastSavedDate = DateTime.UtcNow,
            //                    LatestRev = true
            //                });
            //            }
            //        }
            //    }
            //}

            //var bulkResult = await _meterialRepository.BulkInsertMaterialsAsync(bulkList);
            //{
            //    var allComponents = new List<(MeterialModel component, Guid parentGuid)>();

            //    foreach (var comp in material.Components)
            //    {
            //        allComponents.Add((comp, material.UniqueGUID));
            //    }

            //    int index = 1;
            //    for (int i = 0; i < allComponents.Count; i++)
            //    {
            //        var (component, parentGuid) = allComponents[i];

            //        component.UniqueGUID = Guid.NewGuid();
            //        component.ParentGUID = parentGuid;
            //        component.MaterialID = index.ToString();
            //        component.TemplateGUID = Guid.Empty;
            //        component.CreatedBy = spliceModel.CreatedBy;
            //        component.CreatedDate = DateTime.UtcNow;
            //        component.LastSavedBy = spliceModel.LastSavedBy;
            //        component.LastSavedDate = DateTime.UtcNow;
            //        component.LatestRev = true;

            //        await _meterialRepository.SaveNewDuctMaterialAsync(component);

            //        if (component.Components?.Any() == true)
            //        {
            //            foreach (var child in component.Components)
            //            {
            //                allComponents.Add((child, component.UniqueGUID));
            //            }
            //        }

            //        index++;
            //    }
            //}

            var newSplice = new SpliceModel
            {
                UniqueGUID = spliceModel.UniqueGUID,
                CampusGUID = spliceModel.CampusGUID,
                StructureGUID = spliceModel.StructureGUID,
                SpliceType = spliceModel.SpliceType,
                SpliceID = spliceModel.SpliceID,
                Comments = spliceModel.Comments,
                CreatedBy = spliceModel.CreatedBy,
                LastSavedBy = spliceModel.LastSavedBy,
                LatestRev = true,
                TypeGUID =spliceModel.TypeGUID
            };

            var saved = await _spliceRepository.CreateSpliceAsync(newSplice);
            if (saved > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Equipment created successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.Created
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to create equipment.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }


        public async Task<ToasterModel> UpdateSpliceAsync(SpliceModel model)
        {
            var selectedEquipmentModel = await _meterialRepository.GetMaterialByGuidAsync(model.TypeGUID);

            if (selectedEquipmentModel == null)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Equipment model not found in database.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.NotFound
                };
            }


            var materialModel = new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                MaterialType = selectedEquipmentModel.MaterialType,
                MaterialID = selectedEquipmentModel.MaterialID,
                ManufacturerID = selectedEquipmentModel.ManufacturerID,
                ModelID = selectedEquipmentModel.ModelID,
                TemplateGUID = model.TypeGUID,
                ParentGUID = model.UniqueGUID,
                Comments = selectedEquipmentModel.Comments,
                CreatedBy = selectedEquipmentModel.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                LastSavedBy = selectedEquipmentModel.LastSavedBy,
                LastSavedDate = DateTime.UtcNow,
                LatestRev = true
            };

            var result = await _meterialRepository.SaveNewDuctMaterialAsync(materialModel);
            if (result <= 0)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to save equipment model against splice.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }

            var updatedResult = await _spliceRepository.UpdateSpliceAsync(model);
            if (updatedResult <= 0)
            {
                return new ToasterModel
                {
                    IsError = true,
                    Message = "Failed to update equipment.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }

            var newSplice = new SpliceModel
            {
                UniqueGUID = model.UniqueGUID,
                CampusGUID = model.CampusGUID,
                StructureGUID = model.StructureGUID,
                SpliceType = model.SpliceType,
                SpliceID =  model.SpliceID,
                Comments =  model.Comments,
                CreatedBy = model.CreatedBy,
                LastSavedBy = model.LastSavedBy,
                LatestRev = true,
            };

            var saved = await _spliceRepository.CreateSpliceAsync(newSplice);
            if (saved > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Equipment updated successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to update equipment.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };

            //var parentComponent = model.EquipmentModel?.FirstOrDefault();

            //if (parentComponent is null)
            //{
            //    return new ToasterModel
            //    {
            //        IsError = true,
            //        Message = "Equipment model is null.",
            //        Type = ToasterType.fail.ToString(),
            //        StatusCode = System.Net.HttpStatusCode.BadRequest
            //    };
            //}

            //await SaveComponentsAsync(parentComponent, parentComponent.UniqueGUID, "Edit", "userId");

            //var result =await _spliceRepository.UpdateSpliceAsync(model, "userId");
            //if (result > 0)
            //{
            //    return new ToasterModel
            //    {
            //        IsError = false,
            //        Message = "Equipemt updated successfully.",
            //        Type = ToasterType.success.ToString(),
            //        StatusCode = System.Net.HttpStatusCode.OK
            //    };
            //}
            //return new ToasterModel
            //{
            //    IsError = true,
            //    Message = "Failed to update equipment.",
            //    Type = ToasterType.fail.ToString(),
            //    StatusCode = System.Net.HttpStatusCode.InternalServerError
            //};


        }

        public async Task<ToasterModel> DeleteSpliceAsync(SpliceModel model)
        {
            var result = await _spliceRepository.UpdateSpliceAsync(model);
            if (result > 0)
            {
                return new ToasterModel
                {
                    IsError = false,
                    Message = "Splice deleted successfully.",
                    Type = ToasterType.success.ToString(),
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            return new ToasterModel
            {
                IsError = true,
                Message = "Failed to delete splice.",
                Type = ToasterType.fail.ToString(),
                StatusCode = System.Net.HttpStatusCode.InternalServerError
            };
        }
        private async Task SaveComponentsAsync(MeterialModel parentComponent, Guid parentGuid, string mode, string userId)
        {
            if (parentComponent?.Components == null || !parentComponent.Components.Any())
                return;

            foreach (var (child, index) in parentComponent.Components.Select((c, i) => (c, i)))
            {
                var component = new MeterialModel
                {
                    UniqueID = child.UniqueID,
                    UniqueGUID = mode == "Edit" ? child.UniqueGUID : Guid.NewGuid(),
                    ParentGUID = parentGuid,
                    TemplateGUID = child.TemplateGUID,
                    MaterialType = child.MaterialType,
                    MaterialID = (index + 1).ToString(),
                    ManufacturerID = child.ManufacturerID,
                    ModelID = child.ModelID,
                    Comments = child.Comments,
                    Components = child.Components,
                    LatestRev = child.LatestRev
                };

                if (component.UniqueID == 0 && component.LatestRev)
                {
                    await _meterialRepository.SaveNewDuctMaterialAsync(component);
                    //await component.AddToDBAsync(userId);
                    //component.UniqueID = 0;
                }

                // Recurse for children
                await SaveComponentsAsync(component, component.UniqueGUID, mode, userId);
            }
        }


    }
}
