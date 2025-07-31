#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface ICableService
    {
        Task<List<CableModel>> GetAllCablesAsync(Guid campusGuid, Guid spliceGuid, bool loadFibers);
        Task<CableModel?> GetCableByGuidAsync(Guid cableGuid, string? action);
        Task<List<CableTypeModel>> GetCableTypesAsync();
        Task<List<CableModel>> GetCableModelsAsync();
        Task<ToasterModel> SaveCableAsync(CableModel cableModel);
        Task<ToasterModel> UpdateCableAsync(CableModel cableModel);
        Task<List<FiberModel>> GenerateNewFiberAsync(CableModel cableModel, string cableType, int numOfBuffers, int quantityPerBuffer, int fibersPerRibbon, string fiberType, string userID);
        Task<List<FiberModel>> GetFiberInCablesAsync(CableModel cableModel);
        Task<ToasterModel> DeleteCableAsync(CableModel model);
        Task<(CableModel Cable, List<Guid> AvailableFiberGuids)> GetAvailableFibersForCableAsync(Guid cableGuid,Guid spliceGuid);

    }
}
