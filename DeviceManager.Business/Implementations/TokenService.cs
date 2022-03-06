using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models.Entities.User;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Business.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IUserService _userSvc;
        private readonly IServiceHelper _svcHelper;
        private readonly IConfiguration _config;
        public TokenService(IUserService userSvc, IServiceHelper svcHelper, IConfiguration config)
        {
            _userSvc = userSvc;
            _svcHelper = svcHelper;
            _config = config;
        }
        /// <summary>
        /// Validate User Name and Password
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="GenericException"></exception>
        public async Task<Data.Models.Dtos.Get.GetLoginDto> Auth(Data.Models.Dtos.Post.PostLoginDto login)
        {
            try
            {
                var user = await _userSvc.FindByEmailAsync(login.Username) ?? await _userSvc.FindByNameAsync(login.Username);
                if (user == null) throw new GenericException("User not found", StatusCodes.Status404NotFound);

                var checkuserpassword = await _userSvc.CheckPasswordAsync(user, login.Password);
                if (checkuserpassword)
                {
                    if (!user.IsDefaultAccount())
                    {
                        if (!user.IsConfirmed())
                        {
                            throw new GenericException("Account not active. Please activate your acccount to continue.", StatusCodes.Status400BadRequest);
                        }
                        if (user.AccountLocked())
                        {
                            throw new GenericException("Account locked. Please contact the system administrator.", StatusCodes.Status400BadRequest);
                        }
                        if (user.AccountDeactivated())
                        {
                            throw new GenericException("Account Deactivated. Please contact the system administrator.", StatusCodes.Status400BadRequest);
                        }
                    }

                    var userroles = await _userSvc.GetUserRoles(user);
                    var userClaims = UserTokenClaims(user, userroles);

                    var generateToken = GenerateAccessTokenFromClaims(userClaims.ToArray());

                    user.RefreshToken = generateToken.RefreshToken;
                    user.LastLoginDate = DateTime.Now;
                    user.IsLoggedIn = true;
                    user.LastLoginIP = _svcHelper.GetCurrentUserIP();

                    await _userSvc.UpdateAsync(user);

                    return new Data.Models.Dtos.Get.GetLoginDto
                    {
                        IsSuperAdmin = IsSuperAdmin(userroles),
                        Expires = generateToken.Expires,
                        RefreshToken = generateToken.RefreshToken,
                        Token = generateToken.Token,
                        UserId = user.Id,
                        Validity = generateToken.Validity,
                    };
                }
                throw new GenericException("Invalid Password", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }          
        }
        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Data.Models.Dtos.Get.GetLoginDto> Refresh(Data.Models.Dtos.Post.PostLoginRefreshDto model)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(model.Token);
                if (principal != null)
                {
                    var email = principal.FindFirst(JwtClaimTypes.Email).Value;

                    var user = await _userSvc.FindByEmailAsync(email);

                    if (user is null || user.RefreshToken != model.RefreshToken)
                    {
                        throw new GenericException("Invalid token supplied.", StatusCodes.Status400BadRequest);
                    }

                    var userClaims = user.UserToClaims();
                    var refreshToken = GenerateAccessTokenFromClaims(userClaims.ToArray());

                    user.RefreshToken = refreshToken.RefreshToken;
                    await _userSvc.UpdateAsync(user);

                    var userrole = await _userSvc.GetUserRoles(user);
                    return new Data.Models.Dtos.Get.GetLoginDto()
                    {
                        IsSuperAdmin = IsSuperAdmin(userrole),
                        Token = refreshToken.Token,
                        RefreshToken = refreshToken.RefreshToken,
                        Expires = refreshToken.Expires,
                        Validity = refreshToken.Validity,
                        UserId = user.Id
                    };
                }
                throw new GenericException("An error occured. Kindly log out and log back in", StatusCodes.Status401Unauthorized);
            }
            catch (Exception ex)
            {
                Log.Error("An error occurred " + ex);
                throw;
            }
        }

        /// <summary>
        /// Log out user session and record
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> Logout(long? id)
        {
            try
            {
                string userid = "";

                if (string.IsNullOrEmpty(id?.ToString()) || string.IsNullOrWhiteSpace(id?.ToString()))
                    userid = _svcHelper.GetCurrentUserId()?.ToString();
                else 
                    userid = id?.ToString();

                var user = await _userSvc.FindByIdAsync(userid?.ToString());
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.IsLoggedIn = false;
                    await _userSvc.UpdateAsync(user);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

        /// <summary>
        /// Get Principal User from Expired Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        /// <summary>
        /// Is Role Contain Super Admin
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static bool IsSuperAdmin(IList<string> roles)
        {
            if (roles.Contains("SuperAdmin")) return true;
            return false;
        }
        /// <summary>
        /// Generate User JWT Claims
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static List<Claim> UserTokenClaims(ApplicationUser user, IList<string> roles)
        {
            string role = roles.FirstOrDefault() ?? "";

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Id, user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName),
                new Claim(JwtClaimTypes.Email, user.Email),
                new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                new Claim(JwtClaimTypes.Role, role)
            };

            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                claims.Add(new Claim(JwtClaimTypes.Name, user.Name));
            }

            return claims;
        }

        /// <summary>
        /// Generate Access Token From Claims
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public Data.Models.Dtos.Get.GetLoginDto GenerateAccessTokenFromClaims(params Claim[] claims)
        {
            var TokenDurationInHours = int.Parse(_config["Jwt:TokenDurationInHours"]);
            var issued = DateTime.Now;
            var expires = DateTime.Now.AddHours(TokenDurationInHours);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var jwtToken = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                notBefore: issued,
                expires: expires,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );
            return new Data.Models.Dtos.Get.GetLoginDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = GenerateRefreshToken(),
                Validity = TokenDurationInHours,
                Expires = expires
            };
        }
        /// <summary>
        /// Generate Refresh Token
        /// </summary>
        /// <returns></returns>
        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
