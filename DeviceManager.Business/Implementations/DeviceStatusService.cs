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
    public class DeviceStatusService: IDeviceStatusService  
    {
        private readonly IDeviceStatusRepository _deviceStatusRepo;
        private readonly IServiceHelper _svcHelper;
        public DeviceStatusService(IDeviceStatusRepository deviceStatusRepo, IServiceHelper svcHelper)
        {
            _deviceStatusRepo = deviceStatusRepo;
            _svcHelper = svcHelper;
        }
        public async Task<bool> AddAsync(PostDeviceStatusDto model)
        {
            try
            {
                var add = await _deviceStatusRepo.AddAsync(new Data.Models.Entities.DeviceStatus
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    Status = model.Status
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<IPagedList<GetDeviceStatusDto>> GetPagedAsync(int pageNumber, int pageSize, string query)
        {
            try
            {
                var getall = await GetAllAsync();
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    getall = getall.Where(c => c.Status.ToLower() == query.ToLower()).ToList();
                }
                return await getall.AsQueryable().ToPagedListAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<GetDeviceStatusDto> GetAsync(long Id)
        {
            try
            {
                var device = await _deviceStatusRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false, c => c.Status);
                if (device != null)
                {
                    return new GetDeviceStatusDto
                    {
                        Id = device.Id,
                        CreationTime = device.CreationTime,
                        CreatorUserId = device.CreatorUserId,
                        LastModificationTime = device.LastModificationTime,
                        LastModifierUserId = device.LastModifierUserId,
                        Status = device.Status
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
        public async Task<bool> UpdateAsync(long Id, PutDeviceStatusDto model)
        {
            try
            {
                var deviceStatus = await _deviceStatusRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (deviceStatus != null)
                {
                    deviceStatus.Status = model.Status;
                    deviceStatus.LastModificationTime = DateTime.Now;
                    deviceStatus.LastModifierUserId = _svcHelper.GetCurrentUserId();
                    await _deviceStatusRepo.SaveAsync();
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
                var deviceStatus = await _deviceStatusRepo.GetByIdAsync(id);
                if (deviceStatus != null)
                {
                    deviceStatus.DeletionTime = DateTime.Now;
                    deviceStatus.DeleterUserId = _svcHelper.GetCurrentUserId();
                    deviceStatus.IsDeleted = true;
                    await _deviceStatusRepo.SaveAsync();
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
        private async Task<List<GetDeviceStatusDto>> GetAllAsync()
        {
            var getall = await _deviceStatusRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().ToListAsync();
            return getall.Select(c => new GetDeviceStatusDto
            {
                Id = c.Id,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,
                Status = c.Status
            }).ToList();
        }
    }
}
