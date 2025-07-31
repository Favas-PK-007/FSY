#region Namespaces
using Dapper;
using FhaFacilitiesApplication.Domain.Models;
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
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class StructureRepository : IStructureRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IBuildingService _buildingService;
        #endregion


        #region Constructor
        public StructureRepository(IConfiguration configuration, IBuildingService buildingService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _buildingService = buildingService ?? throw new ArgumentNullException(nameof(buildingService));
        }
        #endregion

        public async Task<List<StructureModel>> GetStructureByCampusAndBuildingAsync(string campusGuid, string buildingGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var procedure = "sp_Structures_LoadFromBuilding";
                var parameters = new DynamicParameters();
                parameters.Add("@BuildingGUID", buildingGuid);
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<StructureModel>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception ex)
            {
                // Throw exception with custom message if database call fails
                throw new Exception("Unable to get structures from database: " + ex.Message);
            }


        }


        #region Create Structure
        public async Task<int> CreateStructureAsync(StructureModel requestModel)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var insertParams = new DynamicParameters();
                insertParams.Add("@CampusGUID", requestModel.CampusGUID);
                insertParams.Add("@BuildingGUID", requestModel.BuildingGUID);
                insertParams.Add("@StructureType", requestModel.StructureType);
                insertParams.Add("@CreatedBy", requestModel.CreatedBy);
                insertParams.Add("@CreatedDate", DateTime.Now);
                insertParams.Add("@LastSavedBy", requestModel.CreatedBy);
                insertParams.Add("@LastSavedDate", DateTime.Now);
                insertParams.Add("@UniqueGUID", requestModel.UniqueGUID);
                insertParams.Add("@StructureID", requestModel.StructureID);
                insertParams.Add("@TypeGUID", requestModel.TypeGUID);
                insertParams.Add("@Latitude", requestModel.Latitude);
                insertParams.Add("@Longitude", requestModel.Longitude);
                insertParams.Add("@LocationDesc", requestModel.LocationDesc);
                insertParams.Add("@Comments", requestModel.Comments);
                insertParams.Add("@LatestRev", true);

                bool isInserted = await connection.QuerySingleAsync<bool>(
                                "sp_Structures_InsertStructure",
                                insertParams,
                                commandType: CommandType.StoredProcedure);

                return isInserted ? 1 : 0; // 1 = success, 0 = insert failed

            }
            catch (Exception ex)
            {
                throw new Exception("Error creating structure: " + ex.Message);
            }
        }
        #endregion

       

        #region Get Structure Model by Strycture Type
        public async Task<List<MeterialModel>> GetStructureModelsByTypeAsync(string meterialType, Guid parentGuid, Guid TemplateGuid, bool isAddEdit)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var procedure = "spLoadMaterialsV1";
                var parameters = new DynamicParameters();
                parameters.Add("@AddEdit", isAddEdit);
                parameters.Add("@MaterialType", meterialType);
                parameters.Add("@ParentGUID", parentGuid);
                parameters.Add("@TemplateGUID", TemplateGuid);
                parameters.Add("@LatestRev", true);
                var result = await connection.QueryAsync<MeterialModel>(procedure, parameters, commandType: CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving structure models by type: " + ex.Message);
            }



        }
        #endregion

        public async Task<StructureModel?> GetStructureAsync(Guid uniqueGuid)
        {
            using IDbConnection connection = new SqlConnection(_fhaDbCon);

            const string procedure = "spLoadStructureV1";

            var parameters = new DynamicParameters();
            parameters.Add("@LatestRev", true);
            parameters.Add("@UniqueGUID", uniqueGuid);

            var structure = await connection.QueryFirstOrDefaultAsync<StructureModel>(
                procedure,
                parameters,
                commandType: CommandType.StoredProcedure);

            if (structure == null)
                return null;

            structure.UniqueGUID = uniqueGuid;
            structure.LatestRev = true;

            // Load Building (if you want to include it)
            structure.Building = await _buildingService.GetBuildingByCampusAndBuildingAsync(structure.CampusGUID, structure.BuildingGUID);

            return structure;
        }


        public async Task<List<StructureModel>> GetStructureIdsAsync(Guid campusGuid, Guid buildingGuid, bool loadBuilding, bool loadAllStructures)
        {
            var structures = new List<StructureModel>();

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                string procedureName = loadAllStructures ? "spLoadStructuresV1" : "sp_Structures_LoadFromBuilding";

                var parameters = new DynamicParameters();
                parameters.Add("@LatestRev", true);
                parameters.Add("@CampusGUID", campusGuid);

                if (!loadAllStructures)
                    parameters.Add("@BuildingGUID", buildingGuid);

                var result = await connection.QueryAsync<StructureModel>(procedureName, parameters, commandType: CommandType.StoredProcedure);
                structures = result.ToList();

                // Attach buildings if needed
                if (loadBuilding && loadAllStructures)
                {
                    foreach (var structure in structures)
                    {
                        //structure.Building = await _buildingService.GetBuildingsAsync(structure.CampusGUID, structure.BuildingGUID);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load structures from database: " + ex.Message, ex);
            }

            return structures;
        }


        public async Task<bool> IsExistingStructureIdAsync(Guid campusGuid, string structureId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@StructureID", structureId);
                    parameters.Add("@CampusGUID", campusGuid);
                    parameters.Add("@LatestRev", true);

                    bool isExist = db.QuerySingle<bool>(
                        "sp_Structures_CheckStructure",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return isExist;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking existing structure ID: " + ex.Message);
            }
        }

        public async Task<StructureModel?> GetStructureByIdAsync(int id)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueID", id);
                parameters.Add("@LatestRev", true);
                var structure = await connection.QueryFirstOrDefaultAsync<StructureModel>(
                    "sp_Structures_GetStructureById",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (structure != null)
                {
                    structure.Building = await _buildingService.GetBuildingByCampusAndBuildingAsync(
                        structure.CampusGUID,
                        structure.BuildingGUID
                    );
                }

                return structure;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving structure by ID: " + ex.Message);
            }

        }

        #region Delete Building
        public async Task<int> DeleteStructureByUniqueGuidAsync(StructureModel structureModel)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);

                var deleteQuery = @"
                UPDATE dbo.Structures
                SET 
                    LatestRev = @LatestRev,
                    LastSavedBy = @LastSavedBy,
                    LastSavedDate = @LastSavedDate
                    WHERE (UniqueGUID = @UniqueGUID) AND (UniqueID = @UniqueID)
                SELECT @@ROWCOUNT AS RowsAffected
                ";

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", structureModel.UniqueGUID);
                parameters.Add("@UniqueID", structureModel.UniqueID);
                parameters.Add("@LastSavedBy", structureModel.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);
                parameters.Add("@LatestRev", false);

                var result = await db.QueryAsync<int>(deleteQuery, parameters);

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete building in database.", ex);
            }
        }
        #endregion

        #region Check if Building Exists
        public async Task<StructureModel?> CheckStructureExistsAsync(string UniqueID)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);

                // Additional inline query,not present in existing application
                var checkQuery = @"
                SELECT 
                   UniqueID
                  ,UniqueGUID
                  ,CampusGUID
                  ,BuildingGUID
                  ,StructureType
                  ,StructureID
                  ,TypeGUID
                  ,Latitude
                  ,Longitude
                  ,LocationDesc
                  ,Comments
                  ,LatestRev
                  ,CreatedBy
                  ,CreatedDate
                  ,LastSavedBy
                  ,LastSavedDate

                FROM dbo.Structures 
                WHERE UniqueID = @StructureId";

                var parameters = new
                {
                    StructureId = UniqueID,
                };

                var structure = await db.QueryFirstOrDefaultAsync<StructureModel>(checkQuery, parameters);

                return structure;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to check the structure exists in database.", ex);
            }
        }
        #endregion
    }
}
