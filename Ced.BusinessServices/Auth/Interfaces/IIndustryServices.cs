using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    public interface IIndustryServices
    {
        IndustryEntity GetIndustryById(int industryId);

        IList<IndustryEntity> GetAllIndustries();

        int CreateIndustry(IndustryEntity industryEntity);

        bool UpdateIndustry(int industryId, IndustryEntity industryEntity);

        bool DeleteIndustry(int industryId);
    }
}
