#region Namespaces
using Dapper;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class DuctRepository : IDuctRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IMeterialService _materialService;
        private readonly IStructureService _structureService;
        #endregion


        #region Constructor
        public DuctRepository(IConfiguration configuration, IMeterialService materialService, IStructureService structureService)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _materialService = materialService;
            _structureService = structureService;
        }
        #endregion

        public async Task<List<DuctModel>> GetDuctsAsync(Guid conduitGuid, bool loadSubDucts)
        {
            using IDbConnection connection = new SqlConnection(_fhaDbCon);

            var parameters = new DynamicParameters();
            parameters.Add("@ConduitGUID", conduitGuid);
            parameters.Add("@LatestRev", true);

            var ducts = (await connection.QueryAsync<DuctModel>(
                "spLoadDuctsV1",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            foreach (var duct in ducts)
            {
                duct.ConduitGUID = conduitGuid;
                duct.LatestRev = true;

                // Fetch material
                duct.Material = await _materialService.GetMeterialsAsync(duct.UniqueGUID, duct.TypeGUID, true, false);

                // Fetch structure
                //duct.Structure = await _structureService.GetStructureAsync(duct.StructureGUID);

                // Recursively fetch sub-ducts
                if (loadSubDucts)
                {
                    duct.SubDucts = await GetDuctsAsync(duct.UniqueGUID, false);
                }
            }

            return ducts;
        }

        public async Task<int> CreateDuctAsync(DuctModel requestModel)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", requestModel.UniqueGUID);
                parameters.Add("@StructureGUID", requestModel.StructureGUID);
                parameters.Add("@ConduitGUID", requestModel.ConduitGUID);
                parameters.Add("@DuctID", requestModel.DuctID);
                parameters.Add("@DuctID_B", requestModel.DuctID_B);
                parameters.Add("@TypeGUID", requestModel.TypeGUID);
                parameters.Add("@Comments", requestModel.Comments);
                parameters.Add("@LatestRev", true);
                parameters.Add("@CreatedBy", requestModel.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@LastSavedBy", requestModel.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);

                // Get the number of affected rows from the stored procedure
                int affectedRows = await connection.QuerySingleAsync<int>(
                    "sp_Ducts_InsertDuct",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return affectedRows > 0 ? 1 : 0;

            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while inserting the duct.", ex);
            }
        }

        public async Task<DuctModel?> GetDuctByGuidAsync(Guid ductGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", ductGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryFirstOrDefaultAsync<DuctModel>(
                    "sp_Ducts_GetDuctByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                throw new Exception("Error retrieving duct by GUID: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateDuctAsync(DuctModel requestModel)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", requestModel.UniqueGUID);
                parameters.Add("@UniqueID", requestModel.UniqueID);
                parameters.Add("@LatestRev", false); // Mark current revision as old
                parameters.Add("@LastSavedBy", "");
                parameters.Add("@LastSavedDate", DateTime.UtcNow);

                var rowsAffected = await connection.ExecuteAsync(
                    "sp_Ducts_UpdateLatestRevisionStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating duct revision status: " + ex.Message, ex);
            }
        }

        public async Task<bool> DeleteDuctAsync(DuctModel requestModel)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                var procedure = "sp_Ducts_UpdateDuct";
                parameters.Add("@UniqueGUID", requestModel.UniqueGUID);
                parameters.Add("@UniqueID", requestModel.UniqueID);
                parameters.Add("@LatestRev", false); // Mark current revision as old
                parameters.Add("@LastSavedBy", "");
                parameters.Add("@LastSavedDate", DateTime.UtcNow);

                var rowsAffected = await connection.QueryAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return rowsAffected.FirstOrDefault() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting duct: " + ex.Message, ex);
            }
        }
    }
}
