#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface ICircuitService
    {
        Task<Dictionary<Guid, CircuitModel>> GetCircuitsByCableAsync(Guid cableGuid);
        //Task<ToasterModel> SaveTracePathAsync(CircuitModel circuitModel,string action, Guid cableGuid, SpliceModel spliceModel);
       // Task<ToasterModel> UpdateCircuitFiberAsync(Guid fiberGuid);
        Task<List<CircuitModel>> GetCircuitByFiberAsync(Guid fiberGuid);

        Task<List<string>> SaveCircuitsAsync(Guid cableGuid, Guid spliceGuid, string userId);

    }
}
