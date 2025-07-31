using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IPortService
    {
        Task<List<PortModel>> GetPortsAsync(Guid spliceGuid);
        Task<int> AddPortAsync(PortModel model);
        Task<PortModel?> GetPortByGuidAsync(Guid uniqueGuid);

        Task<ToasterModel> UpdatePortAsync(PortModel model);
    }
}
