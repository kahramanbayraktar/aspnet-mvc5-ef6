using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices.Auth
{
    public interface IRegionServices
    {
        RegionEntity GetRegionById(int regionId);

        IList<RegionEntity> GetAllRegions();

        int CreateRegion(RegionEntity regionEntity);

        bool UpdateRegion(int regionId, RegionEntity regionEntity);

        bool DeleteRegion(int regionId);
    }
}
