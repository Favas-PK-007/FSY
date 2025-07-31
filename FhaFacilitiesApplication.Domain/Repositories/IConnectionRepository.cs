using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IConnectionRepository
    {
        Task<List<ConnectionModel>> GetConnectionsAsync(Guid spliceGuid);


        Task<ConnectionModel?> GetConnectionByGuidAsync(Guid uniqueGuid);

        Task<int> UpdateConnectionAsync(ConnectionModel model);

        Task<ConnectionModel?> GetNextConnectionAsync(Guid fiberGuid, Guid spliceGuid);

        Task<int> AddConnectionAsync(ConnectionModel model);
    }
}
