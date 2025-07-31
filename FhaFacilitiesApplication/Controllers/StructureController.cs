#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class StructureController : Controller
    {
        #region Declarations
        private readonly IStructureService _structureService;
        private readonly IBuildingService _buildingService;
        private readonly IMeterialService _materialService;
        #endregion

        #region Constructor
        public StructureController(IStructureService structureService, IBuildingService buildingService, IMeterialService materialService)
        {
            _structureService = structureService;
            _buildingService = buildingService;
            _materialService = materialService;
        }
        #endregion

        #region Get Structures by CampusGuid and BuildingGuid
        [HttpGet]
        public async Task<IActionResult> _GetStructure(string campusGuid, string buildingGuid)
        {
            var model = new DashboardViewModel();
            try
            {
                if (string.IsNullOrEmpty(campusGuid) && string.IsNullOrEmpty(buildingGuid))
                    return PartialView(model);

                var structures = await _structureService.GetStructureByCampusAndBuildingAsync(campusGuid, buildingGuid);
                model.Structures = structures?.Select(StructureViewModel.FromModel).ToList() ?? new List<StructureViewModel>();
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving structure: " + ex.Message);
            }
        }
        #endregion

        #region Create/Edit Structure Modal
        [HttpGet]
        public async Task<IActionResult> _StructureCreate(int uniqueId, Guid campusGuid)
        {
            var model = new DashboardViewModel();
            var buildings = await _buildingService.GetBuildingsAsync(campusGuid, true);

            var buildingOptions = buildings.Select(b => new SelectListItem
            {
                Text = b.BuildingID,
                Value = b.UniqueGUID.ToString()
            }).ToList();

            var structureTypeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Structure Type", Value = "" },
                new SelectListItem { Text = "Manhole", Value = "Manhole" },
                new SelectListItem { Text = "Service Vault", Value = "Service Vault" },
                new SelectListItem { Text = "PullBox", Value = "PullBox" },
                new SelectListItem { Text = "Room", Value = "Room" }
            };

            var locationDescList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Location Description", Value = "" },
                new SelectListItem { Text = "Street", Value = "Street" },
                new SelectListItem { Text = "Building", Value = "Building" },
                new SelectListItem { Text = "Room", Value = "Room" },
                new SelectListItem { Text = "Hallway", Value = "Hallway" }
            };

            if (uniqueId > 0)
            {
                var structure = await _structureService.GetStructureByIdAsync(uniqueId);
                if (structure != null)
                {
                    var structureModel = await _materialService.GetMeterialsAsync(structure.UniqueGUID, structure.TypeGUID, false, false);
                    model.StructureViewModel = StructureViewModel.FromModel(structure);
                    model.BuildingViewModel = BuildingViewModel.FromModel(structure.Building);
                    model.StructureViewModel.StructureTypeList = structureTypeList;
                    model.StructureViewModel.LocationDescriptionList = locationDescList;
                    model.StructureViewModel.BuildingOptions = buildingOptions;
                    model.StructureViewModel.UniqueId = uniqueId;
                    model.StructureViewModel.StructureModelList = new List<SelectListItem>
                    {
                        new SelectListItem
                        {
                            Value = structureModel.UniqueGUID.ToString(),
                            Text = structureModel.ModelID
                        }
                    };
                }
                else
                {
                    model.StructureViewModel = new StructureViewModel();
                }
            }
            else
            {
                model.StructureViewModel = new StructureViewModel
                {
                    CampusGuid = campusGuid,
                    StructureTypeList = structureTypeList,
                    LocationDescriptionList = locationDescList,
                    BuildingOptions = buildingOptions
                };
            }

            return PartialView(model);
        }
        #endregion

        #region Save Structure (Add/Update)
        [HttpPost]
        public async Task<IActionResult> SaveStructure([FromBody] StructureViewModel requestModel)
        {
            var result = new ToasterModel();
            if (ModelState.IsValid)
            {
                var model = requestModel.ToModel();
                model.CreatedBy = "";
                model.LastSavedBy = "";
                if (requestModel.UniqueId > 0)
                {
                    result = await _structureService.UpdateStructureAsync(model);
                }
                else
                {
                    result = await _structureService.CreateStructureAsync(model);
                }
                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }
            var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
            return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
        }
        #endregion

        #region Delete Structure
        [HttpPost]
        public async Task<IActionResult> DeleteStructure([FromBody] UniqueIDUniqueGUIDRequest request)
        {
            var structureModel = await _structureService.CheckStructureExistsAsync(request.UniqueID.ToString());
            if (structureModel == null)
            {
                return NotFound($"No structure found with structure Id: {request.UniqueID}");
            }
            var result = await _structureService.DeleteStructureByUniqueGuidAsync(structureModel);
            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
        #endregion

        #region Get Structure Models For Add/Edit Modal
        [HttpGet]
        public async Task<IActionResult> GetStructureModelByType(string structureType)
        {
            try
            {
                var models = await _materialService.GetStructureModels(true, structureType, Guid.Empty, Guid.Empty);
                var modelList = models.Select(m => new SelectListItem
                {
                    Value = m.UniqueGUID.ToString(),
                    Text = m.ModelID
                }).ToList();
                return Json(modelList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving structure models: " + ex.Message);
            }
        }
        #endregion

        #region Get Building IDs for Structure Modal
        [HttpGet]
        public async Task<IActionResult> GetBuildingIds(Guid campusGuid)
        {
            var buildings = await _buildingService.GetBuildingsAsync(campusGuid, true);
            var buildingOptions = buildings.Select(b => new SelectListItem
            {
                Text = b.BuildingID,
                Value = b.UniqueGUID.ToString()
            }).ToList();

            return Ok(buildingOptions);
        }
        #endregion
    }
}
