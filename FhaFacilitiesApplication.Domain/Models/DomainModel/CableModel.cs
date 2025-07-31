using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class CableModel
    {
        public  int  UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public Guid  CampusGUID { get; set; }
        public string CableType { get; set; }
        public string CableID { get; set; }
        public Guid TypeGUID { get; set; }
        public Guid SpliceA_GUID { get; set; }
        public Guid SpliceB_GUID { get; set; }
        public Guid DuctGUID { get; set; }
        public string Comments { get; set; }
        public bool LatestRev { get; set; }
        public string CreatedBy { get; set; } 
        public DateTime CreatedDate { get; set; }
        public string LastSavedBy { get; set; } 
        public DateTime LastSavedDate { get; set; }
        public string Text { get; set; } = null!;
        public Guid Value { get; set; }
        public List<FiberModel> Fiber { get; set; }
        public Guid SelectedCableModel { get; set; }
        public MeterialModel CableModelMaterial { get; set; } = new MeterialModel();
        public override string ToString()
        {
            return CableID;
        }
    }

    public class CableTypeModel
    {
        public string Text { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
