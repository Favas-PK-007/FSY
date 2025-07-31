#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
//using FhaFacilitiesApplication.ValidationExtensions;
using System.ComponentModel.DataAnnotations;
#endregion

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class BuildingViewModel
    {
        [Display(Name = "Building Id")]
        [Required(ErrorMessage = "Building Id is required")]
        [StringLength(50, ErrorMessage = "Building Id cannot be longer than 50 characters")]
        public string BuildingId { get; set; } = null!;
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot be longer than 50 characters")]
        public string Designation { get; set; } = null!;
        [GeoCoordinateValidation]
        public double? Latitude { get; set; } 
        [GeoCoordinateValidation]
        public double? Longitude { get; set; } 
        public string Comments { get; set; } = string.Empty;
        public int UniqueId { get; set; }
        public Guid CampusGuid { get; set; }
        public Guid UniqueGUID { get; set; }

        public BuildingModel ToModel()
        {
            return new BuildingModel
            {
                CampusGUID = CampusGuid,
                BuildingID = BuildingId,
                Designation = Designation,
                Latitude = Latitude,
                Longitude = Longitude,
                Comments = Comments,
                CreatedBy = "",
                CreatedDate = DateTime.UtcNow,
                LastSavedBy = "",
                LastSavedDate = DateTime.UtcNow,
                LatestRev = true,
                UniqueGUID = UniqueGUID,
                UniqueID = UniqueId
            };
        }

        public static BuildingViewModel FromModel(BuildingModel model)
        {
            return new BuildingViewModel
            {
                BuildingId = model.BuildingID,
                Designation = model.Designation,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Comments = model.Comments,
                UniqueId = model.UniqueID,
                CampusGuid = model.CampusGUID,
                UniqueGUID = model.UniqueGUID
            };
        }
    }
   
}
