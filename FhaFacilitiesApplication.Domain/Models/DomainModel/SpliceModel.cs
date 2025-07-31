#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class SpliceModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; } = Guid.NewGuid();
        public Guid CampusGUID { get; set; } = Guid.Empty; 
        public Guid StructureGUID { get; set; } = Guid.Empty;
        public Guid TypeGUID { get; set; } = Guid.Empty;
        public string SpliceType { get; set; } = string.Empty;
        public string SpliceID { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public bool LatestRev { get; set; } = true;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string LastSavedBy { get; set; } = null!;
        public DateTime LastSavedDate { get; set; } = DateTime.Now;
        public StructureModel? Structure { get; set; }
        public List<MeterialModel> EquipmentModel { get; set; } = null!;
        public List<MeterialModel>? Components { get; set; } 

        public override string ToString()
        {
            return SpliceID;
        }
    }
}
