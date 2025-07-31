#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IFiberRepository
    {
        Task<List<FiberModel>> GetFiberAsync(Guid cableGuid, bool loadCircuits);
        Task<FiberModel> LoadFibersAsync(Guid fiberGuid, Guid cableGuid, bool loadCircuits);
        //Task TraceFiberAsync(PortModel model, FiberModel fiberModel, SpliceModel spliceModel, bool csvFormat, List<FiberModel> fibers, StringBuilder fiberDetails);

        Task TraceFiberAsync(PortModel port, FiberModel fiber, SpliceModel splice, bool csvFormat, List<FiberModel> fibers, StringBuilder fiberDetails);

        Task<string> GetCableFiberDetailsAsync(Guid uniqueGuid, bool isCsvFormat);

        Task<FiberModel> LoadFiberAsync(Guid uniqueGuid, Guid cableGuid, bool loadCircuits);

    }
}
