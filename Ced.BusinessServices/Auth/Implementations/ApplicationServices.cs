using AutoMapper;
using Ced.BusinessEntities.Auth;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices.Auth
{
    /// <summary>
    /// Offers services for application specific CRUD operations
    /// </summary>
    public class ApplicationServices : IApplicationServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public ApplicationServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Fetches application details by id
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public ApplicationEntity GetApplicationById(int applicationId)
        {
            var application = _unitOfWork.ApplicationRepository.GetById(applicationId);
            if (application != null)
            {
                //Mapper.CreateMap<Application, ApplicationEntity>();
                var applicationModel = Mapper.Map<Application, ApplicationEntity>(application);
                return applicationModel;
            }
            return null;
        }

        /// <summary>
        /// Fetches all the applications.
        /// </summary>
        /// <returns></returns>
        public IList<ApplicationEntity> GetAllApplications()
        {
            var applications = _unitOfWork.ApplicationRepository.GetAll().ToList();
            if (applications.Any())
            {
                //Mapper.CreateMap<Application, ApplicationEntity>();
                var applicationsModel = Mapper.Map<List<Application>, List<ApplicationEntity>>(applications);
                return applicationsModel;
            }
            return new List<ApplicationEntity>();
        }

        /// <summary>
        /// Creates a application
        /// </summary>
        /// <param name="applicationEntity"></param>
        /// <returns></returns>
        public int CreateApplication(ApplicationEntity applicationEntity)
        {
            var application = new Application
            {
                Name = applicationEntity.Name
            };
            _unitOfWork.ApplicationRepository.Insert(application);
            _unitOfWork.Save();
            return application.ApplicationId;
        }

        /// <summary>
        /// Updates a application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="applicationEntity"></param>
        /// <returns></returns>
        public void UpdateApplication(int applicationId, ApplicationEntity applicationEntity)
        {
            var application = _unitOfWork.ApplicationRepository.GetById(applicationId);
            application.Name = applicationEntity.Name;
            _unitOfWork.ApplicationRepository.Update(application);
            _unitOfWork.Save();
        }

        /// <summary>
        /// Deletes a particular application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public void DeleteApplication(int applicationId)
        {
            var application = _unitOfWork.ApplicationRepository.GetById(applicationId);
            _unitOfWork.ApplicationRepository.Delete(application);
            _unitOfWork.Save();
        }
    }
}
