using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Event;
using Ced.BusinessServices.Helpers;
using Ced.Data.UnitOfWork;
using Ced.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Ced.Utility;
using Event = Ced.Data.Models.Event;

namespace Ced.BusinessServices
{
    public class EventServices : IEventServices
    {
        private readonly UnitOfWork _unitOfWork;

        public EventServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public EventEntity GetEventById(int eventId, string[] eventTypes = null)
        {
            var eventTypeValues = eventTypes != null ? string.Join(",", eventTypes) : null;

            // TODO: PATCH#001
            //if (eventTypes != null)
            //{
            //    var eventTypeValues = string.Join(",", eventTypes);
            //    query = query.Where(x => eventTypeValues.Contains(x.EventTypeCode));
            //}
            var query = _unitOfWork.EventRepository.GetManyQueryable(x =>
                x.EventId == eventId
                && (eventTypeValues == null
                    || x.Editions.Any(ed => eventTypeValues.Contains(ed.EventTypeCode))));

            var @event = query.SingleOrDefault();

            if (@event != null)
            {
                var eventModel = Mapper.Map<Event, EventEntity>(@event);
                return eventModel;
            }
            return null;
        }

        public IList<EventEntity> GetAllEvents(string[] eventTypes = null, int? minFinancialYear = null)
        {
            var query = _unitOfWork.EventRepository
                .GetManyQueryable(x => x.Editions.Any(ed => ed.FinancialYearStart >= minFinancialYear));
            query = FilterEvents(query, eventTypes, minFinancialYear);

            var events = query.ToList();
            if (events.Any())
            {
                var eventsModel = Mapper.Map<List<Event>, List<EventEntity>>(events);
                return eventsModel;
            }
            return new List<EventEntity>();
        }

        public IList<EventEntityLight> SearchEvents(string searchTerm, int pageSize, int pageNum, string email = null, string[] eventTypes = null, int? minFinancialYear = null)
        {
            if (!string.IsNullOrWhiteSpace(email))
                email = email.ToLower();

            var eventTypeValues = eventTypes != null ? string.Join(",", eventTypes) : null;

            var query = _unitOfWork.EventRepository.GetManyQueryableProjected<EventEntityLight>(x =>
                x.MasterName.ToLower().Contains(searchTerm.ToLower())
                && (minFinancialYear == null || x.Editions.Any(ed => ed.FinancialYearStart >= minFinancialYear))
                && (eventTypeValues == null || x.Editions.Any(ed => eventTypeValues.Contains(ed.EventTypeCode)))
                && (email == null || x.EventDirectors.Any(ed => ed.DirectorEmail.ToLower() == email)));

            // PATCH: Ex. GLEE 2018 -> EXH, GLEE 2019 -> NON
            // TODO: This part of code is going to be uncommented later.
            //if (eventTypes != null)
            //{
            //    var eventTypeValues = string.Join(",", eventTypes);
            //    query = query.Where(e => eventTypeValues.Contains(e.EventTypeCode));
            //}

            query = query.OrderBy(x => x.MasterName);
            query = query.Skip(pageSize * (pageNum - 1));
            query = query.Take(pageSize);

            var events = query.ToList();
            return events;
        }

        public IList<EventEditionCustomType> GetEventEditions(int[] editionIds, int? count)
        {
            var langCode = LanguageHelper.GetBaseLanguageCultureName();
            var context = _unitOfWork.EventRepository.Context;

            var events = (from ev in context.Events
                join e in context.Editions on ev.EventId equals e.EventId
                join edt in context.EditionTranslations on e.EditionId equals edt.EditionId
                where
                    editionIds.Contains(e.EditionId)
                    && edt.LanguageCode == langCode
                select new
                {
                    EventId = ev.EventId,
                    MasterName = ev.MasterName,
                    //EditionId = edt.EditionId,
                    //EditionName = edt.EditionName,
                    Logo = edt.WebLogoFileName
                })
                .Take(count ?? int.MaxValue).Distinct()
                .AsEnumerable()
                .Select(x => new EventEditionCustomType
                {
                    EventId = x.EventId,
                    MasterName = x.MasterName,
                    //EditionId = x.EditionId,
                    //EditionName = x.EditionName,
                    Logo = x.Logo
                })
                .ToList();
            return events;
        }

        public IEnumerable<EventEditionCustomType> GetLastViewedEvents(int userId, int? count)
        {
            var context = _unitOfWork.LogRepository.Context;
            //var events = (from l in context.Logs
            //    join e in context.Editions on l.EntityId equals e.EditionId
            //    join ev in context.Events on e.EventId equals ev.EventId
            //    where l.EntityType == "Edition"
            //          && l.ActorUser == userId
            //    select new EventEditionCustomType
            //    {
            //        EventId = ev.EventId,
            //        MasterName = ev.MasterName
            //    }).Take(count ?? 10).Union(
            //        from l in context.Logs
            //        join ev in context.Events on l.EntityId equals ev.EventId
            //        where l.EntityType == "Event"
            //              && l.ActorUser == userId
            //        select new EventEditionCustomType
            //        {
            //            EventId = ev.EventId,
            //            MasterName = ev.MasterName
            //        }
            //    ).Take(count ?? 10);

            //var events2 = (from l in context.Logs
            //              join e in context.Editions on l.EntityId equals e.EditionId
            //              join ev in context.Events on e.EventId equals ev.EventId
            //              where l.EntityType == "Edition"
            //                    && l.ActorUser == userId
            //              select new EventEditionCustomType
            //              {
            //                  EventId = ev.EventId,
            //                  MasterName = ev.MasterName
            //              })
            //              .Take(count ?? 10).Union(
            //        from l in context.Logs
            //        join ev in context.Events on l.EntityId equals ev.EventId
            //        where l.EntityType == "Event"
            //              && l.ActorUser == userId
            //        select new EventEditionCustomType
            //        {
            //            EventId = ev.EventId,
            //            MasterName = ev.MasterName
            //        }
            //    ).Take(count ?? 10);

            //var events = (from l in context.Logs
            //    join e in context.Editions on l.EntityId equals e.EditionId
            //    join ev in context.Events on e.EventId equals ev.EventId
            //    //join et in context.EditionTranslations on e.EditionId equals et.EditionId
            //    where l.EntityType == "Edition"
            //          && l.EntityId > 0
            //          && l.ActorUser == userId
            //          && new[] {"EditionDetails", "EditionEdit"}.Contains(l.ActionType)
            //          //&& et.WebLogoFileName != null
            //    group l by new {ev.EventId, ev.MasterName}
            //    into g
            //    let winner =
            //        (
            //            from groupedItem in g
            //            //from et in context.EditionTranslations
            //            //where g.Key.EventId == et.EditionId
            //            orderby groupedItem.LogId descending
            //            select new EventEditionCustomType
            //            {
            //                EventId = g.Key.EventId,
            //                MasterName = g.Key.MasterName,
            //                //LastVisitDate = g.Max(x => x.CreateTime),
            //                //Logo = g.Max(x => et.WebLogoFileName)
            //                //Logo = et.WebLogoFileName
            //            }
            //            ).FirstOrDefault()
            //    select winner
            //    )
            //    .Union(
            //        from l in context.Logs
            //        join ev in context.Events on l.EntityId equals ev.EventId
            //        //join e in context.Editions on ev.EventId equals e.EventId
            //        //join et in context.EditionTranslations on e.EditionId equals et.EditionId
            //        where l.EntityType == "Event"
            //              && l.EntityId > 0
            //              && l.ActorUser == userId
            //              && new[] {"EventDashboard"}.Contains(l.ActionType)
            //              //&& et.WebLogoFileName != null
            //        group l by new {ev.EventId, ev.MasterName}
            //        into g
            //        let winner =
            //            (
            //                from groupedItem in g
            //                //from et in context.EditionTranslations
            //                //where g.Key.EventId == et.EditionId
            //                orderby groupedItem.LogId descending
            //                select new EventEditionCustomType
            //                {
            //                    EventId = g.Key.EventId,
            //                    MasterName = g.Key.MasterName,
            //                    //LastVisitDate = g.Max(x => x.CreateTime),
            //                    //Logo = g.Max(x => et.WebLogoFileName)
            //                    //Logo = et.WebLogoFileName
            //                }
            //                ).FirstOrDefault()
            //        select winner
            //    );

            var events = (from l in context.Logs
                join ev in context.Events on l.EventId equals ev.EventId
                where
                    l.ActorUserId == userId
                    && l.EventId > 0
                    && l.AdditionalInfo != null && l.AdditionalInfo.Contains("WebLogoFileName") // weblogo sits here
                    && new[] { "Edition", "Event" }.Contains(l.EntityType)
                    && new[] { "EditionDetails", "EditionEdit", "EventDashboard" }.Contains(l.ActionType)
                          group l by new { ev.EventId, ev.MasterName, l.AdditionalInfo } into g
                let winner =
                    (
                        from groupedItem in g
                        orderby groupedItem.LogId descending
                        select new EventEditionCustomType
                        {
                            EventId = g.Key.EventId,
                            MasterName = g.Key.MasterName,
                            Logo = g.Key.AdditionalInfo
                        }
                    ).FirstOrDefault()
                select winner
                )
                .Take(count ?? 10)
                .ToList();

            return events;
        }

        public IEnumerable<EventEntity> GetEventsByDirector(string email, string[] eventTypes = null, int? minFinancialYear = null, int? count = null)
        {
            var query = _unitOfWork.EventRepository.GetManyQueryableProjected<EventEntity>(x =>
                x.EventDirectors.Any(ed => ed.DirectorEmail.ToLower() == email.ToLower()
                                           && ed.ApplicationId == WebConfigHelper.ApplicationIdCed)
                && x.Editions.Any(e => e.FinancialYearStart >= minFinancialYear));

            if (eventTypes != null)
            {
                var eventTypeValues = string.Join(",", eventTypes);
                query = query.Where(x => eventTypeValues.Contains(x.EventTypeCode));
            }

            if (count > 0)
                query = query.Take(count.Value);

            var events = query.ToList();
            return events;
        }

        public IEnumerable<EventEntityLight> GetEventLightsByDirector(string email, string[] eventTypes = null, int? minFinancialYear = null, int? count = null)
        {
            var query = _unitOfWork.EventRepository.GetManyQueryableProjected<EventEntityLight>(x =>
                x.EventDirectors.Any(ed => ed.DirectorEmail.ToLower() == email.ToLower()
                                           && ed.ApplicationId == WebConfigHelper.ApplicationIdCed)
                && x.Editions.Any(e => e.FinancialYearStart >= minFinancialYear));

            if (eventTypes != null)
            {
                var eventTypeValues = string.Join(",", eventTypes);
                query = query.Where(x => eventTypeValues.Contains(x.EventTypeCode));
            }

            if (count > 0)
                query = query.Take(count.Value);

            var events = query.ToList();
            return events;
        }

        public IEnumerable<EventEditionCustomType> GetPastEventsByDirector(string email, int? minFinancialYear = null, EditionStatusType[] statuses = null,
            string[] eventTypes = null, int? theLastXYears = null, int? count = null)
        {
            int[] statusIds = null;
            if (statuses != null && statuses.Length > 0)
                statusIds = statuses.Select(x => x.GetHashCode()).ToArray();
            var statusIdsAvailable = statusIds != null && statusIds.Any();

            string eventTypeValues = null;
            if (eventTypes != null)
                eventTypeValues = string.Join(",", eventTypes);

            var context = _unitOfWork.EventRepository.Context;
            // TODO: Query tamamlanacak ve iyileştirilecek (hızı arttırılacak)
            //var events = (from e in context.Events
            //    join edt in context.Editions on e.EventId equals edt.EventId
            //    join edr in context.EventDirectors on edt.EventId equals edr.EventId
            //    where
            //        edr.DirectorEmail.ToLower() == email.ToLower()
            //        //&& edt.EndDate < DateTime.Now
            //        //&& edt.Year <= DateTime.Now.Year && edt.Year >= DateTime.Now.Year - 1
            //        && (eventTypeValues == null || eventTypeValues.Contains(e.EventType))
            //        && (theLastXYears == null || (DateTime.Now.Year - edt.FinancialYearStart <= theLastXYears))
            //    orderby edt.EndDate
            //    select e).Take(count ?? int.MaxValue).Distinct().ToList();

            var events = (from ev in context.Events
                join ed in context.EventDirectors on ev.EventId equals ed.EventId
                join e in context.Editions on ev.EventId equals e.EventId
                join edt in context.EditionTranslations on e.EditionId equals edt.EditionId
                where
                    ed.ApplicationId == WebConfigHelper.ApplicationIdCed
                    && ed.DirectorEmail.ToLower() == email.ToLower()
                    && (eventTypeValues == null || eventTypeValues.Contains(ev.EventTypeCode))
                    && DateTime.Now > e.EndDate
                    && e.FinancialYearStart >= minFinancialYear
                    && (!statusIdsAvailable || statusIds.Contains(e.Status))
                orderby e.EndDate descending 
                select new
                {
                    EventId = ev.EventId,
                    MasterName = ev.MasterName,
                    EditionId = edt.EditionId,
                    EditionName = e.EditionName,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .Take(count ?? int.MaxValue).Distinct()
                .AsEnumerable()
                .Select(x => new EventEditionCustomType
                {
                    EventId = x.EventId,
                    MasterName = x.MasterName,
                    EditionId = x.EditionId,
                    EditionName = x.EditionName,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate
                })
                .ToList();

            return events;

            //if (events.Any())
            //{
            //    var eventsModel = Mapper.Map<List<Event>, List<EventEntity>>(events);
            //    return eventsModel;
            //}
            //return new List<EventEntity>();
        }

        public EventEntity CreateEvent(EventEntity eventEntity, int userId)
        {
            var @event = Mapper.Map<EventEntity, Event>(eventEntity);
            @event.CreateTime = DateTime.Now;
            @event.CreateUser = userId;

            _unitOfWork.EventRepository.Insert(@event);
            _unitOfWork.Save();

            //return Mapper.Map<Event, EventEntity>(@event);

            eventEntity.EventId = @event.EventId;
            return eventEntity;
        }

        public bool UpdateEvent(int eventId, EventEntity eventEntity, int userId, bool? autoIntegration = false)
        {
            var @event = _unitOfWork.EventRepository.GetById(eventId);

            if (@event != null)
            {
                Mapper.Map(eventEntity, @event);

                if (autoIntegration == false)
                {
                    @event.UpdateTime = DateTime.Now;
                    @event.UpdateUser = userId;
                }
                else
                {
                    @event.UpdateTimeByAutoIntegration = DateTime.Now;
                }

                _unitOfWork.EventRepository.Update(@event);
                _unitOfWork.Save();
            }

            return true;
        }

        public bool DeleteEvent(int eventId, string[] eventTypes = null)
        {
            var success = false;
            if (eventId > 0)
            {
                var @event = GetEventById(eventId, eventTypes);
                if (@event != null)
                {
                    _unitOfWork.EventRepository.Delete(@event);
                    _unitOfWork.Save();

                    success = true;
                }
            }
            return success;
        }

        #region HELPER METHODS

        private static IQueryable<Event> FilterEvents(IQueryable<Event> queryable, string[] eventTypes, int? minFinancialYear)
        {
            var query = queryable;
            if (eventTypes != null)
            {
                var eventTypeValues = string.Join(",", eventTypes);
                query = query.Where(e => eventTypeValues.Contains(e.EventTypeCode));
            }
            if (minFinancialYear.HasValue)
                query = query.Where(e => e.Editions.Any(ed => ed.FinancialYearStart >= minFinancialYear));
            return query;
        }

        #endregion
    }
}