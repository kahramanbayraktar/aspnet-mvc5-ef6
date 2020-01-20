using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    /// <summary>
    /// Role Service Contract
    /// </summary>
    public interface IRoleServices
    {
        RoleEntity GetRoleById(int roleId);

        IList<RoleEntity> GetAllRoles(int? appId);

        int CreateRole(RoleEntity roleEntity);

        bool UpdateRole(int roleId, RoleEntity roleEntity);

        bool DeleteRole(int roleId);
    }
}
