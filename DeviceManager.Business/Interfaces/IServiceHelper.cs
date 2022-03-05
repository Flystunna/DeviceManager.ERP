using System;

namespace DeviceManager.Business.Interfaces
{
    public interface IServiceHelper
    {
        long? GetCurrentUserId();
        Uri GetAbsoluteUri();
        string GetCurrentUserIP();
        string GetCurrentUser();
        string GetCurrentUserEmail();
    }
}
