using FhaFacilitiesApplication.Domain.Models.DomainModel;

namespace FhaFacilitiesApplication.Models.RequestModel
{
    public class ComponentRequestModel
    {
        public Guid EquipmentModelGuid { get; set; }
        public Guid SelectedComponentModelGuid { get; set; }
        public string MaterialType { get; set; }
        //public Guid? ParentComponentGuid { get; set; } 

        public MeterialModel ToModel(MeterialModel selectedComponent, MeterialModel parentMaterial, MeterialModel? parentComponent)
        {
            return new MeterialModel
            {
                UniqueGUID = Guid.NewGuid(),
                TemplateGUID = selectedComponent.UniqueGUID,
                MaterialType = selectedComponent.MaterialType,
                ManufacturerID = selectedComponent.ManufacturerID,
                ModelID = selectedComponent.ModelID,
                LatestRev = true,
                ParentGUID = MaterialType?.ToLowerInvariant() switch
                {
                    "splice module" or "fpp module" => parentComponent?.UniqueGUID ?? Guid.Empty,
                    _ => parentMaterial.UniqueGUID
                },
                Details = selectedComponent.Details
            };
        }

    }
}
