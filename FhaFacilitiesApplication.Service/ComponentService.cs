using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System.Net;


namespace FhaFacilitiesApplication.Service
{
    public class ComponentService : IComponentService
    {
        #region Declarations

        private readonly IComponentRepository _componentRepository;
        private readonly IMeterialRepository _materialRepository;
        #endregion


        #region Constructor
        public ComponentService(IComponentRepository componentRepository, IMeterialRepository materialRepository)
        {
            _componentRepository = componentRepository;
            _materialRepository = materialRepository;
        }
        #endregion

        public async Task<List<MeterialModel>> GetComponentsAsync(string materialType, Guid parentGuid, bool loadDetails, bool loadSubComponents)
        {
            return await _componentRepository.GetComponentsAsync(materialType, parentGuid, loadDetails, loadSubComponents);
        }

        public async Task<ToasterModel> SaveComponentAsync(MeterialModel newComponent, MeterialModel parentMaterial, MeterialModel? parentComponent)
        {
            var materialType = newComponent.MaterialType.ToLowerInvariant();

            switch (materialType)
            {
                case "splice tray":
                case "fpp cartridge":
                    int numOfTrays = Convert.ToInt32(parentMaterial.Details["NumOfTrays"]);
                    int trayHeight = Convert.ToInt32(newComponent.Details["TrayHeight"]);

                    foreach (var existing in parentMaterial.Components)
                        numOfTrays -= Convert.ToInt32(existing.Details["TrayHeight"]);

                    if (numOfTrays < trayHeight)
                    {
                        return new ToasterModel
                        {
                            IsError = true,
                            Message = "Not enough space to install this new Component",
                            Type = ToasterType.fail.ToString(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    break;

                case "splice module":
                case "fpp module":
                    if (parentComponent == null)
                    {
                        return new ToasterModel
                        {
                            IsError = true,
                            Message = "Parent tray not found.",
                            Type = ToasterType.fail.ToString(),
                            StatusCode = HttpStatusCode.BadRequest
                        };
                    }

                    //int numOfModules = Convert.ToInt32(parentComponent.Details["NumOfModules"]);
                    //var trayComponents = parentComponent.Components ?? new List<MeterialModel>();

                    //if (trayComponents.Count >= numOfModules)
                    //{
                    //    return new ToasterModel
                    //    {
                    //        IsError = true,
                    //        Message = "Not enough space to install this Component",
                    //        Type = ToasterType.fail.ToString(),
                    //        StatusCode = HttpStatusCode.BadRequest
                    //    };
                    //}

                    break;

                default:
                    return new ToasterModel
                    {
                        IsError = true,
                        Message = "Unsupported material type.",
                        Type = ToasterType.fail.ToString(),
                        StatusCode = HttpStatusCode.BadRequest
                    };
            }

            // If all checks passed, create component
            var insertedId = await _materialRepository.SaveNewDuctMaterialAsync(newComponent);

            return new ToasterModel
            {
                IsError = insertedId <= 0,
                Message = insertedId > 0 ? "Component added successfully." : "Failed to add component.",
                Type = insertedId > 0 ? ToasterType.success.ToString() : ToasterType.fail.ToString(),
                StatusCode = insertedId > 0 ? HttpStatusCode.OK : HttpStatusCode.InternalServerError
            };
        }
    }
}

