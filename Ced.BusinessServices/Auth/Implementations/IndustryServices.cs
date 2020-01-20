using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices.Auth
{
    public class IndustryServices : IIndustryServices
    {
        private readonly UnitOfWork _unitOfWork;

        public IndustryServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IndustryEntity GetIndustryById(int regionId)
        {
            var region = _unitOfWork.IndustryRepository.GetById(regionId);
            if (region != null)
            {
                //Mapper.CreateMap<Industry, IndustryEntity>();
                var regionModel = Mapper.Map<Industry, IndustryEntity>(region);
                return regionModel;
            }
            return null;
        }

        public IList<IndustryEntity> GetAllIndustries()
        {
            var roles = _unitOfWork.IndustryRepository.GetAll().ToList();
            if (roles.Any())
            {
                //Mapper.CreateMap<Industry, IndustryEntity>();
                var rolesModel = Mapper.Map<List<Industry>, List<IndustryEntity>>(roles);
                return rolesModel;
            }
            return new List<IndustryEntity>();
        }

        public int CreateIndustry(IndustryEntity regionEntity)
        {
            var region = new Industry
            {
                Name = regionEntity.Name
            };
            _unitOfWork.IndustryRepository.Insert(region);
            _unitOfWork.Save();

            return region.IndustryId;
        }

        public bool UpdateIndustry(int regionId, IndustryEntity regionEntity)
        {
            var success = false;
            if (regionEntity != null)
            {
                var region = _unitOfWork.IndustryRepository.GetById(regionId);
                if (region != null)
                {
                    region.Name = regionEntity.Name;
                    _unitOfWork.IndustryRepository.Update(region);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool DeleteIndustry(int regionId)
        {
            var success = false;
            if (regionId > 0)
            {
                var region = _unitOfWork.IndustryRepository.GetById(regionId);
                if (region != null)
                {
                    _unitOfWork.IndustryRepository.Delete(region);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }
    }
}
