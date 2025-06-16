#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models
{
    public class CampusModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public string CampusID { get; set; } = null!;
        public string Designation { get; set; } = null!; 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Comments { get; set; } = string.Empty;
        public bool LatestRev { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string LastSavedBy { get; set; } = null!; // Ensure this is not null, can be set later
        public DateTime LastSavedDate { get; set; }
    }

}
