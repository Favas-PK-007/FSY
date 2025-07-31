using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class SpliceViewModel
    {
        public int UniqueId { get; set; }
        public Guid UniqueGuid { get; set; }
        public Guid StructureGUID { get; set; }
        public string SpliceType { get; set; }
        [Required(ErrorMessage = "Splice ID is required")]
        [StringLength(50)]
        public string SpliceID { get; set; }
        [Required(ErrorMessage = "Please select a equipment model")]

        public Guid TypeGUID { get; set; }
        public string? Comments { get; set; }
        public StructureViewModel Structure { get; set; }
        public List<SelectListItem> InstallationTypeList { get; set; }
        public List<SelectListItem> EquipmentTypeList { get; set; }
        public List<SelectListItem> StructureIdList { get; set; }
        public List<StructureViewModel> StructureIds { get; set; }
        public List<SelectListItem> EquipmentModelList { get; set; }
        public Guid CampusGuid { get; set; }
        public  string SelectedInstallationType { get; set; }
        public string SelectedEquipmentType { get; set; }
        public Guid SelectedStructureID { get; set; }
        public Guid SelectedEquipmentModel { get; set; }
        public List<MaterialViewModel> Components { get; set; }

        public static SpliceViewModel FromModel(SpliceModel model)
        {
            return new SpliceViewModel
            {
                StructureGUID = model.StructureGUID,
                SpliceType = model.SpliceType,
                SpliceID = model.SpliceID,
                TypeGUID = model.TypeGUID,
                Comments = model.Comments,
                Structure = StructureViewModel.FromModel(model.Structure),
                UniqueGuid = model.UniqueGUID,
                UniqueId = model.UniqueID,
                CampusGuid = model.CampusGUID,
                //Components = model.Structure?.Components?.Select(c => MaterialViewModel.FromModel(c)).ToList() ?? new List<MaterialViewModel>(),

            };
        }

        public static List<SpliceViewModel> FromModelList(List<SpliceModel> modelList)
        {
            return modelList.Select(splice => new SpliceViewModel
            {
                StructureGUID = splice.StructureGUID,
                SpliceType = splice.SpliceType,
                SpliceID = splice.SpliceID,
                TypeGUID = splice.TypeGUID,
                Comments = splice.Comments,
                UniqueGuid = splice.UniqueGUID,
                UniqueId = splice.UniqueID
            }).ToList();
        }

        public SpliceModel ToModel()
        {
            return new SpliceModel
            {
                UniqueID = UniqueId,
                UniqueGUID = UniqueGuid,
                StructureGUID = StructureGUID != Guid.Empty ? StructureGUID : Guid.Empty,
                SpliceType = SpliceType,
                SpliceID = SpliceID,
                TypeGUID = TypeGUID != Guid.Empty ? TypeGUID : Guid.Empty,
                Comments = Comments,
                CampusGUID = CampusGuid
            };
        }
    }
}
