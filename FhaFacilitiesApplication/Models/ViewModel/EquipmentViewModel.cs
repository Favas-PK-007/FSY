using FhaFacilitiesApplication.Domain.Models.DomainModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class EquipmentViewModel
    {
        public bool IsNewEquipment { get; set; } = false;
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
        public string SelectedEquipmentID { get; set; }
        public List<SelectListItem> EquipmentTypeList { get; set; }
        public List<SelectListItem> EquipmentIdList { get; set; }
        public List<SelectListItem> EquipmentModelList { get; set; }
        public List<SelectListItem> EquipmentPortIdList { get; set; }



        public EquipmentModel ToModel()
        {
            return new EquipmentModel
            {
                UniqueID = UniqueID,
                UniqueGUID = UniqueGUID,
                CampusGUID = CampusGUID,
                StructureGUID = StructureGUID,
                EquipmentType = EquipmentType,
                PortID = PortID,
                EquipmentID = EquipmentID,
                TypeGUID = TypeGUID,
                ConnectionType = ConnectionType,
                SpliceGUID = SpliceGUID,
                PortGUID = PortGUID,
                FiberA_GUID = FiberA_GUID,
                FiberB_GUID = FiberB_GUID,
                Sequence = Sequence,
                Comments = Comments
            };
        }
    }
}
