using Dapper;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Storage
{
    public class ConnectionRepository : IConnectionRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion


        #region Constructor
        public ConnectionRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<List<ConnectionModel>> GetConnectionsAsync(Guid spliceGuid)
        {
            var connections = new List<ConnectionModel>();
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@SpliceGUID", spliceGuid);
                parameters.Add("@FiberA_GUID", Guid.Empty);
                parameters.Add("@LatestRev", true);
                var result = await connection.QueryAsync<ConnectionModel>("spLoadConnectionsV1",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                foreach (var a in result)
                {
                    a.LatestRev = true;
                    a.SpliceGUID = spliceGuid;

                }
                connections = result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load connections from database." + ex.Message, ex);
            }
            return connections;
        }

        public async Task<ConnectionModel?> GetConnectionByGuidAsync(Guid uniqueGuid)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@LatestRev", true);
                var result = await connection.QuerySingleOrDefaultAsync<ConnectionModel>("sp_Connections_GetConnectionByGuid",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                if (result != null)
                {
                    result.Updated = false; 
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load connection from database." + ex.Message, ex);
            }
        }

        public async Task<int> UpdateConnectionAsync(ConnectionModel model)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@UniqueID", model.UniqueID);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);
                parameters.Add("@LatestRev", false);
                var result = await connection.QuerySingleAsync<int>("sp_Connections_Update",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update connection in database." + ex.Message, ex);
            }
        }

        public async Task<ConnectionModel?> GetNextConnectionAsync(Guid fiberGuid, Guid spliceGuid)
        {
            using IDbConnection db = new SqlConnection(_fhaDbCon);
            var parameters = new DynamicParameters();
            parameters.Add("@FiberGUID", fiberGuid);
            parameters.Add("@SpliceGUID", spliceGuid);
            parameters.Add("@LatestRev", true);

            return await db.QueryFirstOrDefaultAsync<ConnectionModel>(
                "spLoadConnectionV1",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }


        public async Task<int> AddConnectionAsync(ConnectionModel model)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();

                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@ConnectionType", model.ConnectionType);
                parameters.Add("@SpliceGUID", model.SpliceGUID);
                parameters.Add("@PortGUID", model.PortGUID);
                parameters.Add("@FiberA_GUID", model.FiberA_GUID);
                parameters.Add("@FiberB_GUID", model.FiberB_GUID);
                parameters.Add("@Sequence", model.Sequence);
                parameters.Add("@Comments", model.Comments);
                parameters.Add("@LatestRev", model.LatestRev);
                parameters.Add("@CreatedBy", model.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);

                var result = await connection.QuerySingleAsync<int>(
                    "sp_Connections_Add",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add connection to database. " + ex.Message, ex);
            }
        }
    }
}
