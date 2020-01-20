using Ced.BusinessEntities;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    public interface IUserServices
    {
        int Authenticate(string userName, string password);
        IList<UserEntity> GetAllUsers();
        IList<UserEntity> GetUsersByRole(int appId, string role);
        IList<UserEntity> GetUsersByEmail(string email);
        UserEntity GetUser(int id);
        UserEntity GetUser(string email);
        UserEntity CreateUser(UserEntity userEntity);
        bool UpdateUser(int userId, UserEntity userEntity);
        string GetProfilePictureUrl(int userId);
        string GetProfilePictureName(int userId);
    }
}
