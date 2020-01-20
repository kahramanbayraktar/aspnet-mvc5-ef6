using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class CountryServices : ICountryServices
    {
        private readonly UnitOfWork _unitOfWork;

        public CountryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public CountryEntity GetCountryByName(string name)
        {
            var countries = _unitOfWork.CountryRepository.GetManyQueryable(x => x.CountryName.ToLower() == name.ToLower());
            if (countries.Any() && countries.Count() == 1)
            {
                var entity = Mapper.Map<Country, CountryEntity>(countries.First());
                return entity;
            }
            return null;
        }

        public IList<CountryEntity> GetAllCountries()
        {
            var countries = _unitOfWork.CountryRepository.GetAll().ToList();
            if (countries.Any())
            {
                var countriesModel = Mapper.Map<List<Country>, List<CountryEntity>>(countries);
                return countriesModel;
            }
            return new List<CountryEntity>();
        }
    }
}
