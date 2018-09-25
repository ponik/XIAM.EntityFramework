using System.ComponentModel.DataAnnotations;

namespace IdentityServer3.EntityFramework.Entities
{
    public class UserClaim
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string Type { get; set; }

        [Required]
        [StringLength(256)]
        public virtual string Value { get; set; }

        public virtual User User { get; set; }
    }
}