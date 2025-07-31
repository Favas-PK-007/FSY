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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Storage
{
    public class CableRepository : ICableRepository
    {

        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IFiberService _fiberService;
        #endregion

        #region Constructor
        public CableRepository(IConfiguration configuration, IFiberService fiberService)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _fiberService = fiberService;
        }
        #endregion


        public async Task<List<CableModel>> GetAllCablesAsync(Guid campusGuid, Guid spliceGuid, bool loadFibers)
        {
            var cables = new List<CableModel>();
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);

                string storedProc;

                if (spliceGuid == Guid.Empty)
                {
                    storedProc = "spLoadCablesFromCampusV1";
                }
                else
                {
                    storedProc = "spLoadCablesFromSpliceV1";
                    parameters.Add("@SpliceGUID", spliceGuid);
                }

                var result = await dbConnection.QueryAsync<CableModel>(
                    storedProc,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                cables = result.Select(c => { c.CampusGUID = campusGuid; return c; }).ToList();

                // Optional: load fibers here if needed
                if (loadFibers)
                {
                    for (int i = 0; i < cables.Count; i++)
                    {
                        var cable = cables[i];
                        if (cable.Fiber == null)
                        {
                            // Load fibers for the cable
                            cable.Fiber = await _fiberService.GetFiberAsync(cable.UniqueGUID, loadFibers);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading cables: " + ex.Message, ex);
            }

            return cables;
        }

        public async Task<CableModel?> GetCableByGuidAsync(Guid cableGuid, string? action)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@CableGUID", cableGuid);
                parameters.Add("@LatestRev", true);

                // Execute the stored procedure and get the result
                var result = await dbConnection.QueryAsync<CableModel>(
                    "sp_Cables_LoadCableByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var cable = result.FirstOrDefault();

                if(action?.ToLower() != "delete")
                {
                    // If cable exists, load its fiber details
                    if (cable != null)
                    {
                        cable.Fiber = await _fiberService.GetFiberAsync(cable.UniqueGUID, loadCircuits: true);
                    }
                }            

                return cable;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading cable by GUID: " + ex.Message, ex);
            }
        }

        public async Task<bool> IsCableExistsAsync(string cableId, Guid campusGuid)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@CableID", cableId);
                parameters.Add("@CampusGUID", campusGuid);
                parameters.Add("@LatestRev", true);
                var result = await dbConnection.QuerySingleAsync<bool>(
                         "sp_Cable_CheckIfExistingCable",
                         parameters,
                         commandType: CommandType.StoredProcedure);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if cable exists: " + ex.Message, ex);
            }
        }

        public async Task<int> SaveCableAsync(CableModel cable)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", cable.UniqueGUID);
                parameters.Add("@CampusGUID", cable.CampusGUID);
                parameters.Add("@CableType", cable.CableType);
                parameters.Add("@CableID", cable.CableID);
                parameters.Add("@TypeGUID", cable.TypeGUID);
                parameters.Add("@SpliceA_GUID", cable.SpliceA_GUID);
                parameters.Add("@SpliceB_GUID", cable.SpliceB_GUID);
                parameters.Add("@DuctGUID", cable.DuctGUID);
                parameters.Add("@Comments", cable.Comments ?? "");
                parameters.Add("@LatestRev", true);
                parameters.Add("@CreatedBy", "");
                parameters.Add("@CreatedDate", DateTime.UtcNow);
                parameters.Add("@LastSavedBy", "");
                parameters.Add("@LastSavedDate", DateTime.UtcNow);

                var result = await connection.QuerySingleOrDefaultAsync<int>(
                    "sp_Cable_InsertCable",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting cable: " + ex.Message, ex);
            }
        }

        public async Task<int> UpdateCableAsync(CableModel cableModel)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", cableModel.UniqueGUID);
                parameters.Add("@UniqueID", cableModel.UniqueID);
                parameters.Add("@LatestRev", false);
                parameters.Add("@LastSavedBy", cableModel.LastSavedBy);
                parameters.Add("@LastSavedDate", DateTime.Now);

                var affectedRows = await dbConnection.QuerySingleAsync<int>(
                    "sp_Cables_UpdateCable",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return affectedRows;
            }
            catch (Exception ex)
            {
                // Optional: log the error here
                throw new Exception("Error updating cable revision: " + ex.Message, ex);
            }
        }

        public async Task<List<FiberModel>> GenerateNewFiberAsync(
            CableModel cableModel,
            string cableType,
            int numOfBuffers,
            int quantityPerBuffer,
            int fibersPerRibbon,
            string fiberType,
            string userID)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();

                // Parameters for generating new fibers
                parameters.Add("@CableGUID", cableModel.UniqueGUID);
                parameters.Add("@CableType", cableType);
                parameters.Add("@NumOfBuffers", numOfBuffers);
                parameters.Add("@QuantityPerBuffer", quantityPerBuffer);
                parameters.Add("@FibersPerRibbon", fibersPerRibbon);
                parameters.Add("@FiberType", fiberType);
                parameters.Add("@Comments", cableModel.Comments);
                parameters.Add("@LatestRev", true);
                parameters.Add("@Creator", userID);
                parameters.Add("@CreateDate", DateTime.Now);

                // Call SP to generate fibers
                await dbConnection.ExecuteAsync("spGenerateFibersForCableV1", parameters, commandType: CommandType.StoredProcedure);

                // Load the newly generated fibers
                var loadParams = new DynamicParameters();
                loadParams.Add("@CableGUID", cableModel.UniqueGUID);
                loadParams.Add("@LatestRev", true);

                var result = await dbConnection.QueryAsync<FiberModel>(
                    "spLoadFibersFromCableV1",
                    loadParams,
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating new fibers: " + ex.Message, ex);
            }
        }

        #region Get Fibers In Cable Module
        public async Task<List<FiberModel>> GetFiberInCablesAsync(CableModel cableModel)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@CableGUID", cableModel.UniqueGUID);
                parameters.Add("@LatestRev", true);


                var result = await dbConnection.QueryAsync<FiberModel>(
                             "spLoadFibersFromCableV1",
                             parameters,
                             commandType: CommandType.StoredProcedure);

                foreach(var fiber in result)
                {
                    fiber.CableGUID = cableModel.UniqueGUID;
                    fiber.LatestRev = true;
                }
                var fibers = result.ToList();
                if (!fibers.Any())
                    return fibers;

                // 2. Load circuits
                var circuits = await dbConnection.QueryAsync<CircuitModel>(
                                   "spLoadCircuitsFromCableV1",
                                   new { CableGUID = cableModel.UniqueGUID, LatestRev = true },
                                   commandType: CommandType.StoredProcedure);

                // 3. Map circuits to fibers
                var circuitLookup = circuits.ToDictionary(c => c.FiberGUID, c => c);

                foreach (var fiber in fibers)
                {
                    if (circuitLookup.TryGetValue(fiber.UniqueGUID, out var circuit))
                    {
                        if (fiber.Circuits == null)
                            fiber.Circuits = new List<CircuitModel>();

                        fiber.Circuits.Add(circuit);
                    }
                }

                return fibers;

            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving  fibers: " + ex.Message, ex);
            }
            
        }
        #endregion
    }
}
