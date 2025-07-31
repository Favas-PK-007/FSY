#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
//using FhaFacilitiesApplication.ValidationExtensions;
using System.ComponentModel.DataAnnotations;
#endregion
namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class CampusViewModel
    {
        [Display(Name = "Campus ID")]
        [Required(ErrorMessage = "Campus ID is required")]
        [StringLength(50, ErrorMessage = "Campus ID cannot be longer than 50 characters")]
        public string CampusId { get; set; } = null!;

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot be longer than 50 characters")]
        public string Designation { get; set; } = null!;

        [GeoCoordinateValidation]
        public double? Latitude { get; set; }

        [GeoCoordinateValidation]
        public double? Longitude { get; set; }

        [StringLength(200, ErrorMessage = "Comments cannot be longer than 50 characters")]
        public string Comments { get; set; }

        public int UniqueId { get;  set; }

        public Guid? UniqueGUID { get;  set; }

        public CampusModel ToModel(int UniqueID = 0)
        {
            return new CampusModel
            {
                UniqueID = UniqueID,
                UniqueGUID = UniqueGUID,
                CampusID = CampusId,
                Designation = Designation,
                Latitude = Latitude,
                Longitude = Longitude,
                Comments = Comments,
                CreatedDate = DateTime.UtcNow,
                LastSavedDate = DateTime.UtcNow
            };
        }

        public CampusModel ToModel()
        {
            return new CampusModel
            {
                CampusID = this.CampusId,
                Designation = this.Designation,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Comments = this.Comments,
                CreatedDate = DateTime.UtcNow,
                LastSavedDate = DateTime.UtcNow
            };
        }
        public static CampusViewModel FromModel(CampusModel model)
        {
            return new CampusViewModel
            {
                UniqueId = model.UniqueID,
                UniqueGUID = model.UniqueGUID,
                CampusId = model.CampusID,
                Designation = model.Designation,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Comments = model.Comments
            };
        }
    }
}
