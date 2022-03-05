using IdentityModel;
using System.Security.Claims;
using System.Security.Principal;

namespace DeviceManager.API
{
    public class UserClaims : ClaimsPrincipal
    {
        public UserClaims(IPrincipal principal) : base(principal)
        {
        }

        public long Id
        {
            get
            {
                var idClaim = FindFirst(JwtClaimTypes.Id);
                if (idClaim == null)
                    return 0;

                return long.Parse(idClaim.Value);
            }
        }

        public string Email
        {
            get
            {
                var emailClaim = FindFirst(JwtClaimTypes.Email);
                if (emailClaim == null)
                    return string.Empty;

                return emailClaim.Value;
            }
        }

        public string UserName
        {
            get
            {
                var usernameClaim = FindFirst(JwtClaimTypes.PreferredUserName);
                if (usernameClaim == null)
                    return "Anonymous";

                return usernameClaim.Value;
            }
        }
        public string Name
        {
            get
            {
                var usernameClaim = FindFirst(JwtClaimTypes.Name);
                if (usernameClaim == null)
                    return "Anonymous";

                return usernameClaim.Value;
            }
        }
        public string Role
        {
            get
            {
                var usernameClaim = FindFirst(JwtClaimTypes.Role);

                if (usernameClaim == null)
                    return string.Empty;

                return usernameClaim.Value;
            }
        }
    }
}
