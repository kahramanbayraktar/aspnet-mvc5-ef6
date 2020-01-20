namespace Ced.BusinessEntities
{
    public class UserEntity
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string FullName => Name + " " + Surname;

        public string Email { get; set; }

        public string UserPrincipalName { get; set; }

        public string AdLogonName { get; set; }
    }
}
