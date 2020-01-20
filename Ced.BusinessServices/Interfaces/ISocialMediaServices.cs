using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface ISocialMediaServices
    {
        SocialMediaEntity GetSocialMediaById(string socialMediaId);

        IList<SocialMediaEntity> GetAllSocialMedias();

        string CreateSocialMedia(string socialMediaId);
    }
}
