#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class CampusController : Controller
    {
        #region Declarations
        private readonly ICampusService _campusService;
        #endregion

        #region Constructor
        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }
        #endregion

        #region Create View
        [HttpGet]
        public async Task<IActionResult> _CampusCreate(string? uniqueID)
        {
            var model = new DashboardViewModel();
            if (!string.IsNullOrEmpty(uniqueID))
            {
                var campus = await _campusService.GetCampusByUniqueID(uniqueID);
                if (campus != null)
                {
                    model.CampusViewModel = CampusViewModel.FromModel(campus);
                }
                else
                {
                    model.CampusViewModel = new CampusViewModel(); // return empty to avoid null reference
                }
            }
            return PartialView(model);
        }
        #endregion

        #region Create View
        [HttpGet]
        public async Task<IActionResult> _CampusController()
        {
            var campuses = await _campusService.GetAllAsync();
            var model = new DashboardViewModel
            {
                Campuses = campuses.Select(x => CampusViewModel.FromModel(x)).ToList()
            };

            return PartialView(model);
        }
        #endregion



        [HttpPost]
        public async Task<IActionResult> SaveCampus([FromBody] CampusViewModel request)
        {
            var result = new ToasterModel();
            if (ModelState.IsValid)
            {
                if (request.UniqueId > 0)
                {
                    result = await _campusService.UpdateCampusByIdAsync(request.ToModel(request.UniqueId));
                }
                else
                {
                    result = await _campusService.AddCampusAsync(request.ToModel());
                }
                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }

            var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);

            return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCampus([FromBody] UniqueIDUniqueGUIDRequest request)
        {
            var result = await _campusService.DeleteCampusByIdAsync(request.UniqueID, request.UniqueGUID);
            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
    }

}
