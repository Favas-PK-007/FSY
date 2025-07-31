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
    public interface ICircuitRepository
    {
        Task<Dictionary<Guid, CircuitModel>> GetCircuitsFromCableAsync(Guid cableGuid);
        Task<List<CircuitModel>> GetCircuitByFiberAsync(Guid fiberGuid);
        Task<List<CircuitModel>> GetCircuitsByCableAsync(Guid cableGuid);
        Task AddCircuitAsync(CircuitModel circuit, string userId);
    }
}
