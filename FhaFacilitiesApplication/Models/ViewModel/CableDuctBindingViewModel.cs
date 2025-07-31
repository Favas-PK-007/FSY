using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class CableDuctBindingViewModel
    {
        public string CableID { get; set; }
        public List<SelectListItem> ConduitDropdownList { get; set; } = new();
        public List<SelectListItem> DuctDropdownList { get; set; } = new();
        public List<SelectListItem> SubDuctDropdownList { get; set; } = new();
        public Guid? SelectedConduitGuid { get; set; }
        public Guid? SelectedDuctGuid { get; set; }
        public Guid? SelectedSubDuctGuid { get; set; }
        public bool IsSubDuctEnabled { get; set; } = false;
        public Guid CableDuctGuid { get; set; }
    }
}
