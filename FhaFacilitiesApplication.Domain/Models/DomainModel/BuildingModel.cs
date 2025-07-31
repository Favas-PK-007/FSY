namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class BuildingModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } 
        public Guid CampusGUID { get; set; } 
        public string BuildingID { get; set; } = null!;
        public string Designation { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Comments { get; set; } = string.Empty;
        public bool LatestRev { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; } 
        public string LastSavedBy { get; set; } = null!;
        public DateTime LastSavedDate { get; set; } 
    }
}
