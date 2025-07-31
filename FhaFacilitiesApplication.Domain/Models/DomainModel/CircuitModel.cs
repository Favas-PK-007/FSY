#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class CircuitModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } = Guid.NewGuid();
        public Guid FiberGUID { get; set; }
        public Guid BuildingGUID { get; set; }
        public string? CircuitID { get; set; }
        public string? Comments { get; set; }
        public bool LatestRev { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LastSavedBy { get; set; }
        public DateTime LastSavedDate { get; set; }
        public bool Updated { get; set; } = false;
    }
}
