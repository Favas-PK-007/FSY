using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICampusService _campusService;
        public DashboardController(ICampusService campusService)
        {
            _campusService = campusService;
        }
        public async Task<IActionResult> Index()
        {
            var campuses = await _campusService.GetAllAsync();
            var model = new DashboardViewModel
            {
                Campuses = campuses.Select(x => CampusViewModel.FromModel(x)).ToList()
            };
            return View(model);
        }
    }
}
