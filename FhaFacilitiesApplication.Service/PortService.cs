using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Service
{
    public class PortService : IPortService
    {
        #region Declarations

        private readonly IPortRepository _portRepository;
        #endregion

        #region Constructor
        public PortService(IPortRepository portRepository)
        {
            _portRepository = portRepository;
        }
        #endregion

        public async Task<List<PortModel>> GetPortsAsync(Guid spliceGuid)
        {
            return await _portRepository.GetPortsAsync(spliceGuid);
        }

        public async Task<int> AddPortAsync(PortModel model)
        {
            return await _portRepository.AddPortAsync(model);
        }

        public async Task<PortModel?> GetPortByGuidAsync(Guid uniqueGuid)
        {
            return await _portRepository.GetPortByGuidAsync(uniqueGuid);

        }


        public async Task<ToasterModel> UpdatePortAsync(PortModel model)
        {
            var result = new ToasterModel();
            var updated = await _portRepository.UpdatePortAsync(model);
            if (updated > 0)
            {
                var inserPort = await _portRepository.AddPortAsync(model);
                result.IsError = false;
                result.Message = "Port updated successfully.";
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Type = ToasterType.success.ToString();
            }
            else
            {
                result.IsError = true;
                result.Message = "Failed to update port.";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Type = ToasterType.fail.ToString();
            }
            return result;
        }
    }
}
