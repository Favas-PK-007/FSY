#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class CircuitService : ICircuitService
    {
        #region Declarations
        private readonly ICircuitRepository _circuitRepository;
        private readonly IFiberRepository _fiberRepository;
        #endregion

        #region Constructor
        public CircuitService(ICircuitRepository circuitRepository, IFiberRepository fiberRepository)
        {
            _circuitRepository = circuitRepository;
            _fiberRepository = fiberRepository;
        }
        #endregion

        public async Task<Dictionary<Guid, CircuitModel>> GetCircuitsByCableAsync(Guid cableGuid)
        {
            return await _circuitRepository.GetCircuitsFromCableAsync(cableGuid);
        }

        public async Task<List<CircuitModel>> GetCircuitByFiberAsync(Guid fiberGuid)
        {
            return await _circuitRepository.GetCircuitByFiberAsync(fiberGuid);

        }

        public async Task<List<string>> SaveCircuitsAsync(Guid cableGuid, Guid spliceGuid, string userId)
        {
            var results = new List<string>();
            //var circuitsInCable = await _circuitRepository.GetCircuitsByCableAsync(cableGuid);
            //var splice = await _spliceRepository.GetSpliceByGuidAsync(spliceGuid);

            //foreach (var circuit in circuitsInCable.Values)
            //{
            //    if (circuit.UniqueID == 0 && circuit.LatestRev)
            //    {
            //        await SaveTracePathAsync(circuit, "Add", cableGuid, splice, userId);
            //        results.Add($"Circuit {circuit.CircuitID} added.");
            //    }
            //    else if (circuit.UniqueID > 0 && !circuit.LatestRev && circuit.Updated)
            //    {
            //        await SaveTracePathAsync(circuit, "Delete", cableGuid, splice, userId);
            //        results.Add($"Circuit {circuit.CircuitID} deleted.");
            //    }
            //    else if (circuit.UniqueID > 0 && circuit.Updated)
            //    {
            //        await SaveTracePathAsync(circuit, "Edit", cableGuid, splice, userId);
            //        results.Add($"Circuit {circuit.CircuitID} updated.");
            //    }
            //}

            return results;
        }


        public async Task SaveTracePathAsync(CircuitModel circuit, string action, Guid cableGuid, SpliceModel splice, string userId)
        {
            //var fiber = await _fiberRepository.GetFiberAsync(circuit.FiberGUID, cableGuid);
            //var newCircuit = new CircuitModel(circuit); 
            //var fiberDetails = new StringBuilder();
            //var fiberPath = new List<FiberModel> { fiber };

            //await _traceService.TraceFiberAsync(new PortModel(), fiber, splice, false, fiberPath, fiberDetails);

            //foreach (var f in fiberPath)
            //{
            //    var circuits = await _circuitRepository.GetCircuitsByFiberGuidAsync(f.UniqueGUID);

            //    if (action == "Add")
            //    {
            //        if (circuits.Any())
            //            await _circuitRepository.DeleteCircuitAsync(circuits[0], userId);

            //        newCircuit.FiberGUID = f.UniqueGUID;
            //        await _circuitRepository.AddCircuitAsync(newCircuit, userId);
            //    }
            //    else if (action == "Edit")
            //    {
            //        if (circuits.Any() && circuits[0].UniqueGUID != newCircuit.UniqueGUID)
            //        {
            //            await _circuitRepository.DeleteCircuitAsync(circuits[0], userId);
            //            newCircuit.FiberGUID = f.UniqueGUID;
            //            await _circuitRepository.AddCircuitAsync(newCircuit, userId);
            //        }
            //        else
            //        {
            //            newCircuit.UniqueID = circuits[0].UniqueID;
            //            newCircuit.FiberGUID = f.UniqueGUID;
            //            await _circuitRepository.UpdateCircuitAsync(newCircuit, userId);
            //        }
            //    }
            //    else if (action == "Delete")
            //    {
            //        if (circuits.Any() && circuits[0].UniqueGUID == newCircuit.UniqueGUID)
            //        {
            //            newCircuit.UniqueID = circuits[0].UniqueID;
            //            newCircuit.FiberGUID = f.UniqueGUID;
            //            await _circuitRepository.DeleteCircuitAsync(newCircuit, userId);
            //        }
            //    }
            //}
        }



        //public async Task<ToasterModel> SaveTracePathAsync(CircuitModel circuitModel, string action, Guid cableGuid, SpliceModel spliceModel)
        //{
        //    var startFiber = await _fiberRepository.LoadFibersAsync(circuitModel.FiberGUID, cableGuid, true);    

        //    StringBuilder fiberDetails = new StringBuilder();

        //    var fiberPath = new List<FiberModel>();

        //    fiberPath.Add(startFiber);

        //    var portModel = new PortModel();

        //    await _fiberRepository.TraceFiberAsync(portModel, startFiber, spliceModel, false, fiberPath, fiberDetails);



        //}
    }
}
