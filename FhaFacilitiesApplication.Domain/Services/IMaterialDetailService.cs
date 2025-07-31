using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IMaterialDetailService
    {
        Task<Dictionary<string, string>> GetLoadDetailAsync(string materialType, Guid materialGuid, Guid templateGuid);
        Task<int> SaveMaterialDetailsAsync(MeterialModel model);

    }
}
