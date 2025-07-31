#region Namespaces
using Dapper;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class MeterialRepository : IMeterialRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IMaterialDetailRepository _materialDetailRepository;
        private readonly IComponentRepository _componentRepository;
        #endregion


        #region Constructor
        public MeterialRepository(IConfiguration configuration, IMaterialDetailRepository materialDetailRepository, IComponentRepository componentRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _materialDetailRepository = materialDetailRepository;
            _componentRepository = componentRepository;
        }
        #endregion

        public async Task<MeterialModel?> GetMeterialsAsync(Guid parentGuid, Guid templateGuid, bool loadDetails, bool loadComponents)
        {
            using IDbConnection connection = new SqlConnection(_fhaDbCon);

            string procedure = templateGuid == Guid.Empty
                ? "spLoadMaterialV1"
                : "spLoadMaterialFromTemplateV1";

            var parameters = new DynamicParameters();
            parameters.Add("@LatestRev", true);
            parameters.Add("@ParentGUID", parentGuid);

            if (templateGuid != Guid.Empty)
                parameters.Add("@TemplateGUID", templateGuid);

            var meterial = await connection.QueryFirstOrDefaultAsync<MeterialModel>(
                            procedure,
                            parameters,
                            commandType: CommandType.StoredProcedure);
            if (meterial is null)
            {
                return null;
            }

            meterial.ParentGUID = parentGuid;
            meterial.TemplateGUID = templateGuid;
            meterial.LatestRev = true;


            if (loadDetails)
            {
                if (meterial.TemplateGUID == Guid.Empty)
                {
                    meterial.Details = await _materialDetailRepository.GetMaterialDetailsAsync(meterial.MaterialType, meterial.UniqueGUID, Guid.Empty);
                }
                else
                {
                    meterial.Details = await _materialDetailRepository.GetMaterialDetailsAsync(meterial.MaterialType, meterial.TemplateGUID, Guid.Empty);
                }
            }

            // Load components if requested
            if (loadComponents)
            {
                meterial.Components = await _componentRepository.GetComponentsAsync(meterial.MaterialType, meterial.UniqueGUID, loadDetails, loadComponents);
            }

            return meterial;
        }

        public async Task<MeterialModel?> GetTemplateMeterialAsync(Guid uniqueGuid, Guid parentGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@ParentGUID", parentGuid);
                parameters.Add("@LatestRev", true);

                var material = await connection.QueryFirstOrDefaultAsync<MeterialModel>(
                    "sp_Meterial_GetByParentGuidAndUniqueGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return material;
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching template material", ex);
            }
        }


        public async Task<int> SaveNewDuctMaterialAsync(MeterialModel material)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", material.UniqueGUID);
                parameters.Add("@ParentGUID", material.ParentGUID);
                parameters.Add("@TemplateGUID", material.TemplateGUID);
                parameters.Add("@MaterialType", material.MaterialType);
                parameters.Add("@MaterialID", material.MaterialID);
                parameters.Add("@ManufacturerID", material.ManufacturerID);
                parameters.Add("@ModelID", material.ModelID);
                parameters.Add("@Comments", material.Comments);
                parameters.Add("@LatestRev", material.LatestRev);
                parameters.Add("@CreatedBy", material.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@LastSavedBy", material.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);

                bool isInserted = await connection.QuerySingleAsync<bool>(
                               "sp_InsertMaterial",
                               parameters,
                               commandType: CommandType.StoredProcedure);

                return isInserted ? 1 : 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting material", ex);
            }
        }

        public async Task<List<MeterialModel>> GetStructureModels(bool addEdit, string materialType, Guid parentGuid, Guid templateGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@AddEdit", addEdit);
                parameters.Add("@ParentGUID", parentGuid);
                parameters.Add("@LatestRev", true);
                parameters.Add("@TemplateGUID", templateGuid);
                parameters.Add("@MaterialType", materialType);

                var material = await connection.QueryAsync<MeterialModel>(
                    "spLoadMaterialsV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return material.Select(m =>
                {
                    m.LatestRev = true;
                    m.MaterialType = materialType;
                    return m;
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching  material for structure models in structure" + ex.Message);
            }
        }

        public async Task<MeterialModel?> GetMaterialByGuidAsync(Guid uniqueGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<MeterialModel>(
                    "sp_Materials_GetMaterialByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.FirstOrDefault();


            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving material by UniqueGUID: " + ex.Message, ex);
            }
        }

        public async Task<MeterialModel?> GetMaterialByParentGuidAsync(Guid parentGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@ParentGUID", parentGuid);
                parameters.Add("@LatestRev", true);
                parameters.Add("@MaterialType", "Cable");
                var result = await connection.QueryAsync<MeterialModel>(
                    "sp_Materials_GetMaterialByParentGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving material by ParentGUID: " + ex.Message, ex);
            }


        }

        #region Get Subducts for the main ducts if any
        public async Task<List<MeterialModel>> GetSubDuctsAsync(Guid parentGuid, Guid templateGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                string procedure = templateGuid == Guid.Empty
                    ? "spLoadMaterialV1"
                    : "spLoadMaterialFromTemplateV1";

                var parameters = new DynamicParameters();
                parameters.Add("@LatestRev", true);
                parameters.Add("@ParentGUID", parentGuid);

                if (templateGuid != Guid.Empty)
                    parameters.Add("@TemplateGUID", templateGuid);

                var materials = (await connection.QueryAsync<MeterialModel>(
                                    procedure,
                                    parameters,
                                    commandType: CommandType.StoredProcedure))
                                .ToList();

                foreach (var material in materials)
                {
                    material.ParentGUID = parentGuid;
                    material.TemplateGUID = templateGuid;
                    material.LatestRev = true;
                }

                return materials;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while loading subduct materials." + ex.Message);
            }
        }
        #endregion

        public async Task<bool> BulkInsertMaterialsAsync(List<MeterialModel> materials)
        {
            using IDbConnection connection = new SqlConnection(_fhaDbCon);

            var sql = @"INSERT INTO Materials (UniqueGUID, ParentGUID, TemplateGUID, MaterialType, MaterialID,
                                       ManufacturerID, ModelID, Comments, CreatedBy, CreatedDate,
                                       LastSavedBy, LastSavedDate, LatestRev)
                VALUES (@UniqueGUID, @ParentGUID, @TemplateGUID, @MaterialType, @MaterialID,
                        @ManufacturerID, @ModelID, @Comments, @CreatedBy, @CreatedDate,
                        @LastSavedBy, @LastSavedDate, @LatestRev);"
            ;

            var rows = await connection.ExecuteAsync(sql, materials);
            return rows == materials.Count;
        }

        public async Task<int> DeleteMaterialAsync(MeterialModel model)
        {
            using (var connection = new SqlConnection(_fhaDbCon))
            {

                var parameters = new DynamicParameters();
                var procedure = "sp_Materials_UpdateMaterial";
                parameters.Add("@ParentGUID", model.ParentGUID);
                parameters.Add("@LatestRev", false); // Mark current revision as old
                parameters.Add("@LatestRevBefore", true); // Mark current revision as old
                parameters.Add("@LastSavedBy", "");
                parameters.Add("@LastSavedDate", DateTime.UtcNow);

                var rowsAffected = await connection.QueryAsync<int>(procedure, parameters, commandType: CommandType.StoredProcedure);

                return rowsAffected.FirstOrDefault();
            }
        }

        public async Task<List<string>> GetMaterialTypesAsync()
        {
            var materialTypes = new List<string>();

            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@LatestRev", true);

                    var result = await db.QueryAsync<string>(
                        "spLoadMaterialTypesV1",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    materialTypes = result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load material types from database." + ex.Message, ex);
            }

            return materialTypes;
        }

        public async Task<List<MeterialModel>> GetParentMaterialsAsync(string materialType)
        {
            var materials = new List<MeterialModel>();
            string parentMaterial;
            string spName;

            // Map to parent material
            switch (materialType)
            {
                case "Splice Tray":
                    parentMaterial = "Splice";
                    spName = "spLoadParentMaterialsV1";
                    break;
                case "Splice Module":
                    parentMaterial = "Splice Tray";
                    spName = "spLoadParentMaterialsV1";
                    break;
                case "FPP Cartridge":
                    parentMaterial = "FPP Chassis";
                    spName = "spLoadParentMaterialsV1";
                    break;
                case "FPP Module":
                    parentMaterial = "FPP Cartridge";
                    spName = "spLoadParentMaterialsV1";
                    break;
                default:
                    parentMaterial = materialType;
                    spName = "spLoadTopLevelParentMaterialsV1";
                    break;
            }

            using (IDbConnection db = new SqlConnection(_fhaDbCon)) // or use injected IDbConnection
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@LatestRev", true);
                    parameters.Add("@TemplateGUID", Guid.Empty);
                    parameters.Add("@Materialtype", parentMaterial);

                    if (parentMaterial == materialType)
                        parameters.Add("@ParentGUID", Guid.Empty);

                    var result = await db.QueryAsync<MeterialModel>(spName, parameters, commandType: CommandType.StoredProcedure);
                    materials = result.ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to load parent materials: " + ex.Message, ex);
                }
            }

            return materials;
        }

        public async Task<List<MeterialModel>> GetChildMaterialsAsync(Guid parentGuid, string materialType)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ParentGUID", parentGuid);
                    parameters.Add("@TemplateGUID", Guid.Empty);
                    parameters.Add("@LatestRev", true);

                    string storedProc = parentGuid == Guid.Empty
                        ? "spLoadTopLevelChildMaterialsV1"
                        : "spLoadChildMaterialsV1";

                    if (parentGuid == Guid.Empty)
                    {
                        parameters.Add("@MaterialType", materialType);
                    }

                    var result = await db.QueryAsync<MeterialModel>(
                        storedProc,
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    foreach (var material in result)
                    {
                        material.ParentGUID = parentGuid;
                        material.TemplateGUID = Guid.Empty;
                        material.LatestRev = true;
                    }

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading child materials from database: " + ex.Message, ex);
            }
        }

        public async Task<List<string>> GetMaterialManufacturersAsync(string materialType)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@LatestRev", true);
                    parameters.Add("@MaterialType", materialType);

                    var result = await db.QueryAsync<string>(
                        "spLoadMaterialManufacturersV1",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading material manufacturers from database: " + ex.Message, ex);
            }
        }


        public async Task<List<string>> GetMaterialDetailHeadersAsync(string materialType)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@MaterialType", materialType);
                    parameters.Add("@LatestRev", true);

                    var result = await db.QueryAsync<string>(
                        "sp_Material_Details_GetMaterialDetailHeaders", // make sure this matches your stored proc
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );

                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading material detail headers from database: " + ex.Message, ex);
            }
        }

        public async Task<bool> CheckMaterialExistAsync(string modelId, string materialType, string manufacturerId)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ModelID", modelId);
                    parameters.Add("@MaterialType", materialType);
                    parameters.Add("@ManufacturerID", manufacturerId);
                    parameters.Add("@LatestRev", true);
                    var result = await db.QuerySingleOrDefaultAsync<bool>(
                        "sp_Materials_CheckMaterialExist",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if material exists: " + ex.Message, ex);
            }
        }

        public async Task<bool> UpdateMaterialAsync(MeterialModel model)
        {
            try
            {
                using (IDbConnection connection = new SqlConnection(_fhaDbCon))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@LatestRev", false);
                    parameters.Add("@LastSavedBy", model.LastSavedBy);
                    parameters.Add("@LastSavedDate", DateTime.Now);
                    parameters.Add("@UniqueGUID", model.UniqueGUID);
                    parameters.Add("@ParentGUID", model.ParentGUID);
                    parameters.Add("@UniqueID", model.UniqueID);

                    bool isUpdated = await connection.QuerySingleAsync<bool>(
                        "sp_Materials_Update",
                        parameters,
                        commandType: CommandType.StoredProcedure);

                    return isUpdated;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating material revision: " + ex.Message, ex);
            }
        }
    }
}
