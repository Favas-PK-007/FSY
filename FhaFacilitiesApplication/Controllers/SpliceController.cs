#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ResponseModel;
using FhaFacilitiesApplication.Models.ViewModel;
using FhaFacilitiesApplication.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Composition;
using System.Net;
using System.Reflection;
using System.Text;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class SpliceController : Controller
    {
        #region Declarations
        private readonly ISpliceService _spliceService;
        private readonly IStructureService _structureService;
        private readonly IMeterialService _meterialService;
        private readonly IMaterialDetailService _materialDetailService;
        private readonly IComponentService _componentService;
        private readonly ICableService _cableService;
        private readonly IFiberService _fiberService;
        private readonly ICircuitService _circuitService;
        private readonly IPortService _portService;
        private readonly IConnectionService _connectionService;
        private readonly IEquipmentService _equipmentService;
        private List<ConnectionModel> connections;
        private readonly IReportService _reportService;
        #endregion


        #region Constructor
        public SpliceController(ISpliceService spliceService, IStructureService structureService,
            IMeterialService meterialService, IMaterialDetailService materialDetailService,
            IComponentService componentService, ICableService cableService, IFiberService fiberService,
            ICircuitService circuitService, IPortService portService, IConnectionService connectionService,
            IEquipmentService equipmentService, IReportService reportService)
        {
            _spliceService = spliceService;
            _structureService = structureService;
            _meterialService = meterialService;
            _materialDetailService = materialDetailService;
            _componentService = componentService;
            _cableService = cableService;
            _fiberService = fiberService;
            _circuitService = circuitService;
            _portService = portService;
            _connectionService = connectionService;
            _equipmentService = equipmentService;
            _reportService = reportService;
        }
        #endregion


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> _GetSplices(Guid? campusGuid, Guid? structureGuid)
        {
            var model = new DashboardViewModel();
            if (campusGuid is null || structureGuid is null)
            {
                return PartialView(model);
            }
            try
            {
                var splices = await _spliceService.GetSpliceByCampusAndStructureAsync((Guid)campusGuid, (Guid)structureGuid, true);
                if (splices.Any())
                {
                    model.Splices = SpliceViewModel.FromModelList(splices);
                }
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving splices: " + ex.Message);
            }
        }

        public async Task<IActionResult> _CreateSplice(Guid? uniqueGuid, Guid? selectedCampusGuid, Guid? selectedStructureGuid)
        {
            var model = new DashboardViewModel();

            // Case: No parameters provided
            if (uniqueGuid is null && selectedCampusGuid is null && selectedStructureGuid is null)
                return PartialView(model);

            var installationType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Enclosed Case", Value = "Enclosed Case" },
                new SelectListItem { Text = "Wall Mounted", Value = "Wall Mounted" },
                new SelectListItem { Text = "Rack Mounted", Value = "Rack Mounted" }
            };

            var equipmentType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Splice", Value = "Splice" },
                new SelectListItem { Text = "FPP Chassis", Value = "FPP Chassis" }
            };

            // Edit mode
            if (uniqueGuid is not null)
            {
                var splice = await _spliceService.GetSpliceByGuidAsync(uniqueGuid.Value, null);

                if (splice != null)
                {
                    var structureIds = await _structureService.GetStructureIdsAsync(splice.CampusGUID, Guid.Empty, true, true);
                    var material = await _meterialService.GetMeterialsAsync(splice.UniqueGUID, splice.TypeGUID, true, true);

                    var viewModel = new SpliceViewModel
                    {
                        UniqueId = splice.UniqueID,
                        UniqueGuid = splice.UniqueGUID,
                        StructureGUID = splice.StructureGUID,
                        SpliceID = splice.SpliceID,
                        SpliceType = splice.SpliceType,
                        TypeGUID = splice.TypeGUID,
                        Comments = splice.Comments,
                        CampusGuid = splice.CampusGUID,
                        SelectedStructureID = selectedStructureGuid ?? splice.StructureGUID,
                        StructureIdList = structureIds
                            .Select(s => new SelectListItem
                            {
                                Value = s.UniqueGUID.ToString(),
                                Text = s.StructureID,
                                Selected = (s.UniqueGUID == (selectedStructureGuid ?? splice.StructureGUID))
                            }).ToList(),
                        Structure = StructureViewModel.FromModel(structureIds.FirstOrDefault(s => s.UniqueGUID == (selectedStructureGuid ?? splice.StructureGUID))),
                        InstallationTypeList = installationType
                            .Select(i => new SelectListItem
                            {
                                Text = i.Text,
                                Value = i.Value,
                                Selected = i.Value == splice.SpliceType
                            }).ToList(),
                        EquipmentTypeList = equipmentType
                            .Select(i => new SelectListItem
                            {
                                Text = i.Text,
                                Value = i.Value,
                                Selected = i.Value == material.MaterialType
                            }).ToList(),
                        EquipmentModelList = new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Text = material.ToString(),
                                Value = material.UniqueGUID.ToString(),
                                Selected = true
                            }
                        },
                        Components = material.Components.Select(t => new MaterialViewModel
                        {
                            UniqueGUID = t.UniqueGUID,
                            MaterialID = t.MaterialID,
                            MaterialType = t.MaterialType,
                            ParentGuid = t.ParentGUID,
                            TemplateGuid = t.TemplateGUID,
                            ModelID = t.ModelID,
                            Children = t.Components?.Select(m => new MaterialViewModel
                            {
                                UniqueGUID = m.UniqueGUID,
                                MaterialID = m.MaterialID,
                                MaterialType = m.MaterialType,
                                ParentGuid = m.ParentGUID,
                                TemplateGuid = m.TemplateGUID,
                                ModelID = m.ModelID
                            }).ToList()
                        }).ToList()
                    };

                    model.SpliceViewModel = viewModel;
                    return PartialView(model);
                }
            }

            // Add mode
            var selectedCampus = selectedCampusGuid ?? Guid.Empty;
            var selectedStructure = selectedStructureGuid ?? Guid.Empty;

            var structures = await _structureService.GetStructureIdsAsync(selectedCampus, Guid.Empty, true, true);

            var addViewModel = new SpliceViewModel
            {
                UniqueId = 0,
                UniqueGuid = Guid.NewGuid(),
                StructureGUID = Guid.Empty,
                SpliceID = string.Empty,
                SpliceType = string.Empty,
                TypeGUID = Guid.Empty,
                Comments = string.Empty,
                CampusGuid = selectedCampus,
                SelectedStructureID = selectedStructure,
                InstallationTypeList = installationType,
                EquipmentTypeList = equipmentType,
                EquipmentModelList = new List<SelectListItem>(),
                StructureIdList = structures.Select(s => new SelectListItem
                {
                    Value = s.UniqueGUID.ToString(),
                    Text = s.StructureID,
                    Selected = s.UniqueGUID == selectedStructure
                }).ToList(),
                Structure = StructureViewModel.FromModel(structures.FirstOrDefault(s => s.UniqueGUID == selectedStructure))
            };

            model.SpliceViewModel = addViewModel;
            return PartialView(model);
        }



        [HttpPost]
        public async Task<IActionResult> SaveSplice([FromBody] SpliceViewModel requestModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
                    return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
                }

                var userId = "";
                var domainModel = requestModel.ToModel();
                domainModel.CreatedBy = userId;
                domainModel.LastSavedBy = userId;

                ToasterModel result;
                if (requestModel.UniqueId > 0)
                {
                    result = await _spliceService.UpdateSpliceAsync(domainModel);
                }
                else
                {
                    result = await _spliceService.CreateSpliceAsync(domainModel);
                }

                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }
            catch (Exception ex)
            {
                var errorResult = new ToasterModel
                {
                    IsError = true,
                    Message = $"Error saving splice: {ex.Message}",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };

                return ResponseHelper.HandleResponse(errorResult, errorResult.StatusCode);
            }
        }

        // Api to be called when  equipment type is selected in the splice modal
        public async Task<IActionResult> GetSpliceModel(Guid? typeGuid, Guid? spliceGuid, string selectedEquipmentType = null)
        {

            var model = new DashboardViewModel();

            var materials = new List<MaterialViewModel>();

            if (typeGuid is not null)
            {
                // Load specific material based on spliceGuid and typeGuid
                var material = await _meterialService.GetMeterialsAsync(spliceGuid.Value, typeGuid.Value, true, true);

                if (material != null)
                {
                    materials.Add(MaterialViewModel.FromModel(material));
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(selectedEquipmentType))
                {
                    var materialList = await _meterialService.GetStructureModels(true, selectedEquipmentType, Guid.Empty, Guid.Empty);

                    materials = materialList.Select(MaterialViewModel.FromModel).ToList();


                    model.SpliceViewModel.EquipmentModelList = materialList.Select(s => new SelectListItem
                    {
                        Text = s.ToString(),
                        Value = s.UniqueGUID.ToString()

                    }).ToList();

                }
            }

            return Ok(model.SpliceViewModel);

        }

        // populate the equipment model dropdown based on the selected material type
        public async Task<IActionResult> GetEqipmentModel(string materialType)
        {
            var equipmentModelData = await _meterialService.GetStructureModels(true, materialType, Guid.Empty, Guid.Empty);

            var result = MaterialViewModel.FromModelList(equipmentModelData);
            if (result == null || !result.Any())
            {
                return ResponseHelper.HandleResponse("", HttpStatusCode.NoContent);
            }

            return ResponseHelper.HandleResponse(result, HttpStatusCode.OK);
        }

        #region Delete Splice

        [HttpPost]
        public async Task<IActionResult> DeleteSplice([FromBody] UniqueGUIDRequest request)
        {
            if (request == null || request.UniqueGUID == Guid.Empty)
            {
                return ResponseHelper.HandleResponse("Invalid splice GUID.", HttpStatusCode.BadRequest);
            }

            var userId = "";

            var spliceModel = await _spliceService.GetSpliceByGuidAsync(request.UniqueGUID, "Delete");
            if (spliceModel == null)
            {
                return ResponseHelper.HandleResponse("No splice found with this ID.", HttpStatusCode.NotFound);
            }

            spliceModel.LastSavedBy = userId;

            var result = await _spliceService.DeleteSpliceAsync(spliceModel);
            return ResponseHelper.HandleResponse(result, result?.StatusCode ?? HttpStatusCode.InternalServerError);
        }

        #endregion


        #region Component Partial View
        public async Task<IActionResult> _CreateComponent(Guid? parentMaterialGuid, Guid? trayGuid = null)
        {
            if (parentMaterialGuid == null || parentMaterialGuid == Guid.Empty)
            {
                return ResponseHelper.HandleResponse("Invalid parent material GUID.", HttpStatusCode.BadRequest);
            }

            var model = new DashboardViewModel();

            // Get the parent (splice or chassis)
            var parentMaterial = await _meterialService.GetMaterialByGuidAsync(parentMaterialGuid.Value);
            if (parentMaterial == null)
            {
                return ResponseHelper.HandleResponse("Parent material not found.", HttpStatusCode.NotFound);
            }

            string materialType;

            // If a tray is selected → we're adding a Module
            if (trayGuid.HasValue && trayGuid != Guid.Empty)
            {
                // Get tray component (child)
                var trayComponent = await _meterialService.GetMaterialByGuidAsync(trayGuid.Value);
                if (trayComponent == null)
                {
                    return ResponseHelper.HandleResponse("Tray component not found.", HttpStatusCode.NotFound);
                }

                materialType = parentMaterial.MaterialType?.ToLowerInvariant() switch
                {
                    "splice" => "Splice Module",
                    "fpp chassis" => "FPP Module",
                    _ => null
                };

                // Use trayComponent as new parent for module fetch
                parentMaterial = trayComponent;
            }
            else
            {
                // Adding a Tray or Cartridge
                materialType = parentMaterial.MaterialType?.ToLowerInvariant() switch
                {
                    "splice" => "Splice Tray",
                    "fpp chassis" => "FPP Cartridge",
                    _ => null
                };
            }

            if (string.IsNullOrEmpty(materialType))
            {
                return ResponseHelper.HandleResponse("Unsupported material type.", HttpStatusCode.BadRequest);
            }

            // Get dropdown list
            var materialList = await _meterialService.GetComponentModelDropdownAsync(
                materialType,
                parentMaterial.UniqueGUID,
                parentMaterial.TemplateGUID
            );

            var componentDropdown = new List<SelectListItem>
            {
                new SelectListItem { Text = $"Select {materialType}", Value = "", Selected = true }
            };

            componentDropdown.AddRange(materialList.Select(m => new SelectListItem
            {
                Text = m.ToString(),
                Value = m.UniqueGUID.ToString()
            }));

            var viewModel = new ComponentViewModel
            {
                MaterialType = materialType,
                ComponentModelList = componentDropdown
            };

            model.ComponentViewModel = viewModel;
            return PartialView(model);
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> SaveComponent([FromBody] ComponentRequestModel request)
        {
            if (!ModelState.IsValid || request.EquipmentModelGuid == Guid.Empty)
            {
                return ResponseHelper.HandleResponse("Invalid request data.", HttpStatusCode.BadRequest);
            }

            var userId = "";

            // Load selected component and parent material (equipment model)
            var componentModel = await _meterialService.GetMaterialByGuidAsync(request.SelectedComponentModelGuid);
            var parentMaterial = await _meterialService.GetMaterialByGuidAsync(request.EquipmentModelGuid);
            var parentComponent = await _meterialService.GetMaterialByGuidAsync(request.EquipmentModelGuid);

            if (componentModel == null || parentMaterial == null)
            {
                return ResponseHelper.HandleResponse("Component or equipment model not found.", HttpStatusCode.NotFound);
            }

            // Load details
            componentModel.Details = await _materialDetailService.GetLoadDetailAsync(componentModel.MaterialType, componentModel.UniqueGUID, Guid.Empty);
            parentMaterial.Details = await _materialDetailService.GetLoadDetailAsync(parentMaterial.MaterialType, parentMaterial.UniqueGUID, parentMaterial.TemplateGUID);
            parentMaterial.Components = await _componentService.GetComponentsAsync(parentMaterial.MaterialType, parentMaterial.UniqueGUID, true, true);

            var domainModel = request.ToModel(componentModel, parentMaterial, parentComponent);
            domainModel.LastSavedBy = userId;
            domainModel.CreatedBy = userId;
            var result = await _componentService.SaveComponentAsync(domainModel, parentMaterial, parentComponent);

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }


        #region View Connection Partial View
        [HttpGet]
        public async Task<IActionResult> _GetConnectionList(Guid spliceGuid, bool reloadPorts = false)
        {
            var model = new DashboardViewModel();
            var result = new ConnectionListingViewModel();
            var slots = new List<SpliceSlotViewModel>();
            // Special GUIDs as constants
            var UNPATCHED_GUID = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var EQUIPMENT_GUID = Guid.Parse("22222222-2222-2222-2222-222222222222");

            try
            {
                // Load Splice
                var splice = await _spliceService.GetSpliceByGuidAsync(spliceGuid, null).ConfigureAwait(false);
                if (splice == null)
                    return NotFound("Splice not found");

                // Load data asynchronously
                var materialTask = _meterialService.GetMeterialsAsync(splice.UniqueGUID, splice.TypeGUID, true, true);
                var cablesTask = _cableService.GetAllCablesAsync(splice.CampusGUID, splice.UniqueGUID, false);
                var portsTask = _portService.GetPortsAsync(splice.UniqueGUID);
                var connectionsTask = _connectionService.GetConnectionsAsync(splice.UniqueGUID);

                await Task.WhenAll(materialTask, cablesTask, portsTask, connectionsTask);

                var material = materialTask.Result;
                var cables = cablesTask.Result;
                var ports = portsTask.Result;
                connections = connectionsTask.Result;

                // Build lookup dictionaries
                var cableDict = cables.ToDictionary(c => c.UniqueGUID, c => c);

                // Load fibers and circuits per cable parallel
                var cableIds = cables.Select(c => c.UniqueGUID).ToList();
                var fiberTasks = cableIds.Select(id => _fiberService.GetFiberAsync(id, false));
                var circuitTasks = cableIds.Select(id => _circuitService.GetCircuitsByCableAsync(id));

                var fiberResults = await Task.WhenAll(fiberTasks);
                var circuitResults = await Task.WhenAll(circuitTasks);

                var fiberList = fiberResults.SelectMany(f => f).ToList();
                var fiberDict = fiberList.ToDictionary(f => f.UniqueGUID, f => f);

                var circuitDict = circuitResults
                    .SelectMany(dict => dict)
                    .GroupBy(x => x.Key)
                    .ToDictionary(g => g.Key, g => g.First().Value);

                // Prepare dropdown lists
                result.CableAList = cables.Select(c => new SelectListItem { Text = c.CableID, Value = c.UniqueGUID.ToString() }).ToList();
                result.CableBList = new List<SelectListItem>(result.CableAList);

                if (string.Equals(material.MaterialType, "FPP Chassis", StringComparison.OrdinalIgnoreCase))
                {
                    result.CableBList.Insert(0, new SelectListItem { Text = "Unpatched Port", Value = UNPATCHED_GUID.ToString() });
                }

                // Configure headers based on MaterialType
                var trayLabel = material.MaterialType == "Splice" ? "Tray" : "Cartridge";
                var slotLabel = material.MaterialType == "Splice" ? "Slot" : "Port";

                result.Title = splice.SpliceID;
                result.Column0Header = trayLabel;
                result.Column1Header = slotLabel;
                result.Column2Header = "Connection Type";
                result.Column3Header = material.MaterialType == "Splice" ? "Fibers Spliced" : "Fibers Connected";

                // Group connections by PortGUID for quick lookup (only non-expressed connections)
                var connectionDict = connections
                    .Where(c => c.LatestRev && c.ConnectionType != "Expressed" && c.PortGUID.HasValue)
                    .GroupBy(c => c.PortGUID.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                int slotId = 1;    // global slot counter starting at 1
                int trayNumber = 0;

                foreach (var tray in material.Components)
                {
                    trayNumber++;
                    int moduleNumber = 0;

                    foreach (var module in tray.Components)
                    {
                        moduleNumber++;

                        if (!module.Details.TryGetValue("SlotsPerModule", out var slotsPerModuleObj)
                            || !int.TryParse(slotsPerModuleObj.ToString(), out int slotsPerModule))
                        {
                            slotsPerModule = 0; // default if missing or invalid
                        }

                        for (int slotIndex = 0; slotIndex < slotsPerModule; slotIndex++)
                        {
                            int slotNumber = slotIndex + 1;

                            PortModel existingPort = null;

                            // Try to find existing port by tray/module/slot identifiers
                            existingPort = ports.FirstOrDefault(p =>
                                p.TrayID == trayNumber &&
                                p.ModuleID == moduleNumber &&
                                p.PortID == slotNumber);

                            if (reloadPorts && existingPort == null)
                            {
                                // Add new port
                                var newPort = new PortModel
                                {
                                    UniqueGUID = Guid.NewGuid(),
                                    SpliceGUID = splice.UniqueGUID,
                                    TrayID = trayNumber,
                                    ModuleID = moduleNumber,
                                    PortID = slotNumber,
                                    LatestRev = true,
                                    CreatedBy = "", // optionally set user id
                                    LastSavedBy = ""
                                };

                                var rowsAffected = await _portService.AddPortAsync(newPort);
                                if (rowsAffected > 0)
                                {
                                    ports.Add(newPort);
                                    existingPort = newPort;
                                }
                            }

                            var fiberLines = new List<string>();
                            string connType = "";
                            Guid? connectionTypeGuid = null;

                            if (existingPort != null && connectionDict.TryGetValue(existingPort.UniqueGUID, out var portConns))
                            {
                                foreach (var conn in portConns)
                                {
                                    if (!conn.FiberA_GUID.HasValue)
                                        continue;

                                    if (!fiberDict.TryGetValue(conn.FiberA_GUID.Value, out var fiberA) ||
                                        !cableDict.TryGetValue(fiberA.CableGUID, out var cableA))
                                        continue;

                                    string fiberA_ID = $"{cableA.CableID}.{fiberA.BufferID}.{fiberA.RibbonID}.{fiberA.FiberID}";
                                    string line = fiberA_ID;

                                    if (conn.FiberB_GUID == UNPATCHED_GUID)
                                    {
                                        // just fiber A
                                    }
                                    else if (conn.FiberB_GUID == EQUIPMENT_GUID)
                                    {
                                        line += " -> [Equipment]";
                                    }
                                    else if (conn.FiberB_GUID.HasValue &&
                                             fiberDict.TryGetValue(conn.FiberB_GUID.Value, out var fiberB) &&
                                             cableDict.TryGetValue(fiberB.CableGUID, out var cableB))
                                    {
                                        string fiberB_ID = $"{cableB.CableID}.{fiberB.BufferID}.{fiberB.RibbonID}.{fiberB.FiberID}";
                                        line += $" -> {fiberB_ID}";
                                    }

                                    if (circuitDict.TryGetValue(fiberA.UniqueGUID, out var circuit))
                                    {
                                        line += $"\n({circuit.CircuitID})";
                                    }

                                    fiberLines.Add(line);
                                    connType = conn.ConnectionType;
                                    connectionTypeGuid = conn.UniqueGUID;
                                }
                            }

                            slots.Add(new SpliceSlotViewModel
                            {
                                SlotNumber = slotId++,
                                TrayLabel = $"{trayLabel} {trayNumber}",
                                SlotLabel = $"{slotLabel} {slotNumber}",
                                ConnectionType = connType,
                                ConnectionTypeGUID = (Guid)connectionTypeGuid,
                                PortGUID = existingPort?.UniqueGUID ?? Guid.Empty,
                                FiberDetails = string.Join("\n", fiberLines)
                            });
                        }
                    }
                }

                // Load expressed connections grouped by RibbonID
                var expressedConns = connections
                    .Where(c => c.LatestRev && c.ConnectionType == "Expressed" && c.FiberA_GUID.HasValue && c.FiberB_GUID.HasValue)
                    .OrderBy(c => fiberDict.TryGetValue(c.FiberA_GUID.Value, out var f) ? f.RibbonID : 0)
                    .ToList();

                int? currentRibbon = null;
                var expressedLines = new List<string>();

                foreach (var conn in expressedConns)
                {
                    if (!fiberDict.TryGetValue(conn.FiberA_GUID.Value, out var fiberA) ||
                        !fiberDict.TryGetValue(conn.FiberB_GUID.Value, out var fiberB))
                        continue;

                    if (!cableDict.TryGetValue(fiberA.CableGUID, out var cableA) ||
                        !cableDict.TryGetValue(fiberB.CableGUID, out var cableB))
                        continue;

                    string fiberA_ID = $"{cableA.CableID}.{fiberA.BufferID}.{fiberA.RibbonID}.{fiberA.FiberID}";
                    string fiberB_ID = $"{cableB.CableID}.{fiberB.BufferID}.{fiberB.RibbonID}.{fiberB.FiberID}";

                    // New ribbon group detected - add previous group slot
                    if (currentRibbon.HasValue && fiberA.RibbonID != currentRibbon.Value)
                    {
                        slots.Add(new SpliceSlotViewModel
                        {
                            SlotNumber = slotId++,
                            TrayLabel = "Storage",
                            SlotLabel = "",
                            ConnectionType = "Expressed",
                            FiberDetails = string.Join("\n", expressedLines)
                        });

                        expressedLines.Clear();
                    }

                    currentRibbon = fiberA.RibbonID;
                    expressedLines.Add($"{fiberA_ID} -> {fiberB_ID}");
                }

                // Add last expressed ribbon group if any
                if (expressedLines.Count > 0)
                {
                    slots.Add(new SpliceSlotViewModel
                    {
                        SlotNumber = slotId++,
                        TrayLabel = "Storage",
                        SlotLabel = "",
                        ConnectionType = "Expressed",
                        FiberDetails = string.Join("\n", expressedLines)
                    });
                }

                result.Slots = slots;
                model.ConnectionListingViewModel = result;

                

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading connection list: {ex.Message}");
            }
        }
        #endregion




        #region Trace Fiber Functionality
        //public async Task<IActionResult> _GetTraceFiber(Guid spliceGuid, Guid portGuid, bool chkCSVformat = false)
        //{
        //    var fiberDetails = new StringBuilder();
        //    var fibers = new List<FiberModel>();
        //    string demarc = chkCSVformat ? "," : ".";

        //    // Get all connections from the splice
        //    var connectionList = await _connectionService.GetConnectionsAsync(spliceGuid);
        //    var selectedConnection = connectionList?.FirstOrDefault(c => c.PortGUID == portGuid);

        //    if (selectedConnection == null)
        //        return NotFound("Connection not found.");

        //    // Load initial fiber
        //    var fiber = await _fiberService.LoadFiberAsync(selectedConnection.FiberA_GUID ?? Guid.Empty, Guid.Empty, true);
        //    if (fiber == null)
        //        return NotFound("Initial fiber not found.");

        //    fibers.Add(fiber);

        //    // Load starting port
        //    var startPort = await _portService.GetPortByGuidAsync(portGuid);
        //    if (startPort == null)
        //        return NotFound("Start port not found.");

        //    // Add CSV header if needed
        //    if (chkCSVformat)
        //    {
        //        fiberDetails.AppendLine("CableID,BufferID,RibbonID,FiberID,FiberGUID");
        //        demarc = ",";
        //    }

        //    // If connection ends at Equipment
        //    if (selectedConnection.FiberB_GUID == Guid.Parse("22222222-2222-2222-2222-222222222222"))
        //    {
        //        var equipment = await _equipmentService.GetEquipmentAsync(selectedConnection.UniqueGUID, selectedConnection.PortGUID ?? Guid.Empty);
        //        var material = await _meterialService.GetMeterialsAsync(equipment.UniqueGUID, Guid.Empty, false, false);

        //        fiberDetails.AppendLine($"{equipment.EquipmentID}{demarc}{material.ModelID}{demarc}{demarc}{equipment.PortID}");
        //    }

        //    // Load splice info
        //    var startSplice = await _spliceService.GetSpliceByGuidAsync(spliceGuid, null);
        //    if (startSplice == null)
        //        return NotFound("Splice not found.");

        //    // Begin recursive trace
        //    await _fiberService.TraceConnectionsAsync(startPort,fiber, startSplice, chkCSVformat, fiberDetails, fibers);

        //    return PartialView(fiberDetails.ToString());
        //}

        public async Task<IActionResult> _GetTraceFiber(Guid spliceGuid, Guid portGuid, bool chkCSVformat = false)
        {
            var fiberDetails = new StringBuilder();
            var fibers = new List<FiberModel>();
            var fiberLines = new List<string>();
            string demarc = chkCSVformat ? "," : ".";

            var connectionList = await _connectionService.GetConnectionsAsync(spliceGuid);
            var selectedConnection = connectionList.FirstOrDefault(c => c.PortGUID == portGuid);
            if (selectedConnection == null) return NotFound("Connection not found.");

            var fiber = await _fiberService.LoadFiberAsync(selectedConnection.FiberA_GUID.Value, Guid.Empty, true);
            fibers.Add(fiber);

            var startPort = await _portService.GetPortByGuidAsync(portGuid);
            var startSplice = await _spliceService.GetSpliceByGuidAsync(spliceGuid, null);

            if (chkCSVformat)
                fiberDetails.AppendLine("CableID,BufferID,RibbonID,FiberID,FiberGUID");

            // Handle equipment segment
            if (selectedConnection.FiberB_GUID == Guid.Parse("22222222-2222-2222-2222-222222222222"))
            {
                var equipment = await _equipmentService.GetEquipmentAsync(selectedConnection.UniqueGUID, (Guid)selectedConnection.PortGUID);
                var material = await _meterialService.GetMeterialsAsync(equipment.UniqueGUID, Guid.Empty, false, false);
                var line = $"{equipment.EquipmentID}{demarc}{material.ModelID}{demarc}{demarc}{equipment.PortID}";
                fiberDetails.AppendLine(line);
                fiberLines.Add(line);
            }

            // Recursive trace
            await _fiberService.TraceConnectionsAsync(startPort, fiber, startSplice, chkCSVformat, fiberDetails, fibers);

            // Convert all lines to displayable format
            if (!chkCSVformat)
                fiberLines.AddRange(fiberDetails.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            var resultModel = new TraceFiberResultModel
            {
                FiberLines = fiberLines,
                FiberCount = fibers.Count,
                IsCSVFormat = chkCSVformat,
                TraceId = Guid.NewGuid() // Used for download, or skip if not required
            };

            return Ok(resultModel);
            //PartialView("resultModel");
        }

        #endregion




        #region View Connection Save button

        [HttpPost]
        public async Task<IActionResult> SaveConnection([FromBody] SaveConnectionRequestModel requestModel)
        {
            var selectedSplice = await _spliceService.GetSpliceByGuidAsync(requestModel.SpliceGUID, null);
            if (selectedSplice == null)
            {
                return ResponseHelper.HandleResponse(new ToasterModel
                {
                    IsError = true,
                    Message = "Invalid splice selected.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = HttpStatusCode.BadRequest
                }, HttpStatusCode.BadRequest);
            }

            var connectionList = await _connectionService.GetConnectionsAsync(selectedSplice.UniqueGUID);
            var portList = await _portService.GetPortsAsync(selectedSplice.UniqueGUID);

            foreach (var port in portList)
            {
                if (port.Updated)
                {
                    var portToUpdate = await _portService.GetPortByGuidAsync(port.UniqueGUID);
                    if (portToUpdate != null)
                    {
                        portToUpdate.ConnectionGUID = port.ConnectionGUID;
                        portToUpdate.CreatedBy = string.Empty;
                        portToUpdate.LastSavedBy = string.Empty;

                        await _portService.UpdatePortAsync(portToUpdate);
                    }
                }
            }

            foreach (var connection in connectionList)
            {
                connection.CreatedBy = string.Empty;
                connection.LastSavedBy = string.Empty;

                if (connection.UniqueID == 0 && connection.LatestRev)
                {
                    await _connectionService.AddConnectionAsync(connection);
                }
                else if (connection.UniqueID > 0 && !connection.LatestRev && connection.Updated)
                {
                    await _connectionService.UpdateConnectionAsync(connection);
                }
                else if (connection.UniqueID > 0 && connection.LatestRev && connection.Updated)
                {
                    await _connectionService.DeleteConnectionAsync(connection);
                }
                else if (connection.Updated)
                {
                    await _connectionService.UpdateConnectionAsync(connection);
                }
            }

            var result = new ToasterModel
            {
                IsError = false,
                Message = "Connection updated successfully.",
                Type = ToasterType.success.ToString(),
                StatusCode = HttpStatusCode.OK
            };

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }

        #endregion




        #region View Connection CSV Export button

        [HttpPost]
        public async Task<IActionResult> ExportPanelReport([FromBody] List<SpliceTrayRowRequestModel> spliceTrayRows)
        {
            if (spliceTrayRows == null || !spliceTrayRows.Any())
                return BadRequest("Empty input.");

            // Manual mapping from view model to domain model
            var fibers = spliceTrayRows.Select(row => new FiberModel
            {
                UniqueGUID = Guid.Empty,
                CableGUID = Guid.Empty,
                BufferID = 0,
                FiberID = 0,
                RibbonID = 0,
                FiberType = row.CartridgeID,
                Comments = row.SpliceID,
                LatestRev = true,
                CreatedBy = row.PortID,
                CreatedDate = DateTime.UtcNow,
                LastSavedBy = string.Empty,
                LastSavedDate = DateTime.UtcNow,
                Circuits = new List<CircuitModel>
                {
                    new CircuitModel
                    {
                        CircuitID = row.FiberCircuit,
                        Comments = null,
                        CreatedBy = string.Empty,
                        CreatedDate = DateTime.UtcNow,
                        LastSavedBy = string.Empty,
                        LastSavedDate = DateTime.UtcNow,
                        LatestRev = true
                    }
                }
            }).ToList();

            var csvBytes = await _reportService.GeneratePanelReportCsvAsync(fibers);

            // Set only the simple filename in Content-Disposition
            Response.Headers["Content-Disposition"] = "attachment; filename=PanelPortsReport.csv";
            return File(csvBytes, "text/csv");
        }
        #endregion
    }
}
