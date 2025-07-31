using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class ConnectionModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } = Guid.NewGuid();
        public string? ConnectionType { get; set; }
        public Guid? SpliceGUID { get; set; }
        public Guid? PortGUID { get; set; }
        public Guid? FiberA_GUID { get; set; }
        public Guid? FiberB_GUID { get; set; }
        public int? Sequence { get; set; }
        public string? Comments { get; set; }
        public bool LatestRev { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastSavedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastSavedBy { get; set; }
        public bool  Updated { get; set; }

        public override string ToString()
        {
            return ConnectionType;
        }
    }
}
