namespace FhaFacilitiesApplication.Models.RequestModel
{
    public class FiberRemovalRequestModel
    {
        public List<string> SelectedFiberGuids { get; set; }
        public Guid CableGuid { get; set; }
    }

    public class FiberAssignmentRequestModel
    {
        public Guid CableGuid { get; set; }
        public List<string> SelectedFiberGuids { get; set; }
        public string CircuitId { get; set; }
    }
}
