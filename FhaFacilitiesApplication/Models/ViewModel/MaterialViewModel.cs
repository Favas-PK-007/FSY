using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class MaterialViewModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public string MaterialType { get; set; }
        public string MaterialID { get; set; }
        public string ManufacturerID { get; set; }
        public string ModelID { get; set; }
        public List<MaterialViewModel> Children { get; set; }
        public Guid ParentGuid { get; set; }
        public  Guid  TemplateGuid { get; set; }
        public string Comments { get; set; }

        // For Material Modal dropdowns
        public List<SelectListItem> MaterialTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ManufacturerList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ParentMaterialList { get; set; } = new List<SelectListItem>();

        // To identify the action
        public bool IsEdit { get; set; }

        public static MaterialViewModel FromModel(MeterialModel model)
        {
            return new MaterialViewModel
            {
                UniqueID = model.UniqueID,
                UniqueGUID = model.UniqueGUID,
                MaterialType = model.MaterialType,
                MaterialID = model.MaterialID,
                ManufacturerID = model.ManufacturerID,
                ModelID = model.ModelID,
                
            };
        }

        public static List<MaterialViewModel> FromModelList(List<MeterialModel> modelList)
        {
            return modelList.Select(m => FromModel(m)).ToList();
        }
        public MeterialModel ToModel()
        {
            return new MeterialModel
            {
                UniqueID = UniqueID,
                UniqueGUID = UniqueGUID,
                MaterialType = MaterialType,
                MaterialID = MaterialID,
                ManufacturerID = ManufacturerID,
                ModelID = ModelID,
                ParentGUID = ParentGuid,
                TemplateGUID = TemplateGuid,
                Comments = Comments
            };
        }
    }
}

