using IdentityServer3.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer3.EntityFramework
{
    public interface IUserConfigurationDbContext
    {
        DbSet<User> Users { get; set; }
    }
}
