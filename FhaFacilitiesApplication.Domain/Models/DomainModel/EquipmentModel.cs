using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class EquipmentModel
    {
        public int UniqueID { get; set; } 
        public Guid UniqueGUID { get; set; } 
        public Guid CampusGUID { get; set; } 
        public Guid StructureGUID { get; set; } 
        public string? EquipmentType { get; set; }
        public string? PortID { get; set; }
        public string? EquipmentID { get; set; }
        public Guid TypeGUID { get; set; }
        public string? ConnectionType { get; set; }
        public Guid? SpliceGUID { get; set; }
        public Guid? PortGUID { get; set; }
        public Guid? FiberA_GUID { get; set; }
        public Guid FiberB_GUID { get; set; }
        public int Sequence { get; set; }
        public string? Comments { get; set; }
        public bool LatestRev { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? LastSavedBy { get; set; }
        public DateTime LastSavedDate { get; set; }
        public bool IsNewEquipment { get; set; }

        //public string? SelectedEquipmentID { get; set; }
        //public List<string> EquipmentTypeList { get; set; } 

    }
}
