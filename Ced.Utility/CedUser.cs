using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using System.Collections.Generic;
using System.Linq;

namespace Ced.Utility
{
    public class CedUser
    {
        public UserEntity CurrentUser { get; private set; }
        
        //public IList<RoleActionEntity> RoleActions { get; private set; }
        
        public IList<RoleEntity> Roles { get; private set; }
        
        public bool IsSuperAdmin { get; set; }
        
        public bool IsAdmin { get; set; }

        public bool IsEventBasedAdmin { get; set; }
        
        public bool IsViewer { get; set; }

        public bool IsApprover { get; set; }

        public bool IsPrimaryDirector { get; set; }

        public bool IsAssistantDirector { get; set; }

        public bool IsImpersonated { get; set; }

        public CedUser(UserEntity currentUser, IList<RoleEntity> roles)
        {
            CurrentUser = currentUser;
            Roles = roles;

            IsSuperAdmin = roles != null && roles.Any(x => x.Name.ToLower() == "super admin");
            IsAdmin = roles != null && roles.Any(x => x.Name.ToLower() == "admin");
            IsViewer = roles != null && roles.Any(x => x.Name.ToLower().Contains("viewer")
                // TODO: What if a user is both Industry Director and Event Director?
                // Is that user still a Viewer - who is not eligible to edit any entity?
                //|| x.Name.ToLower() == "region director"
                //|| x.Name.ToLower() == "country director"
                //|| x.Name.ToLower() == "industry director"
            );
            IsApprover = roles != null && roles.Any(x => x.Name.ToLower() == "approver");
            IsEventBasedAdmin = roles != null && roles.Any(x => x.Name.ToLower() == "event based admin");
        }
    }
}