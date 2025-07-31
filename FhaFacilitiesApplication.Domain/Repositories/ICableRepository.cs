#region Namespaces
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface ICableRepository
    {
        Task<List<CableModel>> GetAllCablesAsync(Guid campusGuid, Guid spliceGuid, bool loadFibers);
        Task<CableModel?> GetCableByGuidAsync(Guid cableGuid, string? action);
        Task<bool> IsCableExistsAsync(string cableId, Guid campusGuid);
        Task<int> SaveCableAsync(CableModel cableModel);
        Task<int> UpdateCableAsync(CableModel cableModel);
        Task<List<FiberModel>> GenerateNewFiberAsync(CableModel cableModel, string cableType, int numOfBuffers, int quantityPerBuffer, int fibersPerRibbon, string fiberType, string userID);
        Task<List<FiberModel>> GetFiberInCablesAsync(CableModel cableModel);


    }
}
