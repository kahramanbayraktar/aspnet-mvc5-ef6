using AutoMapper;
using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class EventDirectorServices : IEventDirectorServices
    {
        private readonly UnitOfWork _unitOfWork;

        public EventDirectorServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EventDirectorEntity GetEventDirectorById(int eventDirectorId)
        {
            var eventDirector = _unitOfWork.EventDirectorRepository.GetById(eventDirectorId);
            if (eventDirector != null)
            {
                var directorModel = Mapper.Map<EventDirector, EventDirectorEntity>(eventDirector);
                return directorModel;
            }
            return null;
        }

        public EventDirectorEntity GetEventDirector(int eventId, string userEmail, int appId)
        {
            var eventDirectors = _unitOfWork.EventDirectorRepository.GetManyQueryable(x =>
                    x.EventId == eventId
                    && x.DirectorEmail.ToLower() == userEmail.ToLower()
                    && x.ApplicationId == appId)
                .ToList();

            if (eventDirectors.Any() && eventDirectors.Count == 1)
            {
                var eventDirectorModel = Mapper.Map<EventDirector, EventDirectorEntity>(eventDirectors.First());
                return eventDirectorModel;
            }
            return null;
        }

        public int GetEventDirectorsCount(int? eventId, string userEmail, int[] appIds, bool? isPrimary, bool? isAssistant)
        {
            return GetEventDirectorsQueryable(eventId, userEmail, appIds, isPrimary, isAssistant).Count();
        }

        public IList<EventDirectorEntity> GetEventDirectors(int? eventId, string userEmail, int[] appIds, bool? isPrimary, bool? isAssistant)
        {
            return GetEventDirectorsQueryable(eventId, userEmail, appIds, isPrimary, isAssistant).ToList();
        }

        public IList<EventDirectorEntity> GetEventDirectorsByEvent(int eventId, int? appId)
        {
            var query = _unitOfWork.EventDirectorRepository.GetManyQueryable(x => x.EventId == eventId);

            if (appId != null)
                query = query.Where(x => x.ApplicationId == appId);

            var eventDirectors = query
                .Select(x => new EventDirectorEntity
                {
                    EventId = x.EventId,
                    EventName = x.Event.MasterName,
                    DirectorEmail = x.DirectorEmail
                })
                .ToList();

            return eventDirectors;
        }

        public IList<EventDirectorEntity> GetEventDirectorsByUser(string userEmail, int? appId)
        {
            var query = _unitOfWork.EventDirectorRepository.GetManyQueryable(x => x.DirectorEmail.ToLower() == userEmail.ToLower());

            if (appId != null)
                query = query.Where(x => x.ApplicationId == appId);

            var eventDirectors = query
                .Select(x => new EventDirectorEntity
                {
                    EventId = x.EventId,
                    EventName = x.Event.MasterName,
                    DirectorEmail = x.DirectorEmail
                })
                .ToList();

            return eventDirectors;
        }

        public int CreateEventDirector(EventDirectorEntity eventDirectorEntity, int userId)
        {
            var eventDirector = new EventDirector
            {
                EventId = eventDirectorEntity.EventId,
                ApplicationId = eventDirectorEntity.ApplicationId,
                IsPrimary = eventDirectorEntity.IsPrimary,
                IsAssistant = eventDirectorEntity.IsAssistant,
                DirectorEmail = eventDirectorEntity.DirectorEmail,
                DirectorFullName = eventDirectorEntity.DirectorFullName,
                ADLogonName = eventDirectorEntity.ADLogonName,
                CreatedOn = DateTime.Now,
                CreatedBy = userId
            };

            _unitOfWork.EventDirectorRepository.Insert(eventDirector);
            _unitOfWork.Save();

            return eventDirector.EventDirectorId;
        }

        public bool UpdateEventDirector(int eventDirectorId, EventDirectorEntity eventDirectorEntity, int userId)
        {
            var success = false;
            if (eventDirectorEntity != null)
            {
                var eventDirector = _unitOfWork.EventDirectorRepository.GetById(eventDirectorId);
                if (eventDirector != null)
                {
                    eventDirector.IsPrimary = eventDirectorEntity.IsPrimary;
                    eventDirector.IsAssistant = eventDirectorEntity.IsAssistant;
                    eventDirector.UpdatedOn = DateTime.Now;
                    eventDirector.UpdatedBy = userId;

                    _unitOfWork.EventDirectorRepository.Update(eventDirector);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        public bool DeleteEventDirector(int eventDirectorId)
        {
            var success = false;
            if (eventDirectorId > 0)
            {
                var eventDirector = _unitOfWork.EventDirectorRepository.GetById(eventDirectorId);
                if (eventDirector != null)
                {
                    _unitOfWork.EventDirectorRepository.Delete(eventDirector);
                    _unitOfWork.Save();
                    success = true;
                }
            }
            return success;
        }

        // EXTRAS
        public bool IsAuthorized(string userEmail, int eventId, int appId)
        {
            userEmail = userEmail.ToLower();

            var authorized = _unitOfWork.EventDirectorRepository.GetManyQueryable(
                x => x.DirectorEmail.ToLower() == userEmail
                     && x.EventId == eventId
                     && x.ApplicationId == appId)
                .Any();

            return authorized;
        }

        public IList<EventDirectorEntity> GetPrimaryDirectors(int eventId, int appId)
        {
            var eventDirectors = _unitOfWork.EventDirectorRepository.GetManyQueryable(x =>
                x.EventId == eventId
                && x.ApplicationId == appId
                && x.IsPrimary == true)
                .ToList();

            if (!eventDirectors.Any()) return new List<EventDirectorEntity>();

            var entities = Mapper.Map<IList<EventDirector>, IList<EventDirectorEntity>>(eventDirectors);
            return entities;
        }

        public IList<EventDirectorEntity> GetAssistantDirectors(int eventId, int appId)
        {
            var eventDirectors = _unitOfWork.EventDirectorRepository.GetManyQueryable(x =>
                x.EventId == eventId
                && x.ApplicationId == appId
                && x.IsAssistant == true)
                .ToList();

            if (!eventDirectors.Any()) return new List<EventDirectorEntity>();

            var entities = Mapper.Map<IList<EventDirector>, IList<EventDirectorEntity>>(eventDirectors);
            return entities;
        }

        public bool IsPrimaryDirector(string userEmail, int? eventId, int appId)
        {
            userEmail = userEmail.ToLower();

            var isPrimary = _unitOfWork.EventDirectorRepository.GetManyQueryable(
                x => x.DirectorEmail.ToLower() == userEmail
                     && (eventId == null || x.EventId == eventId)
                     && x.ApplicationId == appId
                     && x.IsPrimary == true).Any();

            return isPrimary;
        }

        public bool IsAssistantDirector(string userEmail, int? eventId, int appId)
        {
            userEmail = userEmail.ToLower();

            var isAssistant = _unitOfWork.EventDirectorRepository.GetManyQueryable(
                x => x.DirectorEmail.ToLower() == userEmail
                     && (eventId == null || x.EventId == eventId)
                     && x.ApplicationId == appId
                     && x.IsAssistant == true).Any();

            return isAssistant;
        }

        public string GetRecipientEmails(EditionEntity edition)
        {            
            var assistantDirectors = GetAssistantDirectors(edition.EventId, WebConfigHelper.ApplicationIdCed);

            var allDirectors = new List<EventDirectorEntity>();

            var primaryDirectors = GetPrimaryDirectors(edition.EventId, WebConfigHelper.ApplicationIdCed);
            allDirectors.AddRange(primaryDirectors);

            allDirectors.AddRange(assistantDirectors);

            var recipientEmails = allDirectors.Any()
                ? string.Join(",", allDirectors.Select(x => x.DirectorEmail.ToLower()).ToList())
                : null;
            return recipientEmails;
        }

        private IQueryable<EventDirectorEntity> GetEventDirectorsQueryable(int? eventId, string userEmail, int[] appIds, bool? isPrimary, bool? isAssistant)
        {
            var query = _unitOfWork.EventDirectorRepository.GetManyQueryable(x => true);

            if (eventId > 0)
                query = query.Where(x => x.EventId == eventId.Value);

            if (!string.IsNullOrWhiteSpace(userEmail))
                query = query.Where(x => x.DirectorEmail.ToLower() == userEmail.ToLower());

            if (appIds.Any())
                query = query.Where(x => appIds.Contains(x.ApplicationId));

            if (isPrimary != null)
                query = isPrimary == true
                    ? query.Where(x => x.IsPrimary == true)
                    : query.Where(x => x.IsPrimary == false || x.IsPrimary == null);

            if (isAssistant != null)
                query = isAssistant == true
                    ? query.Where(x => x.IsAssistant == true)
                    : query.Where(x => x.IsAssistant == false || x.IsAssistant == null);

            return query
                .Select(x => new EventDirectorEntity
                {
                    EventDirectorId = x.EventDirectorId,
                    EventId = x.EventId,
                    EventName = x.Event.MasterName,
                    DirectorEmail = x.DirectorEmail,
                    ADLogonName = x.ADLogonName,
                    ApplicationId = x.ApplicationId,
                    IsPrimary = x.IsPrimary,
                    IsAssistant = x.IsAssistant,
                    CreatedOn = x.CreatedOn
                });
        }
    }
}
