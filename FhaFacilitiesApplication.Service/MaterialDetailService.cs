using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Service
{
    public class MaterialDetailService : IMaterialDetailService
    {
        private readonly IMaterialDetailRepository _materialDetailRepository;
        public MaterialDetailService(IMaterialDetailRepository materialDetailRepository)
        {
            _materialDetailRepository = materialDetailRepository;
        }
        public async Task<Dictionary<string, string>> GetLoadDetailAsync(string materialType, Guid materialGuid, Guid templateGuid)
        {
            var loadDetails = await _materialDetailRepository.GetMaterialDetailsAsync(materialType, materialGuid, templateGuid);
            return loadDetails ?? new Dictionary<string, string>();
        }

        public async Task<int> SaveMaterialDetailsAsync(MeterialModel model)
        {
            return await _materialDetailRepository.SaveMaterialDetailsAsync(model);
        }
    }
}
