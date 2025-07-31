using FhaFacilitiesApplication.Domain.Enum;
using FhaFacilitiesApplication.Domain.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace FhaFacilitiesApplication.HelperExtension
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddInvalidModelStateResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new
                        {
                            errorCode = "QPS0204",  // Validation errors
                            description = e.Value?.Errors.First().ErrorMessage
                        })
                        .ToList();

                    var response = new ToasterModel
                    {
                        IsError = true,
                        Message = string.Join(" \n ", errors.Select(x => x.description)),
                        StatusCode = System.Net.HttpStatusCode.BadRequest,
                        Type = ToasterType.fail.ToString(),
                    };

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
