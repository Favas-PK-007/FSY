#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class CircuitController : Controller
    {
        #region Declarations
        private readonly IBuildingService _buildingService;
        private readonly IStructureService _structureService;
        private readonly ISpliceService _spliceService;
        private readonly ICableService _cableService;
        private readonly IFiberService _fiberService;
        private List<CableModel> _cableModel = new List<CableModel>();
        private readonly ICircuitService _circuitService;
        private int longestDesignation = 0;
        #endregion

        #region Constructor
        public CircuitController(IBuildingService buildingService, IStructureService structureService,
            ISpliceService spliceService, ICableService cableService, IFiberService fiberService,
            ICircuitService circuitService)
        {
            _buildingService = buildingService;
            _structureService = structureService;
            _spliceService = spliceService;
            _cableService = cableService;
            _fiberService = fiberService;
            _circuitService = circuitService;
        }
        #endregion

        public async Task<IActionResult> _CreateCircuit(Guid campusGuid)
        {
            try
            {
                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel()
                    {
                        BuildingList = new List<BuildingViewModel>(),
                        BuildingReservedList = new List<BuildingViewModel>(),
                        BufferList = new List<CircuitBufferViewModel>()
                    }
                };

                // Fetch the campus (building) details using the provided GUID
                var buildings = await _buildingService.GetBuildingsAsync(campusGuid, true);

                foreach (var building in buildings)
                {
                    // Track longest building designation length (for formatting or layout purposes)
                    if (!string.IsNullOrEmpty(building.Designation) && building.Designation.Length > longestDesignation)
                    {
                        longestDesignation = building.Designation.Length;
                    }

                    var buildingViewModel = new BuildingViewModel
                    {
                        UniqueGUID = building.UniqueGUID,
                        UniqueId = building.UniqueID,
                        CampusGuid = building.CampusGUID,
                        Designation = building.Designation,
                        BuildingId = building.BuildingID,
                        Latitude = building.Latitude,
                        Longitude = building.Longitude
                    };

                    model.CircuitViewModel.BuildingList.Add(buildingViewModel);
                    model.CircuitViewModel.BuildingReservedList.Add(buildingViewModel);
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                throw new Exception("An error occurred while loading the building." + ex.Message);
            }
        }


        #region Get Splices By Building

        public async Task<IActionResult> _GetSplicesByBuilding(Guid campusGuid, Guid buildingGuid)
        {
            try
            {
                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel()
                    {
                        SpliceList = new List<SpliceViewModel>()
                    }
                };

                var structures = await _structureService.GetStructureByCampusAndBuildingAsync(
                    campusGuid.ToString(), buildingGuid.ToString());

                foreach (var structure in structures)
                {
                    var splices = await _spliceService.GetSpliceByCampusAndStructureAsync(
                        campusGuid, structure.UniqueGUID, false);

                    var spliceViewModels = splices.Select(s => new SpliceViewModel
                    {
                        UniqueGuid = s.UniqueGUID,
                        UniqueId = s.UniqueID,
                        SpliceID = s.SpliceID,
                        SpliceType = s.SpliceType
                    });

                    model.CircuitViewModel.SpliceList.AddRange(spliceViewModels);
                }

                return PartialView(model.CircuitViewModel);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load Splices/Connections." + ex.Message);
            }
        }
        #endregion

        #region Get Cable By Splice

        public async Task<IActionResult> _GetCableBySplice(Guid campusGuid, Guid spliceGuid)
        {
            try
            {
                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel()
                    {
                        CableList = new List<CableViewModel>()
                    }
                };

                _cableModel = await _cableService.GetAllCablesAsync(campusGuid, spliceGuid, false);

                model.CircuitViewModel.CableList.AddRange(_cableModel.Select(c => new CableViewModel
                {
                    UniqueGUID = c.UniqueGUID,
                    UniqueID = c.UniqueID,
                    CableID = c.CableID,
                    CableType = c.CableType
                }));

                return PartialView(model.CircuitViewModel);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load Cables. " + ex.Message, ex);
            }
        }
        #endregion


        #region ReloadForm in Windows

        public async Task<IActionResult> _GetFibersByCable(Guid cableGuid, bool reloadFibers = false)
        {
            try
            {
                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel
                    {
                        BufferList = new List<CircuitBufferViewModel>(),
                    }
                };

                // Always fetch cable by guid from the service directly
                var selectedCable = await _cableService.GetCableByGuidAsync(cableGuid, null);
                if (selectedCable == null)
                    throw new Exception("Cable not found for the given GUID.");

                var fibers = await _fiberService.GetFiberAsync(cableGuid, reloadFibers);
                selectedCable.Fiber = fibers;

                if (reloadFibers)
                {
                    // Group by BufferID
                    var bufferGroups = fibers
                        .GroupBy(f => f.BufferID)
                        .OrderBy(g => g.Key);

                    foreach (var bufferGroup in bufferGroups)
                    {
                        var bufferViewModel = new CircuitBufferViewModel
                        {
                            BufferID = bufferGroup.Key,
                            UniqueGUID = bufferGroup.First().UniqueGUID,
                        };

                        // Group by RibbonID within each buffer
                        var ribbonGroups = bufferGroup
                            .GroupBy(f => f.RibbonID)
                            .OrderBy(g => g.Key);

                        foreach (var ribbonGroup in ribbonGroups)
                        {
                            var ribbonViewModel = new CircuitRibbonViewModel
                            {
                                RibbonID = ribbonGroup.Key,
                                UniqueGUID = ribbonGroup.First().UniqueGUID,
                            };

                            foreach (var fiber in ribbonGroup)
                            {
                                string circuitId = "";
                                bool hasMultipleCircuits = fiber.Circuits?.Count > 1;

                                if (hasMultipleCircuits)
                                {
                                    circuitId = "(Multiple Circuits)";
                                }
                                else if (fiber.Circuits?.Count == 1)
                                {
                                    var circuit = fiber.Circuits[0];
                                    if (circuit.LatestRev)
                                        circuitId = $"({circuit.CircuitID})";
                                }

                                var fiberViewModel = new CircuitFiberViewModel
                                {
                                    UniqueGUID = fiber.UniqueGUID,
                                    FiberID = fiber.FiberID.ToString(),
                                    BufferID = fiber.BufferID,
                                    RibbonID = fiber.RibbonID,
                                    CircuitID = circuitId,
                                    HasMultipleCircuits = hasMultipleCircuits
                                };

                                ribbonViewModel.Fibers.Add(fiberViewModel);
                            }

                            bufferViewModel.Ribbons.Add(ribbonViewModel);
                        }

                        model.CircuitViewModel.BufferList.Add(bufferViewModel);
                        foreach (var buffer in model.CircuitViewModel.BufferList)
                        {
                            buffer.UnSavedItems = false;
                            buffer.CableId = selectedCable.CableID;
                            buffer.CableGUID = selectedCable.UniqueGUID;
                        }
                    }
                }

                model.CircuitViewModel.SerializedModel = JsonConvert.SerializeObject(model.CircuitViewModel);

                return PartialView(model.CircuitViewModel);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to reload form. " + ex.Message, ex);
            }
        }
        #endregion

        #region Setting the Conduit ID when Building Reserved Changes

        public async Task<IActionResult> GetCircuit(string circuitIdText, string buildingReservedId)
        {
            // Get building details (like Designation) from buildingReservedId
            var building = await _buildingService.GetBuildingByUniqueIdAsync(buildingReservedId);
            if (building == null)
                return BadRequest("Invalid building selected.");

            string designation = building.Designation?.Trim();
            circuitIdText = string.Empty;
            string formattedCircuitId = circuitIdText?.Trim() ?? string.Empty;

            if (!string.IsNullOrEmpty(formattedCircuitId) &&
                formattedCircuitId.IndexOf(" |") > -1 &&
                formattedCircuitId.IndexOf(" |") < (longestDesignation + 2))
            {
                formattedCircuitId = designation + " |" + formattedCircuitId.Substring(formattedCircuitId.IndexOf(" |") + 2);
            }
            else
            {
                formattedCircuitId = formattedCircuitId.Insert(0, designation + " | ");
            };

            return Ok(formattedCircuitId);
        }
        #endregion


        #region Fiber Removal


        [HttpPost]
        public async Task<IActionResult> RemoveSelectedCircuits([FromBody] FiberRemovalRequestModel request)
        {
            if (request == null || request.CableGuid == Guid.Empty)
                return BadRequest("Invalid cable GUID.");

            if (request.SelectedFiberGuids == null || !request.SelectedFiberGuids.Any())
                return BadRequest("No fibers selected.");

            try
            {
                var selectedCable = await _cableService.GetCableByGuidAsync(request.CableGuid, null);
                if (selectedCable == null)
                    return NotFound("Cable not found.");

                var fibers = await _fiberService.GetFiberAsync(request.CableGuid, true);

                // Clear Circuit info for selected fibers instead of removing fibers
                foreach (var fiber in fibers)
                {
                    if (request.SelectedFiberGuids.Contains(fiber.UniqueGUID.ToString()))
                    {
                        fiber.Circuits = new List<CircuitModel>(); // Clear circuit associations
                    }
                }

                selectedCable.Fiber = fibers;

                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel
                    {
                        BufferList = new List<CircuitBufferViewModel>()
                    }
                };

                var bufferGroups = fibers
                    .GroupBy(f => f.BufferID)
                    .OrderBy(g => g.Key);

                foreach (var bufferGroup in bufferGroups)
                {
                    var bufferViewModel = new CircuitBufferViewModel
                    {
                        BufferID = bufferGroup.Key,
                        UniqueGUID = bufferGroup.First().UniqueGUID,
                        CableId = selectedCable.CableID,
                        CableGUID = selectedCable.UniqueGUID,
                        UnSavedItems = false
                    };

                    var ribbonGroups = bufferGroup
                        .GroupBy(f => f.RibbonID)
                        .OrderBy(g => g.Key);

                    foreach (var ribbonGroup in ribbonGroups)
                    {
                        var ribbonViewModel = new CircuitRibbonViewModel
                        {
                            RibbonID = ribbonGroup.Key,
                            UniqueGUID = ribbonGroup.First().UniqueGUID
                        };

                        foreach (var fiber in ribbonGroup)
                        {
                            string circuitId = "";
                            bool hasMultipleCircuits = fiber.Circuits?.Count > 1;

                            if (hasMultipleCircuits)
                            {
                                circuitId = "(Multiple Circuits)";
                            }
                            else if (fiber.Circuits?.Count == 1)
                            {
                                var circuit = fiber.Circuits[0];
                                if (circuit.LatestRev)
                                    circuitId = $"({circuit.CircuitID})";
                            }

                            var fiberViewModel = new CircuitFiberViewModel
                            {
                                UniqueGUID = fiber.UniqueGUID,
                                FiberID = fiber.FiberID.ToString(),
                                BufferID = fiber.BufferID,
                                RibbonID = fiber.RibbonID,
                                CircuitID = circuitId,
                                HasMultipleCircuits = hasMultipleCircuits
                            };

                            ribbonViewModel.Fibers.Add(fiberViewModel);
                        }

                        if (ribbonViewModel.Fibers.Any())
                            bufferViewModel.Ribbons.Add(ribbonViewModel);
                    }

                    if (bufferViewModel.Ribbons.Any())
                        model.CircuitViewModel.BufferList.Add(bufferViewModel);
                }

                return PartialView("_GetFibersByCable", model.CircuitViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Error while removing fibers. " + ex.Message);
            }
        }



        #endregion

        #region Fiber Update

        [HttpPost]
        public async Task<IActionResult> AssignSelectedCircuits([FromBody] FiberAssignmentRequestModel request)
        {
            if (request == null || request.CableGuid == Guid.Empty)
                return BadRequest("Invalid cable GUID.");

            if (string.IsNullOrEmpty(request.CircuitId))
                return BadRequest("Circuit ID is required.");

            if (request.SelectedFiberGuids == null || !request.SelectedFiberGuids.Any())
                return BadRequest("No fibers selected.");

            try
            {
                var selectedCable = await _cableService.GetCableByGuidAsync(request.CableGuid, null);
                if (selectedCable == null)
                    return NotFound("Cable not found.");

                var fibers = await _fiberService.GetFiberAsync(request.CableGuid, true);

                // Assign the selected circuit ID to the selected fibers
                foreach (var fiber in fibers)
                {
                    if (request.SelectedFiberGuids.Contains(fiber.UniqueGUID.ToString()))
                    {
                        fiber.Circuits = new List<CircuitModel>
                        {
                            new CircuitModel
                            {
                                CircuitID = request.CircuitId,
                                LatestRev = true
                            }
                        };
                    }
                }

                selectedCable.Fiber = fibers;

                var model = new DashboardViewModel
                {
                    CircuitViewModel = new CircuitViewModel
                    {
                        BufferList = new List<CircuitBufferViewModel>()
                    }
                };

                var bufferGroups = fibers
                    .GroupBy(f => f.BufferID)
                    .OrderBy(g => g.Key);

                foreach (var bufferGroup in bufferGroups)
                {
                    var bufferViewModel = new CircuitBufferViewModel
                    {
                        BufferID = bufferGroup.Key,
                        UniqueGUID = bufferGroup.First().UniqueGUID,
                        CableId = selectedCable.CableID,
                        CableGUID = selectedCable.UniqueGUID,
                        UnSavedItems = false
                    };

                    var ribbonGroups = bufferGroup
                        .GroupBy(f => f.RibbonID)
                        .OrderBy(g => g.Key);

                    foreach (var ribbonGroup in ribbonGroups)
                    {
                        var ribbonViewModel = new CircuitRibbonViewModel
                        {
                            RibbonID = ribbonGroup.Key,
                            UniqueGUID = ribbonGroup.First().UniqueGUID
                        };

                        foreach (var fiber in ribbonGroup)
                        {
                            string circuitId = "";
                            bool hasMultipleCircuits = fiber.Circuits?.Count > 1;

                            if (hasMultipleCircuits)
                            {
                                circuitId = "(Multiple Circuits)";
                            }
                            else if (fiber.Circuits?.Count == 1)
                            {
                                var circuit = fiber.Circuits[0];
                                if (circuit.LatestRev)
                                    circuitId = $"({circuit.CircuitID})";
                            }

                            var fiberViewModel = new CircuitFiberViewModel
                            {
                                UniqueGUID = fiber.UniqueGUID,
                                FiberID = fiber.FiberID.ToString(),
                                BufferID = fiber.BufferID,
                                RibbonID = fiber.RibbonID,
                                CircuitID = circuitId,
                                HasMultipleCircuits = hasMultipleCircuits
                            };

                            ribbonViewModel.Fibers.Add(fiberViewModel);
                        }

                        if (ribbonViewModel.Fibers.Any())
                            bufferViewModel.Ribbons.Add(ribbonViewModel);
                    }

                    if (bufferViewModel.Ribbons.Any())
                        model.CircuitViewModel.BufferList.Add(bufferViewModel);
                }

                return PartialView("_GetFibersByCable", model.CircuitViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest("Error while assigning circuits. " + ex.Message);
            }
        }
        #endregion

        #region Save Circuit

        //public async Task<IActionResult> SaveCircuit(Guid cableGuid, Guid spliceGuid)
        //{
        //    var circuitInCables = await _circuitService.GetCircuitsByCableAsync(cableGuid);

        //    var selectedSplice = await _spliceService.GetSpliceByGuidAsync(spliceGuid, null);

        //    foreach (var circuit in circuitInCables.Values)
        //    {
        //        if ((circuit.UniqueID == 0) && (!circuit.LatestRev))
        //        {
        //            var result = await _circuitService.SaveTracePathAsync(circuit, "Add", cableGuid, selectedSplice);
        //        }
        //        else if ((circuit.UniqueID > 0) && (circuit.LatestRev) && (!circuit.Updated))
        //        {
        //            var result = await _circuitService.UpdateCircuitAsync(circuit);
        //        }
        //        else if ((circuit.UniqueID > 0) && (circuit.Updated))
        //        {
        //            var result = await _circuitService.UpdateCircuitAsync(circuit);
        //        }
        //    }

        //}


        [HttpPost]
        public async Task<IActionResult> SaveCircuitAsync(Guid cableGuid, Guid spliceGuid)
        {
            try
            {
                var result = await _circuitService.SaveCircuitsAsync(cableGuid, spliceGuid, User.Identity.Name);
                return Ok(new { message = "Circuits updated successfully", details = result });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving circuit: {ex.Message}");
            }
        }

        #endregion

    }
}
