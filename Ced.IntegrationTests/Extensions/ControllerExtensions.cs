using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.Utility;
using Ced.Web.Controllers;
using System.Collections.Generic;

namespace Ced.IntegrationTests.Extensions
{
    public static class ControllerExtensions
    {
        public static void SetDefaultUser(this GlobalController controller)
        {
            controller.CurrentCedUser = new CedUser(new UserEntity { Email = "kahraman.bayraktar@ite-turkey.com" }, new List<RoleEntity>());
        }
    }
}
