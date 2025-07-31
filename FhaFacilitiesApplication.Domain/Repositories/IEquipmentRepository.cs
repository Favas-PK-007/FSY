using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IEquipmentRepository
    {
        Task<EquipmentModel?> GetEquipmentAsync(Guid uniqueGuid, Guid portGuid);

        Task<Dictionary<int, EquipmentModel>> GetInstalledEquipmentAsync(Guid structureGuid, Guid spliceGuid);

        Task<List<string>> GetEquipmentTypesAsync();

        Task<List<MeterialModel>> GetEquipmentModelAsync(string equipmentType);

        Task<Dictionary<int, string>> GetAvailableEquipmentPortAsync(Guid equimentStructureGuid, Guid equipmentSpliceGuid, Guid equipmentTypeGuid, string selectedEquipmentld);

        Task<bool> IsEquipmentExistAsync(string equipmentId, Guid equipmentStructureGuid, Guid equipmentSpliceGuid);

        Task<int> SaveEquipmentAsync(EquipmentModel model);

        Task<EquipmentModel?> GerEquipmentByPortAndGuid(Guid portGuid, Guid uniqueGuid);


    }
}
