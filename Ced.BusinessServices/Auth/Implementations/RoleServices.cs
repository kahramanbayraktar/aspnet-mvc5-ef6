using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices.Auth
{
    /// <summary>
    /// Offers services for role specific CRUD operations
    /// </summary>
    public class RoleServices : IRoleServices
    {
        private readonly UnitOfWork _unitOfWork;

        public RoleServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public RoleEntity GetRoleById(int roleId)
        {
            var role = _unitOfWork.RoleRepository.GetById(roleId);
            if (role != null)
            {
                //Mapper.CreateMap<Role, RoleEntity>();
                var roleModel = Mapper.Map<Role, RoleEntity>(role);
                return roleModel;
            }
            return null;
        }

        public IList<RoleEntity> GetAllRoles(int? appId)
        {
            var roles = _unitOfWork.RoleRepository.GetManyQueryable(x => x.ApplicationId == (appId > 0 ? appId : x.ApplicationId)).ToList();
            if (roles.Any())
            {
                //Mapper.CreateMap<Role, RoleEntity>();
                var rolesModel = Mapper.Map<List<Role>, List<RoleEntity>>(roles);
                return rolesModel;
            }
            return new List<RoleEntity>();
        }

        public int CreateRole(RoleEntity roleEntity)
        {
            var role = new Role
            {
                Name = roleEntity.Name
            };
            _unitOfWork.RoleRepository.Insert(role);
            _unitOfWork.Save();

            return role.RoleId;
        }

        public bool UpdateRole(int roleId, RoleEntity roleEntity)
        {
            var success = false;
            if (roleEntity != null)
            {
                var role = _unitOfWork.RoleRepository.GetById(roleId);
                if (role != null)
                {
                    role.Name = roleEntity.Name;
                    _unitOfWork.RoleRepository.Update(role);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool DeleteRole(int roleId)
        {
            var success = false;
            if (roleId > 0)
            {
                var role = _unitOfWork.RoleRepository.GetById(roleId);
                if (role != null)
                {
                    _unitOfWork.RoleRepository.Delete(role);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }
    }
}
