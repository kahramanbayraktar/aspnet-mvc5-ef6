using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices.Auth
{
    public class RegionServices : IRegionServices
    {
        private readonly UnitOfWork _unitOfWork;

        public RegionServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RegionEntity GetRegionById(int regionId)
        {
            var region = _unitOfWork.RegionRepository.GetById(regionId);
            if (region != null)
            {
                //Mapper.CreateMap<Region, RegionEntity>();
                var regionModel = Mapper.Map<Region, RegionEntity>(region);
                return regionModel;
            }
            return null;
        }

        public IList<RegionEntity> GetAllRegions()
        {
            var roles = _unitOfWork.RegionRepository.GetAll().ToList();
            if (roles.Any())
            {
                //Mapper.CreateMap<Region, RegionEntity>();
                var rolesModel = Mapper.Map<List<Region>, List<RegionEntity>>(roles);
                return rolesModel;
            }
            return new List<RegionEntity>();
        }

        public int CreateRegion(RegionEntity regionEntity)
        {
            var region = new Region
            {
                Name = regionEntity.Name
            };
            _unitOfWork.RegionRepository.Insert(region);
            _unitOfWork.Save();

            return region.RegionId;
        }

        public bool UpdateRegion(int regionId, RegionEntity regionEntity)
        {
            var success = false;
            if (regionEntity != null)
            {
                var region = _unitOfWork.RegionRepository.GetById(regionId);
                if (region != null)
                {
                    region.Name = regionEntity.Name;
                    _unitOfWork.RegionRepository.Update(region);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool DeleteRegion(int regionId)
        {
            var success = false;
            if (regionId > 0)
            {
                var region = _unitOfWork.RegionRepository.GetById(regionId);
                if (region != null)
                {
                    _unitOfWork.RegionRepository.Delete(region);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }
    }
}
