#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class ConduitModel
    {
        // Unique database ID
        public int UniqueID { get; set; }

        // GUID for the conduit
        public Guid UniqueGUID { get; set; } 

        // Conduit identifier
        public string ConduitID { get; set; } 

        // Structure A reference
        public Guid StructureA_GUID { get; set; }

        // Structure B reference
        public Guid StructureB_GUID { get; set; }

        // Campus reference
        public Guid CampusGUID { get; set; }

        // Indicates latest revision
        public bool LatestRev { get; set; }

        // Any additional comments
        public string Comments { get; set; } = string.Empty;

        // Created by user
        public string CreatedBy { get; set; } = string.Empty;

        // Date of creation
        public DateTime? CreatedDate { get; set; }

        // Last modified by user
        public string LastSavedBy { get; set; } = string.Empty;

        // Date of last modification
        public DateTime? LastSavedDate { get; set; }

        // List of associated ducts
        public List<DuctModel> Ducts { get; set; } = new List<DuctModel>();
    }
}
