using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices.Auth
{
    // TODO: Improve these Services classes by shrinking them: remove needless methods, make the methods more generic applying func parameter.
    public class UserRoleServices : IUserRoleServices
    {
        private readonly UnitOfWork _unitOfWork;

        public UserRoleServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public UserRoleEntity GetUserRoleById(int userRoleId)
        {
            var userRole = _unitOfWork.UserRoleRepository.GetById(userRoleId);
            if (userRole != null)
            {
                //Mapper.CreateMap<User_Role, UserRoleEntity >()
                //    .ForMember(dest => dest.CountryName, src => src.MapFrom(x => x.Country.CountryName));
                var userRoleModel = Mapper.Map<UserRole, UserRoleEntity>(userRole);
                return userRoleModel;
            }
            return null;
        }

        public IList<UserRoleEntity> Get(int appId, int? userId = null)
        {
            var query = _unitOfWork.UserRoleRepository.GetManyQueryable(x => x.Role.ApplicationId == appId);
            if (userId > 0)
                query = query.Where(x => x.UserId == userId);
            var userRoles = query.ToList();
            if (userRoles.Any())
            {
                //Mapper.CreateMap<User_Role, UserRoleEntity>()
                //    .ForMember(dest => dest.CountryName, src => src.MapFrom(x => x.Country.CountryName));
                var userRoleModel = Mapper.Map<List<UserRole>, List<UserRoleEntity>>(userRoles);
                return userRoleModel;
            }
            return new List<UserRoleEntity>();
        }

        public IList<UserRoleEntity> Search(int?[] appIds, string userEmail, int?[] roleIds, int?[] regionIds, int?[] countryIds, int?[] industryIds)
        {
            var query = _unitOfWork.UserRoleRepository.GetManyQueryable(x => true);

            if (appIds != null && appIds.Any())
                query = query.Where(x => appIds.Contains(x.Role.ApplicationId));

            if (!string.IsNullOrWhiteSpace(userEmail))
                query = query.Where(x => x.User.Email.ToLower() == userEmail.ToLower());

            if (roleIds != null && roleIds.Any())
                query = query.Where(x => roleIds.Contains(x.RoleId));

            if (regionIds != null && regionIds.Any())
                query = query.Where(x => regionIds.Contains(x.RegionId));

            if (countryIds != null && countryIds.Any())
                query = query.Where(x => countryIds.Contains(x.CountryId));

            if (industryIds != null && industryIds.Any())
                query = query.Where(x => industryIds.Contains(x.IndustryId));

            var userRoleViews = query.ToList();

            if (userRoleViews.Any())
            {
                var userRoles = Mapper.Map<IList<UserRole>, IList<UserRoleEntity>>(userRoleViews);
                return userRoles;
            }
            return new List<UserRoleEntity>();
        }

        public int CreateUserRole(UserRoleEntity userRoleEntity)
        {
            var existingUserRoles = _unitOfWork.UserRoleRepository.GetManyQueryable(x =>
                x.UserId == userRoleEntity.UserId
                && x.RoleId == userRoleEntity.RoleId
                && x.RegionId == userRoleEntity.RegionId
                && x.CountryId == userRoleEntity.CountryId
                && x.IndustryId == userRoleEntity.IndustryId);

            if (existingUserRoles.Any())
                return 0;

            // TODO: Mapper
            var userRoleToBeAdded = new UserRole
            {
                UserId = userRoleEntity.UserId,
                RoleId = userRoleEntity.RoleId,
                RegionId = userRoleEntity.RegionId,
                CountryId = userRoleEntity.CountryId,
                IndustryId = userRoleEntity.IndustryId
            };

            try
            {
                _unitOfWork.UserRoleRepository.Insert(userRoleToBeAdded);
                _unitOfWork.Save();
                return 1;
            }
            catch (Exception exc)
            {
                return -1;
            }
        }

        public int UpdateUserRole(int userRoleId, UserRoleEntity userRoleEntity)
        {
            var userRoles = _unitOfWork.UserRoleRepository.GetManyQueryable(x => x.UserRoleId == userRoleId).ToList();

            if (!userRoles.Any())
                return -1;

            var existingUserRoles = _unitOfWork.UserRoleRepository.GetManyQueryable(x =>
                x.UserId == userRoleEntity.UserId
                && x.RoleId == userRoleEntity.RoleId
                && x.RegionId == userRoleEntity.RegionId
                && x.CountryId == userRoleEntity.CountryId
                && x.IndustryId == userRoleEntity.IndustryId);

            if (existingUserRoles.Any())
                return 0;

            // Update the fields
            // TODO: Mapper
            var userRoleToBeUpdated = userRoles.First();
            //userRoleToBeUpdated.UserId = userRoleEntity.UserId;
            userRoleToBeUpdated.RoleId = userRoleEntity.RoleId;
            userRoleToBeUpdated.RegionId = userRoleEntity.RegionId > 0 ? userRoleEntity.RegionId : null;
            userRoleToBeUpdated.CountryId = userRoleEntity.CountryId > 0 ? userRoleEntity.CountryId : null;
            userRoleToBeUpdated.IndustryId = userRoleEntity.IndustryId > 0 ? userRoleEntity.IndustryId : null;

            try
            {
                _unitOfWork.UserRoleRepository.Update(userRoleToBeUpdated);
                _unitOfWork.Save();
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public bool DeleteUserRole(int userRoleId)
        {
            var userRole = _unitOfWork.UserRoleRepository.GetById(userRoleId);
            if (userRole != null)
            {
                try
                {
                    _unitOfWork.UserRoleRepository.Delete(userRole);
                    _unitOfWork.Save();
                    return true;
                }
                catch (Exception exc)
                {
                    return false;
                }
            }
            return false;
        }
    }
}