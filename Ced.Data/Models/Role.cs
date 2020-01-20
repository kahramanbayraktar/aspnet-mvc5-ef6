using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ced.Data.Models
{
    [Table("Role")]
    public class Role
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Role()
        {
            this.UserRole = new HashSet<UserRole>();
        }

        public int RoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public int ApplicationId { get; set; }

        public virtual Application Application { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
