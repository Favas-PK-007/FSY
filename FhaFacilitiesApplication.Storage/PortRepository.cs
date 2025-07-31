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
    public class PortRepository : IPortRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion


        #region Constructor
        public PortRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<List<PortModel>> GetPortsAsync(Guid spliceGuid)
        {
            var ports = new List<PortModel>();

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@SpliceGUID", spliceGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<PortModel>("spLoadPortsV1",
                    param: parameters,
                    commandType: CommandType.StoredProcedure);

                foreach(var port in result)
                {
                    port.SpliceGUID = spliceGuid;
                    port.LatestRev = true;
                }
                ports = result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load ports from database." + ex.Message, ex);
            }

            return ports;
        }

        public async Task<int> AddPortAsync(PortModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@UniqueGUID", model.UniqueGUID);
                    parameters.Add("@SpliceGUID", model.SpliceGUID);
                    parameters.Add("@TrayID", model.TrayID);
                    parameters.Add("@ModuleID", model.ModuleID);
                    parameters.Add("@PortID", model.PortID);
                    parameters.Add("@PortType", model.PortType);
                    parameters.Add("@ConnectionGUID", model.ConnectionGUID);
                    parameters.Add("@Comments", model.Comments);
                    parameters.Add("@LatestRev", model.LatestRev);
                    parameters.Add("@CreatedBy", model.CreatedBy);
                    parameters.Add("@CreatedDate", DateTime.Now);
                    parameters.Add("@LastSavedBy", model.LastSavedBy);
                    parameters.Add("@LastSavedDate", DateTime.Now);

                    // Execute SP and return RowCount
                    var result = await connection.QuerySingleAsync<int>(
                        "sp_Ports_InsertPort",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result; // Should return 1 on success, 0 if not inserted
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add port to database." + ex.Message, ex);
            }

        }

        //public async Task<PortModel> LoadPortByGuidAsync(Guid uniqueGuid)
        //{
        //    try
        //    {
        //        using IDbConnection connection = new SqlConnection(_fhaDbCon);

        //        var parameters = new DynamicParameters();
        //        parameters.Add("@UniqueGUID", uniqueGuid);
        //        parameters.Add("@LatestRev", true);

        //        var port = await connection.QuerySingleOrDefaultAsync<PortModel>(
        //            "sp_Ports_GetPortByGuid",
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //        );

        //        return port;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error loading port data: " + ex.Message, ex);
        //    }
        //}

        public async Task<PortModel?> GetPortByGuidAsync(Guid uniqueGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@LatestRev", true);
                var port = await connection.QuerySingleOrDefaultAsync<PortModel>(
                    "sp_Ports_GetPortByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return port;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading port data: " + ex.Message);
            }
        }

        public async Task<int> UpdatePortAsync(PortModel model)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueID", model.UniqueID);
                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);
                parameters.Add("@LatestRev", false);

                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_Ports_Update",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating the port.", ex);
            }
        }
    }
}
