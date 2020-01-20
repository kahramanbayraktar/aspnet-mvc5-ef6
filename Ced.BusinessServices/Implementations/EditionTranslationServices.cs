using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices.Helpers;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class EditionTranslationServices : IEditionTranslationServices
    {
        private readonly UnitOfWork _unitOfWork;
        private EventServices _eventServices;

        public EditionTranslationServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EditionTranslationEntity GetEditionTranslationById(int editionTranslationId)
        {
            var editionTranslation = _unitOfWork.EditionTranslationRepository.GetById(editionTranslationId);
            if (editionTranslation != null)
            {
                var editionTranslationModel =
                    Mapper.Map<EditionTranslation, EditionTranslationEntity>(editionTranslation);
                return editionTranslationModel;
            }
            return null;
        }

        public EditionTranslationEntity GetEditionTranslation(int editionId, string languageCode)
        {
            var editionTranslation =
                _unitOfWork.EditionTranslationRepository.Get(
                    x => x.EditionId == editionId && x.LanguageCode.ToLower() == languageCode.ToLower());

            if (editionTranslation != null)
            {
                var editionTranslationModel = Mapper.Map<EditionTranslation, EditionTranslationEntity>(editionTranslation);
                return editionTranslationModel;
            }
            return null;
        }

        public bool Exists(int editionId, string languageCode)
        {
            return _unitOfWork.EditionTranslationRepository.Exists(x => x.EditionId == editionId && x.LanguageCode == languageCode);
        }

        public EditionTranslationEntityLight GetEditionTranslationLight(int editionId, string languageCode)
        {
            var query = _unitOfWork.EditionTranslationRepository
                .GetManyQueryableProjected<EditionTranslationEntityLight>(
                    x => x.EditionId == editionId && x.LanguageCode.ToLower() == languageCode.ToLower());
            return query.SingleOrDefault();
        }

        public IList<EditionTranslationEntity> GetEditionTranslationsByEdition(int editionId)
        {
            var editionTranslations = _unitOfWork.EditionTranslationRepository.GetManyQueryable(x => x.EditionId == editionId).ToList();
            if (editionTranslations.Any())
            {
                var editionTranslationModel = Mapper.Map<IList<EditionTranslation>, IList<EditionTranslationEntity>>(editionTranslations);
                return editionTranslationModel;
            }
            return new List<EditionTranslationEntity>();
        }

        //public IList<EditionVenue> SearchVenues(int eventId, string searchTerm, int pageSize, int pageNum)
        public IList<EditionVenue> SearchVenues(EditionEntity edition, string searchTerm, int pageSize, int pageNum)
        {
            //_eventServices = new EventServices(_unitOfWork);

            //var @event = _eventServices.GetEventById(eventId);

            //var query = _unitOfWork.EditionTranslationRepository.GetManyQueryableProjected<EditionVenue>(x =>
            //        x.VenueName.ToLower().Contains(searchTerm.ToLower())
            //        && x.Edition.Event.Country.ToLower() == @event.Country.ToLower())
            //    .Distinct();

            var query = _unitOfWork.EditionTranslationRepository.GetManyQueryableProjected<EditionVenue>(x =>
                    x.VenueName.ToLower().Contains(searchTerm.ToLower())
                    //&& x.Edition.Event.Country.ToLower() == @event.Country.ToLower())
                    && x.Edition.Country.ToLower() == edition.Country.ToLower())
                .Distinct();

            query = query.OrderBy(x => x.VenueName);
            query = query.Skip(pageSize * (pageNum - 1));
            query = query.Take(pageSize);

            var venues = query.ToList();
            return venues;
        }

        public int CreateEditionTranslation(EditionTranslationEntity editionTranslationEntity, int userId)
        {
            var editionTranslation = Mapper.Map<EditionTranslationEntity, EditionTranslation>(editionTranslationEntity);

            editionTranslation.CreateTime = DateTime.Now;
            editionTranslation.CreateUser = userId;

            _unitOfWork.EditionTranslationRepository.Insert(editionTranslation);
            _unitOfWork.Save();

            return editionTranslation.EditionTranslationId;
        }

        public bool UpdateEditionTranslation(EditionTranslationEntity editionTranslationEntity, int userId, bool? autoIntegration = false)
        {
            var success = false;
            if (editionTranslationEntity != null)
            {
                var editionTranslation = _unitOfWork.EditionTranslationRepository.GetById(editionTranslationEntity.EditionTranslationId);
                if (editionTranslation != null)
                {
                    // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                    Mapper.Map(editionTranslationEntity, editionTranslation);

                    if (autoIntegration == false)
                    {
                        editionTranslation.UpdateTime = DateTime.Now;
                        editionTranslation.UpdateUser = userId;
                    }
                    else
                    {
                        editionTranslation.UpdateTimeByAutoIntegration = DateTime.Now;
                    }

                    _unitOfWork.EditionTranslationRepository.Update(editionTranslation);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool DeleteEditionTranslation(int editionTranslationId)
        {
            var success = false;
            if (editionTranslationId > 0)
            {
                var exists = _unitOfWork.EditionTranslationRepository.Exists(editionTranslationId);
                if (exists)
                {
                    _unitOfWork.EditionTranslationRepository.Delete(editionTranslationId);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        // EXTRAS
        public
        bool IsEditionTranslationInfoCompleted(EditionTranslationEntity editionTranslationEntity, EditionInfoType infoType)
        {
            return EditionServiceHelper.IsEditionInfoCompleted(editionTranslationEntity, infoType);
        }
    }
}
