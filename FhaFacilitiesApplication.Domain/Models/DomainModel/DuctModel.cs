#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class DuctModel
    {
        // Unique database ID
        public int UniqueID { get; set; }

        // GUID for the duct
        public Guid UniqueGUID { get; set; } 

        // Associated structure GUID
        public Guid StructureGUID { get; set; } 

        // Associated conduit GUID
        public Guid ConduitGUID { get; set; } 
        public Guid MeterialGuid { get; set; } 

        // Duct identifier
        public string DuctID { get; set; } = string.Empty;

        // Alternate duct identifier
        public string DuctID_B { get; set; } = string.Empty;

        // Duct type reference GUID
        public Guid TypeGUID { get; set; } 

        // Any additional comments
        public string Comments { get; set; } = string.Empty;

        // Indicates latest revision
        public bool LatestRev { get; set; } = true;

        // Created by user
        public string CreatedBy { get; set; } = string.Empty;

        // Date of creation
        public DateTime CreatedDate { get; set; }

        // Last modified by user
        public string LastSavedBy { get; set; } = string.Empty;

        // Date of last modification
        public DateTime LastSavedDate { get; set; }

        // Related structure data
        public StructureModel? Structure { get; set; }

        // Related material data
        public MeterialModel? Material { get; set; }

        // List of nested sub-ducts
        public List<DuctModel> SubDucts { get; set; }

        public override string ToString()
        {
            if (DuctID == DuctID_B)
            {
                return $"{DuctID} ({Material?.ModelID})";
            }
            else
            {
                return $"{DuctID} -> {DuctID_B} ({Material?.ModelID})";
            }
        }
        
 
    }
}
