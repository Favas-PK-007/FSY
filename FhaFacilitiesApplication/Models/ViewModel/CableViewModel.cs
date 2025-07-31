using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class CableViewModel
    {
        public int UniqueID { get; set; } = 0;
        public Guid UniqueGUID { get; set; }
        public Guid CampusGUID { get; set; }
        [Required(ErrorMessage = "Cable Type is required")]
        public string CableType { get; set; }
        [Required(ErrorMessage = "Cable ID is required")]
        [StringLength(50, ErrorMessage = "Cable ID must be less than 50 characters")]
        public string CableID { get; set; }
        [Required(ErrorMessage = "Cable Model is required")]
        public Guid TypeGUID { get; set; }
        public Guid? SpliceAGUID { get; set; }
        public Guid? SpliceBGUID { get; set; }
        public Guid? DuctGUID { get; set; }
        [StringLength(1000, ErrorMessage = "Comments must be less than 1000 characters")]
        public string Comments { get; set; }
        public Guid SelectedCableModel { get; set; } // Selected Cable Model
        public List<SelectListItem> CableTypeList { get; set; } = new();
        public List<SelectListItem> CableModelList { get; set; } = new();
        public List<SelectListItem> SpliceAList { get; set; } = new();
        public List<SelectListItem> SpliceBList { get; set; } = new();
        public string Fibers { get; set; }


        public static List<CableViewModel> FromModelList(List<CableModel> modelList)
        {
            return modelList.Select(cable => new CableViewModel
            {
                UniqueID = cable.UniqueID,
                UniqueGUID = cable.UniqueGUID,
                CampusGUID = cable.CampusGUID,
                TypeGUID = cable.TypeGUID,
                SpliceAGUID = cable.SpliceA_GUID,
                SpliceBGUID = cable.SpliceB_GUID,
                DuctGUID = cable.DuctGUID,
                CableID = cable.CableID,
                Comments = cable.Comments,
                CableType = cable.CableType
            }).ToList();
        }

        public static CableViewModel FromModel(CableModel model)
        {
            return new CableViewModel
            {
                UniqueID = model.UniqueID,
                UniqueGUID = model.UniqueGUID,
                CampusGUID = model.CampusGUID,
                TypeGUID = model.TypeGUID,
                SpliceAGUID = model.SpliceA_GUID,
                SpliceBGUID = model.SpliceB_GUID,
                DuctGUID = model.DuctGUID,
                CableType = model.CableType,
                CableID = model.CableID,
                Comments = model.Comments,
                //Fibers = model.Fiber?.Select(FiberViewModel.FromModel).ToList(),

            };
        }

        public CableModel ToModel()
        {
            return new CableModel
            {
                UniqueID = UniqueID,
                UniqueGUID = UniqueGUID,
                CampusGUID = CampusGUID,
                TypeGUID = TypeGUID,
                SpliceA_GUID = (Guid)SpliceAGUID,
                SpliceB_GUID = (Guid)SpliceBGUID,
                //DuctGUID = DuctGUID is null ? Guid.Empty : DuctGUID,
                CableType = CableType,
                CableID = CableID,
                Comments = Comments,
                SelectedCableModel = SelectedCableModel,
                //Fiber = Fibers?.Select(f => f.ToModel()).ToList()
            };
        }
    }

    public class CableTypeViewModel
    {
        public string Text { get; set; } = null!;
        public string Value { get; set; } = null!;

        public static CableTypeViewModel FromModel(CableTypeModel model)
        {
            return new CableTypeViewModel
            {
                Text = model.Text,
                Value = model.Value
            };
        }
    }

    public class CableModelViewModel
    {
        public string Text { get; set; } = null!;
        public Guid Value { get; set; }
        public static CableModelViewModel FromModel(CableModel model)
        {
            return new CableModelViewModel
            {
                Text = model.Text,
                Value = model.Value
            };
        }
    }
}
