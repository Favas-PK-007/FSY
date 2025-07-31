using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class PortModel
    {
        public int UniqueID { get; set; } = 0;
        public Guid UniqueGUID { get; set; } = Guid.NewGuid();
        public Guid SpliceGUID { get; set; } = Guid.Empty;
        public int? TrayID { get; set; }
        public int? PortID { get; set; }
        public int? ModuleID { get; set; }
        public string? PortType { get; set; }
        public Guid ConnectionGUID { get; set; }
        public string? Comments { get; set; }
        public bool? LatestRev { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastSavedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastSavedBy { get; set; }
        public bool Updated { get; set; }
        public override string ToString()
        {
            return PortID?.ToString() ?? string.Empty;
        }




    }
}
