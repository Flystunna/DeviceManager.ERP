using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;


namespace DeviceManager.Data.Models.Entities.User
{
    public class ApplicationUser: IdentityUser<long>
    {
        [StringLength(255, ErrorMessage = "Name must have max Length of 255 characters")]
        public string Name { get; set; }
        public string RefreshToken { get; set; }
        public bool? IsLoggedIn { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string LastLoginIP { get; set; }
        public int? CreatorUserId { get; set; }
        public int? LastModifierUserId { get; set; }
        public int? DeleterUserId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime? DeletionTime { get; set; }
        public bool IsDeleted { get; set; }
    }
    public static class UserExtensions
    {
        public static bool IsDefaultAccount(this ApplicationUser user)
        {
            return "admin@devicemanager.com" == user.UserName || "admin@devicemanager.com" == user.Email;
        }

        public static bool IsNull(this ApplicationUser user)
        {
            return user == null;
        }

        public static bool IsConfirmed(this ApplicationUser user)
        {
            return user.EmailConfirmed && user.PhoneNumberConfirmed;
        }

        public static bool AccountLocked(this ApplicationUser user)
        {
            return user.LockoutEnabled == true;
        }
        public static bool AccountDeactivated(this ApplicationUser user)
        {
            return user.IsDeleted == true;
        }

        public static bool HasNoPassword(this ApplicationUser user)
        {
            return string.IsNullOrWhiteSpace(user.PasswordHash);
        }
        public static List<Claim> UserToClaims(this ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
            }

            return claims;
        }
    }

}
