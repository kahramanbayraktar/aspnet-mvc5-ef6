using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IEventDirectorServices
    {
        EventDirectorEntity GetEventDirectorById(int eventDirectorId);

        EventDirectorEntity GetEventDirector(int eventId, string userEmail, int appId);

        int GetEventDirectorsCount(int? eventId, string userEmail, int[] appIds, bool? isPrimary, bool? isAssistant);

        IList<EventDirectorEntity> GetEventDirectors(int? eventId, string userEmail, int[] appIds, bool? isPrimary, bool? isAssistant);

        IList<EventDirectorEntity> GetEventDirectorsByEvent(int eventId, int? appId);

        IList<EventDirectorEntity> GetEventDirectorsByUser(string userEmail, int? appId);

        int CreateEventDirector(EventDirectorEntity eventDirectorEntity, int userId);

        bool UpdateEventDirector(int eventDirectorId, EventDirectorEntity eventDirectorEntity, int userId);

        bool DeleteEventDirector(int eventDirectorId);

        bool IsAuthorized(string userEmail, int eventId, int appId);

        IList<EventDirectorEntity> GetPrimaryDirectors(int eventId, int appId);

        bool IsPrimaryDirector(string userEmail, int? eventId, int appId);

        bool IsAssistantDirector(string userEmail, int? eventId, int appId);

        string GetRecipientEmails(EditionEntity edition);
    }
}