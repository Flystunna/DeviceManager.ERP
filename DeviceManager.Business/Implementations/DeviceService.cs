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
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Business.Implementations
{
    public class DeviceService: IDeviceService
    {
        private readonly IDeviceRepository _deviceRepo;
        private readonly IDeviceStatusLogService _deviceStatusLogSvc;
        private readonly IServiceHelper _svcHelper;
        public DeviceService(IDeviceRepository deviceRepo, IDeviceStatusLogService deviceStatusLogSvc, IServiceHelper svcHelper)
        {
            _deviceRepo = deviceRepo;
            _deviceStatusLogSvc = deviceStatusLogSvc;
            _svcHelper = svcHelper;
        }
        public async Task<bool> AddAsync(PostDeviceDto model)
        {
            try
            {
                var add = await _deviceRepo.AddAsync(new Data.Models.Entities.Device
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    StatusId = model.StatusId,
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
                    getall = getall.Where(c => 
                    c.Name.ToLower().ToLower() == query.ToLower() 
                    || c.Status.ToLower() == query.ToLower()
                    || c.DeviceType.ToLower().ToLower() == query.ToLower()
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
                var device = await _deviceRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false, c => c.Status, x=>x.DeviceType);
                if(device != null)
                {
                    return new GetDeviceDto
                    {
                        Id = device.Id,
                        Name = device.Name,
                        CreationTime = device.CreationTime,
                        CreatorUserId = device.CreatorUserId,
                        LastModificationTime = device.LastModificationTime,
                        LastModifierUserId = device.LastModifierUserId,
                        StatusId = device.StatusId,
                        Status = device?.Status?.Status,
                        DeviceTypeId = device.DeviceTypeId,
                        DeviceType = device?.DeviceType?.Type,
                        Temperature = device.Temperature
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
                var device = await _deviceRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (device != null)
                {
                    device.Name = model.Name;
                    device.LastModificationTime = DateTime.Now;
                    device.LastModifierUserId = _svcHelper.GetCurrentUserId();
                    device.Temperature = model.Temperature;
                    device.StatusId = model.StatusId;
                    device.DeviceTypeId = model.DeviceTypeId;
                    await _deviceRepo.SaveAsync();

                    if (device.StatusId != model?.StatusId)
                    {
                        await LogDeviceStatusChange(Id, model.StatusId);
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
        public async Task<List<GetDeviceDto>> GetSimilarDevices(long deviceId)
        {
            try
            {
                var existingDevice = await _deviceRepo.GetAsyncAsNoTracking(c=>c.Id == deviceId && c.IsDeleted == false); 
                if(existingDevice != null)
                {
                    var similar = await _deviceRepo.GetAll().AsNoTracking().Where(c => c.DeviceTypeId == existingDevice.DeviceTypeId && c.IsDeleted == false).ToListAsync();
                    if (similar != null)
                        return ConvertGet(similar);
                    return null;
                }
                throw new GenericException($"No Data Found for device Id {deviceId}", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        private async Task<List<GetDeviceDto>> GetAllAsync()
        {
            var getall = await _deviceRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().Include(c => c.Status).Include(c=>c.DeviceType).ToListAsync();
            if(getall != null)  
                return ConvertGet(getall);
            return null;
        }
        private List<GetDeviceDto> ConvertGet(List<Device> entities)
        {
            return entities.Select(c => new GetDeviceDto
            {
                Id = c.Id,
                Name = c.Name,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,
                StatusId = c.StatusId,
                Status = c?.Status?.Status,
                DeviceTypeId = c.DeviceTypeId,
                DeviceType = c?.DeviceType?.Type,
                Temperature = c.Temperature
            }).ToList();
        }
        private async Task<bool> LogDeviceStatusChange(long deviceId, long StatusId)
        {
            await _deviceStatusLogSvc.AddAsync(new PostDeviceStatusLogDto { DeviceId = deviceId, StatusId = StatusId });
            return true;
        }
    }
}
