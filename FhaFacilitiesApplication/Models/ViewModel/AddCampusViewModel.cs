#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
using System.ComponentModel.DataAnnotations;
#endregion
namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class AddCampusViewModel
    {
        [Required]
        public string CampusId { get; set; }

        [Required]
        public string Designation { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Comments { get; set; }

        public CampusModel ToModel()
        {
            return new CampusModel
            {
                CampusID = this.CampusId,
                Designation = this.Designation,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Comments = this.Comments,
                CreatedBy = "Anonymous", // This can be set to the current user later
                LastSavedBy = "Anonymous", // This can be set to the current user later
                CreatedDate = DateTime.UtcNow,
                LastSavedDate = DateTime.UtcNow
            };
        }
    }
}
