#region Namespaces
using FhaFacilitiesApplication.Domain.Models;
using System.Data;
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Dapper;
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
        public async Task<List<CampusModel>> GetCampusListAsync()
        {
            try
            {
                // Create a SQL connection using the configured connection string
                using (IDbConnection dbConnection = new SqlConnection(_fhaDbCon))
                {
                    // Prepare parameters for the stored procedure
                    var parameters = new DynamicParameters();
                    parameters.Add("@LatestRev", true);

                    // Execute the stored procedure and map results to CampusModel list
                    var result = await dbConnection.QueryAsync<CampusModel>(
                        "spLoadCampusesV1",           // Stored procedure name
                        parameters,                   // Input parameters
                        commandType: CommandType.StoredProcedure
                    ).ConfigureAwait(false);         // Avoid context capture in library code

                    // Return the list of campus records
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                // Throw exception with custom message if database call fails
                throw new Exception("Unable to Get Campuses from Database: " + ex.Message, ex);
            }
        }
        #endregion

        #region AddCampus
        public async Task<string> AddCampusAsync(CampusModel requestModel)
        {
            try
            {
                using var db = new SqlConnection(_fhaDbCon);

                // Check if CampusID already exists
                var checkQuery = @"
                SELECT *
                FROM dbo.Campuses 
                WHERE CampusID = @CampusID AND LatestRev = @LatestRev";

                var checkParams = new
                {
                    CampusID = requestModel.CampusID,
                    LatestRev = true
                };

                var exists = await db.ExecuteScalarAsync<bool>(checkQuery, checkParams);

                if (exists)
                {
                    return "Campus ID already exists.";
                }

                // Prepare data for insert
                requestModel.UniqueGUID = Guid.NewGuid();
                requestModel.LatestRev = true;

                // Insert query
                var insertQuery = @"
                INSERT INTO dbo.Campuses (
                    UniqueGUID, CampusID, Designation, Latitude, Longitude, Comments,
                    LatestRev, CreatedBy, CreatedDate, LastSavedBy, LastSavedDate)
                VALUES (
                    @UniqueGUID, @CampusID, @Designation, @Latitude, @Longitude, @Comments,
                    @LatestRev, @CreatedBy, @CreatedDate, @LastSavedBy, @LastSavedDate);";

                var affected = await db.ExecuteAsync(insertQuery, requestModel);

                return affected > 0 ? "Campus added successfully." : "Insertion failed.";
            }
            catch (Exception ex)
            {
                return "Error occurred while adding campus: " + ex.Message;
            }
        }
        #endregion
    }
}
