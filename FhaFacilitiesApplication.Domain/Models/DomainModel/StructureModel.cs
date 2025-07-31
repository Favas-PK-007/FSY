#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class StructureModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } 
        public Guid CampusGUID { get; set; } 
        public Guid BuildingGUID { get; set; } 
        public string StructureType { get; set; }
        public string StructureID { get; set; } = null!;
        public Guid TypeGUID { get; set; } 
        public double  Latitude { get; set; }
        public double Longitude { get; set; }
        public string? LocationDesc { get; set; } 
        public string? Comments { get; set; } 
        public bool LatestRev { get; set; } = true;
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LastSavedBy { get; set; }
        public DateTime LastSavedDate { get; set; }

        // Related material data
        public BuildingModel? Building { get; set; }

    }
}
