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
    public class ConnectionService : IConnectionService
    {
        #region Declarations
        private readonly IConnectionRepository _connectionRepository;
        #endregion

        #region Constructor
        public ConnectionService(IConnectionRepository connectionRepository)
        {
            _connectionRepository = connectionRepository;
        }
        #endregion

        public async Task<List<ConnectionModel>> GetConnectionsAsync(Guid spliceGuid)
        {
            return await _connectionRepository.GetConnectionsAsync(spliceGuid);
        }

        public async Task<ConnectionModel?> GetConnectionByGuidAsync(Guid uniqueGuid)
        {
            return await _connectionRepository.GetConnectionByGuidAsync(uniqueGuid);

        }

        public async Task<ToasterModel> UpdateConnectionAsync(ConnectionModel model)
        {
            var result = new ToasterModel();
            var updated = await _connectionRepository.UpdateConnectionAsync(model);
            if (updated > 0)
            {
                result.IsError = false;
                result.Message = "Connection updated successfully.";
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Type = ToasterType.success.ToString();
            }
            else
            {
                result.IsError = true;
                result.Message = "Failed to update connection.";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Type = ToasterType.fail.ToString();
            }
            return result;
        }

        public async Task<ToasterModel> AddConnectionAsync(ConnectionModel model)
        {
            var result = new ToasterModel();
            var newConnectionResult = await _connectionRepository.AddConnectionAsync(model);
            if (newConnectionResult > 0)
            {
                result.IsError = false;
                result.Message = "Connection added successfully.";
                result.StatusCode = System.Net.HttpStatusCode.Created;
                result.Type = ToasterType.success.ToString();
            }
            else
            {
                result.IsError = true;
                result.Message = "Failed to add connection.";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Type = ToasterType.fail.ToString();
            }

            return result;
        }


        public async Task<ToasterModel> DeleteConnectionAsync(ConnectionModel model)
        {
            var result = new ToasterModel();
            var updated = await _connectionRepository.UpdateConnectionAsync(model);
            if (updated > 0)
            {
                result.IsError = false;
                result.Message = "Connection deleted successfully.";
                result.StatusCode = System.Net.HttpStatusCode.OK;
                result.Type = ToasterType.success.ToString();
            }
            else
            {
                result.IsError = true;
                result.Message = "Failed to delete connection.";
                result.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                result.Type = ToasterType.fail.ToString();
            }
            return result;
        }
    }
}
