using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Repositories
{
    public interface IComponentRepository
    {
        Task<List<MeterialModel>> GetComponentsAsync(string materialType, Guid parentGuid, bool loadDetails, bool loadSubComponents);

    }
}
