using FhaFacilitiesApplication.Domain.Models.DomainModel;

namespace FhaFacilitiesApplication.Models.ViewModel
{
    public class CableFiberViewModel
    {
        public List<BufferViewModel> Buffers { get; set; } = new();
    }

    public class BufferViewModel
    {
        public string Text { get; set; }      // e.g., "Buffer-1"
        public int Value { get; set; }        // e.g., 1
        public List<RibbonViewModel> Ribbons { get; set; } = new();
        public List<FiberCableViewModel> Fibers { get; set; } = new();
    }

    public class RibbonViewModel
    {
        public string Text { get; set; }      // e.g., "Ribbon-2"
        public int Value { get; set; }        // e.g., 2
        public List<FiberCableViewModel> Fibers { get; set; } = new();
    }

    public class FiberCableViewModel
    {
        public string Text { get; set; }      // e.g., "Fiber-7"
        public int Value { get; set; }        // e.g., 7
    }

    public class FiberTreeViewModel
    {
        public string CableID { get; set; } = string.Empty;
        public List<BufferNode> Buffers { get; set; } = new();

        public class BufferNode
        {
            public Guid UniqueGUID { get; set; }
            public string BufferID { get; set; }
            public List<RibbonNode> Ribbons { get; set; } = new();
        }

        public class RibbonNode
        {
            public Guid UniqueGUID { get; set; }
            public string RibbonID { get; set; }
            public List<FiberNode> Fibers { get; set; } = new();
        }

        public class FiberNode
        {
            public Guid UniqueGUID { get; set; }
            public string FiberID { get; set; }
        }
    }

}
