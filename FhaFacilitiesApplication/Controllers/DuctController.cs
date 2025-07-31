#region Namespaces
using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using FhaFacilitiesApplication.Helper;
using FhaFacilitiesApplication.Models.RequestModel;
using FhaFacilitiesApplication.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using static FhaFacilitiesApplication.Models.ViewModel.DuctViewModel;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class DuctController : Controller
    {

        #region Declarations
        private readonly IDuctService _ductService;
        private readonly IStructureService _structureService;
        private readonly IMeterialService _materialService;
        private readonly IConduitPathService _conduitPathService;
        private readonly ICableService _cableService;
        #endregion

        #region Constructor
        public DuctController(IDuctService ductService, IStructureService structureService,
            IMeterialService meterialService, IConduitPathService conduitPathService, ICableService cableService)
        {
            _ductService = ductService;
            _structureService = structureService;
            _materialService = meterialService;
            _conduitPathService = conduitPathService;
            _cableService = cableService;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> _GetAllDucts(Guid? conduitGuid)
        {
            try
            {
                var model = new DashboardViewModel();

                if (conduitGuid is null)
                {
                    return PartialView(model);
                }

                var ducts = await _ductService.GetDuctsAsync((Guid)conduitGuid, true);

                if (ducts.Any())
                {
                    model.Ducts = DuctViewModel.FromModelList(ducts);
                }

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving ducts: " + ex.Message);
            }

        }

        public async Task<IActionResult> _CreateDuct(Guid? conduitGuid, Guid? structureAGuid, Guid? ductGuid)
        {
            try
            {
                var model = new DashboardViewModel();

                if (conduitGuid == null || structureAGuid == null)
                {
                    return PartialView(model);
                }

                var conduitPathData = await _conduitPathService.GetConduitByUniqueGuid((Guid)conduitGuid);


                if (ductGuid is not null)
                {
                    var duct = await _ductService.GetDuctByGuidAsync((Guid)ductGuid, true, "");
                    var structureA = duct.Structure.StructureID;
                    var structureB = await _structureService.GetStructureAsync((Guid)conduitPathData.StructureB_GUID);
                    var subDuctsData = await _ductService.GetDuctsAsync(duct.UniqueGUID, false);
                    var ductModel = new DuctViewModel
                    {
                        UniqueId = duct.UniqueID,
                        UniqueGuid = duct.UniqueGUID,
                        DuctId = duct.DuctID,
                        Comments = duct.Comments,
                        DuctIdb = duct.DuctID_B,
                        StructureA = structureA,
                        StructureB = structureB.StructureID,
                        StructureAGuid = duct.StructureGUID,
                        StructureBGuid = structureB.UniqueGUID,
                        ConduitGuid = conduitGuid ?? Guid.Empty,
                        DuctTypesList = duct.Material != null
                        ? new List<SelectListItem>
                        {
                            new SelectListItem
                            {
                                Value = duct.Material.UniqueGUID.ToString(),
                                Text = duct.Material.ModelID,
                            }
                        }
                        : new List<SelectListItem>(),
                        DuctType = duct.Material?.ModelID ?? string.Empty,
                        SubDuctList = subDuctsData.Select(m => new SubDuctItem
                        {
                            Id = m.UniqueGUID,
                            Label = m.ToString()
                        }).ToList()

                    };

                    model.DuctViewModel = ductModel;

                    return PartialView(model);
                }
                else
                {
                    var structureA = await _structureService.GetStructureAsync((Guid)structureAGuid);
                    var structureB = await _structureService.GetStructureAsync((Guid)conduitPathData.StructureB_GUID);
                    var ductTypesList = await _ductService.GetDuctTypesAsync(true, "Duct", Guid.Empty, Guid.Empty);

                    var ductModel = new DuctViewModel
                    {
                        Comments = string.Empty,
                        DuctId = string.Empty,
                        DuctIdb = string.Empty,
                        StructureA = structureA.StructureID,
                        StructureAGuid = (Guid)structureAGuid,
                        StructureB = structureB.StructureID,
                        StructureBGuid = structureB.UniqueGUID,
                        ConduitGuid = conduitGuid ?? Guid.Empty,
                        DuctTypesList = ductTypesList.Select(dt => new SelectListItem
                        {
                            Value = dt.Value.ToString(),
                            Text = dt.Text
                        }).ToList(),
                        SubDuctList = new List<SubDuctItem>(),
                    };
                    model.DuctViewModel = ductModel;

                    return PartialView(model);

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving duct: " + ex.Message);
            }
        }


        public async Task<IActionResult> _CreateSubDuct(Guid? conduitGuid, Guid? structureAGuid, Guid? ductGuid)
        {
            try
            {
                var model = new DashboardViewModel();

                if (conduitGuid == null || structureAGuid == null || ductGuid == null)
                {
                    return PartialView(model);
                }

                var conduitPathData = await _conduitPathService.GetConduitByUniqueGuid((Guid)conduitGuid);

                var duct = await _ductService.GetDuctByGuidAsync((Guid)ductGuid, true, "");
                var structureA = duct.Structure.StructureID;
                var structureB = await _structureService.GetStructureAsync((Guid)conduitPathData.StructureB_GUID);
                //var subDuctsData = await _materialService.GetSubDuctsAsync(duct.UniqueGUID, duct.TypeGUID);
                var ductTypesList = await _ductService.GetDuctTypesAsync(true, "Duct", Guid.Empty, Guid.Empty);

                var ductModel = new DuctViewModel
                {
                    //UniqueId = duct.UniqueID,
                    //UniqueGuid = duct.UniqueGUID,
                    //DuctId = duct.DuctID,
                    //Comments = duct.Comments,
                    //DuctIdb = duct.DuctID_B,
                    ParentGuid = ductGuid,
                    StructureA = structureA,
                    StructureB = structureB.StructureID,
                    StructureAGuid = duct.StructureGUID,
                    StructureBGuid = structureB.UniqueGUID,
                    ConduitGuid = conduitGuid ?? Guid.Empty,
                    DuctTypesList = ductTypesList.Select(dt => new SelectListItem
                    {
                        Value = dt.Value.ToString(),
                        Text = dt.Text
                    }).ToList(),
                    DuctType = duct.Material?.ModelID ?? string.Empty
                };

                model.DuctViewModel = ductModel;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving duct: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveDuct([FromBody] DuctViewModel entryModel)
        {
            if (!ModelState.IsValid)
            {
                var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
                return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
            }

            ToasterModel<Guid> result;
            var userId = "";

            var conduitPathData = await _conduitPathService.GetConduitByUniqueGuid(entryModel.ConduitGuid);
            var domainModel = entryModel.ToModel();
            domainModel.CreatedBy = userId;
            domainModel.LastSavedBy = userId;
            domainModel.StructureGUID = conduitPathData.StructureA_GUID;

            // Check if this is an update or a create
            if (entryModel.UniqueId > 0)
            {
                result = await _ductService.UpdateDuctAsync(domainModel);
            }
            else
            {
                domainModel.UniqueGUID = Guid.NewGuid();
                domainModel.MeterialGuid = Guid.NewGuid();
                result = await _ductService.SaveDuctAsync(domainModel);
            }

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubDuct([FromBody] DuctViewModel subductEntryModel)
        {
            if (!ModelState.IsValid)
            {
                var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
                return Ok(toasterModel);
            }

            ToasterModel<Guid> result;
            var userId = "";

            var conduitPathData = await _conduitPathService.GetConduitByUniqueGuid(subductEntryModel.ConduitGuid);
            var domainModel = subductEntryModel.ToModel();
            domainModel.CreatedBy = userId;
            domainModel.LastSavedBy = userId;
            domainModel.ConduitGUID = (Guid)subductEntryModel.ParentGuid; // Main Duct GUID is used as ConduitGUID for SubDucts
            domainModel.StructureGUID = conduitPathData.StructureA_GUID;
            domainModel.UniqueGUID = Guid.NewGuid();
            result = await _ductService.SaveDuctAsync(domainModel);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDuct([FromBody] UniqueGUIDRequest request)
        {
            try
            {
                var ductModel = await _ductService.GetDuctByGuidAsync(request.UniqueGUID, false, "Delete");

                if (ductModel == null)
                {
                    return NotFound(new ToasterModel
                    {
                        IsError = true,
                        Message = "Duct not found.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    });
                }

                var result = await _ductService.DeleteDuctAsync(ductModel);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the duct." + ex.Message);
            }
        }


        #region Link to Duct
        public async Task<IActionResult> _CreateLinkToDuct(Guid cableGuid, Guid? structureGuid)
        {
            var model = new DashboardViewModel();

            try
            {
                var cable = await _cableService.GetCableByGuidAsync(cableGuid, null);
                var structure = await _structureService.GetStructureAsync(structureGuid.Value);
                var conduits = await _conduitPathService.GetConduitsAsync(
                    cable.CampusGUID.ToString(), structure.UniqueGUID.ToString(), true);

                var conduitDropdownList = new List<SelectListItem>();
                var ductDropdownList = new List<SelectListItem>();
                var subDuctDropdownList = new List<SelectListItem>();

                Guid? selectedConduitGuid = null;
                Guid? selectedDuctGuid = null;
                Guid? selectedSubDuctGuid = null;
                bool isSubDuctEnabled = false;

                // Loop through conduits
                foreach (var conduit in conduits)
                {
                    conduitDropdownList.Add(new SelectListItem
                    {
                        Text = conduit.ConduitID,
                        Value = conduit.UniqueGUID.ToString()
                    });

                    // Load ducts for this conduit
                    conduit.Ducts = await _ductService.GetDuctsAsync(conduit.UniqueGUID, true);

                    for (int ductIndex = 0; ductIndex < conduit.Ducts.Count; ductIndex++)
                    {
                        var duct = conduit.Ducts[ductIndex];

                        ductDropdownList.Add(new SelectListItem
                        {
                            Text = duct.ToString(),
                            Value = duct.UniqueGUID.ToString()
                        });

                        // If duct has subducts
                        if (duct.SubDucts != null && duct.SubDucts.Count > 0)
                        {
                            bool hasMatchingSubduct = false;

                            for (int subDuctIndex = 0; subDuctIndex < duct.SubDucts.Count; subDuctIndex++)
                            {
                                var subDuct = duct.SubDucts[subDuctIndex];

                                subDuctDropdownList.Add(new SelectListItem
                                {
                                    Text = subDuct.ToString(),
                                    Value = subDuct.UniqueGUID.ToString()
                                });

                                // If current subduct matches cable.DuctGUID
                                if (subDuct.UniqueGUID == cable.DuctGUID)
                                {
                                    selectedConduitGuid = conduit.UniqueGUID;
                                    selectedDuctGuid = duct.UniqueGUID;
                                    selectedSubDuctGuid = subDuct.UniqueGUID;
                                    hasMatchingSubduct = true;
                                }
                            }

                            if (hasMatchingSubduct)
                                isSubDuctEnabled = true;
                            else
                            {
                                subDuctDropdownList.Clear();
                                isSubDuctEnabled = false;
                            }
                        }

                        // If duct itself matches the cable.DuctGUID (not subduct)
                        if (duct.UniqueGUID == cable.DuctGUID)
                        {
                            selectedConduitGuid = conduit.UniqueGUID;
                            selectedDuctGuid = duct.UniqueGUID;
                            isSubDuctEnabled = false; // No subduct selected
                            subDuctDropdownList.Clear(); // Reset subducts
                        }
                    }

                    // If nothing selected, clear duct and subduct lists
                    if (selectedDuctGuid == null && selectedSubDuctGuid == null)
                    {
                        ductDropdownList.Clear();
                        subDuctDropdownList.Clear();
                        isSubDuctEnabled = false;
                    }
                    else
                    {
                        // Exit outer loop once a match is found
                        break;
                    }
                }

                var viewModel = new CableDuctBindingViewModel
                {
                    CableID = cable.CableID,
                    CableDuctGuid = cable.DuctGUID,
                    ConduitDropdownList = conduitDropdownList,
                    DuctDropdownList = ductDropdownList,
                    SubDuctDropdownList = subDuctDropdownList,
                    SelectedConduitGuid = selectedConduitGuid ?? null,
                    SelectedDuctGuid = selectedDuctGuid ?? null,
                    SelectedSubDuctGuid = selectedSubDuctGuid ?? null,
                    IsSubDuctEnabled = isSubDuctEnabled
                };

                model.CableDuctBindingViewModel = viewModel;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching cable binding data: " + ex.Message);
            }
        }
        #endregion

        #region Load Ducts based on conduit in LinkToDuct Modal

        public async Task<IActionResult> _GetDuctsByConduit(Guid conduitGuid,Guid cableDuctGuid)
        {
            try
            {
                var model = new CableDuctBindingViewModel();

                // Get the selected conduit
                var conduit = await _conduitPathService.GetConduitByUniqueGuid(conduitGuid);

                // Get the list of ducts under that conduit
                conduit.Ducts = await _ductService.GetDuctsAsync(conduitGuid, true);

                // Populate the duct dropdown
                var ductDropdown = new List<SelectListItem>();
                foreach (var duct in conduit.Ducts)
                {
                    ductDropdown.Add(new SelectListItem
                    {
                        Text = duct.ToString(),
                        Value = duct.UniqueGUID.ToString(),
                        Selected = duct.UniqueGUID == cableDuctGuid
                    });
                }

                // Assign populated list to model
                model.DuctDropdownList = ductDropdown;

                return PartialView(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error retrieving ducts: " + ex.Message);
            }
        }
        #endregion


        #region Load SubDucts based on duct in LinkToDuct Modal

        public async Task<IActionResult> _GetSubDuctsByDuct(Guid ductGuid, Guid cableDuctGuid)
        {
            try
            {
                var model = new CableDuctBindingViewModel();

                // Fetch the duct and its subducts
                var duct = await _ductService.GetDuctByGuidAsync(ductGuid, true, "");

                var subducts = duct?.SubDucts ?? new List<DuctModel>();

                // Populate subduct dropdown list
                model.SubDuctDropdownList = subducts.Select(subduct => new SelectListItem
                {
                    Text = subduct.ToString(),  
                    Value = subduct.UniqueGUID.ToString(),
                    Selected = subduct.UniqueGUID == cableDuctGuid
                }).ToList();

                return PartialView(model);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving subducts: " + ex.Message);
            }
        }

        #endregion


        //public async Task<IActionResult> SaveLinkDuct([FromBody] CableDuctBindingViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var toasterModel = ModelStateHelper.ReturnModelSateInfo(ModelState.Values);
        //        return ResponseHelper.HandleResponse(toasterModel, toasterModel.StatusCode);
        //    }
        //    try
        //    {
        //        var result = await _cableService.LinkCableToDuctAsync(model.CableID, model.SelectedConduitGuid, model.SelectedDuctGuid, model.SelectedSubDuctGuid);
        //        return ResponseHelper.HandleResponse(result, result.StatusCode);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, "Error linking cable to duct: " + ex.Message);
        //    }
        //}
    }

}
