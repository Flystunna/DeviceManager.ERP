using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using DeviceManager.Repository.Interfaces;
using IPagedList;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManager.Business.Implementations
{
    public class DeviceStatusLogService: IDeviceStatusLogService    
    {
        private readonly IDeviceStatusLogRepository _deviceStatusLogRepo;
        private readonly IServiceHelper _svcHelper;
        public DeviceStatusLogService(IDeviceStatusLogRepository deviceStatusLogRepo, IServiceHelper svcHelper)
        {
            _deviceStatusLogRepo = deviceStatusLogRepo;
            _svcHelper = svcHelper;
        }
        public async Task<bool> AddAsync(PostDeviceStatusLogDto model)
        {
            try
            {
                var add = await _deviceStatusLogRepo.AddAsync(new Data.Models.Entities.DeviceStatusLog
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    StatusId = model.StatusId,
                    DeviceId = model.DeviceId
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<IPagedList<GetDeviceStatusLogDto>> GetPagedAsync(int pageNumber, int pageSize, string query)
        {
            try
            {
                var getall = await GetAllAsync();
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    getall = getall.Where(c => c.Status.ToLower() == query.ToLower() || c.Device.ToLower() == query.ToLower()).ToList();
                }
                return await getall.AsQueryable().ToPagedListAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

        public async Task<GetDeviceStatusLogDto> GetAsync(long Id)
        {
            try
            {
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false, c => c.Status);
                if (deviceStatusLog != null)
                {
                    return new GetDeviceStatusLogDto
                    {
                        Id = deviceStatusLog.Id,
                        CreationTime = deviceStatusLog.CreationTime,
                        CreatorUserId = deviceStatusLog.CreatorUserId,
                        LastModificationTime = deviceStatusLog.LastModificationTime,
                        LastModifierUserId = deviceStatusLog.LastModifierUserId,

                        Status = deviceStatusLog.Status?.IsDeleted == false ? deviceStatusLog.Status?.Status : null,
                        StatusId = deviceStatusLog.Status?.IsDeleted == false ? deviceStatusLog?.StatusId : null,
                        Device = deviceStatusLog.Device?.IsDeleted == false ? deviceStatusLog.Device?.Name : null,
                        DeviceId = deviceStatusLog.Device?.IsDeleted == false ? deviceStatusLog?.DeviceId : null
                    };
                }
                throw new GenericException("No Data Found", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

        public async Task<bool> UpdateAsync(long Id, PutDeviceStatusLogDto model)
        {
            try
            {
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (deviceStatusLog != null)
                {
                    deviceStatusLog.StatusId = model.StatusId;
                    deviceStatusLog.DeviceId = model.DeviceId;
                    deviceStatusLog.LastModificationTime = DateTime.Now;
                    deviceStatusLog.LastModifierUserId = _svcHelper.GetCurrentUserId();
                    await _deviceStatusLogRepo.SaveAsync();
                    return true;
                }
                throw new GenericException("No Data Found", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

        public async Task<bool> DeleteAsync(long id)
        {
            try
            {
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsync(c=>c.IsDeleted == false);
                if (deviceStatusLog != null)
                {
                    deviceStatusLog.DeletionTime = DateTime.Now;
                    deviceStatusLog.DeleterUserId = _svcHelper.GetCurrentUserId();
                    deviceStatusLog.IsDeleted = true;
                    await _deviceStatusLogRepo.SaveAsync();
                    return true;
                }
                throw new GenericException("No Data Found", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        private async Task<List<GetDeviceStatusLogDto>> GetAllAsync()
        {
            var getall = await _deviceStatusLogRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().Include(c=>c.Status).Include(c=>c.Device).ToListAsync();
            return getall.Select(c => new GetDeviceStatusLogDto
            {
                Id = c.Id,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,

                Status = c.Status?.IsDeleted == false ? c.Status?.Status : null,    
                StatusId = c.Status?.IsDeleted == false ? c?.StatusId : null,
                Device = c.Device?.IsDeleted == false ? c.Device?.Name : null,  
                DeviceId = c.Device?.IsDeleted == false ? c?.DeviceId : null
            }).ToList();
        }
    }
}
