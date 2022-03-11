using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Entities.User;
using DeviceManager.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManager.Business.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IServiceHelper _svcHelper; 
        public UserService(UserManager<ApplicationUser> userManager, IServiceHelper svcHelper)
        {
            _userManager = userManager;
            _svcHelper = svcHelper;
        }
        public async Task<bool> RegisterUserAsync(PostNewUserDto model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password)) 
                    throw new GenericException("Email or Password or Phone Number cannot be null", StatusCodes.Status400BadRequest);

                var models = new ApplicationUser
                {
                    Name = model.Name,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    CreationTime = System.DateTime.Now,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.Username ?? model.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    NormalizedEmail = model.Email
                };
                var customerAcctResult = await CreateAsync(models, model.Password);
                if (customerAcctResult.Succeeded)
                {
                    var user = await FindByEmailAsync(models.Email);
                    if (user != null)
                    {
                        try
                        {
                            await AddToRoleAsync(user, model.Role ?? "User");
                            return true;
                        }
                        catch (GenericException ex)
                        {
                            Log.Error("User Created With error " + ex.Message);
                            throw new GenericException("User Created With error " + ex.Message);
                        }
                    }
                    throw new GenericException("Internal Server Error. We are working on this", StatusCodes.Status500InternalServerError);
                }
                else
                {
                    Log.Error("An error occurred " + customerAcctResult.Errors.FirstOrDefault()?.Description);
                    throw new GenericException(customerAcctResult.Errors.FirstOrDefault()?.Description, StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user)
        {
            return await _userManager.CreateAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> FindByNameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<string> FindEmailByIdAsync(string userId)
        {
            if (userId != null)
            {
                var user = await FindByIdAsync(userId.ToString());
                if (user != null) return user.Email;
            }
            return null;
        }
        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IList<string>> GetUserRoles(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }
        protected virtual Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            return _userManager.AddToRoleAsync(user, role);
        }
    }
}
