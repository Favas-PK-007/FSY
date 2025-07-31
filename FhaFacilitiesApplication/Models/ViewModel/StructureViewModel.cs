#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
//using FhaFacilitiesApplication.ValidationExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
#endregion

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class StructureViewModel
    {
        [Required(ErrorMessage = "Structure Type is required.")]
        [Display(Name = "Structure Type")]
        [StringLength(50, ErrorMessage = "Structure Type cannot be longer than 50 characters")]
        public string StructureType { get; set; }

        [Display(Name = "Location Description")]
        [Required(ErrorMessage = "Location Description is required.")]
        [StringLength(100, ErrorMessage = "Location Description cannot be longer than 100 characters")]
        public string LocationDescription { get; set; }
        
        //[Required(ErrorMessage = "Building Id is required.")]
        //[Display(Name = "Building Id")]
        //[StringLength(50, ErrorMessage = "Building Id cannot be longer than 50 characters")]
        //public string BuildingId { get; set; }
        
        [Required(ErrorMessage = "Structure Id is required.")]
        [Display(Name = "Structure Id")]
        [StringLength(50, ErrorMessage = "Structure Id cannot be longer than 50 characters")]
        public string StructureId { get; set; }
        public string? StructureModel { get; set; }
        [GeoCoordinateValidation]
        public double Latitude { get; set; } = 0.0;
        [GeoCoordinateValidation]
        public double Longitude { get; set; } = 0.0;
        public string Comments { get; set; }
        public Guid CampusGuid { get; set; } 
        public Guid? BuildingGuid { get; set; }
        public int UniqueId { get; set; } = 0;
        public Guid UniqueGuid { get; set; }
        public Guid? TypeGuid { get; set; }

        public List<SelectListItem> StructureTypeList { get; set; } = new();
        public List<SelectListItem> LocationDescriptionList { get; set; } = new();
        public List<SelectListItem> BuildingOptions { get; set; } = new();
        public List<SelectListItem> StructureModelList { get; set; } = new();


        
        public static StructureViewModel FromModel(StructureModel model)
        {
            return new StructureViewModel
            {
                UniqueId = model.UniqueID,
                UniqueGuid = model.UniqueGUID,
                CampusGuid = model.CampusGUID,
                BuildingGuid = model.BuildingGUID,
                StructureType = model.StructureType,
                StructureId = model.StructureID,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                LocationDescription = model.LocationDesc,
                Comments = model.Comments
            };
        }

        public StructureModel ToModel()
        {
            return new StructureModel
            {
                UniqueID = UniqueId,
                CampusGUID = CampusGuid == Guid.Empty ? Guid.NewGuid() : CampusGuid,
                BuildingGUID = BuildingGuid ?? Guid.Empty,
                StructureType = StructureType,
                StructureID = StructureId,
                Latitude = Latitude,
                Longitude = Longitude,
                LocationDesc = LocationDescription ?? "",
                Comments = Comments,
                TypeGUID = StructureModel != null ? Guid.Parse(StructureModel) : Guid.Empty,
                UniqueGUID = UniqueGuid,

            };
        }
    }

}
