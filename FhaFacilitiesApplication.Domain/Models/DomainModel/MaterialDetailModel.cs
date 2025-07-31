using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class MaterialDetailModel
    {
        public string Header { get; set; } 
        public string Value { get; set; } 
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } 
        public string MaterialType { get; set; }
        public Guid MaterialGUID { get; set; }
        public string Comments { get; set; }
        public bool LatestRev { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastSavedBy { get; set; }
        public DateTime LastSavedDate { get; set; }
    }
}
