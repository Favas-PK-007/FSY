#region Namespaces
using Microsoft.AspNetCore.Mvc;
using System.Net;
#endregion

namespace FhaFacilitiesApplication.Helper
{
    public class ResponseHelper
    {
        public static IActionResult HandleResponse<T>(T result, HttpStatusCode? statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
                HttpStatusCode.Conflict => new BadRequestObjectResult(result),
                HttpStatusCode.NotFound => new NotFoundObjectResult(result),
                HttpStatusCode.NoContent => new NotFoundObjectResult(result),
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(result),
                _ => new ObjectResult(result) { StatusCode = (int)(statusCode ?? HttpStatusCode.BadRequest) }
            };
        }
    }
}
