using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.Utility.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Ced.BusinessServices.Auth
{
    public class UserServices : IUserServices
    {
        private readonly UnitOfWork _unitOfWork;

        public UserServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public int Authenticate(string userName, string password)
        {
            var user = _unitOfWork.UserRepository.Get(u => u.Email == userName && u.Password == password);
            if (user != null && user.UserId > 0)
                return user.UserId;
            return 0;
        }

        public UserEntity GetUser(int userId)
        {
            var user = _unitOfWork.UserRepository.Get(u => u.UserId == userId);

            //Mapper.CreateMap<User, UserEntity>();
            var userModel = Mapper.Map<User, UserEntity>(user);
            return userModel;
        }

        public UserEntity GetUser(string email)
        {
            var user = _unitOfWork.UserRepository.Get(u => u.Email.ToLower() == email.ToLower());

            //Mapper.CreateMap<User, UserEntity>();
            var userModel = Mapper.Map<User, UserEntity>(user);
            return userModel;
        }

        /// <summary>
        /// Fetches all the users.
        /// </summary>
        /// <returns></returns>
        public IList<UserEntity> GetAllUsers()
        {
            var users = _unitOfWork.UserRepository.GetAll().ToList();
            if (users.Any())
            {
                //Mapper.CreateMap<User, UserEntity>();
                var rolesModel = Mapper.Map<List<User>, List<UserEntity>>(users);
                return rolesModel;
            }
            return new List<UserEntity>();
        }

        public IList<UserEntity> GetUsersByRole(int appId, string role)
        {
            role = role.ToLower();

            var users = _unitOfWork.UserRoleRepository.GetManyQueryable(x =>
                x.Role.Name.ToLower() == role && x.Role.ApplicationId == appId)
                .Select(x => x.User)
                .ToList();

            if (users.Any())
            {
                //Mapper.CreateMap<User, UserEntity>();
                var rolesModel = Mapper.Map<List<User>, List<UserEntity>>(users);
                return rolesModel;
            }
            return new List<UserEntity>();
        }

        public IList<UserEntity> GetUsersByEmail(string email)
        {
            email = email.ToLower();

            var users = _unitOfWork.UserRepository.GetManyQueryable(x =>
                    x.Email.ToLower().Contains(email))
                .ToList();

            if (users.Any())
            {
                //Mapper.CreateMap<User, UserEntity>();
                var rolesModel = Mapper.Map<List<User>, List<UserEntity>>(users);
                return rolesModel;
            }
            return new List<UserEntity>();
        }

        public UserEntity CreateUser(UserEntity userEntity)
        {
            using (var scope = new TransactionScope())
            {
                var existingUser = _unitOfWork.UserRepository.GetManyQueryable(x =>
                    x.Email.Trim().ToLower() == userEntity.Email.Trim().ToLower());

                if (existingUser != null && existingUser.Any())
                {
                    return null;
                }

                var user = new User
                {
                    Email = userEntity.Email.Trim().ToLower(),
                    Name = userEntity.Name?.ToLower(),
                    Surname = userEntity.Surname?.ToLower(),
                    ADLogonName = userEntity.AdLogonName?.Trim().ToLower(),
                    UserPrincipalName = userEntity.Email.Trim().ToLower(),
                    CreateTime = DateTime.Now
                };
                _unitOfWork.UserRepository.Insert(user);
                _unitOfWork.Save();
                scope.Complete();

                //Mapper.CreateMap<User, UserEntity>();
                var newUser = Mapper.Map<User, UserEntity>(user);
                return newUser;
            }
        }

        public bool UpdateUser(int userId, UserEntity userEntity)
        {
            var success = false;
            try
            {
                var user = _unitOfWork.UserRepository.GetById(userId);
                if (user != null)
                {
                    user.Name = userEntity.Name;
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();
                    
                    success = true;
                }
            }
            catch (Exception exc)
            {
                success = false;
            }
            return success;
        }

        public string GetProfilePictureUrl(int userId)
        {
            var profilePicName = $"{userId}.jpg";
            return UserImageType.ProfilePic.BlobFullUrl(profilePicName);
        }

        public string GetProfilePictureName(int userId)
        {
            return userId + ".jpg";
        }
    }
}
