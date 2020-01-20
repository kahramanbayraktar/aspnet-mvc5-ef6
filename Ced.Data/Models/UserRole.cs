using System.ComponentModel.DataAnnotations.Schema;

namespace Ced.Data.Models
{
    [Table("UserRole")]
    public class UserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? IndustryId { get; set; }
        public int? RegionId { get; set; }
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual Region Region { get; set; }
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }
    }
}
