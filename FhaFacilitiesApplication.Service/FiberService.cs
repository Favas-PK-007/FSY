#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Service
{
    public class FiberService : IFiberService
    {
        #region Declarations
        private readonly IFiberRepository _fiberRepository;
        private readonly ISpliceRepository _spliceRepository;
        private readonly IPortRepository _portRepository;
        private readonly IConnectionRepository _connectionRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IMeterialRepository _meterialRepository;
        #endregion

        #region Constructor
        public FiberService(IFiberRepository fiberRepository, ISpliceRepository spliceRepository,
            IPortRepository portRepository, IConnectionRepository connectionRepository, 
            IEquipmentRepository equipmentRepository, IMeterialRepository meterialRepository)
        {
            _fiberRepository = fiberRepository ?? throw new ArgumentNullException(nameof(fiberRepository));
            _spliceRepository = spliceRepository;
            _portRepository = portRepository;
            _connectionRepository = connectionRepository;
            _equipmentRepository = equipmentRepository;
            _meterialRepository = meterialRepository;
        }
        #endregion


        public async Task<List<FiberModel>> GetFiberAsync(Guid cableGuid, bool loadCircuits)
        {
            return await _fiberRepository.GetFiberAsync(cableGuid, loadCircuits);
        }


        public async Task<string> GetCableFiberDetailsAsync(FiberModel fiber, bool csvFormat)
        {
            return await _fiberRepository.GetCableFiberDetailsAsync(fiber.UniqueGUID, csvFormat);         
        }


        public async Task TraceConnectionsAsync(FiberModel currentFiber,SpliceModel currentSplice,bool csvFormat,
            StringBuilder fiberDetails,List<FiberModel> fibers)
        {
            var demarc = csvFormat ? "," : ".";

            var newConnection = await _connectionRepository.GetNextConnectionAsync(currentFiber.UniqueGUID, currentSplice.UniqueGUID);
            if (newConnection == null) return;

            // End at equipment (dummy fiber)
            if (newConnection.FiberB_GUID == Guid.Parse("22222222-2222-2222-2222-222222222222"))
            {
                var newSplice = await _spliceRepository.GetSpliceByGuidAsync(newConnection.SpliceGUID.Value);
                var newPort = await _portRepository.GetPortByGuidAsync(newConnection.PortGUID.Value);

                fiberDetails.Append($"\n{newSplice?.SpliceID}{demarc}{newPort?.TrayID}{demarc}");
                if (csvFormat) fiberDetails.Append(demarc);
                fiberDetails.AppendLine(newPort.PortID.ToString());

                var equipment = await _equipmentRepository.GetEquipmentAsync(newConnection.UniqueGUID, newConnection.PortGUID.Value);
                var material = await _meterialRepository.GetMeterialsAsync(equipment.UniqueGUID, Guid.Empty, false, false);

                fiberDetails.AppendLine($"{equipment.EquipmentID}{demarc}{material.ModelID}{demarc}{demarc}{equipment.PortID}");
                return;
            }

            // End at port (dummy port)
            if (newConnection.FiberB_GUID == Guid.Parse("11111111-1111-1111-1111-111111111111"))
            {
                var newSplice = await _spliceRepository.GetSpliceByGuidAsync(newConnection.SpliceGUID.Value);
                var newPort = await _portRepository.GetPortByGuidAsync(newConnection.PortGUID.Value);

                fiberDetails.Append($"\n{newSplice.SpliceID}{demarc}{newPort.TrayID}{demarc}");
                if (csvFormat) fiberDetails.Append(demarc);
                fiberDetails.AppendLine(newPort.PortID.ToString());
                return;
            }

            // Continue tracing
            List<FiberModel> nextFibers;
            if (newConnection.FiberA_GUID == currentFiber.UniqueGUID)
            {
                nextFibers = await _fiberRepository.GetFiberAsync(newConnection.FiberB_GUID.Value, true);
            }
            else
            {
                nextFibers = await _fiberRepository.GetFiberAsync(newConnection.FiberA_GUID.Value, true);
            }

            if (nextFibers == null || !nextFibers.Any())
                return;

            var newSpliceInfo = await _spliceRepository.GetSpliceByGuidAsync(newConnection.SpliceGUID.Value);

            foreach (var nextFiber in nextFibers)
            {
                fibers.Add(nextFiber);

                string fiberInfo = await GetCableFiberDetailsAsync(nextFiber, csvFormat);
                fiberDetails.Append($"\n{fiberInfo}");

                // Recursive call
                await TraceConnectionsAsync(nextFiber, newSpliceInfo, csvFormat, fiberDetails, fibers);
            }
        }



        public async Task<FiberModel> LoadFiberAsync(Guid uniqueGuid, Guid cableGuid, bool loadCircuits)
        {
            return await _fiberRepository.LoadFiberAsync(uniqueGuid, cableGuid, loadCircuits);
        }

        public async Task TraceConnectionsAsync(PortModel model, FiberModel currentFiber, SpliceModel currentSplice, bool csvFormat, StringBuilder fiberDetails, List<FiberModel> fibers)
        {
            string demarc = csvFormat ? "," : ".";

            fiberDetails.Append($"\n{currentSplice.SpliceID}{demarc}{model.TrayID}{demarc}");

            if (csvFormat)
                fiberDetails.Append(demarc);

            fiberDetails.Append(model.PortID);


            var a = await _fiberRepository.GetCableFiberDetailsAsync(currentFiber.UniqueGUID, csvFormat);

            fiberDetails.Append($"\n{a}");

            await TraceConnectionsAsync(currentFiber, currentSplice, csvFormat, fiberDetails, fibers);

        }
    }
}
