#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using FhaFacilitiesApplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class ConduitPathController : Controller
    {
        #region Declarations
        private readonly IConduitPathService _conduitPathService;
        private readonly ISpliceService _spliceService;
        private readonly IStructureService _structureService;
        private readonly IDuctService _ductService;
        #endregion

        #region Constructor
        public ConduitPathController(IConduitPathService conduitPathService, ISpliceService spliceService, IStructureService structureService, IDuctService ductService)
        {
            _conduitPathService = conduitPathService;
            _spliceService = spliceService;
            _structureService = structureService;
            _ductService = ductService;
        }
        #endregion


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> _GetConduitsAndSplices(string campusGuid, string structureGuid, bool loadDucts)
        {
            var conduits = await _conduitPathService.GetConduitsAsync(campusGuid, structureGuid, false);
            //var splices = await _spliceService.GetSpliceByCampusAndStructureAsync(campusGuid,structureGuid,true);

            var conduitViewModel = conduits.Select(ConduitViewModel.FromModel).ToList();
            //var spliceViewModel = splices.Select(SpliceViewModel.FromModel).ToList();

            // Create the view model for the dashboard.
            var model = new DashboardViewModel
            {
                Conduits = conduitViewModel,
                //Splices = spliceViewModel
            };
           
            return PartialView(model);
        }

        public async Task<IActionResult> _CreateConduit(Guid? conduitGuid, Guid? campusGuid)
        {
            var model = new DashboardViewModel();

            // if both are null, return empty partial
            if (conduitGuid is null && campusGuid is null)
            {
                return PartialView(model);
            }

            if (conduitGuid is not null)
            {
                // Edit mode
                var conduitPathData = await _conduitPathService.GetConduitByUniqueGuid((Guid)conduitGuid);

                if (conduitPathData == null)
                {
                    return PartialView(model); // or return NotFound()
                }              

                var structureAList = await _structureService.GetStructureIdsAsync((Guid)campusGuid, Guid.Empty, true, true);
                var structureBList = await _structureService.GetStructureIdsAsync((Guid)campusGuid, Guid.Empty, true, true);

                var selectedStructureAGuid = conduitPathData.StructureA_GUID;
                var selectedStructureBGuid = conduitPathData.StructureB_GUID;

                var orderedStructureAList = structureAList
                    .OrderBy(s => s.UniqueGUID == selectedStructureAGuid ? 0 : 1)
                    .Select(s => new SelectListItem
                    {
                        Value = s.UniqueGUID.ToString(),
                        Text = s.StructureID,
                        Selected = s.UniqueGUID == selectedStructureAGuid
                    }).ToList();

                var orderedStructureBList = structureBList
                    .OrderBy(s => s.UniqueGUID == selectedStructureBGuid ? 0 : 1)
                    .Select(s => new SelectListItem
                    {
                        Value = s.UniqueGUID.ToString(),
                        Text = s.StructureID,
                        Selected = s.UniqueGUID == selectedStructureBGuid
                    }).ToList();

                var ducts = await _ductService.GetDuctsAsync(conduitPathData.UniqueGUID, true);
                var ductList = new List<SelectListItem>();
                var subDuctList = new List<SelectListItem>();

                foreach (var duct in ducts)
                {
                    string FormatDuctText(dynamic d) =>
                        d.DuctID == d.DuctID_B ? $"{d.DuctID} ({d.Material?.ModelID})" : $"{d.DuctID} -> {d.DuctID_B} ({d.Material?.ModelID})";

                    ductList.Add(new SelectListItem
                    {
                        Value = duct.UniqueGUID.ToString(),
                        Text = FormatDuctText(duct)
                    });

                    if (duct.SubDucts?.Any() == true)
                    {
                        subDuctList.AddRange(duct.SubDucts.Select(sub => new SelectListItem
                        {
                            Value = sub.UniqueGUID.ToString(),
                            Text = FormatDuctText(sub)
                        }));
                    }
                }

                var conduit = new ConduitViewModel
                {
                    UniqueGuid = conduitPathData.UniqueGUID,
                    UniqueId = conduitPathData.UniqueID,
                    CampusGuid = (Guid)campusGuid,
                    ConduitId = conduitPathData.ConduitID,
                    Comments = conduitPathData.Comments,
                    StructureAList = orderedStructureAList,
                    StructureBList = orderedStructureBList,
                    StructureAGuid = selectedStructureAGuid,
                    StructureBGuid = selectedStructureBGuid,
                    DuctList = ductList,
                    SubDuctList = subDuctList 
                };

                model.ConduitViewModel = conduit;
            }
            else
            {
                // Create mode
                var structureAList = await _structureService.GetStructureIdsAsync((Guid)campusGuid, Guid.Empty, true, true);
                var structureBList = await _structureService.GetStructureIdsAsync((Guid)campusGuid, Guid.Empty, true, true);

                var conduit = new ConduitViewModel
                {
                    UniqueGuid = Guid.NewGuid(),
                    UniqueId = 0,
                    CampusGuid = campusGuid ?? Guid.Empty,
                    ConduitId = string.Empty,
                    Comments = string.Empty,
                    StructureAList = structureAList.Select(s => new SelectListItem
                    {
                        Value = s.UniqueGUID.ToString(),
                        Text = s.StructureID
                    }).ToList(),
                    StructureBList = structureBList.Select(s => new SelectListItem
                    {
                        Value = s.UniqueGUID.ToString(),
                        Text = s.StructureID
                    }).ToList(),
                    DuctList = null
                };

                model.ConduitViewModel = conduit;
            }

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveConduit([FromBody] ConduitViewModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                var errorResult = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
                return ResponseHelper.HandleResponse(errorResult, errorResult.StatusCode);
            }

            var userId = ""; 
            var domainModel = requestModel.ToModel();
            domainModel.CreatedBy = userId;
            domainModel.LastSavedBy = userId;

            ToasterModel result = requestModel.UniqueId > 0
                ? await _conduitPathService.UpdateConduitPathAsync(domainModel)
                : await _conduitPathService.CreateConduitAsync(domainModel);

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }

        #region Delete Building

        [HttpPost]
        public async Task<IActionResult> DeleteBuilding([FromBody] UniqueIDUniqueGUIDRequest request)
        {
            var conduit = await _conduitPathService.GetConduitByUniqueId(request.UniqueID);
            if (conduit == null)
            {
                return NotFound($"No building found with building Id: {request.UniqueID}");
            }
            var result = await _conduitPathService.DeleteConduitPathAsync(conduit);
            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
        #endregion
    }
}
