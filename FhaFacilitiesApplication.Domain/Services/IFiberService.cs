#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IFiberService
    {
        Task<List<FiberModel>> GetFiberAsync(Guid cableGuid, bool loadCircuits);

        Task<string> GetCableFiberDetailsAsync(FiberModel fiber, bool csvFormat);

        //Task TraceConnectionsAsync(FiberModel currentFiber, SpliceModel currentSplice, bool csvFormat, StringBuilder fiberDetails, List<FiberModel> fibers, ConnectionModel connection);

        Task<FiberModel> LoadFiberAsync(Guid uniqueGuid, Guid cableGuid, bool loadCircuits);

        Task TraceConnectionsAsync(PortModel port,FiberModel currentFiber,SpliceModel currentSplice,bool csvFormat,StringBuilder fiberDetails,List<FiberModel> fibers);
    }

}
