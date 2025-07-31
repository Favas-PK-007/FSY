
namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class DashboardViewModel
    {
        public CampusViewModel CampusViewModel { get; set; } = new CampusViewModel();
        public List<CampusViewModel> Campuses { get; set; }
        public BuildingViewModel BuildingViewModel { get; set; } = new BuildingViewModel();
        public List<BuildingViewModel> Buildings { get; set; }
        public StructureViewModel StructureViewModel { get; set; } = new StructureViewModel();
        public List<StructureViewModel> Structures { get; set; }
        public DuctViewModel DuctViewModel { get; set; } = new DuctViewModel();
        public List<DuctViewModel> Ducts { get; set; } = new List<DuctViewModel>();
        public List<CableViewModel> Cables { get; set; } = new List<CableViewModel>();
        public CableViewModel CableViewModel { get; set; }
        public ConduitViewModel ConduitViewModel { get; set; } = new ConduitViewModel();
        public List<ConduitViewModel> Conduits { get; set; }
        public SpliceViewModel SpliceViewModel { get; set; } = new SpliceViewModel();
        public List<SpliceViewModel> Splices { get; set; }
        public CableFiberViewModel CableFiberViewModel { get; set; } = new CableFiberViewModel();
        public List<CableFiberViewModel> CableFibers { get; set; } = new List<CableFiberViewModel>();
        public ComponentViewModel ComponentViewModel { get; set; } = new ComponentViewModel();
        public List<ComponentTreeNode> ComponentsTree { get; set; } = new();
        public List<MaterialViewModel> Materials { get; set; } = new List<MaterialViewModel>();
        public MaterialViewModel MaterialViewModel { get; set; } = new MaterialViewModel();
        public CableDuctBindingViewModel CableDuctBindingViewModel { get; set; }
        public CircuitViewModel CircuitViewModel { get; set; }
        public EquipmentViewModel EquipmentViewModel { get; set; } = new EquipmentViewModel();
        public ConnectionListingViewModel ConnectionListingViewModel { get; set; } = new ConnectionListingViewModel();



    }
}
