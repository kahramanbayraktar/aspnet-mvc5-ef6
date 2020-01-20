using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.ExternalEntities;
using Ced.BusinessServices.Helpers;
using Ced.BusinessServices.Models;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Ced.BusinessServices
{
    public class EditionServices : IEditionServices
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EditionCohostServices _editionCohostServices;
        private readonly EditionTranslationServices _editionTranslationServices;
        private readonly EditionTranslationSocialMediaServices _editionTranslationSocialMediaServices;
        private readonly FileServices _fileServices;
        private readonly NotificationServices _notificationServices;
        private readonly EditionServiceHelper _editionServiceHelper;

        public EditionServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
            _editionCohostServices = new EditionCohostServices(unitOfWork);
            _editionTranslationServices = new EditionTranslationServices(unitOfWork);
            _editionTranslationSocialMediaServices = new EditionTranslationSocialMediaServices(unitOfWork);
            _fileServices = new FileServices(_unitOfWork);
            _notificationServices = new NotificationServices(unitOfWork);
            _editionServiceHelper = new EditionServiceHelper(unitOfWork);
        }

        public EditionEntity GetEditionById(int editionId, string[] eventTypes = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => x.EditionId == editionId);

            // TODO: PATCH#001
            //if (eventTypes != null && eventTypes.Length > 0)
            //{
            //    eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
            //    query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            //}
            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.EventTypeCode.ToLower()));
            }

            var edition = query.SingleOrDefault();
            if (edition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(edition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetEditionByAxId(int axId, string[] eventTypes = null)
        {
            // TODO: Context yerine Repository'yi kullan!
            var context = _unitOfWork.EditionRepository.Context;
            IQueryable<Edition> query = context.Set<Edition>();
            query = query.Where(x => x.AxEventId == axId);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            var edition = query.SingleOrDefault();

            // TODO: Gereksiz sorgular yaptığı için performansı düşürüyor.
            //var query = _unitOfWork.EditionRepository.GetManyQueryable(x => x.AxEventId == axId);
            //if (eventTypes != null)
            //{
            // eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
            // query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            //}
            //var edition = query.SingleOrDefault();

            if (edition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(edition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetLastEdition(int eventId, EditionStatusType[] statuses = null, string[] eventTypes = null)
        {
            var now = DateTime.Now.Date;
            // TODO: Contains yerine == kullanmak query'yi çok hızlandırır mı?
            var query = _unitOfWork.EditionRepository
               .GetManyQueryable(x => x.EventId == eventId);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.Status));
            }

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            var edition = query
               .OrderByDescending(x => x.EndDate)
               .Take(1)
               .SingleOrDefault();

            if (edition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(edition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetLastFinishedEdition(int eventId, string[] eventTypes = null)
        {
            var now = DateTime.Now.Date;
            // TODO: Contains yerine == kullanmak query'yi çok hızlandırır mı?
            var query = _unitOfWork.EditionRepository
               .GetManyQueryable(x => x.EventId == eventId);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            query = query.Where(x => x.EndDate < now);

            var edition = query
               .OrderByDescending(x => x.EndDate)
               .Take(1)
               .SingleOrDefault();

            if (edition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(edition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetClosestEdition(int eventId, string[] eventTypes = null)
        {
            var now = DateTime.Now.Date;
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.EventId == eventId
                && x.StartDate > now);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            query = query
                .OrderBy(x => x.StartDate)
                .Take(1);

            var edition = query.SingleOrDefault();
            if (edition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(edition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetPreviousEdition(EditionEntity edition, string[] eventTypes = null)
        {
            var now = DateTime.Now.Date;
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.EventId == edition.EventId
                && x.EditionId != edition.EditionId
                && x.StartDate < edition.StartDate);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            query = query
                .OrderByDescending(x => x.StartDate)
                .Take(1);

            var prevEdition = query.SingleOrDefault();
            if (prevEdition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(prevEdition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetPreviousEdition(int editionId, string[] eventTypes = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true)
                .Join(_unitOfWork.EditionRepository.GetManyQueryable(x => true),
                    ePrev => ePrev.EventId,
                    eCurrent => eCurrent.EventId,
                    (eCurrent, ePrev) => new { eCurrent, ePrev })
                .Where(result => result.eCurrent.EditionId == editionId && result.ePrev.StartDate < result.eCurrent.StartDate);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.ePrev.Event.EventTypeCode.ToLower()));
            }

            var prevEdition = query.Select(result => result.ePrev).OrderByDescending(x => x.StartDate).Take(1).SingleOrDefault();
            if (prevEdition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(prevEdition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetPreviousEditionByAxId(string axId, string[] eventTypes = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true)
                .Join(_unitOfWork.EditionRepository.GetManyQueryable(x => true),
                    ePrev => ePrev.EventId,
                    eCurrent => eCurrent.EventId,
                    (eCurrent, ePrev) => new { eCurrent, ePrev })
                .Where(result => result.eCurrent.AxEventId.ToString() == axId && result.ePrev.StartDate < result.eCurrent.StartDate);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.ePrev.Event.EventTypeCode.ToLower()));
            }

            var prevEdition = query.Select(result => result.ePrev).OrderByDescending(x => x.StartDate).Take(1).SingleOrDefault();
            if (prevEdition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(prevEdition);
                return editionModel;
            }
            return null;
        }

        public EditionEntity GetNextEdition(EditionEntity edition, string[] eventTypes = null)
        {
            var now = DateTime.Now.Date;
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.EventId == edition.EventId
                && x.EditionId != edition.EditionId
                && x.StartDate > edition.StartDate);

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            query = query
                .OrderBy(x => x.StartDate)
                .Take(1);

            var prevEdition = query.SingleOrDefault();
            if (prevEdition != null)
            {
                var editionModel = Mapper.Map<Edition, EditionEntity>(prevEdition);
                return editionModel;
            }
            return null;
        }

        public int GetEditionsCount(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null)
        {
            return GetEditionsQueryable(directorEmail, eventId, mustBePrimaryDirector, minFinancialYear, statuses, eventTypes, eventActivities).Count();
        }

        public int GetEditionsCount2(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null,
            string[] regions = null, string country = null, string city = null)
        {
            return GetEditionsQueryable2(directorEmail, eventId, mustBePrimaryDirector, minFinancialYear, statuses, eventTypes, eventActivities, regions, country, city).Count();
        }

        public IList<EditionEntityLight> GetEditions(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null)
        {
            return GetEditionsQueryable(directorEmail, eventId, mustBePrimaryDirector, minFinancialYear, statuses, eventTypes, eventActivities).ToList();
        }

        public IList<EditionEntityLight> GetEditions2(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null,
            string[] regions = null, string country = null, string city = null)
        {
            return GetEditionsQueryable2(directorEmail, eventId, mustBePrimaryDirector, minFinancialYear, statuses, eventTypes, eventActivities, regions, country, city).ToList();
        }

        #region EDITIONS FOR NOTIFICATION

        public IList<EditionEntity> GetEditionsByNotificationType(List<int> checkDays, int? lastXDays, NotificationType notificationType, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            switch (notificationType)
            {
                case NotificationType.EditionExistence:
                    return GetEditionsByNextEditionExistence(checkDays, lastXDays, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.GeneralInfoCompleteness:
                    return GetEditionsByNextEdition(checkDays, lastXDays, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.PostShowMetricsInfoCompleteness:
                    return GetEditionsByCheckDays(checkDays, lastXDays, true, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.PostShowMetricsInfoCompleteness2:
                    return GetEditionsByCheckDays(checkDays, lastXDays, true, minFinancialYear, statuses, eventTypes, eventActivities, eventId);
            }
            return new List<EditionEntity>();
        }

        public IList<EditionEntity> GetEditionsByNotificationType(int checkDay, int? lastXDays, NotificationType notificationType, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            switch (notificationType)
            {
                case NotificationType.EditionExistence:
                    return GetEditionsByNextEdition(checkDay, lastXDays, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.GeneralInfoCompleteness:
                    return GetEditionsByNextEdition(checkDay, lastXDays, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.PostShowMetricsInfoCompleteness:
                    return GetEditionsByCheckDays(checkDay, lastXDays, true, minFinancialYear, statuses, eventTypes, eventActivities, eventId);

                case NotificationType.PostShowMetricsInfoCompleteness2:
                    return GetEditionsByCheckDays(checkDay, lastXDays, true, minFinancialYear, statuses, eventTypes, eventActivities, eventId);
            }
            return new List<EditionEntity>();
        }

        public IList<EditionEntity> GetEditionsByNextEditionExistence(IList<int> checkDays, int? lastXDays, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true)
                .Join(_unitOfWork.EditionRepository.GetManyQueryable(x => true),
                    eNext => eNext.EventId,
                    eCurrent => eCurrent.EventId,
                    (eCurrent, eNext) => new { eCurrent, eNext });

            if (lastXDays > 0)
            {
                var emailSendingStartDate = DateTime.Today.AddDays(-lastXDays.Value);
                var emailSendingEndDate = DateTime.Today;
                query = query.Where(x =>
                    (x.eCurrent.StartDate >= emailSendingStartDate && x.eCurrent.StartDate <= emailSendingEndDate)
                    || (x.eCurrent.EndDate >= emailSendingStartDate && x.eCurrent.EndDate <= emailSendingEndDate));
            }
            else
            {
                var now = DateTime.Now.Date;
                var dates = checkDays.Select(day => now.AddDays(day)).Select(dummy => (DateTime?)dummy).ToList();
                query = query.Where(x => dates.Contains(x.eCurrent.StartDate));
            }

            query = query.Where(result =>
                //result.eNext == null &&
                (eventId == null || result.eCurrent.EventId == eventId));

            if (minFinancialYear != null)
                query = query.Where(x => x.eCurrent.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.eCurrent.Status));
            }

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.eCurrent.Event.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Length > 0)
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.eCurrent.EventActivity.ToLower()));
            }

            var editions = query.Select(result => result.eCurrent).ToList();

            // PATCH:
            editions = editions.OrderBy(x => x.StartDate).GroupBy(x => x.EventId).Select(y => y.First()).ToList();

            // TODO: Improve: Remove patch by improving the query above (result.eNext == null).
            // PATCH2: If next edition is available then remove current from the list.
            var mappedEditions = new List<EditionEntity>();
            foreach (var edition in editions)
            {
                var editionEntity = Mapper.Map<Edition, EditionEntity>(edition);
                var nextEdition = GetNextEdition(editionEntity);
                if (nextEdition == null)
                {
                    mappedEditions.Add(editionEntity);
                }
            }

            return mappedEditions;
        }

        public IList<EditionEntity> GetEditionsByNextEdition(IList<int> checkDays, int? lastXDays, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true)
                .Join(_unitOfWork.EditionRepository.GetManyQueryable(x => true),
                    eNext => eNext.EventId,
                    eCurrent => eCurrent.EventId,
                    (eCurrent, eNext) => new { eCurrent, eNext });

            if (lastXDays > 0)
            {
                var emailSendingStartDate = DateTime.Today.AddDays(-lastXDays.Value);
                var emailSendingEndDate = DateTime.Today;
                query = query.Where(x =>
                    (x.eCurrent.StartDate >= emailSendingStartDate && x.eCurrent.StartDate <= emailSendingEndDate)
                    || (x.eCurrent.EndDate >= emailSendingStartDate && x.eCurrent.EndDate <= emailSendingEndDate));
            }
            else
            {
                var now = DateTime.Now.Date;
                var dates = checkDays.Select(day => now.AddDays(day)).Select(dummy => (DateTime?)dummy).ToList();
                query = query.Where(x => dates.Contains(x.eCurrent.StartDate));
            }

            query = query.Where(result =>
                result.eNext.StartDate > result.eCurrent.StartDate
                && (eventId == null || result.eCurrent.EventId == eventId));

            if (minFinancialYear != null)
                query = query.Where(x => x.eNext.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.eNext.Status));
            }

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.eNext.Event.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Length > 0)
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.eNext.EventActivity.ToLower()));
            }

            var editions = query.Select(result => result.eNext).ToList();

            // PATCH:
            editions = editions.OrderBy(x => x.StartDate).GroupBy(x => x.EventId).Select(y => y.First()).ToList();            

            var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);
            return mappedEditions;
        }

        public IList<EditionEntity> GetEditionsByNextEdition(int checkDay, int? lastXDays, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true)
                .Join(_unitOfWork.EditionRepository.GetManyQueryable(x => true),
                    eNext => eNext.EventId,
                    eCurrent => eCurrent.EventId,
                    (eCurrent, eNext) => new { eCurrent, eNext });

            if (lastXDays > 0)
            {
                var emailSendingStartDate = DateTime.Today.AddDays(-lastXDays.Value);
                var emailSendingEndDate = DateTime.Today;
                query = query.Where(x =>
                    (x.eCurrent.StartDate >= emailSendingStartDate && x.eCurrent.StartDate <= emailSendingEndDate)
                    || (x.eCurrent.EndDate >= emailSendingStartDate && x.eCurrent.EndDate <= emailSendingEndDate));
            }
            else
            {
                var now = DateTime.Now.Date;
                var date = now.AddDays(checkDay);
                query = query.Where(x => x.eCurrent.StartDate == date);
            }

            query = query.Where(result =>
                result.eNext.StartDate > result.eCurrent.StartDate
                && (eventId == null || result.eCurrent.EventId == eventId));

            if (minFinancialYear != null)
                query = query.Where(x => x.eNext.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.eNext.Status));
            }

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.eNext.Event.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Length > 0)
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.eNext.EventActivity.ToLower()));
            }

            var editions = query.Select(result => result.eNext).ToList();

            // PATCH:
            //editions = editions.OrderBy(x => x.StartDate).GroupBy(x => x.EventId).Select(y => y.First()).ToList(); // Why is this?!!
            // TODO: Test this!
            editions = editions.Distinct().ToList();

            var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);
            return mappedEditions;
        }

        //public IList<EditionEntity> GetEditionsByCheckDays(IEnumerable<int> checkDays, int? lastXDays, bool after, int? minFinancialYear = null,
        //    EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        //{
        //    var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true);

        //    if (lastXDays > 0)
        //    {
        //        var emailSendingStartDate = DateTime.Today.AddDays(-lastXDays.Value);
        //        var emailSendingEndDate = DateTime.Today;
        //        query = query.Where(x => (x.StartDate >= emailSendingStartDate && x.StartDate <= emailSendingEndDate)
        //                                 || (x.EndDate >= emailSendingStartDate && x.EndDate <= emailSendingEndDate));
        //    }
        //    else
        //    {
        //        var now = DateTime.Now.Date;
        //        query = after
        //            ? query.Where(x => checkDays.Contains(DbFunctions.DiffDays(DbFunctions.TruncateTime(x.EndDate), now).Value))
        //            : query.Where(x => checkDays.Contains(DbFunctions.DiffDays(now, DbFunctions.TruncateTime(x.StartDate)).Value));
        //    }

        //    if (minFinancialYear != null)
        //        query = query.Where(x => x.FinancialYearStart >= minFinancialYear);

        //    if (statuses != null && statuses.Length > 0)
        //    {
        //        var statusIds = statuses.Select(x => x.GetHashCode());
        //        query = query.Where(x => statusIds.Contains(x.Status));
        //    }

        //    if (eventTypes != null && eventTypes.Length > 0)
        //    {
        //        eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
        //        query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
        //    }

        //    if (eventActivities != null && eventActivities.Length > 0)
        //    {
        //        eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
        //        query = query.Where(x => eventActivities.Contains(x.EventActivity.ToLower()));
        //    }

        //    if (eventId > 0)
        //    {
        //        query = query.Where(x => x.EventId == eventId);
        //    }

        //    var editions = query.ToList();
        //    var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);

        //    return mappedEditions;
        //}

        public IList<EditionEntity> GetEditionsByCheckDays(IEnumerable<int> checkDays, int? lastXDays, bool after, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            var allEditions = new List<EditionEntity>();
            foreach (var checkDay in checkDays)
            {
                var editions = GetEditionsByCheckDays(checkDay, lastXDays, after, minFinancialYear, statuses, eventTypes, eventActivities, eventId);
                allEditions.AddRange(editions);
            }

            return allEditions;
        }

        public IList<EditionEntity> GetEditionsByCheckDays(int checkDay, int? lastXDays, bool after, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true);

            if (lastXDays > 0)
            {
                var emailSendingStartDate = DateTime.Today.AddDays(-lastXDays.Value);
                var emailSendingEndDate = DateTime.Today;
                query = query.Where(x => (x.StartDate >= emailSendingStartDate && x.StartDate <= emailSendingEndDate)
                                         || (x.EndDate >= emailSendingStartDate && x.EndDate <= emailSendingEndDate));
            }
            else
            {
                var now = DateTime.Now.Date;
                query = after
                    ? query.Where(x => DbFunctions.DiffDays(DbFunctions.TruncateTime(x.EndDate), now).Value == checkDay)
                    : query.Where(x => DbFunctions.DiffDays(now, DbFunctions.TruncateTime(x.StartDate)).Value == checkDay);
            }

            if (minFinancialYear != null)
                query = query.Where(x => x.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.Status));
            }

            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Length > 0)
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.EventActivity.ToLower()));
            }

            if (eventId > 0)
            {
                query = query.Where(x => x.EventId == eventId);
            }

            var editions = query.ToList();
            var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);

            return mappedEditions;
        }

        public IList<string> GetCities(string searchTerm, string countryCode = null)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => true);

            if (!string.IsNullOrWhiteSpace(countryCode))
                query = query.Where(x => x.CountryCode.ToLower() == countryCode);

            query = query.Where(x => x.City.ToLower().Contains(searchTerm.ToLower()));

            var cities = query.Select(x => x.City).Distinct().OrderBy(x => x).ToList();
            return cities;
        }

        #endregion

        public IList<EditionEntity> GetEditionsByEvent(int eventId)
        {
            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => x.EventId == eventId);

            var editions = query.ToList();

            var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);
            return mappedEditions;
        }

        public IList<EditionEntity> GetEditionsByStatus(EditionStatusType[] statuses)
        {
            var statusIds = statuses.Select(x => x.GetHashCode());

            var query = _unitOfWork.EditionRepository.GetManyQueryable(x => statusIds.Contains(x.Status));

            var editions = query.ToList();

            var mappedEditions = Mapper.Map<IList<Edition>, IList<EditionEntity>>(editions);
            return mappedEditions;
        }

        public IList<EditionEntityLight> SearchEditionsAsCohost(int editionId, string searchTerm, int pageSize, int pageNum)
        {
            var edition = GetEditionById(editionId);
            var statusId = EditionStatusType.Published.GetHashCode();

            var query = _unitOfWork.EditionRepository.GetManyQueryableProjected<EditionEntityLight>(x =>
                x.EditionName.ToLower().Contains(searchTerm.ToLower())
                && x.Country == edition.Country
                && x.City == edition.City
                && x.Status == statusId
            );

            var numberOfDays = WebConfigHelper.CohostEditionsAcceptanceNumberOfDays;

            query = query.Where(x => DbFunctions.DiffDays(edition.StartDate, x.StartDate) <= numberOfDays);
            query = query.Where(x => DbFunctions.DiffDays(edition.StartDate, x.StartDate) >= -numberOfDays);
            query = query.Where(x => DbFunctions.DiffDays(x.EndDate, edition.EndDate) <= numberOfDays);
            query = query.Where(x => DbFunctions.DiffDays(x.EndDate, edition.EndDate) >= -numberOfDays);

            query = query.OrderBy(x => x.EditionName);
            query = query.Skip(pageSize * (pageNum - 1));
            query = query.Take(pageSize);

            var editions = query.ToList();
            return editions;
        }

        public IList<Wb365ApiEventEntity> GetEditionsForWb365()
        {
            var now = DateTime.Now.Date;
            var normalCode = TradeShowConnectDisplay.Normal.GetHashCode();
            var tcDisplayCode = TradeShowConnectDisplay.TcDisplay.GetHashCode();

            var query = _unitOfWork.EditionTranslationRepository.GetManyQueryable(x =>
                (x.Edition.TradeShowConnectDisplay == normalCode &&
                 DbFunctions.DiffDays(DbFunctions.TruncateTime(x.Edition.EndDate), now).Value <= 1)
                ||
                (x.Edition.TradeShowConnectDisplay == tcDisplayCode &&
                 DbFunctions.DiffMonths(DbFunctions.TruncateTime(x.Edition.EndDate), now).Value <= 6)
            );

            var editionTranslations = query.ToList();
            var result = Mapper.Map<IList<EditionTranslation>, IList<Wb365ApiEventEntity>>(editionTranslations);
            return result;
        }

        public IList<WisentApiEventEntity> GetEditionsForWisent(string eventName, short? financialYearStart, int? count)
        {
            const int financialYearStartMin = 2016;
            var eventTypeCodes = new List<string>
            {
                EventType.Conference.GetDescription().ToLower(),
                EventType.ConferenceExhibition.GetDescription().ToLower(),
                EventType.Exhibition.GetDescription().ToLower()
            };
            var eventBusinessClassification = EventBusinessClassification.Event.GetDescription().ToLower();

            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.FinancialYearStart >= financialYearStartMin
                && eventTypeCodes.Contains(x.Event.EventTypeCode.ToLower())
                && x.Event.EventBusinessClassification.ToLower() == eventBusinessClassification
                && x.Event.Industry.ToLower() != "NON-SHOW SPECIFIC".ToLower()
            );

            if (!string.IsNullOrEmpty(eventName))
            {
                eventName = eventName.ToLower();
                query = query.Where(x =>
                    x.InternationalName.ToLower().Contains(eventName)
                    || x.EditionName.ToLower().Contains(eventName)
                    || x.LocalName.ToLower().Contains(eventName)
                    || x.ReportingName.ToLower().Contains(eventName));
            }

            query = financialYearStart.HasValue
                ? query.Where(x => x.FinancialYearStart == financialYearStart)
                : query.Where(x => x.FinancialYearStart >= financialYearStartMin);

            query = query.OrderByDescending(x => x.StartDate);

            if (count.HasValue)
                query = query.Take(count.Value);

            var editions = query.ToList();
            var result = Mapper.Map<IList<Edition>, IList<WisentApiEventEntity>>(editions);
            return result;
        }

        public EditionStartDiff GetEditionStartDateDiff(int axId)
        {
            var editions = _unitOfWork.EditionRepository
                .GetManyQueryable(x => x.AxEventId == axId)
                .Select(x => new EditionStartDiff
                {
                    AxId = x.AxEventId,
                    WeekDiff = x.StartWeekOfYearDiff,
                    DayDiff = x.StartDayOfYearDiff
                });

            if (editions.Any())
                return editions.First();
            return null;
        }

        public int CreateEdition(EditionEntity editionEntity, int userId)
        {
            var edition = Mapper.Map<EditionEntity, Edition>(editionEntity);
            edition.CreateTime = DateTime.Now;
            edition.CreateUser = userId;

            _unitOfWork.EditionRepository.Insert(edition);
            _unitOfWork.Save();
            return edition.EditionId;
        }

        public int CloneEdition(int editionId, int userId)
        {
            using (var scope = new TransactionScope())
            {
                var sourceEdition = GetEditionById(editionId);

                if (sourceEdition != null)
                {
                    //var editionWithTheSameAxEventId = GetEditionByAxId(sourceEdition.AxEventId);
                    //if (editionWithTheSameAxEventId != null)
                    //    throw new Exception(
                    //        $"An edition with the same AxEventId {sourceEdition.AxEventId} already exists. That means you have another Draft edition belonging to this event.");

                    var targetEdition = (EditionEntity)sourceEdition.Clone();
                    targetEdition.EditionName = "Copy of " + sourceEdition.EditionName;
                    targetEdition.InternationalName = "Copy of " + sourceEdition.InternationalName;
                    targetEdition.LocalName = "Copy of " + sourceEdition.LocalName;
                    targetEdition.AxEventId = -1 * sourceEdition.AxEventId;
                    targetEdition.CohostedEvent = false;
                    targetEdition.CohostedEventCount = 0;
                    targetEdition.DwEventId = 0;
                    targetEdition.ReportingName = null;

                    EditionServiceHelper.ResetEdition(targetEdition);

                    var newEditionId = CreateEdition(targetEdition, userId);

                    var newEdition = GetEditionById(newEditionId);

                    if (newEditionId > 0)
                    {
                        foreach (var sourceEditionTranslation in sourceEdition.EditionTranslations)
                        {
                            var targetEditionTranslation = (EditionTranslationEntity)sourceEditionTranslation.Clone();
                            targetEditionTranslation.EditionTranslationId = 0;
                            targetEditionTranslation.EditionId = newEditionId;
                            targetEditionTranslation.UpdateTime = null;
                            targetEditionTranslation.UpdateUser = 0;

                            EditionServiceHelper.CopyEditionImages(newEdition.EditionId, newEdition.EditionName, sourceEditionTranslation, targetEditionTranslation);

                            _editionServiceHelper.CopySocialMediaAccounts(sourceEdition.EditionId, sourceEditionTranslation.LanguageCode, targetEditionTranslation);

                            _editionTranslationServices.CreateEditionTranslation(targetEditionTranslation, userId);
                        }
                        scope.Complete();
                        return newEditionId;
                    }
                }
            }
            return -1;
        }

        public bool UpdateEdition(int editionId, EditionEntity updatedEditionEntity, int userId, bool? autoIntegration = false)
        {
            var success = false;
            if (updatedEditionEntity != null)
            {
                var currentEdition = _unitOfWork.EditionRepository.GetById(editionId);
                if (currentEdition != null)
                {
                    // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                    Mapper.Map(updatedEditionEntity, currentEdition);

                    if (autoIntegration == false)
                    {
                        currentEdition.UpdateTime = updatedEditionEntity.UpdateTime = DateTime.Now;
                        currentEdition.UpdateUser = updatedEditionEntity.UpdateUser = userId;
                    }
                    else
                    {
                        currentEdition.UpdateTimeByAutoIntegration = updatedEditionEntity.UpdateTimeByAutoIntegration = DateTime.Now;
                    }

                    _unitOfWork.EditionRepository.Update(currentEdition);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool UpdateEditionGeneralInfo(int editionId, EditionEntity updatedEditionEntity, int editionTranslationId,
            EditionTranslationEntity updatedEditionTranslationEntity, IList<EditionTranslationSocialMediaEntity> socialMediaEntities,
            int userId, bool? autoIntegration = false)
        {
            var success = false;
            if (updatedEditionEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    // Get Edition
                    var currentEdition = _unitOfWork.EditionRepository.GetById(editionId);
                    if (currentEdition != null)
                    {
                        // Map Edition
                        // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                        //Mapper.Map(updatedEditionEntity, currentEdition);
                        currentEdition.EditionName = updatedEditionEntity.EditionName;
                        currentEdition.LocalName = updatedEditionEntity.LocalName;
                        currentEdition.InternationalName = updatedEditionEntity.InternationalName;
                        currentEdition.EditionNo = updatedEditionEntity.EditionNo;
                        currentEdition.ManagingOfficeEmail = updatedEditionEntity.ManagingOfficeEmail;
                        currentEdition.ManagingOfficePhone = updatedEditionEntity.ManagingOfficePhone;
                        currentEdition.EventWebSite = updatedEditionEntity.EventWebSite;
                        currentEdition.MarketoPreferenceCenterLink = updatedEditionEntity.MarketoPreferenceCenterLink;
                        currentEdition.VenueCoordinates = updatedEditionEntity.VenueCoordinates;
                        currentEdition.CountryLocalName = updatedEditionEntity.CountryLocalName;
                        currentEdition.CityLocalName = updatedEditionEntity.CityLocalName;
                        currentEdition.StartDate = updatedEditionEntity.StartDate;
                        currentEdition.EndDate = updatedEditionEntity.EndDate;
                        currentEdition.VisitStartTime = updatedEditionEntity.VisitStartTime.GetValueOrDefault();
                        currentEdition.VisitEndTime = updatedEditionEntity.VisitEndTime.GetValueOrDefault();
                        currentEdition.CoolOffPeriodStartDate = updatedEditionEntity.CoolOffPeriodStartDate;
                        currentEdition.CoolOffPeriodEndDate = updatedEditionEntity.CoolOffPeriodEndDate;
                        currentEdition.InternationalDate = updatedEditionEntity.InternationalDate;
                        currentEdition.LocalDate = updatedEditionEntity.LocalDate;
                        currentEdition.AllDayEvent = updatedEditionEntity.AllDayEvent;
                        currentEdition.CoHostedEvent = updatedEditionEntity.CohostedEvent;
                        currentEdition.CoHostedEventCount = updatedEditionEntity.CohostedEventCount;
                        currentEdition.Promoted = updatedEditionEntity.Promoted;
                        currentEdition.TradeShowConnectDisplay = updatedEditionEntity.TradeShowConnectDisplay;
                        currentEdition.DisplayOnIteGermany = updatedEditionEntity.DisplayOnIteGermany;
                        currentEdition.DisplayOnIteAsia = updatedEditionEntity.DisplayOnIteAsia;
                        currentEdition.DisplayOnIteI = updatedEditionEntity.DisplayOnIteI;
                        currentEdition.DisplayOnIteModa = updatedEditionEntity.DisplayOnIteModa;
                        currentEdition.DisplayOnItePoland = updatedEditionEntity.DisplayOnItePoland;
                        currentEdition.DisplayOnIteTurkey = updatedEditionEntity.DisplayOnIteTurkey;
                        currentEdition.DisplayOnIteRussia = updatedEditionEntity.DisplayOnIteRussia;
                        currentEdition.DisplayOnIteEurasia = updatedEditionEntity.DisplayOnIteEurasia;
                        currentEdition.DisplayOnTradeLink = updatedEditionEntity.DisplayOnTradeLink;
                        currentEdition.DisplayOnIteUkraine = updatedEditionEntity.DisplayOnIteUkraine;
                        currentEdition.DisplayOnIteBuildInteriors = updatedEditionEntity.DisplayOnIteBuildInteriors;
                        currentEdition.DisplayOnIteFoodDrink = updatedEditionEntity.DisplayOnIteFoodDrink;
                        currentEdition.DisplayOnIteOilGas = updatedEditionEntity.DisplayOnIteOilGas;
                        currentEdition.DisplayOnIteTravelTourism = updatedEditionEntity.DisplayOnIteTravelTourism;
                        currentEdition.DisplayOnIteTransportLogistics = updatedEditionEntity.DisplayOnIteTransportLogistics;
                        currentEdition.DisplayOnIteFashion = updatedEditionEntity.DisplayOnIteFashion;
                        currentEdition.DisplayOnIteSecurity = updatedEditionEntity.DisplayOnIteSecurity;
                        currentEdition.DisplayOnIteBeauty = updatedEditionEntity.DisplayOnIteBeauty;
                        currentEdition.DisplayOnIteHealthCare = updatedEditionEntity.DisplayOnIteHealthCare;
                        currentEdition.DisplayOnIteMining = updatedEditionEntity.DisplayOnIteMining;
                        currentEdition.DisplayOnIteEngineeringIndustrial = updatedEditionEntity.DisplayOnIteEngineeringIndustrial;
                        currentEdition.HiddenFromWebSites = updatedEditionEntity.HiddenFromWebSites;
                        currentEdition.EventFlagPictureFileName = updatedEditionEntity.EventFlagPictureFileName;
                        currentEdition.Status = (byte) updatedEditionEntity.Status;
                        currentEdition.StatusUpdateTime = updatedEditionEntity.StatusUpdateTime;

                        // Update Edition
                        currentEdition.EditionName = updatedEditionEntity.EditionName;

                        if (autoIntegration == false)
                        {
                            currentEdition.UpdateTime = updatedEditionEntity.UpdateTime = DateTime.Now;
                            currentEdition.UpdateUser = updatedEditionEntity.UpdateUser = userId;
                        }
                        else
                        {
                            currentEdition.UpdateTimeByAutoIntegration = updatedEditionEntity.UpdateTimeByAutoIntegration = DateTime.Now;
                        }

                        _unitOfWork.EditionRepository.Update(currentEdition);

                        // Get EditionTranslation
                        var currentEditionTranslation = _unitOfWork.EditionTranslationRepository.GetById(editionTranslationId);
                        if (currentEditionTranslation == null)
                            currentEditionTranslation = new EditionTranslation();

                        // Map EditionTranslation
                        // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                        //Mapper.Map(updatedEditionTranslationEntity, editionTranslation);
                        currentEditionTranslation.EditionId = updatedEditionTranslationEntity.EditionId;
                        currentEditionTranslation.LanguageCode = updatedEditionTranslationEntity.LanguageCode;
                        //currentEditionTranslation.EditionName = updatedEditionTranslationEntity.EditionName;
                        currentEditionTranslation.VenueName = updatedEditionTranslationEntity.VenueName;
                        //currentEditionTranslation.MapVenueName = updatedEditionTranslationEntity.MapVenueName;
                        currentEditionTranslation.MapVenueFullAddress = updatedEditionTranslationEntity.MapVenueFullAddress;
                        currentEditionTranslation.Summary = updatedEditionTranslationEntity.Summary;
                        currentEditionTranslation.Description = updatedEditionTranslationEntity.Description;
                        currentEditionTranslation.ExhibitorProfile = updatedEditionTranslationEntity.ExhibitorProfile;
                        currentEditionTranslation.VisitorProfile = updatedEditionTranslationEntity.VisitorProfile;
                        currentEditionTranslation.BookStandUrl = updatedEditionTranslationEntity.BookStandUrl;
                        currentEditionTranslation.OnlineInvitationUrl = updatedEditionTranslationEntity.OnlineInvitationUrl;

                        // Add or Update EditionTranslation
                        if (updatedEditionTranslationEntity.EditionTranslationId > 0)
                        {
                            if (autoIntegration == false)
                            {
                                currentEditionTranslation.UpdateTime = DateTime.Now;
                                currentEditionTranslation.UpdateUser = userId;
                            }
                            else
                            {
                                currentEditionTranslation.UpdateTimeByAutoIntegration = DateTime.Now;
                            }
                            _unitOfWork.EditionTranslationRepository.Update(currentEditionTranslation);
                        }
                        else
                        {
                            currentEditionTranslation.CreateTime = DateTime.Now;
                            _unitOfWork.EditionTranslationRepository.Insert(currentEditionTranslation);
                        }

                        // Update EditionTranslationSocialMedia
                        if (socialMediaEntities != null)
                        {
                            foreach (var socialMediaEntity in socialMediaEntities)
                            {
                                var currentSocialMedias = _unitOfWork.EditionTranslationSocialMediaRepository.GetManyQueryable(x =>
                                    x.EditionTranslationId == editionTranslationId &&
                                    x.SocialMediaId == socialMediaEntity.SocialMediaId).ToList();

                                if (currentSocialMedias.Any())
                                {
                                    currentSocialMedias.First().AccountName = socialMediaEntity.AccountName;
                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(socialMediaEntity.AccountName))
                                    {
                                        // Create a new one
                                        var newSocialMedia = new EditionTranslationSocialMedia
                                        {
                                            EditionTranslationId = editionTranslationId,
                                            EditionId = editionId,
                                            SocialMediaId = socialMediaEntity.SocialMediaId,
                                            AccountName = socialMediaEntity.AccountName,
                                            CreatedOn = DateTime.Now,
                                            CreatedBy = userId
                                        };
                                        _unitOfWork.EditionTranslationSocialMediaRepository.Insert(newSocialMedia);
                                    }
                                }
                            }
                        }
                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        public bool UpdateEditionSalesMetrics(int editionId, EditionEntity updatedEditionEntity, int userId,
            bool? autoIntegration = false)
        {
            var success = false;
            if (updatedEditionEntity != null)
            {
                // Get Edition
                var currentEdition = _unitOfWork.EditionRepository.GetById(editionId);
                if (currentEdition != null)
                {
                    // Map Edition
                    // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                    //Mapper.Map(updatedEditionEntity, currentEdition);
                    //currentEdition.LocalSqmSold = updatedEditionEntity.LocalSqmSold;
                    //currentEdition.InternationalSqmSold = updatedEditionEntity.InternationalSqmSold;
                    currentEdition.SqmSold = updatedEditionEntity.SqmSold;
                    currentEdition.SponsorCount = updatedEditionEntity.SponsorCount;

                    // Update Edition
                    if (autoIntegration == false)
                    {
                        currentEdition.UpdateTime = updatedEditionEntity.UpdateTime = DateTime.Now;
                        currentEdition.UpdateUser = updatedEditionEntity.UpdateUser = userId;
                    }
                    else
                    {
                        currentEdition.UpdateTimeByAutoIntegration = updatedEditionEntity.UpdateTimeByAutoIntegration = DateTime.Now;
                    }

                    _unitOfWork.EditionRepository.Update(currentEdition);

                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool UpdateEditionExhibitorVisitorStats(int editionId, EditionEntity updatedEditionEntity,
            EditionTranslationEntity updatedEditionTranslationEntity,
            int userId, bool? autoIntegration = false)
        {
            var success = false;
            if (updatedEditionEntity != null)
            {
                using (var scope = new TransactionScope())
                {
                    // Get Edition
                    var currentEdition = _unitOfWork.EditionRepository.GetById(editionId);
                    if (currentEdition != null)
                    {
                        // Map Edition
                        // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                        //Mapper.Map(updatedEditionEntity, currentEdition);
                        //currentEdition.LocalExhibitorCount = updatedEditionEntity.LocalExhibitorCount;
                        //currentEdition.InternationalExhibitorCount = updatedEditionEntity.InternationalExhibitorCount;
                        currentEdition.ExhibitorCount = updatedEditionEntity.ExhibitorCount;
                        currentEdition.ExhibitorCountryCount = updatedEditionEntity.ExhibitorCountryCount;
                        currentEdition.NationalGroupCount = updatedEditionEntity.NationalGroupCount;
                        //currentEdition.LocalExhibitorRetentionRate = updatedEditionEntity.LocalExhibitorRetentionRate;
                        //currentEdition.InternationalExhibitorRetentionRate = updatedEditionEntity.InternationalExhibitorRetentionRate;
                        currentEdition.ExhibitorRetentionRate = updatedEditionEntity.ExhibitorRetentionRate;
                        currentEdition.LocalVisitorCount = updatedEditionEntity.LocalVisitorCount;
                        currentEdition.InternationalVisitorCount = updatedEditionEntity.InternationalVisitorCount;
                        //currentEdition.LocalRepeatVisitCount = updatedEditionEntity.LocalRepeatVisitCount;
                        //currentEdition.InternationalRepeatVisitCount = updatedEditionEntity.InternationalRepeatVisitCount;
                        currentEdition.RepeatVisitCount = updatedEditionEntity.RepeatVisitCount;
                        currentEdition.VisitorCountryCount = updatedEditionEntity.VisitorCountryCount;
                        currentEdition.OnlineRegistrationCount = updatedEditionEntity.OnlineRegistrationCount;
                        currentEdition.OnlineRegisteredVisitorCount = updatedEditionEntity.OnlineRegisteredVisitorCount;
                        currentEdition.OnlineRegisteredBuyerVisitorCount = updatedEditionEntity.OnlineRegisteredBuyerVisitorCount;
                        currentEdition.LocalDelegateCount = updatedEditionEntity.LocalDelegateCount;
                        currentEdition.InternationalDelegateCount = updatedEditionEntity.InternationalDelegateCount;
                        currentEdition.LocalPaidDelegateCount = updatedEditionEntity.LocalPaidDelegateCount;
                        currentEdition.InternationalPaidDelegateCount = updatedEditionEntity.InternationalPaidDelegateCount;
                        currentEdition.CoHostedEvent = updatedEditionEntity.CohostedEvent;
                        currentEdition.CoHostedEventCount = updatedEditionEntity.CohostedEventCount;

                        // Update Edition
                        if (autoIntegration == false)
                        {
                            currentEdition.UpdateTime = updatedEditionEntity.UpdateTime = DateTime.Now;
                            currentEdition.UpdateUser = updatedEditionEntity.UpdateUser = userId;
                        }
                        else
                        {
                            currentEdition.UpdateTimeByAutoIntegration = updatedEditionEntity.UpdateTimeByAutoIntegration = DateTime.Now;
                        }

                        _unitOfWork.EditionRepository.Update(currentEdition);

                        // Add or Update EditionTranslation
                        if (updatedEditionTranslationEntity.EditionTranslationId == 0)
                        {
                            var currentEditionTranslation = new EditionTranslation();
                            Mapper.Map(updatedEditionTranslationEntity, currentEditionTranslation);
                            currentEditionTranslation.CreateTime = DateTime.Now;
                            _unitOfWork.EditionTranslationRepository.Insert(currentEditionTranslation);
                        }

                        // Update (Add-Delete) EditionCountry
                        UpdateEditionCountries(editionId, updatedEditionEntity.TopExhibitorCountries, BusinessEntities.EditionCountryRelationType.Exhibitor, userId);
                        UpdateEditionCountries(editionId, updatedEditionEntity.TopVisitorCountries, BusinessEntities.EditionCountryRelationType.Visitor, userId);
                        UpdateEditionCountries(editionId, updatedEditionEntity.DelegateCountries, BusinessEntities.EditionCountryRelationType.Delegate, userId);

                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }

        private void UpdateEditionCountries(int editionId, string countryCodes, BusinessEntities.EditionCountryRelationType relationType,
            int userId, bool? autoIntegration = false)
        {
            // TODO: Edition vb de geliyor mu?
            var currentEditionCountries = _unitOfWork.EditionCountryRepository.GetManyQueryable(
                x => x.EditionId == editionId && x.RelationType == (byte)relationType);

            if (countryCodes != null && countryCodes.Any())
            {
                var countryCodeArray = countryCodes.Split(',');

                // Delete old ones
                foreach (var currentEditionCountry in currentEditionCountries)
                {
                    if (!countryCodeArray.Contains(currentEditionCountry.CountryCode))
                    {
                        _unitOfWork.EditionCountryRepository.Delete(currentEditionCountry);
                    }
                }

                // Add new ones
                foreach (var countryCode in countryCodeArray)
                {
                    var relationTypeId = relationType.GetHashCode();
                    var existingEditionCountry = _unitOfWork.EditionCountryRepository.GetManyQueryable(
                        x => x.EditionId == editionId && x.CountryCode == countryCode && x.RelationType == relationTypeId);
                    if (existingEditionCountry == null || !existingEditionCountry.Any())
                    {
                        var editionCountry = new EditionCountry
                        {
                            EditionId = editionId,
                            CountryCode = countryCode,
                            RelationType = (byte)relationType,
                            CreatedBy = userId,
                            CreatedOn = DateTime.Now
                        };
                        _unitOfWork.EditionCountryRepository.Insert(editionCountry);
                    }
                }
            }
            else
            {
                // Delete all existing EditionCountry records   
                foreach (var currentEditionCountry in currentEditionCountries)
                {
                    _unitOfWork.EditionCountryRepository.Delete(currentEditionCountry);
                }
            }
        }

        public bool UpdateEditionPostShowMetrics(int editionId, EditionEntity updatedEditionEntity, int userId,
            bool? autoIntegration = false)
        {
            var success = false;
            if (updatedEditionEntity != null)
            {
                // Get Edition
                var currentEdition = _unitOfWork.EditionRepository.GetById(editionId);
                if (currentEdition != null)
                {
                    // Map Edition
                    // TODO: Mapping overrides the values of the properties which are unintended to be mapped.
                    //Mapper.Map(updatedEditionEntity, currentEdition);
                    currentEdition.NPSScoreVisitor = updatedEditionEntity.NPSScoreVisitor;
                    currentEdition.NPSScoreExhibitor = updatedEditionEntity.NPSScoreExhibitor;
                    currentEdition.NPSSatisfactionVisitor = updatedEditionEntity.NPSSatisfactionVisitor;
                    currentEdition.NPSSatisfactionExhibitor = updatedEditionEntity.NPSSatisfactionExhibitor;
                    //currentEdition.NPSAverageVisitor = updatedEditionEntity.NPSAverageVisitor;
                    //currentEdition.NPSAverageExhibitor = updatedEditionEntity.NPSAverageExhibitor;
                    currentEdition.NetEasyScoreVisitor = updatedEditionEntity.NetEasyScoreVisitor;
                    currentEdition.NetEasyScoreExhibitor = updatedEditionEntity.NetEasyScoreExhibitor;

                    // Update Edition
                    if (autoIntegration == false)
                    {
                        currentEdition.UpdateTime = updatedEditionEntity.UpdateTime = DateTime.Now;
                        currentEdition.UpdateUser = updatedEditionEntity.UpdateUser = userId;
                    }
                    else
                    {
                        currentEdition.UpdateTimeByAutoIntegration = updatedEditionEntity.UpdateTimeByAutoIntegration = DateTime.Now;
                    }

                    _unitOfWork.EditionRepository.Update(currentEdition);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        public bool DeleteEdition(int editionId)
        {
            if (editionId > 0)
            {
                using (var scope = new TransactionScope())
                {
                    var exists = _unitOfWork.EditionRepository.Exists(editionId);
                    if (exists)
                    {
                        var socialMedias = _editionTranslationSocialMediaServices.GetByEdition(editionId);
                        foreach (var socialMedia in socialMedias)
                        {
                            _editionTranslationSocialMediaServices.Delete(socialMedia.EditionTranslationSocialMediaId);
                        }

                        var notifications = _notificationServices.GetNotificationsByEdition(editionId);
                        foreach (var notification in notifications)
                        {
                            _notificationServices.DeleteNotification(notification.NotificationId);
                        }

                        var editionTranslations = _editionTranslationServices.GetEditionTranslationsByEdition(editionId);
                        foreach (var editionTranslation in editionTranslations)
                        {
                            _unitOfWork.EditionTranslationRepository.Delete(editionTranslation.EditionTranslationId);
                        }

                        var editionCohosts = _editionCohostServices.GetEditionCohosts(editionId);
                        foreach (var editionCohost in editionCohosts)
                        {
                            _unitOfWork.EditionCohostRepository.Delete(editionCohost.EditionCohostId);
                        }

                        _unitOfWork.EditionRepository.Delete(editionId);
                        _unitOfWork.Save();

                        scope.Complete();
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        // EXTRAS
        public bool RequiresNotification(EditionEntity edition, NotificationType notificationType)
        {
            var now = DateTime.Now.Date;

            switch (notificationType)
            {
                case NotificationType.EditionExistence:
                    {
                        return true;
                    }
                case NotificationType.GeneralInfoCompleteness: // Checks if Edition has a complete EditionInfo.
                    {
                        var generalInfoCompletedForEdition = EditionServiceHelper.IsEditionInfoCompleted(edition, EditionInfoType.GeneralInfo);
                        var generalInfoCompletedForEditionTranslation = true;
                        if (generalInfoCompletedForEdition)
                        {
                            foreach (var editionTranslation in edition.EditionTranslations)
                            {
                                var completed = _editionTranslationServices.IsEditionTranslationInfoCompleted(editionTranslation, EditionInfoType.GeneralInfo);
                                if (!completed)
                                {
                                    generalInfoCompletedForEditionTranslation = false;
                                    break;
                                }
                            }
                        }
                        return !generalInfoCompletedForEdition || !generalInfoCompletedForEditionTranslation;
                    }
                case NotificationType.PostShowMetricsInfoCompleteness: // Checks if Edition has a complete PostShowMetricsInfo.
                    {
                        var postShowMetricsInfoCompleted = EditionServiceHelper.IsEditionInfoCompleted(edition, EditionInfoType.PostShowMetricsInfo);
                        //var hasFiles = EditionHasAnyFile(edition);
                        if (!postShowMetricsInfoCompleted) // || !hasFiles)
                            return true;
                        return false;
                    }
                case NotificationType.PostShowMetricsInfoCompleteness2: // Checks if Edition has a complete PostShowMetricsInfo.
                    {
                        var postShowMetricsInfoCompleted = EditionServiceHelper.IsEditionInfoCompleted(edition, EditionInfoType.PostShowMetricsInfo);
                        //var hasFiles = EditionHasAnyFile(edition);
                        if (!postShowMetricsInfoCompleted) // || !hasFiles)
                            return true;
                        return false;
                    }
            }
            return false;
        }

        public bool IsEditionNameUnique(int id, string name)
        {
            name = name.ToLower();
            var exists = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.EditionName.ToLower() == name && x.EditionId != id)
                .Any();
            return !exists;
        }

        public bool IsInternationalNameUnique(int id, string name)
        {
            name = name.ToLower();
            var exists = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.InternationalName.ToLower() == name && x.EditionId != id)
                .Any();
            return !exists;
        }

        public bool IsLocalNameUnique(int id, string name)
        {
            name = name.ToLower();
            var exists = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                x.LocalName.ToLower() == name && x.EditionId != id)
                .Any();
            return !exists;
        }

        // KEEP this method for future uses.
        private bool HasAnyFile(EditionEntity edition)
        {
            foreach (EditionFileType fileType in Enum.GetValues(typeof(EditionFileType)))
            {
                var fileCount = _fileServices.GetFileCount(edition.EditionId, EntityType.Edition.GetDescription(), fileType, null);
                if (fileCount == 0)
                    return false;
            }
            return true;
        }

        // EXTRAS
        public bool ClonedEditionAlreadyExists(int eventId)
        {
            var statusId = EditionStatusType.Published.GetHashCode();
            return _unitOfWork.EditionRepository.GetManyQueryable(
                x => x.EventId == eventId
                     && x.Status != statusId).Any();
        }

        #region HELPER METHODS

        private IQueryable<EditionEntityLight> GetEditionsQueryable(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null)
        {
            if (!string.IsNullOrWhiteSpace(directorEmail))
                directorEmail = directorEmail.ToLower();

            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                directorEmail == null ||
                x.Event.EventDirectors.Any(ed =>
                    ed.DirectorEmail.ToLower() == directorEmail.ToLower() &&
                    (!mustBePrimaryDirector || ed.IsPrimary == true)));

            if (minFinancialYear > 0)
                query = query.Where(x => x.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Length > 0)
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.Status));
            }

            // TODO: PATCH#001
            //if (eventTypes != null && eventTypes.Length > 0)
            //{
            //    eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
            //    query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            //}
            if (eventTypes != null && eventTypes.Length > 0)
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Length > 0)
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.EventActivity.ToLower()));
            }

            if (eventId != null)
                query = query.Where(x => x.EventId == eventId.Value);

            var queryable = query
                .Select(result => new EditionEntityLight
                {
                    EditionId = result.EditionId,
                    EditionName = result.EditionName,
                    EventId = result.Event.EventId,
                    EventName = result.Event.MasterName,
                    EditionNo = result.EditionNo,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate,
                    Status = result.Status,
                    DirectorEmail = result.Event.EventDirectors.FirstOrDefault(x =>
                        x.IsPrimary == true && x.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorEmail,
                    DirectorFullName = result.Event.EventDirectors.FirstOrDefault(x =>
                        x.IsPrimary == true && x.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorFullName,
                    EventActivity = result.EventActivity,
                    DirectorEmails = result.Event.EventDirectors.Select(d => d.DirectorEmail.ToLower())
                });
            return queryable;
        }

        private IQueryable<EditionEntityLight> GetEditionsQueryable2(
            string directorEmail,
            int? eventId = null,
            bool mustBePrimaryDirector = false,
            int? minFinancialYear = null,
            EditionStatusType[] statuses = null,
            string[] eventTypes = null,
            string[] eventActivities = null,
            string[] regions = null,
            string country = null,
            string city = null)
        {
            if (!string.IsNullOrWhiteSpace(directorEmail))
                directorEmail = directorEmail.ToLower();

            var query = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                directorEmail == null ||
                x.Event.EventDirectors.Any(ed =>
                    ed.DirectorEmail.ToLower() == directorEmail.ToLower() &&
                    (!mustBePrimaryDirector || ed.IsPrimary == true)));

            if (minFinancialYear != null)
                query = query.Where(x => x.FinancialYearStart >= minFinancialYear);

            if (statuses != null && statuses.Any())
            {
                var statusIds = statuses.Select(x => x.GetHashCode());
                query = query.Where(x => statusIds.Contains(x.Status));
            }

            if (eventTypes != null && eventTypes.Any())
            {
                eventTypes = eventTypes.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventTypes.Contains(x.Event.EventTypeCode.ToLower()));
            }

            if (eventActivities != null && eventActivities.Any())
            {
                eventActivities = eventActivities.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => eventActivities.Contains(x.EventActivity.ToLower()));
            }

            if (eventId != null)
                query = query.Where(x => x.EventId == eventId.Value);

            if (regions != null && regions.Any())
            {
                regions = regions.Select(x => x.ToLower()).ToArray();
                query = query.Where(x => regions.Contains(x.Event.Region.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(country))
                query = query.Where(x => x.CountryCode.ToLower() == country.ToLower());

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(x => x.City.ToLower() == city.ToLower());

            var queryable = query
                .Select(result => new EditionEntityLight
                {
                    EditionId = result.EditionId,
                    EditionName = result.EditionName,
                    EventId = result.Event.EventId,
                    EventName = result.Event.MasterName,
                    EditionNo = result.EditionNo,
                    StartDate = result.StartDate,
                    EndDate = result.EndDate,
                    Status = result.Status,
                    DirectorEmail = result.Event.EventDirectors.FirstOrDefault(x =>
                        x.IsPrimary == true && x.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorEmail,
                    DirectorFullName = result.Event.EventDirectors.FirstOrDefault(x =>
                        x.IsPrimary == true && x.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorFullName,
                    EventActivity = result.EventActivity,
                    DirectorEmails = result.Event.EventDirectors.Select(d => d.DirectorEmail.ToLower())
                });
            return queryable;
        }

        #endregion
    }
}