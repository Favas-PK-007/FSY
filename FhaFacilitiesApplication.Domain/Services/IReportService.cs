using FhaFacilitiesApplication.Domain.Models.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Domain.Services
{
    public interface IReportService
    {
        Task<byte[]> GeneratePanelReportCsvAsync(List<FiberModel> fiberModels);
    }
}
