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
using System.Net;
#endregion

namespace FhaFacilitiesApplication.Controllers
{
    public class MaterialController : Controller
    {
        #region Declarations

        private readonly IMeterialService _meterialService;
        private readonly IMaterialDetailService _materialDetailService;
        #endregion

        #region Constructor

        public MaterialController(IMeterialService meterialService, IMaterialDetailService materialDetailService)
        {
            _meterialService = meterialService;
            _materialDetailService = materialDetailService;
        }
        #endregion


        public async Task<IActionResult> _CreateMaterial(bool isEdit = false)
        {
            var model = new DashboardViewModel();
            var materialTypes = await _meterialService.GetMaterialTypesAsync();
            var materialTypeList = materialTypes.Select(type => new SelectListItem
            {
                Value = type,
                Text = type
            }).ToList();
            model.MaterialViewModel.MaterialTypeList = materialTypeList;
            model.MaterialViewModel.IsEdit = isEdit;

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMaterial([FromBody] MaterialRequestModel requestModel)
        {
            if (requestModel == null)
            {
                var error = new ToasterModel
                {
                    IsError = true,
                    Message = "Invalid request data.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return ResponseHelper.HandleResponse(error, error.StatusCode);
            }

            if (!ModelState.IsValid)
            {
                var error = new ToasterModel
                {
                    IsError = true,
                    Message = "Invalid model state.",
                    Type = ToasterType.fail.ToString(),
                    StatusCode = HttpStatusCode.BadRequest
                };
                return ResponseHelper.HandleResponse(error, error.StatusCode);
            }

            var userId = ""; 
            var result = new ToasterModel();

            var materialModel = requestModel.ToModel();
            materialModel.LastSavedBy = userId;
            materialModel.UniqueID = requestModel.UniqueID;

            if (materialModel.UniqueID == 0)
            {
                // INSERT logic
                bool exists = await _meterialService.CheckIfMaterialExistsAsync(materialModel);

                if (exists)
                {
                    result = new ToasterModel
                    {
                        IsError = true,
                        Message = $"Material with Model ID '{materialModel.ModelID}' already exists.",
                        Type = ToasterType.warning.ToString(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                else
                {
                    materialModel.UniqueGUID = Guid.NewGuid();
                    materialModel.CreatedBy = userId;
                    materialModel.CreatedDate = DateTime.Now;
                    materialModel.LatestRev = true;
                    materialModel.TemplateGUID = Guid.Empty;

                    result = await _meterialService.SaveMaterialAsync(materialModel);
                }
            }
            else
            {
                // UPDATE logic
                materialModel.UniqueGUID =(Guid)requestModel.UniqueGuid;
               
                materialModel.CreatedBy = userId;

                result = await _meterialService.UpdateMaterialAsync(materialModel);
            }

            return ResponseHelper.HandleResponse(result, result.StatusCode);
        }

        public async Task<IActionResult> GetParentMaterial(string materialType)
        {
            if (string.IsNullOrWhiteSpace(materialType))
                return ResponseHelper.HandleResponse("Material Type is required.", HttpStatusCode.BadRequest);

            var parentMaterialResult = await _meterialService.GetParentMaterialDropdownAsync(materialType);

            if (parentMaterialResult == null || !parentMaterialResult.Any())
                return Ok(new List<SelectListItem>());

            return Ok(parentMaterialResult);

        }


        public async Task<IActionResult> GetChildMaterials(Guid parentGuid, string materialType)
        {
            string parentMaterialType;

            switch (materialType?.ToLower())
            {
                case "splice tray":
                    parentMaterialType = "Splice";
                    break;
                case "splice module":
                    parentMaterialType = "Splice Tray";
                    break;
                case "fpp cartridge":
                    parentMaterialType = "FPP Chassis";
                    break;
                case "fpp module":
                    parentMaterialType = "FPP Cartridge";
                    break;
                default:
                    parentMaterialType = materialType;
                    break;
            }
            var models = await _meterialService.GetChildMaterialsAsync(parentGuid, parentMaterialType);
            return Ok(models);
        }

        public async Task<IActionResult> GetManufacturers(string materialType)
        {
            var manufacturers = await _meterialService.GetManufacturersAsync(materialType);
            return Ok(manufacturers);
        }

        public async Task<IActionResult> GetMaterialDetailsHeaders(string materialType)
        {
            var headers = await _meterialService.GetDetailHeadersAsync(materialType);
            return Ok(headers);
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialByGuid(Guid selectedModelIdGuid)
        {
            if (selectedModelIdGuid == Guid.Empty)
                return BadRequest("Invalid Model ID.");

            var domainModel = await _meterialService.GetMaterialByGuidAsync(selectedModelIdGuid);
            if (domainModel == null)
                return NotFound("Material not found.");

            var details = await _materialDetailService.GetLoadDetailAsync(
                domainModel.MaterialType,
                string.IsNullOrEmpty(domainModel.TemplateGUID.ToString()) || domainModel.TemplateGUID.ToString() == Guid.Empty.ToString()
                    ? domainModel.UniqueGUID
                    : domainModel.TemplateGUID,Guid.Empty
            );

            domainModel.Details = details;

            var responseModel = new MaterialResponseModel
            {
                ManufacturerID = domainModel.ManufacturerID,
                ModelID = domainModel.ModelID,
                Comments = domainModel.Comments,
                UniqueGUID = domainModel.UniqueGUID,
                UniqueID = domainModel.UniqueID,
                MaterialType = domainModel.MaterialType,
                MaterialID = domainModel.MaterialID,
                Details = domainModel.Details ?? new Dictionary<string, string>()
            };

            return Ok(responseModel);
        }

    }
}
