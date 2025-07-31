#region Namespaces
using Dapper;
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
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class SpliceRepository : ISpliceRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IStructureRepository _structureRepository;
        #endregion


        #region Constructor
        public SpliceRepository(IConfiguration configuration, IStructureRepository structureRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _structureRepository = structureRepository ?? throw new ArgumentNullException(nameof(structureRepository));
        }
        #endregion


        public async Task<List<SpliceModel>> GetSpliceByCampusAndStructureAsync(Guid campusGuid, Guid structureGuid, bool loadStructure)
        {
            var splices = new List<SpliceModel>();

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                string procedure = structureGuid == Guid.Empty
                    ? "spLoadSplicesV1"
                    : "spLoadSplicesFromStructureV1";

                var parameters = new DynamicParameters();
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);

                if (structureGuid != Guid.Empty)
                    parameters.Add("@StructureGUID", structureGuid);

                var results = await connection.QueryAsync<SpliceModel>(procedure, parameters, commandType: CommandType.StoredProcedure);

                splices = results.Select(splice =>
                {
                    splice.CampusGUID = campusGuid;
                    splice.StructureGUID = structureGuid;
                    return splice;
                }).ToList();

                if (loadStructure)
                {
                    foreach (var splice in splices)
                    {
                        splice.Structure = await _structureRepository.GetStructureAsync(splice.StructureGUID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load splices from database." + ex.Message, ex);
            }

            return splices;
        }


        public async Task<SpliceModel?> GetSpliceByGuidAsync(Guid spliceGuid)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", spliceGuid);
                parameters.Add("@LatestRev", true);

                var result = await dbConnection.QueryFirstOrDefaultAsync<SpliceModel>(
                    "sp_Splices_GetSpliceByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving splice from database: " + ex.Message, ex);
            }
        }

        public async Task<bool> IsSpliceExistsAsync(string spliceId, Guid campusGuid)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@SpliceID", spliceId);
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);

                var result = await dbConnection.QueryFirstOrDefaultAsync<bool>(
                    "sp_Splices_CheckSpliceID",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if Splice ID exists: " + ex.Message, ex);
            }
        }

        public async Task<int> CreateSpliceAsync(SpliceModel model)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@CampusGUID", model.CampusGUID);
                parameters.Add("@StructureGUID", model.StructureGUID);
                parameters.Add("@SpliceType", model.SpliceType);
                parameters.Add("@SpliceID", model.SpliceID);
                parameters.Add("@TypeGUID", model.TypeGUID);
                parameters.Add("@Comments", model.Comments);
                parameters.Add("@LatestRev", model.LatestRev);
                parameters.Add("@CreatedBy", model.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);

                var rowsAffected = await dbConnection.QueryAsync<int>(
                    "sp_Splices_InsertSplice",parameters,commandType: CommandType.StoredProcedure);

                return rowsAffected.FirstOrDefault();
            }
            catch(Exception ex)
            {
                throw new Exception("Error creating splice: " + ex.Message, ex);
            }

        }

        public async Task<int> UpdateSpliceAsync(SpliceModel model)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();

                parameters.Add("@LatestRev", false);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.UtcNow);
                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@UniqueID", model.UniqueID);

                var result = await connection.QueryAsync<int>(
                    "sp_Splices_UpdateSplice",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.FirstOrDefault(); 
            }
            catch (Exception ex)
            {
               throw new Exception("Error updating splice: " + ex.Message, ex);
            }
        }


    }
}
