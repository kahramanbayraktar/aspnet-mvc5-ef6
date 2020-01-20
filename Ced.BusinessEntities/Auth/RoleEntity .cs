namespace Ced.BusinessEntities.Auth
{
    public class RoleEntity
    {
        public int RoleId { get; set; }

        public string Name { get; set; }

        public int ApplicationId { get; set; }

        public string ApplicationName { get; set; }
    }
}
