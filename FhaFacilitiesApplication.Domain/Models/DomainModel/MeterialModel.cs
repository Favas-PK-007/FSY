#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.DomainModel
{
    public class MeterialModel
    {
        // === Identification ===
        public int UniqueID { get; set; } = 0;
        public Guid UniqueGUID { get; set; } 
        public Guid ParentGUID { get; set; } = Guid.Empty; 
        public Guid TemplateGUID { get; set; } = Guid.Empty;

        // === Material Info ===
        public string MaterialType { get; set; } = string.Empty;
        public string MaterialID { get; set; } = string.Empty;
        public string ManufacturerID { get; set; } = string.Empty;
        public string ModelID { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;

        // === Revision & Audit ===
        public bool LatestRev { get; set; } = true;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string LastSavedBy { get; set; } = string.Empty;
        public DateTime LastSavedDate { get; set; } = DateTime.Now;

        // === Related Data ===
        public Dictionary<string, string> Details { get; set; } = new();
        public List<MeterialModel> Components { get; set; } = new();

        public override string ToString()
        {
            var manufacturer = !string.IsNullOrWhiteSpace(ManufacturerID)
                   ? $" ({ManufacturerID})"
                   : "";

            if (string.IsNullOrWhiteSpace(MaterialID))
            {
                return $"{ModelID}{manufacturer}";
            }
            else
            {
                return $"{MaterialType}-{MaterialID} ({ModelID})";
            }
        }

    }

    public class MaterialTypeModel
    {
        public string Text { get; set; } = null!;
        public Guid Value { get; set; }
    }


    public class ComponentTreeNode
    {
        public Guid ComponentId { get; set; }
        public string ComponentName { get; set; }
        public List<ComponentTreeNode> Children { get; set; }
    }
}
