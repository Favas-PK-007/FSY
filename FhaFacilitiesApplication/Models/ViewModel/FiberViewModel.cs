using FhaFacilitiesApplication.Domain.Models.DomainModel;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class FiberViewModel
    {
        public int UniqueID { get; set; }
        public Guid UniqueGUID { get; set; }
        public Guid CableGUID { get; set; }
        public int BufferID { get; set; }
        public int FiberID { get; set; }
        public int RibbonID { get; set; }
        public string FiberType { get; set; }

        public static FiberViewModel FromModel(FiberModel model)
        {
            return new FiberViewModel
            {
                UniqueID = model.UniqueID,
                UniqueGUID = model.UniqueGUID,
                CableGUID = model.CableGUID,
                BufferID = model.BufferID,
                FiberID = model.FiberID,
                RibbonID = model.RibbonID,
                FiberType = model.FiberType
            };
        }

        public FiberModel ToModel()
        {
            return new FiberModel
            {
                UniqueID = UniqueID,
                UniqueGUID = UniqueGUID,
                CableGUID = CableGUID,
                BufferID = BufferID,
                FiberID = FiberID,
                RibbonID = RibbonID,
                FiberType = FiberType
            };
        }

    }
}
