namespace FhaFacilitiesApplication.Models.ResponseModel
{
    public class TraceFiberResultModel
    {
        public List<string> FiberLines { get; set; } = new();
        public int FiberCount { get; set; }
        public bool IsCSVFormat { get; set; }
        public Guid TraceId { get; set; }
    }
}
