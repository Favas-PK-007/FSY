using FhaFacilitiesApplication.Domain.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary;

namespace FhaFacilitiesApplication.Helper
{
    public class ModelStateHelper
    {
        public static ToasterModel ReturnModelSateInfo(ValueEnumerable model)
        {
            var errors = model
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var errorMessage = string.Join("; ", errors);

            var toasterModel = new ToasterModel
            {
                IsError = true,
                Type = "error",
                Message = errorMessage,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            return toasterModel;
        }
    }
    }

     
