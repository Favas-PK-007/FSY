#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace FhaFacilitiesApplication.Domain.Models.Common
{
    public class ToasterModel<T>
    {
        public bool IsError { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
        public HttpStatusCode? StatusCode { get; set; }
        public T? Response { get; set; }
    }

    public class ToasterModel
    {
        public bool IsError { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
        public HttpStatusCode? StatusCode { get; set; }
    }
}
