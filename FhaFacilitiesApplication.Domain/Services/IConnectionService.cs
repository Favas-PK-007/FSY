using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IConnectionService
    {
        Task<List<ConnectionModel>> GetConnectionsAsync(Guid spliceGuid);


        Task<ConnectionModel?> GetConnectionByGuidAsync(Guid uniqueGuid);


        Task<ToasterModel> UpdateConnectionAsync(ConnectionModel model);


        Task<ToasterModel> AddConnectionAsync(ConnectionModel model);

        Task<ToasterModel> DeleteConnectionAsync(ConnectionModel model);

    }
}
