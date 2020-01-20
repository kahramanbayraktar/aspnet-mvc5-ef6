using Ced.BusinessEntities.Auth;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ced.Web.Models.UserRole
{
    public class UserRoleEditModel
    {
        public int UserRoleId { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "User is required.")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int RoleId { get; set; }

        public int? RegionId { get; set; }

        public int? CountryId { get; set; }

        public int? IndustryId { get; set; }

        public string RoleName { get; set; }

        public IList<ApplicationEntity> Applications { get; set; }

        public IList<RoleEntity> Roles { get; set; }

        public IList<RegionEntity> Regions { get; set; }

        public IList<CountryEntity> Countries { get; set; }

        public IList<IndustryEntity> Industries { get; set; }
    }
}