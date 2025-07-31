namespace FhaFacilitiesApplication.Models.ResponseModel
{
    public class MaterialResponseModel
    {
        public Guid UniqueGUID { get; set; }
        public int UniqueID { get; set; }
        public string MaterialType { get; set; }
        public string MaterialID { get; set; }
        public string ManufacturerID { get; set; }
        public string ModelID { get; set; }
        public string Comments { get; set; }
        public Dictionary<string, string> Details { get; set; } = new();
    }
}
