using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.Web.Models.UserRole
{
    public class UserRoleSearchModel
    {
        public string UserEmail { get; set; }

        public int?[] ApplicationIds { get; set; }

        public int?[] RoleIds { get; set; }

        public int?[] RegionIds { get; set; }

        public int?[] CountryIds { get; set; }

        public int?[] IndustryIds { get; set; }

        public IList<ApplicationEntity> Applications { get; set; }

        public IList<RoleEntity> Roles { get; set; }

        public IList<RegionEntity> Regions { get; set; }

        public IList<CountryEntity> Countries { get; set; }

        public IList<IndustryEntity> Industries { get; set; }
    }
}