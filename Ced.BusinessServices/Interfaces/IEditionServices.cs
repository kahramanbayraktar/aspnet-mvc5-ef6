using System.Collections.Generic;
using Ced.BusinessEntities;
using Ced.BusinessEntities.ExternalEntities;
using Ced.BusinessServices.Models;

namespace Ced.BusinessServices
{
    public interface IEditionServices
    {
        EditionEntity GetEditionById(int editionId, string[] eventTypes = null);

        EditionEntity GetEditionByAxId(int axId, string[] eventTypes = null);

        EditionEntity GetLastEdition(int eventId, EditionStatusType[] statuses = null,string[] eventTypes = null);

        EditionEntity GetLastFinishedEdition(int eventId, string[] eventTypes = null);

        EditionEntity GetClosestEdition(int eventId, string[] eventTypes = null);

        EditionEntity GetPreviousEdition(EditionEntity edition, string[] eventTypes = null);

        EditionEntity GetPreviousEdition(int editionId, string[] eventTypes = null);

        EditionEntity GetPreviousEditionByAxId(string axId, string[] eventTypes = null);

        EditionEntity GetNextEdition(EditionEntity edition, string[] eventTypes = null);

        int GetEditionsCount(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null);

        int GetEditionsCount2(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null,
            string[] regions = null, string country = null, string city = null);

        IList<EditionEntityLight> GetEditions(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null,  EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null);

        IList<EditionEntityLight> GetEditions2(string directorEmail, int? eventId = null, bool mustBePrimaryDirector = false,
            int? minFinancialYear = null, EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null,
            string[] regions = null, string country = null, string city = null);

        IList<EditionEntity> GetEditionsByEvent(int eventId);

        IList<EditionEntity> GetEditionsByStatus(EditionStatusType[] statuses);

        IList<string> GetCities(string searchTerm, string countryCode = null);

        #region EDITIONS FOR NOTIFICATION

        IList<EditionEntity> GetEditionsByNotificationType(List<int> checkDays, int? lastXDays, NotificationType notificationType, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        IList<EditionEntity> GetEditionsByNotificationType(int checkDay, int? lastXDays, NotificationType notificationType, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        IList<EditionEntity> GetEditionsByCheckDays(IEnumerable<int> checkDays, int? lastXDays, bool after, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        IList<EditionEntity> GetEditionsByCheckDays(int checkDay, int? lastXDays, bool after, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        IList<EditionEntity> GetEditionsByNextEdition(IList<int> checkDays, int? lastXDays, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        IList<EditionEntity> GetEditionsByNextEdition(int checkDay, int? lastXDays, int? minFinancialYear = null,
            EditionStatusType[] statuses = null, string[] eventTypes = null, string[] eventActivities = null, int? eventId = null);

        #endregion

        IList<EditionEntityLight> SearchEditionsAsCohost(int editionId, string searchTerm, int pageSize, int pageNum);

        IList<Wb365ApiEventEntity> GetEditionsForWb365();

        IList<WisentApiEventEntity> GetEditionsForWisent(string eventName, short? financialYearStart, int? count);

        EditionStartDiff GetEditionStartDateDiff(int axId);

        int CloneEdition(int editionId, int userId);

        bool UpdateEdition(int editionId, EditionEntity editionEntity, int userId, bool? autoIntegration = false);

        bool UpdateEditionGeneralInfo(int editionId, EditionEntity editionEntity, int editionTranslationId,
            EditionTranslationEntity editionTranslationEntity, IList<EditionTranslationSocialMediaEntity> socialMediaEntities,
            int userId, bool? autoIntegration = false);

        bool UpdateEditionSalesMetrics(int editionId, EditionEntity editionEntity, int userId, bool? autoIntegration = false);
        
        bool UpdateEditionExhibitorVisitorStats(int editionId, EditionEntity editionEntity, EditionTranslationEntity editionTranslationEntity,
            int userId, bool? autoIntegration = false);

        bool UpdateEditionPostShowMetrics(int editionId, EditionEntity editionEntity, int userId, bool? autoIntegration = false);

        bool DeleteEdition(int editionId);

        // EXTRAS
        bool RequiresNotification(EditionEntity edition, NotificationType notificationType);

        bool IsEditionNameUnique(int id, string name);

        bool IsInternationalNameUnique(int id, string name);

        bool IsLocalNameUnique(int id, string name);

        bool ClonedEditionAlreadyExists(int eventId);
    }
}