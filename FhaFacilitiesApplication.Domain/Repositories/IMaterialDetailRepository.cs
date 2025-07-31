using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IMaterialDetailRepository
    {
        Task<Dictionary<string, string>> GetMaterialDetailsAsync(string materialType, Guid materialGuid, Guid templateGuid);
        Task<int> SaveMaterialDetailsAsync(MeterialModel model);
    }
}
