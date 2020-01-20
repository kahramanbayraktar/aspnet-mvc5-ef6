using Ced.BusinessEntities.Auth;
using System.Collections.Generic;

namespace Ced.BusinessServices
{
    public interface ICountryServices
    {
        CountryEntity GetCountryByName(string name);

        IList<CountryEntity> GetAllCountries();
    }
}
