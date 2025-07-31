#region Namespaces
using Dapper;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class ConduitPathRepository : IConduitPathRepository
    {

        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IDuctService _ductService;
        #endregion

        #region Constructor
        public ConduitPathRepository(IConfiguration configuration, IDuctService ductService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _ductService = ductService;
        }
        #endregion


        public async Task<List<ConduitModel>> GetConduitsAsync(string campusGuid, string structureGuid, bool loadDucts)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                string procedure = structureGuid == Guid.Empty.ToString()
                    ? "spLoadConduitsV1"
                    : "spLoadConduitsFromStructureV1";

                var parameters = new DynamicParameters();
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);

                if (structureGuid != Guid.Empty.ToString())
                    parameters.Add("@StructureGUID", structureGuid);

                var conduits = (await connection.QueryAsync<ConduitModel>(procedure, parameters, commandType: CommandType.StoredProcedure)).ToList();

                if (loadDucts)
                {
                    foreach (var conduit in conduits)
                    {
                        conduit.Ducts = await _ductService.GetDuctsAsync(conduit.UniqueGUID, loadDucts);
                    }
                }

                return conduits;
            }
            catch (Exception ex)
            {
                throw new Exception("" +ex.Message);
            }
        }

        public async Task<int> CreateConduitAsync(ConduitModel model)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();

                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@CampusGUID", model.CampusGUID);
                parameters.Add("@ConduitID", model.ConduitID);
                parameters.Add("@StructureA_GUID", model.StructureA_GUID);
                parameters.Add("@StructureB_GUID", model.StructureB_GUID);
                parameters.Add("@Comments", model.Comments);
                parameters.Add("@LatestRev", true);
                parameters.Add("@CreatedBy", model.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.UtcNow);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.UtcNow);

                bool isInserted = await connection.QuerySingleAsync<bool>(
                                    "sp_InsertConduitPath",
                                    parameters,
                                    commandType: CommandType.StoredProcedure);

                return isInserted ? 1 : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting conduit path.", ex);
            }

        }

        public async Task<bool> CheckConduitPathExists(ConduitModel requestModel)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var checkProcedure = "sp_CheckConduitPathExists";
                var checkParams = new DynamicParameters();
                checkParams.Add("@CampusGUID", requestModel.CampusGUID);
                checkParams.Add("@ConduitID", requestModel.ConduitID);
                checkParams.Add("@LatestRev", true);

                var existingConduitPath = await connection.QueryFirstOrDefaultAsync<bool>(
                    checkProcedure,
                    checkParams,
                    commandType: CommandType.StoredProcedure
                );

                return existingConduitPath; // returns 1 if exists, 0 otherwise
            }
            catch (Exception ex)
            {
                // Log or rethrow as needed
                throw new Exception("Error while checking conduit path existence.", ex);
            }
        }

        public async Task<int> DeleteConduitPathAsync(ConduitModel requestModel)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@LatestRev", false); 
                parameters.Add("@LastSavedBy", requestModel.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.UtcNow);
                parameters.Add("@UniqueGUID", requestModel.UniqueGUID);
                parameters.Add("@UniqueID", requestModel.UniqueID);

                var result = await connection.QueryAsync<int>
                    ("sp_Conduits_UpdateConduit",parameters, commandType: CommandType.StoredProcedure);

                int rowsAffected = result.FirstOrDefault();

                return rowsAffected > 0 ? 1 : 0;
            }
            catch(Exception ex)
            {
                throw new Exception("Error updating conduit path.", ex);
            }
        }

        public async Task<ConduitModel?> GetConduitByUniqueGuid(Guid conduitGuid)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", conduitGuid);
                parameters.Add("@LatestRev", true);

                var result = await db.QueryAsync<ConduitModel>(
                    "sp_Conduits_GetConduitByUniqueGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var conduit = result.FirstOrDefault();

                if (conduit != null)
                {
                    conduit.UniqueGUID = conduitGuid;
                    conduit.LatestRev = true;
                }

                return conduit;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the conduit data." + ex.Message);
            }
        }

        public async Task<ConduitModel?> GetConduitByUniqueId(int conduitId)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueId", conduitId);

                var result = await db.QueryAsync<ConduitModel>(
                    "sp_Conduits_GetConduitByUniqueId",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var conduit = result.FirstOrDefault();

                if (conduit != null)
                {
                    conduit.UniqueID = conduitId;
                }

                return conduit;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the conduit data." + ex.Message);
            }
        }
    }
}
