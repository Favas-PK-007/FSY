namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class CircuitViewModel
    {
        public List<BuildingViewModel> BuildingList { get; set; }
        public List<BuildingViewModel> BuildingReservedList { get; set; }
        public List<SpliceViewModel> SpliceList { get; set; }
        public List<CableViewModel> CableList { get; set; }
        public List<CircuitBufferViewModel> BufferList { get; set; }    
        public string CircuitDescription { get; set; }
        public string Comments { get; set; }
        public string SerializedModel { get; set; }

    }

    public class CircuitBufferViewModel
    {
        public  string CableId { get; set; }
        public Guid CableGUID { get; set; }
        public bool UnSavedItems { get; set; }
        public int BufferID { get; set; }
        public Guid UniqueGUID { get; set; }
        public List<CircuitRibbonViewModel> Ribbons { get; set; } = new();
    }

    public class CircuitRibbonViewModel
    {
        public int RibbonID { get; set; }
        public Guid UniqueGUID { get; set; }
        public List<CircuitFiberViewModel> Fibers { get; set; } = new();
    }

    public class CircuitFiberViewModel
    {
        public Guid UniqueGUID { get; set; }
        public string FiberID { get; set; }
        public int BufferID { get; set; }
        public int RibbonID { get; set; }
        public string CircuitID { get; set; }
        public bool HasMultipleCircuits { get; set; }
    }
}
