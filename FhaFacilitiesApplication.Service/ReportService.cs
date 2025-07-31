using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Service
{
    public class ReportService : IReportService
    {
        public async Task<byte[]> GeneratePanelReportCsvAsync(List<FiberModel> fiberModels)
        {
            if (fiberModels == null || fiberModels.Count == 0)
                return Array.Empty<byte>();

            var sb = new StringBuilder();
            sb.AppendLine("Panel ID,Cartridge ID,Port ID,Assigned Circuit");

            foreach (var fiber in fiberModels)
            {
                string panelId = fiber.Comments ?? ""; // You can adjust this
                string cartridgeId = GetValueAfterSpace(fiber.FiberType); // Example mapping
                string portId = GetValueAfterSpace(fiber.CreatedBy); // Sample use
                string circuit = string.Join(" / ", fiber.Circuits.Select(c => c.CircuitID ?? ""));

                sb.AppendLine($"{panelId},{cartridgeId},{portId},\"{circuit}\"");
            }

            return await Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
        }


        private string GetValueAfterSpace(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            int i = value.IndexOf(' ');
            return i >= 0 && i < value.Length - 1 ? value[(i + 1)..] : value;
        }
    }
}
