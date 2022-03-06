using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
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
    public class DeviceService: IDeviceService
    {
        private readonly IDeviceStatusService _deviceStatusSvc;
        private readonly IDeviceTypeService _deviceTypeSvc;
        private readonly IDeviceRepository _deviceRepo;
        private readonly IDeviceStatusLogService _deviceStatusLogSvc;
        private readonly IServiceHelper _svcHelper;
        public DeviceService(IDeviceRepository deviceRepo, IDeviceStatusService deviceStatusSvc, IDeviceTypeService deviceTypeSvc, IDeviceStatusLogService deviceStatusLogSvc, IServiceHelper svcHelper)
        {
            _deviceRepo = deviceRepo;
            _deviceStatusLogSvc = deviceStatusLogSvc;
            _deviceStatusSvc = deviceStatusSvc;
            _deviceTypeSvc = deviceTypeSvc;
            _svcHelper = svcHelper;
        }     
        public async Task<bool> AddAsync(PostDeviceDto model)
        {
            try
            {
                var ifExistDeviceStatus = await _deviceStatusSvc.IfExists(model.DeviceStatusId);
                if (!ifExistDeviceStatus)
                    throw new GenericException("Invalid Device Status", StatusCodes.Status400BadRequest);

                var ifExistDeviceType = await _deviceTypeSvc.IfExists(model.DeviceTypeId);
                if (!ifExistDeviceType)
                    throw new GenericException("Invalid Device Type", StatusCodes.Status400BadRequest);

                await _deviceRepo.InsertAsync(new Device
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    DeviceStatusId = model.DeviceStatusId,
                    Name = model.Name,
                    Temperature = model.Temperature,
                    DeviceTypeId = model.DeviceTypeId
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<IPagedList<GetDeviceDto>> GetPagedAsync(int pageNumber, int pageSize, string query)
        {
            try
            {
                var getall = await GetAllAsync();
                if(getall == null)
                    throw new GenericException("No Data Found", StatusCodes.Status400BadRequest);

                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    getall = getall.Where(c => c.Name.ToLower().Contains(query.ToLower())
                    || c.DeviceStatus.ToLower().Contains(query.ToLower())
                    || c.DeviceType.ToLower().Contains(query.ToLower())
                    ).ToList();         
                }
                return await getall.AsQueryable().ToPagedListAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<GetDeviceDto> GetAsync(long Id)
        {
            try
            {
                var device = await _deviceRepo.GetAsyncAsNoTracking(c => c.Id == Id && c.IsDeleted == false, c => c.DeviceStatus, x=>x.DeviceType);
                if(device != null)
                {
                    var deviceStatusActivityLog = await _deviceStatusLogSvc.GetDeviceStatusActivityLog(Id);
                    var similarDevices = await GetSimilarDevices(Id);
                    return new GetDeviceDto
                    {
                        Id = device.Id,
                        Name = device.Name,
                        CreationTime = device.CreationTime,
                        CreatorUserId = device.CreatorUserId,
                        LastModificationTime = device.LastModificationTime,
                        LastModifierUserId = device.LastModifierUserId,
                        DeviceStatusId = device.DeviceStatusId,
                        DeviceStatus = device.DeviceStatus.Status,
                        DeviceTypeId = device.DeviceTypeId,
                        DeviceType = device.DeviceType.Type,
                        Temperature = device.Temperature,
                        SimilarDevices = similarDevices,
                        DeviceStatusActivityLog = deviceStatusActivityLog
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
        public async Task<bool> UpdateAsync(long Id, Data.Models.Dtos.Put.PutDeviceDto model)
        {
            try
            {
                if (model.DeviceStatusId != null)
                {
                    var ifExistDeviceStatus = await _deviceStatusSvc.IfExists(model.DeviceStatusId.GetValueOrDefault());
                    if (!ifExistDeviceStatus)
                        throw new GenericException("Invalid Device Status", StatusCodes.Status400BadRequest);
                }

                if (model.DeviceTypeId != null)
                {
                    var ifExistDeviceType = await _deviceTypeSvc.IfExists(model.DeviceTypeId.GetValueOrDefault());
                    if (!ifExistDeviceType)
                        throw new GenericException("Invalid Device Type", StatusCodes.Status400BadRequest);
                }

                var device = await _deviceRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (device != null)
                {
                    device.Name = model.Name ?? device.Name;
                    device.LastModificationTime = DateTime.Now;
                    device.LastModifierUserId = _svcHelper.GetCurrentUserId();

                    device.Temperature = model.Temperature ?? device.Temperature;
                    device.DeviceStatusId = model.DeviceStatusId ?? device.DeviceStatusId;
                    device.DeviceTypeId = model.DeviceTypeId ?? device.DeviceTypeId;
                    await _deviceRepo.SaveAsync();

                    if (device.DeviceStatusId != model?.DeviceStatusId && model?.DeviceStatusId != null)
                    {
                        await LogDeviceStatusChange(Id, model.DeviceStatusId.GetValueOrDefault());
                    }
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
                var device = await _deviceRepo.GetByIdAsync(id);
                if (device != null)
                {
                    device.DeletionTime = DateTime.Now;
                    device.DeleterUserId = _svcHelper.GetCurrentUserId();
                    device.IsDeleted = true;
                    await _deviceRepo.SaveAsync();
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
        public async Task<List<GetSimilarDeviceDto>> GetSimilarDevices(long deviceId)
        {
            try
            {
                var device = await _deviceRepo.GetAsyncAsNoTracking(c=>c.Id == deviceId && c.IsDeleted == false);
                if (device == null)
                {
                    return null;
                }

                var similar = await _deviceRepo.GetAll().AsNoTracking()
                    .Where(c => c.DeviceTypeId == device.DeviceTypeId 
                    && c.IsDeleted == false 
                    && c.Id != deviceId)
                    .Include(c=>c.DeviceStatus).Include(c=>c.DeviceType).ToListAsync();

                if (similar != null)
                {
                    return GetSimilarDevicesConverter(similar);
                }
                else 
                    return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        private async Task<List<GetDeviceDto>> GetAllAsync()
        {
            var getall = await _deviceRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().Include(c => c.DeviceStatus).Include(c=>c.DeviceType).ToListAsync();
            if(getall != null)  
                return GetDevicesConverter(getall);
            return null;
        }
        private static List<GetDeviceDto> GetDevicesConverter(List<Device> entities)
        {
            return entities.Select(c => new GetDeviceDto
            {
                Id = c.Id,
                Name = c.Name,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,
                Temperature = c.Temperature,

                DeviceStatus = c.DeviceStatus?.IsDeleted == false ? c.DeviceStatus.Status : null,
                DeviceStatusId = c.DeviceStatus?.IsDeleted == false ? c.DeviceStatusId : null,
                DeviceType = c.DeviceType?.IsDeleted == false ? c.DeviceType.Type : null,
                DeviceTypeId = c.DeviceType?.IsDeleted == false ? c.DeviceTypeId : null
            }).ToList();
        }
        private static List<GetSimilarDeviceDto> GetSimilarDevicesConverter(List<Device> entities)
        {
            return entities.Select(c => new GetSimilarDeviceDto
            {
                Id = c.Id,
                Name = c.Name,
                Temperature = c.Temperature,

                DeviceStatus = c.DeviceStatus?.IsDeleted == false ? c.DeviceStatus.Status : null,
                DeviceStatusId = c.DeviceStatus?.IsDeleted == false ? c.DeviceStatusId : null,
                DeviceType = c.DeviceType?.IsDeleted == false ? c.DeviceType.Type : null,
                DeviceTypeId = c.DeviceType?.IsDeleted == false ? c.DeviceTypeId : null
            }).ToList();
        }
        private async Task<bool> LogDeviceStatusChange(long deviceId, long StatusId)
        {
            await _deviceStatusLogSvc.AddAsync(new PostDeviceStatusLogDto { DeviceId = deviceId, DeviceStatusId = StatusId });
            return true;
        }
    }
}
