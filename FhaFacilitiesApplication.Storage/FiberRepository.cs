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
    public class FiberRepository : IFiberRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly ICircuitRepository _circuitRepository;
        private readonly ISpliceRepository _spliceRepository;
        private readonly IPortRepository _portRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMeterialRepository _materialRepository;
        #endregion


        #region Constructor
        public FiberRepository(IConfiguration configuration, ICircuitRepository circuitRepository,
            ISpliceRepository spliceRepository, IPortRepository portRepository, IEquipmentRepository equipmentRepository,
            IMeterialRepository materialRepository)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _circuitRepository = circuitRepository;
            _spliceRepository = spliceRepository;
            _portRepository = portRepository;
            _equipmentRepository = equipmentRepository;
            _materialRepository = materialRepository;
        }
        #endregion

        public async Task<List<FiberModel>> GetFiberAsync(Guid cableGuid, bool loadCircuits)
        {
            List<FiberModel> fibers = new();
            Dictionary<Guid, CircuitModel> circuitsInCable = new();

            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                // Load associated circuits for this cable if requested
                if (loadCircuits)
                {
                    circuitsInCable = await _circuitRepository.GetCircuitsFromCableAsync(cableGuid);
                }

                var parameters = new DynamicParameters();
                parameters.Add("@CableGUID", cableGuid);
                parameters.Add("@LatestRev", true);

                var results = await connection.QueryAsync<FiberModel>(
                    "spLoadFibersFromCableV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                foreach (var fiber in results)
                {
                    fiber.CableGUID = cableGuid;
                    fiber.LatestRev = true;

                }

                fibers = results.ToList();

                // Attach the corresponding circuit to each fiber if found
                if (loadCircuits)
                {
                    foreach (var fiber in fibers.Where(f => circuitsInCable.ContainsKey(f.UniqueGUID)))
                    {
                        fiber.Circuits ??= new List<CircuitModel>();
                        fiber.Circuits.Add(circuitsInCable[fiber.UniqueGUID]);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading fibers from database: " + ex.Message, ex);
            }

            return fibers;
        }

        public async Task<FiberModel> LoadFibersAsync(Guid uniqueGuid, Guid cableGuid, bool loadCircuit)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@CableGUID", cableGuid);
                parameters.Add("@LatestRev", true);

                var fiber = await connection.QuerySingleOrDefaultAsync<FiberModel>(
                    "spLoadFiberByGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (fiber != null && loadCircuit)
                {
                    fiber.Circuits = await _circuitRepository.GetCircuitByFiberAsync(cableGuid);
                }

                return fiber;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading fiber data: " + ex.Message, ex);
            }
        }

        public async Task TraceFiberAsync(PortModel port, FiberModel fiber, SpliceModel splice, bool csvFormat, List<FiberModel> fibers, StringBuilder fiberDetails)
        {
            //var delimiter = csvFormat ? "," : ".";
            //fiberDetails.Append($"{splice.SpliceID}{delimiter}{port.TrayID}{delimiter}");
            //if (csvFormat) fiberDetails.Append(delimiter);
            //fiberDetails.Append($"{port.PortID}\n");

            //fiberDetails.Append(await _fiberRepository.GetCableFiberDetailsAsync(fiber, csvFormat));

            //await TraceConnectionsAsync(fiber, splice, fiberDetails, csvFormat, fibers);
        }

        public async Task TraceConnectionsAsync(FiberModel fiber, SpliceModel splice, StringBuilder fiberDetails, bool csvFormat, List<FiberModel> fibers)
        {
            //var conn = await _connectionRepository.GetFiberConnectionAsync(fiber.UniqueGUID, splice.UniqueGUID);
            //if (conn == null) return;

            //string delimiter = csvFormat ? "," : ".";

            //if (conn.FiberB_GUID == Consts.UNPATCHED_GUID)
            //{
            //    // Equipment end
            //    var newSplice = await _spliceRepository.GetSpliceByGuidAsync(conn.SpliceGUID.Value);
            //    var port = await _portRepository.GetPortByGuidAsync(conn.PortGUID.Value);
            //    var equip = await _equipmentRepository.GetEquipmentAsync(conn.UniqueID, conn.UniqueGUID, conn.PortGUID.Value);
            //    var material = await _materialRepository.GetMeterialsAsync(equip.UniqueGUID, Guid.Empty, false, false);

            //    fiberDetails.Append($"\n{newSplice.SpliceID}{delimiter}{port.TrayID}{delimiter}");
            //    if (csvFormat) fiberDetails.Append(delimiter);
            //    fiberDetails.Append($"{port.PortID}\n");

            //    fiberDetails.Append($"{equip.EquipmentID}{delimiter}{material?.ModelID}{delimiter}{delimiter}{equip.PortID}\n");
            //}
            //else if (conn.FiberB_GUID == Consts.EQUIPMENT_GUID)
            //{
            //    // End point
            //    var newSplice = await _spliceRepository.GetSpliceByGuidAsync(conn.SpliceGUID.Value);
            //    var port = await _portRepository.GetPortByGuidAsync(conn.PortGUID.Value);

            //    fiberDetails.Append($"\n{newSplice.SpliceID}{delimiter}{port.TrayID}{delimiter}");
            //    if (csvFormat) fiberDetails.Append(delimiter);
            //    fiberDetails.Append($"{port.PortID}\n");
            //}
            //else
            //{
            //    // Trace next fiber
            //    Guid nextFiberGuid = (conn.FiberA_GUID == fiber.UniqueGUID) ? conn.FiberB_GUID.Value : conn.FiberA_GUID.Value;

            //    var nextFiber = await _fiberRepository.GetFiberByGuidAsync(nextFiberGuid, fiber.CableGUID);
            //    var nextSplice = await _spliceRepository.GetSpliceByGuidAsync(conn.SpliceGUID.Value);

            //    fibers.Add(nextFiber);
            //    fiberDetails.Append($"\n{await _fiberRepository.GetCableFiberDetailsAsync(nextFiber, csvFormat)}");

            //    await TraceConnectionsAsync(nextFiber, nextSplice, fiberDetails, csvFormat, fibers);
            //}
        }


        public async Task<string> GetCableFiberDetailsAsync(Guid uniqueGuid, bool isCsvFormat)
        {
            string result = string.Empty;
            string demarc = isCsvFormat ? "," : ".";

            using IDbConnection connection = new SqlConnection(_fhaDbCon);
            var parameters = new DynamicParameters();
            parameters.Add("@FiberGUID", uniqueGuid);
            parameters.Add("@LatestRev", true);

            try
            {
                var data = await connection.QuerySingleOrDefaultAsync<dynamic>(
                    "sp_GetCableFiberDetailsByFiberGuid",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (data != null)
                {
                    var sb = new StringBuilder();

                    if (data.CableID != null)
                        sb.Append(data.CableID + demarc);

                    if (data.BufferID != null)
                        sb.Append(data.BufferID.ToString() + demarc);

                    if (data.RibbonID != null)
                        sb.Append(data.RibbonID.ToString() + demarc);

                    if (data.FiberID != null)
                        sb.Append(data.FiberID.ToString());

                    if (isCsvFormat)
                        sb.Append(demarc + uniqueGuid);

                    result = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to fetch Cable/Fiber details: " + ex.Message, ex);
            }

            return result;
        }



        public async Task<FiberModel> LoadFiberAsync(Guid uniqueGuid, Guid cableGuid, bool loadCircuits)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);
                var parameters = new DynamicParameters();
                parameters.Add("@UniqueGUID", uniqueGuid);
                parameters.Add("@CableGUID", cableGuid);
                parameters.Add("@LatestRev", true);
                var fiber = await connection.QuerySingleOrDefaultAsync<FiberModel>(
                    "sp_Fiber_LoadFiber",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                if (fiber != null && loadCircuits)
                {
                    fiber.Circuits = await _circuitRepository.GetCircuitByFiberAsync(cableGuid);
                }
                return fiber;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading fiber data: " + ex.Message, ex);
            }



            //    public async Task TraceFiberAsync(PortModel model, FiberModel fiberModel, SpliceModel startSplice, bool csvFormat, List<FiberModel> fibers, StringBuilder fiberDetails)
            //    {
            //        string demarc = ".";
            //        if (csvFormat)
            //        {
            //            demarc = ",";
            //        }

            //        fiberDetails.Append(spliceModel.SpliceID + demarc + model.TrayID + demarc);

            //        if (csvFormat)
            //        {
            //            fiberDetails.Append(demarc);
            //        }

            //        fiberDetails.Append(model.PortID + "\n");
            //        fiberDetails.Append(GetCableFiberDetails(fiberModel, csvFormat));

            //        var traceConnections = TraceConnections(fiberModel, startSplice, fiberDetails, csvFormat, fibers);


            //    }

            //    public async Task<string> GetCableFiberDetails(FiberModel fiberModel, bool csvFormat)
            //    {
            //        try
            //        {
            //            using IDbConnection connection = new SqlConnection(_fhaDbCon);
            //            var parameters = new DynamicParameters();
            //            parameters.Add("@FiberGUID", fiberModel.UniqueGUID);
            //            parameters.Add("@LatestRev", true);

            //            var details = (await connection.QueryAsync<FiberModel>(
            //                "sp_Fibers_GetCableFiberDetails",
            //                parameters,
            //                commandType: CommandType.StoredProcedure
            //            )).ToList();

            //            if (details == null || details.Count == 0)
            //                return string.Empty;

            //            var delimiter = csvFormat ? "," : ".";
            //            var sb = new StringBuilder();

            //            foreach (var detail in details)
            //            {
            //                if (!string.IsNullOrEmpty(detail.CableID))
            //                    sb.Append(detail.CableID + delimiter);

            //                if (detail.BufferID.HasValue)
            //                    sb.Append(detail.BufferID.Value.ToString() + delimiter);

            //                if (detail.RibbonID.HasValue)
            //                    sb.Append(detail.RibbonID.Value.ToString() + delimiter);

            //                if (detail.FiberID.HasValue)
            //                    sb.Append(detail.FiberID.Value.ToString());

            //                if (csvFormat)
            //                    sb.Append(delimiter + fiberModel.UniqueGUID.ToString());
            //            }

            //            return sb.ToString();
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception("Unable to load cable and fiber details: " + ex.Message, ex);
            //        }
            //    }

            //    public async Task TraceConnectionsAsync(FiberModel fiber, SpliceModel startSplice, StringBuilder fiberDetails, bool csvFormat, List<FiberModel> fibers)
            //    {
            //        string delimiter = csvFormat ? "," : ".";

            //        try
            //        {
            //            using IDbConnection connection = new SqlConnection(_fhaDbCon);

            //            var parameters = new DynamicParameters();
            //            parameters.Add("@FiberGUID", fiber.UniqueGUID);
            //            parameters.Add("@SpliceGUID", startSplice.UniqueGUID);
            //            parameters.Add("@LatestRev", true);

            //            var connectionData = await connection.QuerySingleOrDefaultAsync<ConnectionModel>(
            //                "spLoadConnectionV1",
            //                parameters,
            //                commandType: CommandType.StoredProcedure);

            //            if (connectionData == null) return;

            //            var fiberB_GUID = connectionData.FiberB_GUID?.ToString();
            //            var fiberA_GUID = connectionData.FiberA_GUID?.ToString();

            //            if (fiberB_GUID == "22222222-2222-2222-2222-222222222222")
            //            {
            //                startSplice = await _spliceRepository.GetSpliceByGuidAsync(connectionData.SpliceGUID.Value);
            //                var endPort = await _portRepository.GetPortByPortId(connectionData.PortGUID.Value);

            //                fiberDetails.Append($"\n{startSplice.SpliceID}{delimiter}{endPort.TrayID}{delimiter}");

            //                if (csvFormat) fiberDetails.Append(delimiter);

            //                fiberDetails.Append($"{endPort.PortID}\n");

            //                var equipment = await _equipmentRepository.GetEquipmentAsync(connectionData.UniqueID, connectionData.UniqueGUID, connectionData.PortGUID.Value);
            //                var material = await _materialRepository.GetMeterialsAsync(equipment.UniqueGUID, Guid.Empty, false, false);

            //                fiberDetails.Append($"{equipment.EquipmentID}{delimiter}{material?.ModelID}{delimiter}{delimiter}{equipment.PortID}\n");
            //            }
            //            else if (fiberB_GUID == "11111111-1111-1111-1111-111111111111")
            //            {
            //                startSplice = await _spliceRepository.GetSpliceByGuidAsync(connectionData.SpliceGUID.Value);
            //                var endPort = await _portRepository.GetPortByPortId(connectionData.PortGUID.Value);

            //                fiberDetails.Append($"\n{startSplice.SpliceID}{delimiter}{endPort.TrayID}{delimiter}");

            //                if (csvFormat) fiberDetails.Append(delimiter);

            //                fiberDetails.Append($"{endPort.PortID}\n");
            //            }
            //            else
            //            {
            //                var currentFiberGuid = fiber.UniqueGUID;
            //                var nextFiberGuid = connectionData.FiberA_GUID == currentFiberGuid
            //                    ? connectionData.FiberB_GUID.Value
            //                    : connectionData.FiberA_GUID.Value;

            //                var nextFiber = await LoadFibersAsync(nextFiberGuid, fiber.CableGUID, true);
            //                startSplice = await _spliceRepository.GetSpliceByGuidAsync(connectionData.SpliceGUID.Value);

            //                fibers.Add(nextFiber);
            //                fiberDetails.Append($"\n{GetCableFiberDetails(nextFiber, csvFormat)}");

            //                await TraceConnectionsAsync(nextFiber, startSplice, fiberDetails, csvFormat, fibers);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception("Unable to trace fiber connection path: " + ex.Message, ex);
            //        }
            //    }

            //}


        }
    }
}
