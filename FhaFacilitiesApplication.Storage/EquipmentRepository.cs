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

namespace FhaFacilitiesApplication.Storage
{
    public class EquipmentRepository : IEquipmentRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IMeterialRepository _meterialRepository;
        private readonly IMaterialDetailRepository _materialDetailRepository;
        #endregion


        #region Constructor
        public EquipmentRepository(IConfiguration configuration, IMeterialRepository meterialRepository, IMaterialDetailRepository materialDetailRepository)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _meterialRepository = meterialRepository ?? throw new ArgumentNullException(nameof(meterialRepository));
            _materialDetailRepository = materialDetailRepository;
        }
        #endregion

        public async Task<EquipmentModel?> GetEquipmentAsync(Guid uniqueGuid, Guid portGuid)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@PortGUID", portGuid);
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@LatestRev", true);

                var equipment = await connection.QueryFirstOrDefaultAsync<EquipmentModel>(
                    "sp_Equipment_GetEquipment",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                return equipment;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load equipment from database.", ex);
            }
        }

        public async Task<Dictionary<int, EquipmentModel>> GetInstalledEquipmentAsync(Guid structureGuid, Guid spliceGuid)
        {
            var result = new Dictionary<int, EquipmentModel>();

            var parameters = new DynamicParameters();
            parameters.Add("@StructureGUID", structureGuid);
            parameters.Add("@SpliceGUID", spliceGuid);
            parameters.Add("@LatestRev", true);

            const string storedProcedure = "spLoadInstalledEquipmentV1";

            using (IDbConnection connection = new SqlConnection(_fhaDbCon))
            {
                var equipmentList = await connection.QueryAsync<EquipmentModel>(
                    storedProcedure,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                foreach (var item in equipmentList)
                {
                    result[item.UniqueID] = item;
                }
            }

            return result;
        }

        public async Task<List<string>> GetEquipmentTypesAsync()
        {

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@MaterialType", "Equipment");
                parameters.Add("@Header", "EquipmentType");
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<string>(
                    "spLoadEquipmentTypesV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load equipment types from database.", ex);
            }
        }

        public async Task<List<MeterialModel>> GetEquipmentModelAsync(string equipmentType)
        {
            var equipmentModelList = new List<MeterialModel>();

            try
            {
                using IDbConnection db = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@MaterialType", "Equipment");
                parameters.Add("@Value", equipmentType);
                parameters.Add("@LatestRev", true);

                // Step 1: Get Material GUIDs and basic details using stored procedure
                var materialDetails = await db.QueryAsync<MaterialDetailModel>(
                    "sp_Material_Details_GetEquipmentModels",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Step 2: For each item, load full MeterialModel
                foreach (var detail in materialDetails)
                {
                    if (detail.MaterialGUID != Guid.Empty)
                    {
                        var material = await _meterialRepository.GetMeterialsAsync(
                            detail.MaterialGUID,
                            Guid.Empty,
                            false,
                            false
                        );

                        if (material == null)
                        {
                            // Fallback if full material is not found
                            material = new MeterialModel
                            {
                                UniqueGUID = detail.MaterialGUID,
                                ParentGUID = Guid.Empty
                            };
                        }

                        equipmentModelList.Add(material);
                    }
                }

                // Step 3: Load template and equipment details
                for (int i = 0; i < equipmentModelList.Count; i++)
                {
                    var mat = equipmentModelList[i];

                    var templateMaterial = await _meterialRepository.GetTemplateMeterialAsync(
                        mat.UniqueGUID,
                        mat.ParentGUID
                    );

                    if (templateMaterial != null)
                    {
                        // Apply template values (if needed)
                        mat.ModelID = templateMaterial.ModelID;
                        mat.MaterialType = templateMaterial.MaterialType;

                    }

                    // Load additional material details
                    mat.Details = await _materialDetailRepository.GetMaterialDetailsAsync(
                        "Equipment",
                        mat.UniqueGUID,
                        Guid.Empty
                    );
                }

                return equipmentModelList;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load equipment models from the database.", ex);
            }
        }


        public async Task<Dictionary<int, string>> GetAvailableEquipmentPortAsync(Guid equimentStructureGuid, Guid equipmentSpliceGuid, Guid equipmentTypeGuid, string selectedEquipmentId)
        {
            var availablePorts = new Dictionary<int, string>();

            using IDbConnection db = new SqlConnection(_fhaDbCon);

            var parameters = new DynamicParameters();
            parameters.Add("@StructureGUID", equimentStructureGuid);
            parameters.Add("@SpliceGUID", equipmentSpliceGuid);
            parameters.Add("@TypeGUID", equipmentTypeGuid);
            parameters.Add("@EquipmentID", selectedEquipmentId);
            parameters.Add("@FiberA_GUID", Guid.Empty);
            parameters.Add("@LatestRev", true);

            var results = await db.QueryAsync<dynamic>(
                "sp_Equipements_GetAvailableEquipmentPorts",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            foreach (var item in results)
            {
                if (item.PortID != null)
                {
                    availablePorts[(int)item.UniqueID] = (string)item.PortID;
                }
            }

            return availablePorts;
        }


        public async Task<bool> IsEquipmentExistAsync(string equipmentId, Guid equipmentStructureGuid, Guid equipmentSpliceGuid)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@EquipmentID", equipmentId);
                parameters.Add("@StructureGUID", equipmentStructureGuid);
                parameters.Add("@SpliceGUID", equipmentSpliceGuid);
                parameters.Add("@LatestRev", true);
                var result = await db.QueryFirstOrDefaultAsync<bool>(
                    "sp_Equipment_CheckIfExistingEquipmentID",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to check if equipment exists in the database.", ex);
            }

        }

        public async Task<int> SaveEquipmentAsync(EquipmentModel model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();

                parameters.Add("@UniqueGUID", model.UniqueGUID);
                parameters.Add("@CampusGUID", model.CampusGUID);
                parameters.Add("@StructureGUID", model.StructureGUID);
                parameters.Add("@EquipmentType", model.EquipmentType);
                parameters.Add("@EquipmentID", model.EquipmentID);
                parameters.Add("@PortID", model.PortID);
                parameters.Add("@TypeGUID", model.TypeGUID);
                parameters.Add("@ConnectionType", model.ConnectionType);
                parameters.Add("@SpliceGUID", model.SpliceGUID);
                parameters.Add("@PortGUID", model.PortGUID);
                parameters.Add("@FiberA_GUID", model.FiberA_GUID);
                parameters.Add("@FiberB_GUID", model.FiberB_GUID);
                parameters.Add("@Sequence", model.Sequence);
                parameters.Add("@Comments", model.Comments);
                parameters.Add("@LatestRev", model.LatestRev);
                parameters.Add("@CreatedBy", model.CreatedBy);
                parameters.Add("@CreatedDate", DateTime.Now);
                parameters.Add("@LastSavedBy", model.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);


                var affectedRows = await db.QuerySingleAsync<int>(
                 "sp_Equipment_AddEquipment",
                 parameters,
                 commandType: CommandType.StoredProcedure);

                return affectedRows;

            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding equipment.", ex);
            }
        }


        public async Task<EquipmentModel?> GerEquipmentByPortAndGuid(Guid portGuid, Guid uniqueGuid)
        {
            try
            {
                using var connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@PortGUID", portGuid);
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@LatestRev", true);
                var equipment = await connection.QueryFirstOrDefaultAsync<EquipmentModel>(
                    "sp_Equipment_GetByPortAndGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure);
                return equipment;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load equipment by port and GUID from database.", ex);
            }
        }
    }
}
