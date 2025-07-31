using Dapper;
using FhaFacilitiesApplication.Domain.Models.DomainModel;
using FhaFacilitiesApplication.Domain.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhaFacilitiesApplication.Storage
{
    public class ComponentRepository : IComponentRepository
    {
        #region Declarations
        private readonly IConfiguration _configuration;
        private readonly string? _fhaDbCon;
        private readonly IMaterialDetailRepository _materialDetailRepository;
        #endregion



        #region Constructor
        public ComponentRepository(IConfiguration configuration, IMaterialDetailRepository materialDetailRepository)
        {
            _configuration = configuration;
            _fhaDbCon = _configuration.GetConnectionString("DefaultConnection");
            _materialDetailRepository = materialDetailRepository;
        }
        #endregion



        public async Task<List<MeterialModel>> GetComponentsAsync(string materialType, Guid parentGuid, bool loadDetails, bool loadSubComponents)
        {
            var result = new List<MeterialModel>();

            string componentType = materialType switch
            {
                "Splice" => "Splice Tray",
                "Splice Tray" => "Splice Module",
                "FPP Chassis" => "FPP Cartridge",
                "FPP Cartridge" => "FPP Module",
                _ => string.Empty
            };


            try
            {
                using IDbConnection connection = new SqlConnection(_fhaDbCon);

                var parameters = new DynamicParameters();
                parameters.Add("@ParentGUID", parentGuid);
                parameters.Add("@TemplateGUID", Guid.Empty);
                parameters.Add("@MaterialType", componentType);
                parameters.Add("@LatestRev", true);

                var components = (await connection.QueryAsync<MeterialModel>(
                    "spLoadComponentsV1",
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                foreach (var component in components)
                {
                    component.ParentGUID = parentGuid;
                    component.MaterialType = componentType;
                    component.LatestRev = true;

                    if (loadDetails)
                    {
                        var templateGuid =  component.TemplateGUID == Guid.Empty ? component.UniqueGUID : component.TemplateGUID;

                        component.Details = await _materialDetailRepository.GetMaterialDetailsAsync(component.MaterialType, templateGuid, Guid.Empty);
                    }

                    if (loadSubComponents)
                    {
                        component.Components = await GetComponentsAsync(component.MaterialType, component.UniqueGUID, true, true);
                    }
                }

                return components;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load components from database: " + ex.Message, ex);
            }
        }
    }
}
