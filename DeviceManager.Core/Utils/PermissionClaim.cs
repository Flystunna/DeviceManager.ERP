using System.Collections.Generic;
using System.Security.Claims;

namespace DeviceManager.Core.Utils
{
    public class PermissionClaim : Claim
    {
        public PermissionClaim(string value) : base("Permission", value)
        {
        }
    }

    public class PermissionClaimsProvider
    {
        public static readonly PermissionClaim Dashboard = new PermissionClaim("Dashboard");
        public static readonly PermissionClaim UserManagement = new PermissionClaim("UserManagement");
        public static readonly PermissionClaim Configuration = new PermissionClaim("Configuration");
        public static Dictionary<string, IEnumerable<PermissionClaim>> GetSystemDefaultRoles()
        {
            return new Dictionary<string, IEnumerable<PermissionClaim>>
            {
                {   Roles.SuperAdmin, new PermissionClaim []{ Configuration, Dashboard }}
            };
        }
    }
    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
    }
}
