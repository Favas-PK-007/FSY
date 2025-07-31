#region Namespaces
using System.Data;
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Models.Common;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class CampusRepository : ICampusRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion

        #region Constructor
        public CampusRepository(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        #region GetCampus
        public async Task<List<CampusModel>> GetAllAsync()
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var procedure = "spLoadCampusesV1";
                var parameters = new DynamicParameters();
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<CampusModel>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return result.ToList();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get campuses from database: " + ex.Message);
            }
        }
        #endregion




        #region Get Campus By UniqueID
        public async Task<CampusModel?> GetCampusByUniqueID(string uniqueID)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);
                var query = @"
                SELECT *
                FROM dbo.Campuses
                WHERE UniqueID = @UniqueID AND LatestRev = @LatestRev";
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueID", uniqueID);
                parameters.Add("@LatestRev", true);
                var campus = await db.QueryFirstOrDefaultAsync<CampusModel>(query, parameters);
                return campus;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving campus by UniqueID", ex);
            }
        }
        #endregion



        #region AddCampus
        public async Task<int> AddCampusAsync(CampusModel requestModel)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                var procedure = "SP_Campus_InsertIfNotExists";
                parameters.Add("@UniqueGUID", requestModel.UniqueGUID);
                parameters.Add("@CampusID", requestModel.CampusID);
                parameters.Add("@Designation", requestModel.Designation);
                parameters.Add("@Latitude", requestModel.Latitude);
                parameters.Add("@Longitude", requestModel.Longitude);
                parameters.Add("@Comments", requestModel.Comments);
                parameters.Add("@LatestRev", true);
                parameters.Add("@CreatedBy", requestModel.CreatedBy);
                parameters.Add("@CreatedDate", requestModel.CreatedDate);
                parameters.Add("@LastSavedBy", requestModel.LastSavedBy);
                parameters.Add("@LastSavedDate", requestModel.LastSavedDate);

                var result = await connection.QueryAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while adding campus", ex);
            }
        }
        public async Task<int> SoftDeleteCampusByIdAsync(int uniqueID,Guid? uniqueGUID,string user)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var procedure = "SP_Campuses_SoftDelete";
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueID", uniqueID);
                parameters.Add("@UniqueGUID", uniqueGUID);
                parameters.Add("@LatestRev", false);
                parameters.Add("@LastSavedBy", user);
                parameters.Add("@LastSavedDate", DateTime.Now);

                var result = await connection.QueryAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while updating campus", ex);
            }
        }
        #endregion
    }
}
