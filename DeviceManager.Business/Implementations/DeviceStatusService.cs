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
    public class DeviceStatusService: IDeviceStatusService  
    {
        private readonly IDeviceStatusRepository _deviceStatusRepo;
        private readonly IServiceHelper _svcHelper;
        public DeviceStatusService(IDeviceStatusRepository deviceStatusRepo, IServiceHelper svcHelper)
        {
            _deviceStatusRepo = deviceStatusRepo;
            _svcHelper = svcHelper;
        }
        public async Task<bool> IfExists(long Id)
        {
            var exists = await _deviceStatusRepo.FirstOrDefaultAsync(c => c.Id == Id && c.IsDeleted == false);
            if (exists == null) return false;
            else return true;
        }
        public async Task<GetDeviceStatusDto> AddAsync(PostDeviceStatusDto model)
        {
            try
            {
                var newEntity = new DeviceStatus
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    Status = model.Status
                };

                await _deviceStatusRepo.InsertAsync(newEntity);

                var entity = await _deviceStatusRepo.FirstOrDefaultAsync(c => c.Status == newEntity.Status
                && c.CreationTime == newEntity.CreationTime
                && c.CreatorUserId == newEntity.CreatorUserId);

                if (entity != null)
                    return GetDeviceStatusConverter(entity);
                throw new GenericException("An error occurred while saving data", StatusCodes.Status400BadRequest);
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
                var getall = GetAll();
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    return await getall.Where(c => c.Status.ToLower().Contains(query.ToLower())).ToPagedListAsync(pageNumber, pageSize);
                }
                return await getall.ToPagedListAsync(pageNumber, pageSize);
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
                var device = await _deviceStatusRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
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
        public async Task<GetDeviceStatusDto> UpdateAsync(long Id, PutDeviceStatusDto model)
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
                    return GetDeviceStatusConverter(deviceStatus);
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

        private GetDeviceStatusDto GetDeviceStatusConverter(DeviceStatus entity)
        {
            return new GetDeviceStatusDto
            {
                Id = entity.Id,
                CreationTime = entity.CreationTime,
                CreatorUserId = entity.CreatorUserId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierUserId = entity.LastModifierUserId,
                Status = entity.Status
            };
        }

        private IQueryable<GetDeviceStatusDto> GetAll()
        {
            var getall = _deviceStatusRepo.GetAll(c => c.IsDeleted == false).AsNoTracking();
            return getall.Select(c => new GetDeviceStatusDto
            {
                Id = c.Id,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId,
                Status = c.Status
            });
        }
    }
}
