#region Namespaces
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class BuildingRepository : IBuildingRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion


        #region Constructor
        public BuildingRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        #region Get Buildings
        public async Task<List<BuildingModel>> GetBuildingsAsync(Guid campusGUID, bool onlyBuildings)
        {
            var buildings = new List<BuildingModel>();

            if (!onlyBuildings)
            {
                buildings.Add(new BuildingModel
                {
                    UniqueID = 0,
                    UniqueGUID = Guid.NewGuid(),
                    CampusGUID = campusGUID,
                    BuildingID = "No Building Selected",
                    Designation = "OSP",
                    Latitude = 0.0,
                    Longitude = 0.0,
                    Comments = "",
                    LatestRev = true,
                    CreatedBy = "",
                    CreatedDate = DateTime.UtcNow,
                    LastSavedBy = "",
                    LastSavedDate = DateTime.UtcNow
                });
            }

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var result = await connection.QueryAsync<BuildingModel>(
                    "spLoadBuildingsV1",
                    new { CampusGUID = campusGUID, LatestRev = true },
                    commandType: CommandType.StoredProcedure
                );

                foreach(var building in result)
                {
                    building.CampusGUID = campusGUID;
                    building.LatestRev = true;
                }

                buildings.AddRange(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get buildings from database.", ex);
            }

            return buildings;
        }
        #endregion

        #region Add Building
        public async Task<int> AddBuildingAsync(BuildingModel buildingModel)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);

                // Check if buildingId already exists
                var checkQuery = @"
                SELECT * FROM dbo.Buildings 
                WHERE (BuildingID = @BuildingID) AND (CampusGUID = @CampusGUID)
                AND (LatestRev = @LatestRev)";

                var checkParams = new
                {
                    buildingId = buildingModel.BuildingID,
                    campusGuid = buildingModel.CampusGUID,
                    latestRev = true
                };

                var exists = await db.ExecuteScalarAsync<int>(checkQuery, checkParams);

                if (exists > 0)
                {
                    return  -1; // Building already exists
                }

                // Insert query
                var insertQuery = @"
                INSERT INTO dbo.Buildings (UniqueGUID, CampusGUID, BuildingID, Designation, Latitude, Longitude, Comments, LatestRev, CreatedBy, CreatedDate, LastSavedBy, LastSavedDate) VALUES " +
                "(@UniqueGUID, @CampusGUID, @BuildingID, @Designation, @Latitude, @Longitude, @Comments, @LatestRev, @CreatedBy, @CreatedDate, @LastSavedBy, @LastSavedDate)";

                //buildingModel.UniqueGUID = Guid.NewGuid();
                buildingModel.LatestRev = true;

                var result = await db.ExecuteAsync(insertQuery, buildingModel);

                return result > 0 ? 1 : 0; // 1 = success, 0 = failed
            
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to add building to database.", ex);
            }
        }
        #endregion

        #region Get Building by UniqueId
        public async Task<BuildingModel?> GetBuildingByUniqueIdAsync(string buildingUniqueId)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var result = await connection.QueryAsync<BuildingModel>
                (
                    "sp_Building_GetByUniqueId",
                    new { UniqueId = buildingUniqueId },
                    commandType: CommandType.StoredProcedure
                );

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get building by Unique ID from database.", ex);
            }
        }
        #endregion


      


        #region Delete Building
        public async Task<int> DeleteBuildingByUniqueGuidAsync(BuildingModel buildingModel)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);

                var deleteQuery = @"
                UPDATE dbo.Buildings
                SET 
                    LatestRev = @LatestRev,
                    LastSavedBy = @LastSavedBy,
                    LastSavedDate = @LastSavedDate
                    WHERE (UniqueGUID = @UniqueGUID) AND (UniqueID = @UniqueID)";

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", buildingModel.UniqueGUID);
                parameters.Add("@UniqueID", buildingModel.UniqueID);
                parameters.Add("@LastSavedBy", buildingModel.LastSavedBy);
                parameters.Add("@LastSavedDate", buildingModel.LastSavedDate);
                parameters.Add("@LatestRev", false);

                var result = await db.ExecuteAsync(deleteQuery, parameters);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete building in database.", ex);
            }
        }
        #endregion

        #region Check if Building Exists
        public async Task<BuildingModel?> CheckBuildingExistsAsync(string buildingId)
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
                   ,BuildingID
                   ,Designation
                   ,Latitude
                   ,Longitude
                   ,Comments
                   ,LatestRev
                   ,CreatedBy
                   ,CreatedDate
                   ,LastSavedBy
                   ,LastSavedDate

                FROM dbo.Buildings 
                WHERE UniqueID = @BuildingID";

                var parameters = new
                {
                    BuildingID = buildingId,
                };

                var building = await db.QueryFirstOrDefaultAsync<BuildingModel>(checkQuery, parameters);

                return building;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to check the building exists in database.", ex);
            }
        }
        #endregion







        public async Task<BuildingModel?> GetBuildingByCampusAndBuildingAsync(Guid campusGuid, Guid buildingGuid)
        {
            using IDbConnection connection = new SqlConnection(_fhaDbCon);
            const string procedure = "spLoadBuildingV1";

            var parameters = new DynamicParameters();
            parameters.Add("@CampusGUID", campusGuid);
            parameters.Add("@UniqueGUID", buildingGuid);
            parameters.Add("@LatestRev", true);

            try
            {
                var building = await connection.QueryFirstOrDefaultAsync<BuildingModel>(
                    procedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                if (building != null)
                {
                    building.CampusGUID = campusGuid;
                    building.UniqueGUID = buildingGuid;
                    building.LatestRev = true;
                }

                return building;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load building from database." + ex.Message, ex);
            }
        }
    }
}
