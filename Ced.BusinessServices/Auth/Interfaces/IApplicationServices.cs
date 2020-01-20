using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    /// <summary>
    /// Application Service Contract
    /// </summary>
    public interface IApplicationServices
    {
        ApplicationEntity GetApplicationById(int applicationId);

        IList<ApplicationEntity> GetAllApplications();
        
        int CreateApplication(ApplicationEntity applicationEntity);

        void UpdateApplication(int applicationId, ApplicationEntity applicationEntity);

        void DeleteApplication(int applicationId);
    }
}
