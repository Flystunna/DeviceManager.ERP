using System.Collections.Generic;

namespace DeviceManager.Business.Interfaces
{
    public interface IServiceResponse<T>
    {
        string Code { get; set; }
        string ShortDescription { get; set; }
        T Object { get; set; }
        Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}
