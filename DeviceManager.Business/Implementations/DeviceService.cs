using DeviceManager.Business.Interfaces;
using DeviceManager.Core.ExceptionHelpers;
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
        private readonly IServiceHelper _svcHelper;
        public DeviceService(IDeviceRepository deviceRepo, IServiceHelper svcHelper)
        {
            _deviceRepo = deviceRepo;
            _svcHelper = svcHelper;
        }
        public async Task<IPagedList<Device>> Get(int pageNumber, int pageSize, string query)
        {
            try
            {
                var all = await _deviceRepo.GetAll(c=>c.IsDeleted == false).AsNoTracking().Include(c=>c.Status).ToListAsync();
                if(!string.IsNullOrEmpty(query) && !string.IsNullOrWhiteSpace(query))
                {
                    all = all.Where(c => c.Name.ToLower().ToLower() == query.ToLower() || c.Status.Status.ToLower() == query.ToLower()).ToList();         
                }
                return await all.AsQueryable().OrderByDescending(c => c.CreationTime).ToPagedListAsync(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
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
                    Temperature = model.Temperature
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToBetterString());
                throw;
            }
        }

    }
}
