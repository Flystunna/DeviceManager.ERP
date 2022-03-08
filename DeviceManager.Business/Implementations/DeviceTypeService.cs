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
    public class DeviceTypeService: IDeviceTypeService
    {
        private readonly IDeviceTypeRepository _deviceTypeRepo;
        private readonly IServiceHelper _svcHelper;
        public DeviceTypeService(IDeviceTypeRepository deviceTypeRepo, IServiceHelper svcHelper)
        {
            _deviceTypeRepo = deviceTypeRepo;
            _svcHelper = svcHelper;
        }
        public async Task<bool> IfExists(long Id)
        {
            var exists = await _deviceTypeRepo.FirstOrDefaultAsync(c => c.Id == Id && c.IsDeleted == false);
            if (exists == null) return false;
            else return true;
        }
        public async Task<GetDeviceTypeDto> AddAsync(PostDeviceTypeDto model)
        {
            try
            {
                var newEntity = new DeviceType
                {
                    CreationTime = DateTime.Now,
                    CreatorUserId = _svcHelper.GetCurrentUserId(),
                    Type = model.Type
                };

                await _deviceTypeRepo.InsertAsync(newEntity);

                var entity = await _deviceTypeRepo.FirstOrDefaultAsync(c => c.Type == newEntity.Type 
                && c.CreationTime == newEntity.CreationTime
                && c.CreatorUserId == newEntity.CreatorUserId);

                if (entity != null)
                    return GetDeviceTypeConverter(entity);
                throw new GenericException("An error occurred while saving data", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<IPagedList<GetDeviceTypeDto>> GetPagedAsync(int pageNumber, int pageSize, string query)
        {
            try
            {
                var getall = await GetAllAsync();
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    getall = getall.Where(c => c.Type.ToLower().ToLower().Contains(query.ToLower())).ToList();
                }
                return await getall.AsQueryable().ToPagedListAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }
        public async Task<GetDeviceTypeDto> GetAsync(long Id)
        {
            try
            {
                var device = await _deviceTypeRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (device != null)
                {
                    return new GetDeviceTypeDto
                    {
                        Id = device.Id,
                        Type = device.Type,
                        CreationTime = device.CreationTime,
                        CreatorUserId = device.CreatorUserId,
                        LastModificationTime = device.LastModificationTime,
                        LastModifierUserId = device.LastModifierUserId
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
        public async Task<GetDeviceTypeDto> UpdateAsync(long Id, Data.Models.Dtos.Put.PutDeviceTypeDto model)
        {
            try
            {
                var entity = await _deviceTypeRepo.GetAsync(c => c.Id == Id && c.IsDeleted == false);
                if (entity != null)
                {
                    entity.Type = model.Type;
                    entity.LastModificationTime = DateTime.Now;
                    entity.LastModifierUserId = _svcHelper.GetCurrentUserId();
                    await _deviceTypeRepo.SaveAsync();
                    return GetDeviceTypeConverter(entity);
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
                var device = await _deviceTypeRepo.GetByIdAsync(id);
                if (device != null)
                {
                    device.DeletionTime = DateTime.Now;
                    device.DeleterUserId = _svcHelper.GetCurrentUserId();
                    device.IsDeleted = true;
                    await _deviceTypeRepo.SaveAsync();
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
        private GetDeviceTypeDto GetDeviceTypeConverter(DeviceType entity)
        {
            return new GetDeviceTypeDto
            {
                Id = entity.Id,
                Type = entity.Type,
                CreationTime = entity.CreationTime,
                CreatorUserId = entity.CreatorUserId,
                LastModificationTime = entity.LastModificationTime,
                LastModifierUserId = entity.LastModifierUserId
            };
        }
        private async Task<List<GetDeviceTypeDto>> GetAllAsync()
        {
            var getall = await _deviceTypeRepo.GetAll(c => c.IsDeleted == false).AsNoTracking().ToListAsync();
            return getall.Select(c => new GetDeviceTypeDto
            {
                Id = c.Id,
                Type = c.Type,
                CreationTime = c.CreationTime,
                CreatorUserId = c.CreatorUserId,
                LastModificationTime = c.LastModificationTime,
                LastModifierUserId = c.LastModifierUserId
            }).ToList();
        }
    }
}
