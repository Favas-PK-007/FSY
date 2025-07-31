using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IEquipmentService
    {
        Task<Dictionary<int, EquipmentModel>> GetInstalledEquipmentAsync(Guid structureGuid, Guid spliceGuid);

        Task<List<string>> GetEquipmentTypesAsync();

        Task<List<MeterialModel>> GetEquipmentModelAsync(string equipmentType);

        Task<Dictionary<int, string>> GetAvailableEquipmentPortAsync(Guid equimentStructureGuid, Guid equipmentSpliceGuid, Guid equipmentTypeGuid,string selectedEquipmentld);

        Task<bool> IsEquipmentExistAsync(string equipmentId, Guid equipmentStructureGuid, Guid equipmentSpliceGuid);
        
        Task<ToasterModel> SaveNewEquipmentMaterialAsync(EquipmentModel equipmentModel);

        Task<ToasterModel> SaveEquipmentPortsAsync(bool newEquipment, EquipmentModel model);

        Task<EquipmentModel?> GerEquipmentByPortAndGuid(Guid portGuid, Guid uniqueGuid);

        Task<EquipmentModel?> GetEquipmentAsync(Guid uniqueGuid, Guid portGuid);
    }
}
