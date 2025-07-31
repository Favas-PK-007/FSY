#region Namespaces
using FhaFacilitiesApplication.Domain.Models.Common;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IDuctRepository
    {
        Task<List<DuctModel>> GetDuctsAsync(Guid conduitGuid, bool loadSubDucts);
        Task<int> CreateDuctAsync(DuctModel requestModel);
        Task<DuctModel?> GetDuctByGuidAsync(Guid ductGuid);
        Task<bool> UpdateDuctAsync(DuctModel requestModel);
        Task<bool> DeleteDuctAsync(DuctModel model);



    }
}
