namespace Ced.BusinessEntities.Auth
{
    public class UserRoleEntity
    {
        public int UserRoleId { get; set; }

        public int UserId { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserEmail { get; set; }

        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public int? RegionId { get; set; }

        public string RegionName { get; set; }

        public int? CountryId { get; set; }

        public string CountryName { get; set; }

        public int? IndustryId { get; set; }

        public string IndustryName { get; set; }

        public string ApplicationCode { get; set; }
    }
}
