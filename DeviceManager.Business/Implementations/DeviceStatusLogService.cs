using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using DeviceManager.Data.Models.Dtos.Put;
using DeviceManager.Data.Models.Entities;
using DeviceManager.Data.Models.Enums;
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
        public async Task<GetDeviceStatusLogDto> AddAsync(PostDeviceStatusLogDto model)
        {
            try
            {
                var newEntity = new DeviceStatusLog
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    DeviceStatusId = model.DeviceStatusId,
                    DeviceId = model.DeviceId
                };
                await _deviceStatusLogRepo.InsertAsync(newEntity);

                var entity = await _deviceStatusLogRepo.FirstOrDefaultAsync(c => c.DeviceStatusId == newEntity.DeviceStatusId
                && c.CreationTime == newEntity.CreationTime
                && c.CreatorUserId == newEntity.CreatorUserId
                && c.DeviceId == newEntity.DeviceId);

                if (entity != null)
                    return GetDeviceStatusLogConverter(entity);
                throw new GenericException("An error occurred while saving data", StatusCodes.Status400BadRequest);
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
                var getall = GetAll();
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    return await getall.Where(c => c.DeviceStatus.ToLower().Contains(query.ToLower()) 
                    || c.Device.ToLower().Contains(query.ToLower()))
                        .ToPagedListAsync(pageNumber, pageSize);
                }
                return await getall.ToPagedListAsync(pageNumber, pageSize);
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
                        DeviceStatusId = deviceStatusLog.DeviceStatusId,
                        Device = deviceStatusLog.Device.IsDeleted == false ? deviceStatusLog.Device.Name : null,
                        DeviceId = deviceStatusLog.DeviceId
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
        public async Task<List<GetDeviceStatusActivityLogDto>> GetAllDeviceStatusActivityLog(long DeviceStatusId)
        {
            var getall = GetAll();
            if (getall != null)
            {
                if(DeviceStatusId != 0)
                {
                    var results = getall.Where(c=>c.DeviceStatusId == DeviceStatusId).GroupBy(x => new { x.DeviceId, x.DeviceStatusId })
                        .Select(r => new GetDeviceStatusActivityLogDto
                        {
                            DeviceId = (long)r.Key.DeviceId,
                            DeviceStatusId = (long)r.Key.DeviceStatusId,
                            DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId((long)r.Key.DeviceStatusId),
                            Count = r.Count()
                        });
                    return await results.ToListAsync();
                }
                else
                {
                    var results = getall.GroupBy(x => new { x.DeviceId, x.DeviceStatusId })
                        .Select(r => new GetDeviceStatusActivityLogDto
                        {
                            DeviceId = (long)r.Key.DeviceId,
                            DeviceStatusId = (long)r.Key.DeviceStatusId,
                            DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId((long)r.Key.DeviceStatusId),
                            Count = r.Count()
                        });
                    return await results.ToListAsync();
                }
            }
            return null;
        }
        public async Task<List<GetDeviceStatusActivityLogDto>> GetDeviceStatusActivityLog(long deviceId, GroupDeviceStatusActivityLogFilter filter)
        {
            try
            {
                var devicelog = _deviceStatusLogRepo.GetAll(c => c.IsDeleted == false && c.DeviceId == deviceId).AsNoTracking().Include(c => c.DeviceStatus);
                if (devicelog != null)
                {
                    switch (filter)
                    {
                        case GroupDeviceStatusActivityLogFilter.Daily:
                            return await devicelog.GroupBy(x => new { x.DeviceId, x.DeviceStatusId, x.CreationTime.Date })
                                .Select(r => new GetDeviceStatusActivityLogDto
                                {
                                    DeviceId = r.Key.DeviceId,
                                    DeviceStatusId = r.Key.DeviceStatusId,
                                    DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId(r.Key.DeviceStatusId),
                                    Date = r.Key.Date.ToShortDateString(),
                                    Count = r.Count()
                                }).ToListAsync();
                        case GroupDeviceStatusActivityLogFilter.Monthly:
                            return await devicelog.GroupBy(x => new { x.DeviceId, x.DeviceStatusId, x.CreationTime.Month, x.CreationTime.Year })
                                .Select(r => new GetDeviceStatusActivityLogDto
                                {
                                    DeviceId = r.Key.DeviceId,
                                    DeviceStatusId = r.Key.DeviceStatusId,
                                    DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId(r.Key.DeviceStatusId),
                                    Month = r.Key.Month.ToString(),
                                    Year = r.Key.Year.ToString(),
                                    Count = r.Count()
                                }).ToListAsync();
                        case GroupDeviceStatusActivityLogFilter.Yearly:
                            return await devicelog.GroupBy(x => new { x.DeviceId, x.DeviceStatusId, x.CreationTime.Year })
                                .Select(r => new GetDeviceStatusActivityLogDto
                                {
                                    DeviceId = r.Key.DeviceId,
                                    DeviceStatusId = r.Key.DeviceStatusId,
                                    DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId(r.Key.DeviceStatusId),
                                    Year = r.Key.Year.ToString(),
                                    Count = r.Count()
                                }).ToListAsync();
                        default:
                            return await devicelog.GroupBy(x => new { x.DeviceId, x.DeviceStatusId, x.CreationTime.Date })
                               .Select(r => new GetDeviceStatusActivityLogDto
                               {
                                   DeviceId = r.Key.DeviceId,
                                   DeviceStatusId = r.Key.DeviceStatusId,
                                   DeviceStatus = _deviceStatusRepo.LookUpDeviceStatusByDeviceStatusId(r.Key.DeviceStatusId),
                                   Date = r.Key.Date.ToString(),
                                   Count = r.Count()
                               }).ToListAsync();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<GetDeviceStatusLogDto> UpdateAsync(long Id, PutDeviceStatusLogDto model)
        {
            try
            {
                var deviceStatusLog = await _deviceStatusLogRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false, x => x.DeviceStatus, y => y.Device);
                if (deviceStatusLog != null)
                {
                    deviceStatusLog.DeviceStatusId = model.DeviceStatusId ?? deviceStatusLog.DeviceStatusId;
                    deviceStatusLog.DeviceId = model.DeviceId ?? deviceStatusLog.DeviceId;
                    deviceStatusLog.LastModificationTime = DateTime.Now;
                    deviceStatusLog.LastModifierUserId = _svcHelper.GetCurrentUserId();
                    await _deviceStatusLogRepo.SaveAsync();
                    return GetDeviceStatusLogConverter(deviceStatusLog);
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
                var deviceStatusLog = await _deviceStatusLogRepo.GetByIdAsync(id);
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
        private GetDeviceStatusLogDto GetDeviceStatusLogConverter(DeviceStatusLog entity)
        {
            return new GetDeviceStatusLogDto
            {
                Id = entity.Id,
                CreationTime = entity.CreationTime,
                CreatorUserId = entity.CreatorUserId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierUserId = entity.LastModifierUserId,

                DeviceStatus = entity.DeviceStatus?.IsDeleted == false ? entity.DeviceStatus?.Status : null,
                DeviceStatusId = entity.DeviceStatusId,
                Device = entity.Device?.IsDeleted == false ? entity.Device?.Name : null,
                DeviceId = entity.DeviceId
            };
        }
        private IQueryable<GetDeviceStatusLogDto> GetAll()
        {
            var getall = _deviceStatusLogRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().Include(c => c.DeviceStatus).Include(c => c.Device);
            return getall.Select(c => new GetDeviceStatusLogDto
            {
                Id = c.Id,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,

                DeviceStatus = c.DeviceStatus != null ? c.DeviceStatus.IsDeleted == false ? c.DeviceStatus.Status : null : null,
                DeviceStatusId = c.DeviceStatusId,
                Device = c.Device != null ? c.Device.IsDeleted == false ? c.Device.Name : null : null,
                DeviceId = c.DeviceId
            });
        }
    }
}
