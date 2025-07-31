using Dapper;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace FhaFacilitiesApplication.Storage
{
    public class MaterialDetailRepository : IMaterialDetailRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion

        #region Constructor
        public MaterialDetailRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<Dictionary<string, string>> GetMaterialDetailsAsync(string materialType, Guid materialGuid, Guid templateGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@MaterialType", materialType);
                parameters.Add("@MaterialGUID", materialGuid);
                parameters.Add("@TemplateGUID", templateGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<MaterialDetailModel>(
                    "sp_Material_Details_GetMaterialDetails", // Created by own
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToDictionary(x => x.Header, x => x.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load material details. " + ex.Message, ex);
            }
        }

        public async Task<int> SaveMaterialDetailsAsync(MeterialModel material)
        {
            int totalRowsAffected = 0;

            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                foreach (var detail in material.Details)
                {
                    var parameters = new DynamicParameters();

                    parameters.Add("@MaterialType", material.MaterialType);
                    parameters.Add("@MaterialGUID", material.UniqueGUID);
                    parameters.Add("@Header", detail.Key); // Dictionary key
                    parameters.Add("@Value", detail.Value); // Dictionary value
                    parameters.Add("@Comments", material.Comments);
                    parameters.Add("@LatestRev", material.LatestRev);
                    parameters.Add("@CreatedBy", material.CreatedBy);
                    parameters.Add("@CreatedDate", material.CreatedDate);
                    parameters.Add("@LastSavedBy", material.LastSavedBy);
                    parameters.Add("@LastSavedDate", material.LastSavedDate);

                    int rowsAffected = await connection.QuerySingleAsync<int>(
                        "sp_Material_Details_SaveMaterialDetail",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    totalRowsAffected += rowsAffected;
                }

                return totalRowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to save material details", ex);
            }
        }
    }
}
