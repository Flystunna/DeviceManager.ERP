using DeviceManager.Business.Interfaces;
using System.Collections.Generic;

namespace DeviceManager.Business.Implementations
{
    public class ServiceResponse<T> : IServiceResponse<T>
    {
        public ServiceResponse(T response) : this()
        {
            Object = response;
        }

        public ServiceResponse()
        {
            ValidationErrors = new Dictionary<string, IEnumerable<string>>();
        }

        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public T Object { get; set; }

        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}
