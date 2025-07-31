using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IPortRepository
    {
        Task<List<PortModel>> GetPortsAsync(Guid spliceGuid);
        Task<int> AddPortAsync(PortModel model);
        //Task<PortModel> GetPortByPortId(Guid portId);
        Task<PortModel?> GetPortByGuidAsync(Guid uniqueGuid);

        Task<int> UpdatePortAsync(PortModel model);


    }
}
