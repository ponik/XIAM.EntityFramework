using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer3.EntityFramework.Entities
{
    public class User
    {
        [Key]
        public virtual string Subject { get; set; }
        
        public virtual bool Enabled { get; set; }
        
        [Required]
        [StringLength(64)]
        public virtual string Username { get; set; }
        [Required]
        [StringLength(64)]
        public virtual string Password { get; set; }
        [StringLength(128)]
        public virtual string Provider { get; set; }
        [StringLength(128)]
        public virtual string ProviderId { get; set; }
        
        public virtual ICollection<UserClaim> Claims { get; set; }
    }
}
