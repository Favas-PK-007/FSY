#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class BuildingController : Controller
    {
        #region Declarations
        private readonly IBuildingService _buildingService;
        #endregion

        #region Constructor
        public BuildingController(IBuildingService buildingService)
        {
            _buildingService = buildingService;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }


        #region Create View

        [HttpGet]
        public async Task<IActionResult> _CreateBuilding(string uniqueId)
        {
            var model = new DashboardViewModel();
            if (!string.IsNullOrEmpty(uniqueId))
            {
                var building = await _buildingService.GetBuildingByUniqueIdAsync(uniqueId);
                if (building != null)
                {
                    model.BuildingViewModel = BuildingViewModel.FromModel(building);
                }
                else
                {
                    model.BuildingViewModel = new BuildingViewModel();
                }
            }
            else
            {
                model.BuildingViewModel = new BuildingViewModel();
            }
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBuilding([FromBody] BuildingViewModel request)
        {
            var result = new ToasterModel();
            if (ModelState.IsValid)
            {
                if (request.UniqueId > 0)
                {
                    result = await _buildingService.UpdateBuildingAsync(request.ToModel());
                }
                else
                {
                    result = await _buildingService.AddBuildingAsync(request.ToModel());
                }
                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }

            var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
            return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
        }
        #endregion

        #region Controller Partial

        [HttpGet]
        public async Task<IActionResult> _GetBuildings(Guid? campusGuid)
        {
            var model = new DashboardViewModel();
            if (campusGuid is null)
            {
                return PartialView(model);
            }
            var buildings = await _buildingService.GetBuildingsAsync((Guid)campusGuid, false);
            model = new DashboardViewModel
            {
                Buildings = buildings.Select(b => BuildingViewModel.FromModel(b)).ToList()
            };
            return PartialView(model);
        }
        #endregion

        #region Delete Building

        [HttpPost]
        public async Task<IActionResult> DeleteBuilding([FromBody] UniqueIDUniqueGUIDRequest request)
        {
            var buildingModel = await _buildingService.CheckBuildingExistsAsync(request.UniqueID.ToString());
            if (buildingModel == null)
            {
                return NotFound($"No building found with building Id: {request.UniqueID}");
            }
            var result = await _buildingService.DeleteBuildingByUniqueGuidAsync(buildingModel);
            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
        #endregion
    }
}
