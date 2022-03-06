using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using DeviceManager.Data.Models.Entities;
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
    public class DeviceStatusLogService : IDeviceStatusLogService
    {
        private readonly IDeviceStatusLogRepository _deviceStatusLogRepo;
        private readonly IDeviceStatusRepository _deviceStatusRepo;
        private readonly IServiceHelper _svcHelper;
        public DeviceStatusLogService(IDeviceStatusLogRepository deviceStatusLogRepo, IDeviceStatusRepository deviceStatusRepo, IServiceHelper svcHelper)
        {
            _deviceStatusLogRepo = deviceStatusLogRepo;
            _deviceStatusRepo = deviceStatusRepo;
            _svcHelper = svcHelper;
        }
        public async Task<bool> AddAsync(PostDeviceStatusLogDto model)
        {
            try
            {
                await _deviceStatusLogRepo.InsertAsync(new DeviceStatusLog
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    DeviceStatusId = model.DeviceStatusId,
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
                    getall = getall.Where(c => c.DeviceStatus.ToLower().Contains(query.ToLower()) || c.Device.ToLower().Contains(query.ToLower())).ToList();
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
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsyncAsNoTracking(c => c.Id == Id && c.IsDeleted == false, x => x.DeviceStatus, y => y.Device);
                if (deviceStatusLog != null)
                {
                    return new GetDeviceStatusLogDto
                    {
                        Id = deviceStatusLog.Id,
                        CreationTime = deviceStatusLog.CreationTime,
                        CreatorUserId = deviceStatusLog.CreatorUserId,
                        LastModificationTime = deviceStatusLog.LastModificationTime,
                        LastModifierUserId = deviceStatusLog.LastModifierUserId,

                        DeviceStatus = deviceStatusLog.DeviceStatus.IsDeleted == false ? deviceStatusLog.DeviceStatus.Status : null,
                        DeviceStatusId = deviceStatusLog.DeviceStatus.IsDeleted == false ? deviceStatusLog.DeviceStatusId : null,
                        Device = deviceStatusLog.Device.IsDeleted == false ? deviceStatusLog.Device.Name : null,
                        DeviceId = deviceStatusLog.Device.IsDeleted == false ? deviceStatusLog.DeviceId : null
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
        public async Task<List<GetDeviceStatusActivityLogDto>> GetDeviceStatusActivityLog(long deviceId)
        {
            try
            {
                var devicelog = await _deviceStatusLogRepo.GetAll().Where(c => c.IsDeleted == false && c.DeviceId == deviceId).Include(c => c.DeviceStatus).ToListAsync();
                if (devicelog != null)
                {
                    return devicelog.GroupBy(x => new { x.DeviceId, x.DeviceStatusId, x.CreationTime.Date })
                        .Select(r => new GetDeviceStatusActivityLogDto
                        {
                            DeviceId = r.Key.DeviceId,
                            DeviceStatusId = r.Key.DeviceStatusId,
                            DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId(r.Key.DeviceStatusId),
                            Date = r.Key.Date,
                            Count = r.Count()
                        }).ToList();
                }
                return null;
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
                    deviceStatusLog.DeviceStatusId = model.DeviceStatusId ?? deviceStatusLog.DeviceStatusId;
                    deviceStatusLog.DeviceId = model.DeviceId ?? deviceStatusLog.DeviceId;
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
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsync(c => c.IsDeleted == false);
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
            var getall = await _deviceStatusLogRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().Include(c => c.DeviceStatus).Include(c => c.Device).ToListAsync();
            return getall.Select(c => new GetDeviceStatusLogDto
            {
                Id = c.Id,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,

                DeviceStatus = c.DeviceStatus?.IsDeleted == false ? c.DeviceStatus?.Status : null,
                DeviceStatusId = c.DeviceStatus?.IsDeleted == false ? c?.DeviceStatusId : null,
                Device = c.Device?.IsDeleted == false ? c.Device?.Name : null,
                DeviceId = c.Device?.IsDeleted == false ? c?.DeviceId : null
            }).ToList();
        }
    }
}
