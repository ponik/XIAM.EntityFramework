using IdentityServer3.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer3.EntityFramework
{
    public class UserConfigurationDbContext : BaseDbContext, IUserConfigurationDbContext
    {
        public UserConfigurationDbContext()
            : this(EfConstants.ConnectionName)
        {
        }

        public UserConfigurationDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public UserConfigurationDbContext(string connectionString, string schema)
            : base(connectionString, schema)
        {
        }

        protected override void ConfigureChildCollections()
        {
            this.RegisterUserChildTablesForDelete<User>();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureUsers(Schema);
        }
    }
}
