using DeviceManager.Business.Interfaces;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using System;

namespace DeviceManager.Business.Implementations
{
    public class ServiceHelper : IServiceHelper
    {
        readonly IHttpContextAccessor _httpContext;
        public ServiceHelper(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public long? GetCurrentUserId()
        {
            var id = GetCurrentUser();
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrWhiteSpace(id))
            {
                var try_parse = long.TryParse(id, out long number);
                return try_parse == true ? number : null;
            }
            else return null;
        }
        public string GetCurrentUserIP()
        {
            return _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "No IP";
        }
        public string GetCurrentUserEmail()
        {
            var email = _httpContext.HttpContext?.User?.FindFirst(JwtClaimTypes.Email)?.Value;
            return !string.IsNullOrEmpty(email) ? email : "Anonymous";
        }
        public Uri GetAbsoluteUri()
        {
            var request = _httpContext.HttpContext?.Request;
            var uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            return uriBuilder.Uri;
        }
        public string GetCurrentUser()
        {
            var id = _httpContext.HttpContext?.User?.FindFirst("id")?.Value;
            return id is null ? "" : id;
        }
    }
}
