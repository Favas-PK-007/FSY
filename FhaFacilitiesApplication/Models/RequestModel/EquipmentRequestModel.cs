namespace FhaFacilitiesApplication.Models.RequestModel
{
    public class EquipmentRequestModel
    {
        public bool IsNewEquipment { get; set; }
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public Guid CampusGUID { get; set; }
        public Guid StructureGUID { get; set; }
        public string EquipmentType { get; set; }
        public string PortID { get; set; }
        public string EquipmentID { get; set; }
        public Guid TypeGUID { get; set; }
        public string ConnectionType { get; set; }
        public Guid SpliceGUID { get; set; }
        public Guid PortGUID { get; set; }
        public Guid FiberA_GUID { get; set; }
        public Guid FiberB_GUID { get; set; }
        public int Sequence { get; set; }
        public string Comments { get; set; }
        public Guid ConnectionGUID { get; set; } // selected slot from the table
    }
}
