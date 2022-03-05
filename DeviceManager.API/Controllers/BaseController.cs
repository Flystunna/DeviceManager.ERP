using DeviceManager.Business.Implementations;
using DeviceManager.Core.ExceptionHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DeviceManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public UserClaims CurrentUser
        {
            get
            {
                if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                {
                    return new UserClaims(HttpContext.User);
                }

                return null;
            }
        }
        public string GetCurrentUser
        {
            get
            {
                if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                {
                    return new UserClaims(HttpContext.User)?.UserName;
                }

                return "Anonymous";
            }
        }
        protected async Task<ServiceResponse<T>> HandleApiOperationAsync<T>(Func<Task<ServiceResponse<T>>> action, [CallerLineNumber] int lineNo = 0, [CallerMemberName] string method = "")
        {
            var _logger = Log.ForContext(typeof(BaseController));

            _logger.Information($">>> {GetCurrentUser} ENTERS ({method}) >>> ");

            var serviceResponse = new ServiceResponse<T>
            {
                Code = StatusCodes.Status200OK.ToString(),
                ShortDescription = "SUCCESS"
            };
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException("There were errors in your input, please correct them and try again.", StatusCodes.Status400BadRequest);

                var actionResponse = await action();

                serviceResponse.Object = actionResponse.Object;
                serviceResponse.ShortDescription = actionResponse.ShortDescription ?? serviceResponse.ShortDescription;
                serviceResponse.Code = actionResponse.Code ?? serviceResponse.Code;

            }
            catch (GenericException ex)
            {
                Log.Error($"L{lineNo} - {ex.ErrorCode}: {ex.Message}");
                _logger.Error($"L{lineNo} - {ex.ErrorCode}: {ex.Message}");

                serviceResponse.ShortDescription = ex.Message;
                serviceResponse.Code = ex.ErrorCode ?? StatusCodes.Status400BadRequest.ToString();

                if (!ModelState.IsValid)
                {
                    serviceResponse.ValidationErrors = ModelState.ToDictionary(
                        m =>
                        {
                            var tokens = m.Key.Split('.');
                            return tokens.Length > 0 ? tokens[tokens.Length - 1] : tokens[0];
                        },
                        m => m.Value.Errors.Select(e => e.Exception?.Message ?? e.ErrorMessage)
                    );
                }
            }
            catch (DbUpdateException duex)
            {
                _logger.Error(duex.Message, duex);
                serviceResponse.ShortDescription = $"An error occured while updating your record. {duex.Message} . {duex?.InnerException}";
                serviceResponse.Code = StatusCodes.Status400BadRequest.ToString();
            }

            catch (Exception ex)
            {
                _logger.Error($"L{lineNo} - DBV001: {ex.Message}");
                serviceResponse.ShortDescription = ex.Message;
                serviceResponse.Code = StatusCodes.Status500InternalServerError.ToString();

                _logger.Error(ex.Message, ex);
            }

            _logger.Information($"<<< EXITS ({method}) <<< ");

            return serviceResponse;
        }

        /// <summary>
        /// Read ModelError into string collection
        /// </summary>
        /// <returns></returns>
        private List<string> ListModelErrors
        {
            get
            {
                return ModelState.Values
                  .SelectMany(x => x.Errors
                    .Select(ie => ie.ErrorMessage))
                    .ToList();
            }
        }
    }
}
