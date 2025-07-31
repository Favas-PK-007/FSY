using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System.ComponentModel.DataAnnotations;

namespace FhaFacilitiesApplication.Models.RequestModel
{
    public class MaterialRequestModel
    {
        [Required(ErrorMessage = "Material Type is required")]
        public string MaterialType { get; set; }
        public Guid? UniqueGuid { get; set; }
        public int UniqueID { get; set; }
        public string ManufacturerID { get; set; }
        [Required(ErrorMessage = "Model ID is required")]
        [StringLength(50, ErrorMessage = "Model ID cannot exceed 50 characters")]
        public string ModelID { get; set; }
        public string Comments { get; set; } = null;
        public Guid? ParentGuid { get; set; }
        public List<MaterialDetailRequestModel> MaterialDetails { get; set; }

        public MeterialModel ToModel()
        {
            return new MeterialModel
            {
                MaterialType = MaterialType,
                ManufacturerID = ManufacturerID,
                ModelID = ModelID,
                Comments = Comments,
                ParentGUID = ParentGuid ?? Guid.Empty,
                Details = MaterialDetails?.ToDictionary(
                    detail => detail.Header,
                    detail => detail.Value) ?? new Dictionary<string, string>(),
            };
        }
    }

    public class MaterialDetailRequestModel
    {
        public string Header { get; set; }
        public string Value { get; set; }
    }
}
