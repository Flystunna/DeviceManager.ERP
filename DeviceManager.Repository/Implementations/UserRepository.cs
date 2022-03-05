using DeviceManager.Data.Models.Entities.User;
using DeviceManager.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Repository.Implementations
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }
    }
}
