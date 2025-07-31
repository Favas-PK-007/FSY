using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Controllers
{
    public class ComponentController : Controller
    {
        private readonly IMeterialService _materialService;

        public ComponentController(IMeterialService materialService)
        {
            _materialService = materialService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> _CreateComponent(Guid? parentMaterialGuid, string? parentMaterialType, int? selectedNodeLevel)
        {
            //if (!parentMaterialGuid.HasValue || parentMaterialGuid == Guid.Empty)
            //{
            //    ModelState.AddModelError("ParentMaterialGuid", "Parent material GUID is required.");
            //    return PartialView("ComponentForm");
            //}

            //if (string.IsNullOrWhiteSpace(parentMaterialType))
            //{
            //    ModelState.AddModelError("ParentMaterialType", "Parent material type is required.");
            //    return PartialView("ComponentForm");
            //}

            //var parentMaterial = await _materialService.GetMaterialByGuidAsync(parentMaterialGuid.Value);
            //if (parentMaterial == null)
            //{
            //    ModelState.AddModelError("ParentMaterial", "Parent material not found.");
            //    return PartialView("ComponentForm");
            //}

            //// Determine child material type based on level and parent material type
            //string childMaterialType = string.Empty;

            //switch (selectedNodeLevel)
            //{
            //    case 0: // Level 0: Splice or Chassis
            //        childMaterialType = parentMaterialType.ToLower() switch
            //        {
            //            "splice" => "Splice Tray",
            //            "fpp chassis" => "FPP Cartridge",
            //            _ => string.Empty
            //        };
            //        break;

            //    case 1: // Level 1: Tray or Cartridge
            //        childMaterialType = parentMaterialType.ToLower() switch
            //        {
            //            "splice" => "Splice Module",
            //            "fpp chassis" => "FPP Module",
            //            _ => string.Empty
            //        };
            //        break;

            //    default:
            //        ModelState.AddModelError("TreeLevel", "Invalid tree node level.");
            //        return PartialView("ComponentForm");
            //}

            //if (string.IsNullOrEmpty(childMaterialType))
            //{
            //    ModelState.AddModelError("ChildMaterialType", "Could not resolve child component type.");
            //    return PartialView("ComponentForm");
            //}

            //// Load available child components (type templates) for dropdown
            //var childTemplates = await _materialService.GetMeterialsAsync(
            //    loadLatestRev: true,
            //    materialType: childMaterialType,
            //    parentGuid: parentMaterial.UniqueGUID,
            //    templateGuid: parentMaterial.TemplateGUID
            //);

            //var componentTypeDropdown = childTemplates.Select(c => new SelectListItem
            //{
            //    Text = $"{c.ModelID} ({c.MaterialID})",
            //    Value = c.UniqueGUID.ToString()
            //}).ToList();

            //var viewModel = new ComponentViewModel
            //{
            //    ParentMaterialGUID = parentMaterial.UniqueGUID,
            //    ParentMaterialType = parentMaterial.MaterialType,
            //    AvailableComponentTemplates = componentTypeDropdown,
            //    Action = "Add",
            //    ChildMaterialType = childMaterialType
            //};

            return PartialView("ComponentForm");
        }

        [HttpPost]
        public async Task<IActionResult> CreateComponent(MeterialModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic to save the component
                // This could involve calling a service or repository to persist the data
                return PartialView("Index");
            }
            // If model state is invalid, return the same view with validation errors
            return PartialView(model);
        }
    }
}
