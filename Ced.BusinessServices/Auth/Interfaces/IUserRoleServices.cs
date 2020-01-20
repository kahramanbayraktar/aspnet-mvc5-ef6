using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    public interface IUserRoleServices
    {
        UserRoleEntity GetUserRoleById(int userRoleId);

        IList<UserRoleEntity> Get(int appId, int? userId = null);

        IList<UserRoleEntity> Search(int?[] appIds, string userEmail, int?[] roleIds, int?[] regionIds, int?[] countryIds, int?[] industryIds);

        int CreateUserRole(UserRoleEntity userRoleEntity);

        int UpdateUserRole(int userRoleId, UserRoleEntity userRoleEntity);

        bool DeleteUserRole(int userRoleId);
    }
}
