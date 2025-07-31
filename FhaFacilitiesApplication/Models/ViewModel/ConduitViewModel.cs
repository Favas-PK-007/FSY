#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#endregion

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class ConduitViewModel
    {
        public int UniqueId { get; set; }

        public Guid UniqueGuid { get; set; }

        public Guid CampusGuid { get; set; }

        [Required(ErrorMessage = "Conduit ID is required.")]
        [MaxLength(50, ErrorMessage = "Conduit ID cannot exceed 50 characters.")]
        public string ConduitId { get; set; }

        [Required(ErrorMessage = "Please select a structure.")]
        public Guid StructureAGuid { get; set; }

        [Required(ErrorMessage = "Please select a structure.")]
        public Guid StructureBGuid { get; set; }

        public string Comments { get; set; } 

        public List<DuctViewModel> DuctsList { get; set; } = new();

        public List<SelectListItem> StructureAList { get; set; } = new();
        public List<SelectListItem> StructureBList { get; set; } = new();
        public List<SelectListItem> DuctList { get; set; }
        public List<SelectListItem> SubDuctList { get; set; }

        public static ConduitViewModel FromModel(ConduitModel model)
        {
            return new ConduitViewModel
            {
                UniqueGuid = model.UniqueGUID,
                ConduitId = model.ConduitID,
                StructureAGuid = model.StructureA_GUID,
                StructureBGuid = model.StructureB_GUID,
                Comments = model.Comments,
                UniqueId = model.UniqueID,
                CampusGuid = model.CampusGUID,
                DuctsList = model.Ducts?.Select(DuctViewModel.FromModel).ToList() ?? new()
            };
        }

        public ConduitModel ToModel()
        {
            return new ConduitModel
            {
                CampusGUID = CampusGuid,
                ConduitID = ConduitId,
                StructureA_GUID = StructureAGuid.ToString() == null ?  Guid.Empty : StructureAGuid,
                StructureB_GUID = StructureBGuid.ToString() == null ?  Guid.Empty : StructureBGuid,
                Comments = Comments != null ? Comments : string.Empty,
                Ducts = DuctsList?.Select(d => d.ToModel()).ToList() ?? null,
                UniqueGUID = UniqueGuid,
                UniqueID = UniqueId


            };
        }
    }
}
