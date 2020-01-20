using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class EditionCountryServices : IEditionCountryServices
    {
        private readonly UnitOfWork _unitOfWork;

        public EditionCountryServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionCountryEntity GetEditionCountryById(int editionCountryId)
        {
            var editionCountry = _unitOfWork.EditionCountryRepository.GetById(editionCountryId);
            if (editionCountry != null)
            {
                var entity = Mapper.Map<EditionCountry, EditionCountryEntity>(editionCountry);
                return entity;
            }
            return null;
        }

        public IList<EditionCountryEntity> GetEditionCountriesByEdition(int editionId, BusinessEntities.EditionCountryRelationType relationType)
        {
            var editionCountries = _unitOfWork.EditionCountryRepository
                .GetManyQueryable(x => x.EditionId == editionId && x.RelationType == (byte) relationType)
                .OrderBy(x => x.CountryCode)
                .ToList();
            if (editionCountries.Any())
            {
                var entity = Mapper.Map<List<EditionCountry>, List<EditionCountryEntity>>(editionCountries);
                return entity;
            }
            return new List<EditionCountryEntity>();
        }

        public int CreateEditionCountry(EditionCountryEntity editionCountryEntity, int userId)
        {
            // TODO:
            //Mapper.CreateMap<EditionCountryEntity, EditionCountry>()
            //    .ForMember(dest => dest.EditionId, src => src.MapFrom(x => x.EditionId))
            //    .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
            //var editionCountry = Mapper.Map<EditionCountryEntity, EditionCountry>(editionCountryEntity);
            var editionCountry = new EditionCountry
            {
                EditionId = editionCountryEntity.EditionId,
                CountryCode = editionCountryEntity.CountryCode,
                RelationType = (byte) editionCountryEntity.RelationType,
                CreatedOn = DateTime.Now,
                CreatedBy = userId
            };

            _unitOfWork.EditionCountryRepository.Insert(editionCountry);
            _unitOfWork.Save();
            return editionCountry.EditionCountryId;
        }

        public bool UpdateEditionCountry(int editionCountryId, EditionCountryEntity editionCountryEntity, int userId)
        {
            var success = false;
            if (editionCountryEntity != null)
            {
                var editionCountry = _unitOfWork.EditionCountryRepository.GetById(editionCountryId);
                if (editionCountry != null)
                {
                    // TODO:
                    //Mapper.CreateMap<EditionCountryEntity, EditionCountry>()
                    //    .ForMember(dest => dest.EditionId, opt => opt.Ignore())
                    //    .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                    //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                    // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                    //Mapper.Map(editionCountryEntity, editionCountry);
                    editionCountry.CountryCode = editionCountryEntity.CountryCode;
                    editionCountry.RelationType = (byte)editionCountryEntity.RelationType;
                    editionCountry.UpdatedOn = DateTime.Now;
                    editionCountry.UpdatedBy = userId;

                    _unitOfWork.EditionCountryRepository.Update(editionCountry);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        public bool DeleteEditionCountry(int editionCountryId)
        {
            var success = false;
            if (editionCountryId > 0)
            {
                var exists = _unitOfWork.EditionCountryRepository.Exists(editionCountryId);
                if (exists)
                {
                    _unitOfWork.EditionCountryRepository.Delete(editionCountryId);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        public bool DeleteAllEditionCountriesByEdition(int editionId)
        {
            var success = false;
            if (editionId > 0)
            {
                // TODO: Edition vb entity'ler de geliyor mu?
                var editionCountries =_unitOfWork.EditionCountryRepository.GetManyQueryable(x => x.EditionId == editionId);

                foreach (var editionCountry in editionCountries)
                {
                    _unitOfWork.EditionCountryRepository.Delete(editionCountry);
                }

                _unitOfWork.Save();
                success = true;
            }
            return success;
        }
    }
}
