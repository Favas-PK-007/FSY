using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class ComponentViewModel
    {
        public string MaterialType { get; set; }
        public List<SelectListItem> ComponentModelList { get; set; } = new();
    }

    public class ComponentTreeNode
    {
        public int ComponentId { get; set; }
        public string ComponentName { get; set; }
        public List<ComponentTreeNode> Children { get; set; }
    }
}
