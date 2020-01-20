using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using System;

namespace Ced.IntegrationTests.Extensions
{
    public static class EntityDefaultValuesExtensions
    {
        public static void FillWithDefaultValues(this Edition edition, Event @event)
        {
            edition.EditionName = "Edition" + DateTime.Now.ToFileTimeUtc();
            edition.StartDate = DateTime.Today.AddDays(10);
            edition.EndDate = DateTime.Today.AddDays(12);
            edition.VisitStartTime = TimeSpan.FromHours(9);
            edition.VisitEndTime = TimeSpan.FromHours(20);
            edition.FinancialYearStart = DateTime.Now.Year;
            edition.FinancialYearEnd = DateTime.Now.Year;
            edition.Status = (byte)EditionStatusType.Published.GetHashCode();
            edition.EventTypeCode = EventType.Exhibition.GetDescription();
            edition.EventActivity = EventActivity.ShowHeld.GetDescription();
            edition.AxEventId = 1001;
            edition.CoHostedEvent = false;
            edition.AllDayEvent = false;
            edition.DwEventID = 100001;
            edition.EditionNo = 1;
            edition.MatchedKenticoEventId = 0;
            edition.CreateTime = DateTime.Now;
            edition.CreateUser = 1;
            edition.UpdateUser = 1;
            edition.Event = @event;
        }

        public static void FillWithDefaultValues(this Event @event)
        {
            @event.MasterName = "Event" + DateTime.Now.ToFileTimeUtc();
            @event.EventType = EventType.Exhibition.ToString();
            @event.EventTypeCode = EventType.Exhibition.GetDescription();
            @event.CreateTime = DateTime.Now;
            @event.CreateUser = 1;
            @event.UpdateUser = 1;
        }

        public static void FillWithDefaultValues(this EditionTranslation editionTranslation, Edition edition)
        {
            editionTranslation.Edition = edition;
            editionTranslation.LanguageCode = "en-gb";
            editionTranslation.CreateTime = DateTime.Now;
            editionTranslation.CreateUser = 1;
            editionTranslation.UpdateUser = 1;
        }

        public static void FillWithDefaultValues(this EventDirector eventDirector, Event @event)
        {
            eventDirector.Event = @event;
            eventDirector.DirectorEmail = Constants.DefaultTestValues.UserEmail;
            eventDirector.ApplicationId = WebConfigHelper.ApplicationIdCed;
            eventDirector.CreatedOn = DateTime.Now;
            eventDirector.CreatedBy = 1;
        }

        public static void FillWithDefaultValues(this Notification notification, Edition edition)
        {
            notification.Edition = edition;
            notification.NotificationType = (byte) NotificationType.GeneralInfoCompleteness.GetHashCode();
            notification.ReceiverEmail = Constants.DefaultTestValues.UserEmail;
            notification.CreatedOn = DateTime.Now;
            notification.CreatedBy = 1;
        }
    }
}