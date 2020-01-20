using Ced.BusinessEntities;

namespace Ced.BusinessServices.Auth
{
    public interface IActiveDirectoryUserServices
    {
        UserEntity Authenticate(string email, string password);
    }
}
