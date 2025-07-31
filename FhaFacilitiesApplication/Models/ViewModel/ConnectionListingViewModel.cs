using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class ConnectionListingViewModel
    {
        public string Title { get; set; }
        public List<SelectListItem> CableAList { get; set; }
        public List<SelectListItem> CableBList { get; set; }
        public string Column0Header { get; set; }
        public string Column1Header { get; set; }
        public string Column2Header { get; set; }
        public string Column3Header { get; set; }
        //public bool ShowRibbonSplice { get; set; }
        //public bool RibbonSpliceChecked { get; set; }
        //public bool ShowTraceFiber { get; set; }
        //public bool ShowCSVFormat { get; set; }
        //public bool ShowExportReport { get; set; }
        //public bool ShowEquipmentButtons { get; set; }
        // === Slot table display  ===
        public List<SpliceSlotViewModel> Slots { get; set; } = new();
        public FiberTreeViewModel CableAFiberTree { get; set; }
        public FiberTreeViewModel CableBFiberTree { get; set; }

    }

    public class SpliceSlotViewModel
    {
        public int SlotNumber { get; set; }               // Maps to the slot ID
        public string TrayLabel { get; set; }             // "Tray X" or "Storage"
        public string SlotLabel { get; set; }             // "Slot Y" or empty
        public string ConnectionType { get; set; }        // "Expressed", "Equipment", etc.
        public string FiberDetails { get; set; }
        public Guid ConnectionTypeGUID { get; set; }
        public Guid PortGUID { get; set; }
    }
}
