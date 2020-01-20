using System.Collections.Generic;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Event;

namespace Ced.BusinessServices
{
    public interface IEventServices
    {
        EventEntity GetEventById(int eventId, string[] eventTypes = null);

        IList<EventEntity> GetAllEvents(string[] eventTypes = null, int? minFinancialYear = null);

        IList<EventEntityLight> SearchEvents(string searchTerm, int pageSize, int pageNum, string email = null, string[] eventTypes = null, int? minFinancialYear = null);

        IList<EventEditionCustomType> GetEventEditions(int[] editionIds, int? count);

        IEnumerable<EventEditionCustomType> GetLastViewedEvents(int userId, int? count);

        IEnumerable<EventEntity> GetEventsByDirector(string email, string[] eventTypes = null, int? minFinancialYear = null, int? count = null);

        IEnumerable<EventEntityLight> GetEventLightsByDirector(string email, string[] eventTypes = null, int? minFinancialYear = null, int? count = null);

        IEnumerable<EventEditionCustomType> GetPastEventsByDirector(string email, int? minFinancialYear = null, EditionStatusType[] statuses = null,
            string[] eventTypes = null, int? theLastXYears = null, int? count = null);

        EventEntity CreateEvent(EventEntity eventEntity, int userId);

        bool UpdateEvent(int eventId, EventEntity eventEntity, int userId, bool? autoIntegration = false);
        
        bool DeleteEvent(int eventId, string[] eventTypes = null);
    }
}