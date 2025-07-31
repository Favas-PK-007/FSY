using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Controllers
{
    public class EquipmentController : Controller
    {
        #region Declarations

        private readonly IEquipmentService _equipmentService;
        private readonly IConnectionService _connectionService;
        private readonly IPortService _portService;
        private readonly ISpliceService _spliceService;
        #endregion

        #region Constructor
        public EquipmentController(IEquipmentService equipmentService, IConnectionService connectionService,
            IPortService portService, ISpliceService spliceService)
        {
            _equipmentService = equipmentService;
            _connectionService = connectionService;
            _portService = portService;
            _spliceService = spliceService;
        }
        #endregion

        public async Task<IActionResult> _CreateEquipment(Guid connectionGuid)
        {
            var model = new DashboardViewModel();

            var selectedConnectionType = await _connectionService.GetConnectionByGuidAsync(connectionGuid);

            var portData = await _portService.GetPortByGuidAsync((Guid)selectedConnectionType.PortGUID);

            var spliceData = await _spliceService.GetSpliceByGuidAsync((Guid)selectedConnectionType.SpliceGUID, null);

            var equipmentDict = await _equipmentService.GetInstalledEquipmentAsync(spliceData.StructureGUID, spliceData.UniqueGUID);

            var equipmentTypes = await _equipmentService.GetEquipmentTypesAsync();

            var equipmentModel = new EquipmentModel
            {
                CampusGUID = spliceData.CampusGUID,
                StructureGUID = spliceData.StructureGUID,
                ConnectionType = "Equipment",
                SpliceGUID = selectedConnectionType.SpliceGUID,
                PortGUID = selectedConnectionType.PortGUID,
                FiberA_GUID = selectedConnectionType.FiberA_GUID,
                FiberB_GUID = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Sequence = 1,
                UniqueGUID = Guid.NewGuid(),
                TypeGUID = Guid.Empty
            };

            model.EquipmentViewModel = new EquipmentViewModel
            {
                UniqueGUID = equipmentModel.UniqueGUID,
                CampusGUID = equipmentModel.CampusGUID,
                StructureGUID = equipmentModel.StructureGUID,
                SpliceGUID = (Guid)equipmentModel.SpliceGUID,
                PortGUID = (Guid)equipmentModel.PortGUID,
                FiberA_GUID = (Guid)equipmentModel.FiberA_GUID,
                FiberB_GUID = equipmentModel.FiberB_GUID,
                ConnectionType = equipmentModel.ConnectionType,
                EquipmentTypeList = new List<SelectListItem>(),
                EquipmentIdList = new List<SelectListItem>(),
                EquipmentModelList = new List<SelectListItem>()
            };

            model.EquipmentViewModel.EquipmentTypeList = equipmentTypes
                                    .Select(type => new SelectListItem
                                    {
                                        Text = type,
                                        Value = type
                                    }).ToList();

            model.EquipmentViewModel.EquipmentIdList = equipmentDict
                                    .Select(kvp => new SelectListItem
                                    {
                                        Value = kvp.Key.ToString(),
                                        Text = kvp.Value.EquipmentID
                                    }).ToList();
            model.EquipmentViewModel.EquipmentPortIdList = null;

            return PartialView(model);
        }

        public async Task<IActionResult> _GetEquipmentModel(string equipmentType)
        {
            var model = new DashboardViewModel();

            var equipmentModelData = await _equipmentService.GetEquipmentModelAsync(equipmentType);

            model.EquipmentViewModel.EquipmentModelList = equipmentModelData
                         .Select(m => new SelectListItem
                         {
                             Value = m.UniqueGUID.ToString(),
                             Text = m.ToString()
                         }).ToList();
            

            return PartialView(model.EquipmentViewModel);
        }

        public async Task<IActionResult> _GetPortId(string equipmentType,Guid? selectedEquipmentModel,Guid equimentStructureGuid,Guid equipmentSpliceGuid, Guid equipmentTypeGuid,string selectedEquipmentId,int equipmentUniqueId,bool isChecked = false)
        {
            var model = new DashboardViewModel();

            var equipmentModelData = await _equipmentService.GetEquipmentModelAsync(equipmentType);

            var portdropdownList = new List<SelectListItem>();

            if (selectedEquipmentModel is not null)
            {
                if (isChecked)
                {
                    foreach (var detailDict in equipmentModelData.Select(x => x.Details))
                    {
                        foreach (var kvp in detailDict)
                        {
                            if (kvp.Key.Length > 3 && kvp.Key.Substring(0, 4).Equals("Port", StringComparison.OrdinalIgnoreCase))
                            {
                                portdropdownList.Add(new SelectListItem
                                {
                                    Text = $"{kvp.Value} ({kvp.Key})",
                                    Value = kvp.Key
                                });
                            }
                        }
                    }
                }
                else
                {
                    Dictionary<int, string> availablePorts = await _equipmentService
                        .GetAvailableEquipmentPortAsync(equimentStructureGuid, equipmentSpliceGuid, equipmentTypeGuid, selectedEquipmentId);

                    if (equipmentUniqueId > 0)
                    {
                        // Optional: Add default selected port from current equipment if needed
                        // portdropdownList.Add(new SelectListItem { Text = currentPort, Value = currentPort });
                    }

                    foreach (string portId in availablePorts.Values)
                    {
                        portdropdownList.Add(new SelectListItem
                        {
                            Text = portId,
                            Value = portId
                        });
                    }
                }
            }

            model.EquipmentViewModel.EquipmentPortIdList = portdropdownList;

            return PartialView( model.EquipmentViewModel);
        }

        public async Task<IActionResult> SaveEquipment([FromBody] EquipmentRequestModel entryModel)
        {
            var userId = ""; // Get from token/context as needed
            var result = new ToasterModel();
            bool newEquipment = true;

            // Validation checks
            if (entryModel.IsNewEquipment && string.IsNullOrEmpty(entryModel.EquipmentID))
            {
                result.IsError = true;
                result.Type = ToasterType.fail.ToString();
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Message = "Equipment ID is required.";
                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }

            if (string.IsNullOrEmpty(entryModel.PortID))
            {
                result.IsError = true;
                result.Type = ToasterType.fail.ToString();
                result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                result.Message = "Port ID is required.";
                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }

            // Map to domain model
            var domainModel = new EquipmentModel
            {
                IsNewEquipment = entryModel.IsNewEquipment,
                UniqueID = entryModel.UniqueID,
                UniqueGUID = entryModel.UniqueGUID,
                CampusGUID = entryModel.CampusGUID,
                StructureGUID = entryModel.StructureGUID,
                EquipmentType = entryModel.EquipmentType,
                PortID = entryModel.PortID,
                EquipmentID = entryModel.EquipmentID,
                TypeGUID = entryModel.TypeGUID,
                ConnectionType = entryModel.ConnectionType,
                SpliceGUID = entryModel.SpliceGUID,
                PortGUID = entryModel.PortGUID,
                FiberA_GUID = entryModel.FiberA_GUID,
                FiberB_GUID = entryModel.FiberB_GUID,
                Sequence = entryModel.Sequence,
                Comments = entryModel.Comments,
                CreatedBy = userId,
                LastSavedBy = userId
            };

            if (domainModel.UniqueID > 0)
            {
                // Update scenario
                result = await _equipmentService.SaveNewEquipmentMaterialAsync(domainModel);
                if (result.IsError)
                    return ResponseHelper.HandleResponse(result, result.StatusCode);

                result.IsError = false;
                result.Message = "Equipment updated successfully.";
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Type = ToasterType.success.ToString();

                return ResponseHelper.HandleResponse(result, result.StatusCode);
            }
            else
            {
                // Add new equipment
                if (domainModel.IsNewEquipment)
                {
                    var isExists = await _equipmentService.IsEquipmentExistAsync(
                        domainModel.EquipmentID,
                        domainModel.StructureGUID,
                        (Guid)domainModel.SpliceGUID
                    );

                    if (isExists)
                    {
                        result.IsError = true;
                        result.Type = ToasterType.fail.ToString();
                        result.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        result.Message = "Equipment already exists.";
                        return ResponseHelper.HandleResponse(result, result.StatusCode);
                    }

                    newEquipment = false;
                    domainModel.UniqueGUID = Guid.NewGuid();
                    domainModel.LatestRev = true;

                    result = await _equipmentService.SaveNewEquipmentMaterialAsync(domainModel);
                    if (result.IsError)
                        return ResponseHelper.HandleResponse(result, result.StatusCode);

                    result = await _equipmentService.SaveEquipmentPortsAsync(newEquipment, domainModel);
                    if (result.IsError)
                        return ResponseHelper.HandleResponse(result, result.StatusCode);

                    // Update connection and port metadata
                    var existingConnection = await _connectionService.GetConnectionByGuidAsync(entryModel.ConnectionGUID);
                    if (existingConnection != null)
                    {
                        existingConnection.LastSavedBy = userId;
                        await _connectionService.UpdateConnectionAsync(existingConnection);
                    }

                    var existingPort = await _portService.GetPortByGuidAsync((Guid)entryModel.PortGUID);
                    if (existingPort != null)
                    {
                        existingPort.LastSavedBy = userId;
                        await _portService.UpdatePortAsync(existingPort);
                    }

                    result.IsError = false;
                    result.Message = "Equipment saved successfully.";
                    result.StatusCode = System.Net.HttpStatusCode.OK;
                    result.Type = ToasterType.success.ToString();

                    return ResponseHelper.HandleResponse(result, result.StatusCode);
                }
            }

            // Fallback error
            result.IsError = true;
            result.Message = "Something went wrong.";
            result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            result.Type = ToasterType.fail.ToString();

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }
    }
}
