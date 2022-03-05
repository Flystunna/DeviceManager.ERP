﻿using DeviceManager.Data.Models.Dtos.Get;
using DeviceManager.Data.Models.Dtos.Post;
using IPagedList;
using System.Threading.Tasks;

namespace DeviceManager.Business.Interfaces
{
    public interface IDeviceTypeService
    {
        Task<bool> AddAsync(PostDeviceTypeDto model);
        Task<IPagedList<GetDeviceTypeDto>> GetPagedAsync(int pageNumber, int pageSize, string query);
        Task<GetDeviceTypeDto> GetAsync(long Id);
        Task<bool> UpdateAsync(long Id, Data.Models.Dtos.Put.PutDeviceTypeDto model);
        Task<bool> DeleteAsync(long id);
    }
}
