using Ced.BusinessEntities;
using Ced.Web.Models.UpdateInfo;

namespace Ced.Web.Helpers
{
    public static class LogControllerHelper
    {
        public static void UpdateLogInMemory(LogEntity log, EditionEntity edition, string updatedFields = null)
        {
            if (log != null)
            {
                if (log.EntityId == null || log.EntityId == 0)
                {
                    log.EntityId = edition.EditionId;
                    log.EntityName = edition.EditionName;
                    log.EventId = edition.EventId;
                    log.EventName = edition.Event.MasterName;
                }
                log.AdditionalInfo = updatedFields != null ? "{" + updatedFields + "}" : null;
            }
        }

        public static string GetUpdatedFields(object currentEdition, object edition, object currentEditionTranslation, object editionTranslation, UpdateDisplayType displayType)
        {
            var updatedFields = NotificationControllerHelper.GetUpdatedFields(currentEdition, edition, "Edition", displayType);

            if (currentEditionTranslation != null && editionTranslation != null)
            {
                var updatedFieldsEditionTranslation = NotificationControllerHelper.GetUpdatedFields(currentEditionTranslation, editionTranslation, "EditionTranslation", displayType);
                if (displayType == UpdateDisplayType.Json) // TODO: Improve it (maybe using UpdatedFieldsDisplayType class?)
                {
                    if (!string.IsNullOrWhiteSpace(updatedFields))
                        updatedFields += ",";
                }
                updatedFields += updatedFieldsEditionTranslation;
            }

            return updatedFields;
        }
    }
}