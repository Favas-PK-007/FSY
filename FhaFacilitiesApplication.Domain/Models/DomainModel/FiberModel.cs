using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class FiberModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public Guid CableGUID { get; set; }
        public int BufferID { get; set; }
        public int FiberID { get; set; }
        public int RibbonID { get; set; }
        public string? FiberType { get; set; }
        public string? Comments { get; set; } 
        public bool LatestRev { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string LastSavedBy { get; set; } = null!;
        public DateTime LastSavedDate { get; set; }
        public List<CircuitModel> Circuits { get; set; }

        public override string ToString()
        {
            return "Buffer-" + BufferID + ": Ribbon-" + RibbonID + ": Fiber-" + FiberID;
        }

    }
}
