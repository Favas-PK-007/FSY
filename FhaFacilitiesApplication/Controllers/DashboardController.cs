#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class DashboardController : Controller
    {
        #region Declarations
        private readonly ILogger<DashboardController> _logger;
        private readonly ICampusService _campusService;
        #endregion

        #region Constructor
        public DashboardController(ILogger<DashboardController> logger, ICampusService campusService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _campusService = campusService ?? throw new ArgumentNullException(nameof(campusService));
        }
        #endregion

        /// <summary>
        /// Retrieves the list of campuses and returns the view with the campus list as the model.
        /// </summary>
        /// <returns>A View with the list of campuses.</returns>
        public async Task<IActionResult> GetCampus()
        {
            var campuses = await _campusService.GetCampusListAsync();
            return View(campuses); // Pass campus list to the view
        }

        #region AddCampus View
        public IActionResult AddCampus()
        {
            return View(); // Return the view for adding a new campus
        }
        #endregion

        #region AddCampus Post

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCampus(AddCampusViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Save to database (map to entity and call service/repo)
                var result = await _campusService.AddCampusAsync(model.ToModel());

                TempData["CampusMessage"] = result;

                // Return same view or redirect to the GetCampus view after insert
                return RedirectToAction("AddCampus");
            }

            return View(model); // return view with validation errors
            
        }
        #endregion


    }
}
