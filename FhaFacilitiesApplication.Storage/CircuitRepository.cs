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
    public class CircuitRepository : ICircuitRepository
    {

        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        #endregion

        #region Constructor
        public CircuitRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
        }
        #endregion

        public async Task<Dictionary<Guid, CircuitModel>> GetCircuitsFromCableAsync(Guid cableGuid)
        {
            var circuits = new Dictionary<Guid, CircuitModel>();

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@CableGUID", cableGuid);
                parameters.Add("@LatestRev", true);

                var result = await connection.QueryAsync<CircuitModel>(
                    "spLoadCircuitsFromCableV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Populate dictionary using FiberGUID as key
                foreach (var circuit in result)
                {
                    circuit.LatestRev = true;
                    if (!circuits.ContainsKey(circuit.FiberGUID))
                    {
                        circuits[circuit.FiberGUID] = circuit;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading circuits from cable: " + ex.Message, ex);
            }

            return circuits;
        }

        public async Task<List<CircuitModel>> GetCircuitByFiberAsync(Guid fiberGuid)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@FiberGUID", fiberGuid);
                parameters.Add("@LatestRev", true);

                var result = (await connection.QueryAsync<CircuitModel>(
                    "spLoadCircuitsV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                foreach (var circuit in result)
                {
                    circuit.LatestRev = true;
                    circuit.FiberGUID = fiberGuid;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading circuit by fiber: " + ex.Message, ex);
            }
        }

        public async Task<List<CircuitModel>> GetCircuitsByCableAsync(Guid cableGuid)
        {
            using IDbConnection conn = new SqlConnection(_fhaDbCon);
            var result = await conn.QueryAsync<CircuitModel>("sp_GetCircuitsByCable", new { CableGUID = cableGuid }, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task AddCircuitAsync(CircuitModel circuit, string userId)
        {
            using IDbConnection conn = new SqlConnection(_fhaDbCon);
            await conn.ExecuteAsync("sp_AddCircuit", new
            {
                circuit.UniqueGUID,
                circuit.FiberGUID,
                circuit.BuildingGUID,
                circuit.CircuitID,
                circuit.Comments,
                circuit.LatestRev,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                LastSavedBy = userId,
                LastSavedDate = DateTime.Now
            }, commandType: CommandType.StoredProcedure);
        }

    }
}
