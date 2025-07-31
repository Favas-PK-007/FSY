#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using FhaFacilitiesApplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using static FhaFacilitiesApplication.Models.ViewModel.FiberTreeViewModel;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class CableController : Controller
    {

        #region Declarations

        private readonly ICableService _cableService;
        private readonly ISpliceService _spliceService;
        private readonly IMeterialService _materialService;
        private readonly IMaterialDetailService _materialDetailService;
        #endregion


        #region Constructor
        public CableController(ICableService cableService, ISpliceService spliceService, IMeterialService meterialService, IMaterialDetailService materialDetailService)
        {
            _cableService = cableService;
            _spliceService = spliceService;
            _materialService = meterialService;
            _materialDetailService = materialDetailService;
        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> _GetAllCables(Guid? campusGuid, Guid? spliceGuid)
        {
            var model = new DashboardViewModel();
            if (campusGuid is null || spliceGuid is null)
            {
                return PartialView(model);
            }
            try
            {
                var cables = await _cableService.GetAllCablesAsync((Guid)campusGuid, (Guid)spliceGuid, false);
                if (cables.Any())
                {
                    model.Cables = CableViewModel.FromModelList(cables);
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving cables: " + ex.Message);
            }

        }

        public async Task<IActionResult> _CreateCable(Guid? cableGuid, Guid? campusGuid)
        {
            var model = new DashboardViewModel();

            // Load common dropdown data
            var cableTypes = await _cableService.GetCableTypesAsync();
            var cableModels = await _cableService.GetCableModelsAsync();
            var spliceA = await _spliceService.GetSplicesAsync((Guid)campusGuid);
            var spliceB = await _spliceService.GetSplicesAsync((Guid)campusGuid);

            if (cableGuid.HasValue && cableGuid.Value != Guid.Empty)
            {
                var cableData = await _cableService.GetCableByGuidAsync(cableGuid.Value, null);

                // Fiber type and template info
                var material = await _materialService.GetMaterialByGuidAsync(cableData.TypeGUID);
                var materialDetails = await _materialDetailService.GetLoadDetailAsync(
                    material.MaterialType, material.UniqueGUID, material.TemplateGUID);

                var cableType = materialDetails["FiberType"];

                cableData.Fiber = await _cableService.GetFiberInCablesAsync(cableData);

                var buffers = new List<BufferViewModel>();

                foreach (var fiber in cableData.Fiber.OrderBy(f => f.BufferID).ThenBy(f => f.RibbonID).ThenBy(f => f.FiberID))
                {
                    var buffer = buffers.FirstOrDefault(b => b.Value == fiber.BufferID);
                    if (buffer == null)
                    {
                        buffer = new BufferViewModel
                        {
                            Value = fiber.BufferID,
                            Text = $"Buffer-{fiber.BufferID}",
                            Ribbons = new List<RibbonViewModel>(),
                            Fibers = new List<FiberCableViewModel>()
                        };
                        buffers.Add(buffer);
                    }

                    var fiberLabel = $"Fiber-{fiber.FiberID}";

                    if (cableType == "Ribbon")
                    {
                        // Add Ribbon if needed
                        var ribbon = buffer.Ribbons.FirstOrDefault(r => r.Value == fiber.RibbonID);
                        if (ribbon == null)
                        {
                            ribbon = new RibbonViewModel
                            {
                                Value = fiber.RibbonID,
                                Text = $"Ribbon-{fiber.RibbonID}",
                                Fibers = new List<FiberCableViewModel>()
                            };
                            buffer.Ribbons.Add(ribbon);
                        }

                        // Add circuit if available
                        if (fiber.Circuits?.Any() == true)
                        {
                            fiberLabel += $" ({fiber.Circuits.First().CircuitID})";
                        }

                        ribbon.Fibers.Add(new FiberCableViewModel
                        {
                            Value = fiber.FiberID,
                            Text = fiberLabel
                        });
                    }
                    else // Loose Cable
                    {
                        buffer.Fibers.Add(new FiberCableViewModel
                        {
                            Value = fiber.FiberID,
                            Text = fiberLabel
                        });
                    }
                }

                model.CableFiberViewModel.Buffers = buffers;

                // Build cable view model
                model.CableViewModel = new CableViewModel
                {
                    UniqueGUID = cableData.UniqueGUID,
                    UniqueID = cableData.UniqueID,
                    CampusGUID = cableData.CampusGUID,
                    CableType = cableData.CableType,
                    CableID = cableData.CableID,
                    TypeGUID = cableData.TypeGUID,
                    SpliceAGUID = cableData.SpliceA_GUID,
                    SpliceBGUID = cableData.SpliceB_GUID,
                    DuctGUID = cableData.DuctGUID,
                    Comments = cableData.Comments,

                    CableTypeList = cableTypes.Select(ct => new SelectListItem
                    {
                        Text = ct.Text,
                        Value = ct.Value,
                        Selected = ct.Value == cableData.CableType
                    }).ToList(),

                    CableModelList = cableModels.Select(cm => new SelectListItem
                    {
                        Text = cm.Text,
                        Value = cm.Value.ToString(),
                        Selected = cm.Value == cableData.TypeGUID
                    }).ToList(),

                    SpliceAList = spliceA.Select(sa => new SelectListItem
                    {
                        Text = sa.SpliceID,
                        Value = sa.UniqueGUID.ToString(),
                        Selected = sa.UniqueGUID == cableData.SpliceA_GUID
                    }).ToList(),

                    SpliceBList = spliceB.Select(sb => new SelectListItem
                    {
                        Text = sb.SpliceID,
                        Value = sb.UniqueGUID.ToString(),
                        Selected = sb.UniqueGUID == cableData.SpliceB_GUID
                    }).ToList()
                };

                return PartialView(model);
            }

            // Create new
            var cableViewModel = new CableViewModel
            {
                UniqueGUID = Guid.NewGuid(),
                UniqueID = 0,
                CampusGUID = campusGuid ?? Guid.Empty,
                CableType = string.Empty,
                CableID = string.Empty,
                TypeGUID = Guid.Empty,
                SpliceAGUID = null,
                SpliceBGUID = null,
                DuctGUID = Guid.Empty,
                Comments = string.Empty,
                Fibers = null,

                CableTypeList = cableTypes.Select(ct => new SelectListItem
                {
                    Text = ct.Text,
                    Value = ct.Value
                }).ToList(),

                CableModelList = cableModels.Select(cm => new SelectListItem
                {
                    Text = cm.Text,
                    Value = cm.Value.ToString(),
                    Selected = false
                }).ToList(),

                SpliceAList = spliceA.Select(sa => new SelectListItem
                {
                    Text = sa.SpliceID,
                    Value = sa.UniqueGUID.ToString(),
                    Selected = false
                }).ToList(),

                SpliceBList = spliceB.Select(sb => new SelectListItem
                {
                    Text = sb.SpliceID,
                    Value = sb.UniqueGUID.ToString(),
                    Selected = false
                }).ToList()
            };

            model.CableViewModel = cableViewModel;
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveCable([FromBody] CableViewModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
                return ResponseHelper.HandleResponse(errors, errors.StatusCode);
            }

            var userId = "";
            var domainModel = requestModel.ToModel();

            if (requestModel.UniqueID == 0)
                domainModel.UniqueGUID = Guid.NewGuid();

            domainModel.CreatedBy = userId;
            domainModel.LastSavedBy = userId;

            var result = requestModel.UniqueID > 0
                ? await _cableService.UpdateCableAsync(domainModel)
                : await _cableService.SaveCableAsync(domainModel);

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }

        #region API to be called when Generate Fibers button is clicked
        public async Task<IActionResult> _GetGenerateFibers(Guid cableModelGuid, Guid cableGuid)
        {
            try
            {
                var dashboardModel = new DashboardViewModel();
                var model = await _materialService.GetMaterialByGuidAsync(cableModelGuid);
                var data = await _materialDetailService.GetLoadDetailAsync(model.MaterialType, model.UniqueGUID, model.TemplateGUID);

                var cableType = data["FiberType"];
                int fibersPerRibbon = int.Parse(data["FibersPerRibbon"]);
                int numOfBuffers = int.Parse(data["NumOfBuffers"]);
                int quantityPerBuffer = int.Parse(data["QuantityPerBuffer"]);

                var cableData = await _cableService.GetCableByGuidAsync(cableGuid, null);
                cableData.Fiber = await _cableService.GenerateNewFiberAsync(cableData, cableType, numOfBuffers, quantityPerBuffer, fibersPerRibbon, "", "userId");

                var buffers = new List<BufferViewModel>();

                foreach (var fiber in cableData.Fiber.OrderBy(f => f.BufferID).ThenBy(f => f.RibbonID).ThenBy(f => f.FiberID))
                {
                    var buffer = buffers.FirstOrDefault(b => b.Value == fiber.BufferID);
                    if (buffer == null)
                    {
                        buffer = new BufferViewModel
                        {
                            Value = fiber.BufferID,
                            Text = $"Buffer-{fiber.BufferID}"
                        };
                        buffers.Add(buffer);
                    }

                    var fiberVM = new FiberCableViewModel
                    {
                        Value = fiber.FiberID,
                        Text = $"Fiber-{fiber.FiberID}"
                    };

                    if (cableType == "Ribbon")
                    {
                        var ribbon = buffer.Ribbons.FirstOrDefault(r => r.Value == fiber.RibbonID);
                        if (ribbon == null)
                        {
                            ribbon = new RibbonViewModel
                            {
                                Value = fiber.RibbonID,
                                Text = $"Ribbon-{fiber.RibbonID}"
                            };
                            buffer.Ribbons.Add(ribbon);
                        }
                        ribbon.Fibers.Add(fiberVM);
                    }
                    else
                    {
                        buffer.Fibers.Add(fiberVM);
                    }
                }

                dashboardModel.CableFiberViewModel.Buffers = buffers;

                return PartialView("_GetGenerateFibers", dashboardModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving fibers: " + ex.Message);
            }
        }
        #endregion


        #region Delete Cable
        [HttpPost]
        public async Task<IActionResult> DeleteCable([FromBody] UniqueGUIDRequest request)
        {
            var userId = "";
            if (request == null)
            {
                return ResponseHelper.HandleResponse("Invalid cable GUID.", HttpStatusCode.BadRequest);
            }

            var cableModel = await _cableService.GetCableByGuidAsync(request.UniqueGUID, "Delete");
            if (cableModel == null)
            {
                return ResponseHelper.HandleResponse("Cable not found.", HttpStatusCode.NotFound);
            }

            cableModel.LastSavedBy = userId;
            var result = await _cableService.DeleteCableAsync(cableModel);
            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
        #endregion


        #region API to be called when Cable Dropdown Changes in View Connection Modal
        public async Task<IActionResult> _GetFiberTree(Guid cableGuid, Guid spliceGuid)
        {
            var model = new DashboardViewModel();

            // Step 1: Fetch cable and list of available (unused) fibers using the service
            var (cable, availableFiberGuids) = await _cableService.GetAvailableFibersForCableAsync(cableGuid, spliceGuid);

            // Step 2: If cable is invalid or no fibers, return an empty tree
            if (cable.UniqueGUID == Guid.Empty || cable.Fiber == null || cable.Fiber.Count == 0)
                return Ok(new FiberTreeViewModel { CableID = cable.CableID });

            // Step 3: Initialize the root node of the fiber tree
            var tree = new FiberTreeViewModel
            {
                CableID = cable.CableID // Root node displays the cable name
            };

            // Step 4: Group all fibers by BufferID
            var groupedByBuffer = cable.Fiber
                .GroupBy(f => f.BufferID)
                .OrderBy(g => g.Key);

            foreach (var bufferGroup in groupedByBuffer)
            {
                // Step 5: Get the first fiber to access the actual BufferGUID
                var firstBufferFiber = bufferGroup.FirstOrDefault();

                // Step 6: Create buffer node with GUID and label
                var bufferNode = new BufferNode
                {
                    UniqueGUID = firstBufferFiber.UniqueGUID,
                    BufferID = $"Buffer-{bufferGroup.Key}" // Label shown in UI
                };

                // Step 7: Group fibers within buffer by RibbonID
                var groupedByRibbon = bufferGroup
                    .GroupBy(f => f.RibbonID)
                    .OrderBy(g => g.Key);

                foreach (var ribbonGroup in groupedByRibbon)
                {
                    // Step 8: Get the first fiber to access the actual RibbonGUID
                    var firstRibbonFiber = ribbonGroup.FirstOrDefault();

                    // Step 9: Create ribbon node with GUID and label
                    var ribbonNode = new RibbonNode
                    {
                        UniqueGUID = firstRibbonFiber.UniqueGUID,
                        RibbonID = $"Ribbon-{ribbonGroup.Key}" // Label shown in UI
                    };

                    // Step 10: Add all available (unused) fibers under this ribbon
                    foreach (var fiber in ribbonGroup.OrderBy(f => f.FiberID))
                    {
                        if (availableFiberGuids.Contains(fiber.UniqueGUID))
                        {
                            ribbonNode.Fibers.Add(new FiberNode
                            {
                                UniqueGUID = fiber.UniqueGUID,
                                FiberID = $"Fiber-{fiber.FiberID}" // Label shown in UI
                            });
                        }
                    }

                    // Step 11: Add ribbon to buffer even if it has 0 fibers (for completeness)
                    bufferNode.Ribbons.Add(ribbonNode);
                }

                // Step 12: Add buffer to root even if it has 0 ribbons (for completeness)
                tree.Buffers.Add(bufferNode);
            }

            model.ConnectionListingViewModel.CableAFiberTree = tree;
            model.ConnectionListingViewModel.CableBFiberTree = tree;
            // Step 13: Return the final fiber tree structure
            return PartialView(model);
        }
        #endregion
    }
}
