using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices.Helpers;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using Ced.Utility.Email;
using Ced.Utility.Notification;
using Ced.Utility.Web;
using ITE.Logger;
using ITE.Utility.Extensions;
using ITE.Utility.ObjectComparison;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Transactions;
using Constants = Ced.BusinessServices.Helpers.Constants;
using Edition = Ced.Data.Models.Edition;

namespace Ced.BusinessServices
{
    public class TaskServices : ServiceBase, ITaskServices
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly EditionServices _editionServices;
        private readonly EditionTranslationServices _editionTranslationServices;
        private readonly EventServices _eventServices;
        private readonly EventDirectorServices _eventDirectorServices;
        private readonly EditionServiceHelper _editionServiceHelper;

        private readonly IEditionHelper _editionHelper;
        private readonly IEmailNotificationHelper _emailNotificationHelper;

        public TaskServices(IUnitOfWork unitOfWork, IEditionHelper editionHelper, IEmailNotificationHelper emailNotificationHelper)
            : base(unitOfWork)
        {
            _unitOfWork = (UnitOfWork) unitOfWork;
            _editionServices = new EditionServices(unitOfWork);
            _editionTranslationServices = new EditionTranslationServices(unitOfWork);
            _eventServices = new EventServices(unitOfWork);
            _eventDirectorServices = new EventDirectorServices(unitOfWork);
            _editionServiceHelper = new EditionServiceHelper(unitOfWork);
            _editionHelper = editionHelper;
            _emailNotificationHelper = emailNotificationHelper;
        }

        #region STAGING

        public void UpdateEventEditionFromStagingDb(EditionEntity existingEdition, DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate)
        {
            using (var scope = new TransactionScope())
            {
                var existingEvent = existingEdition.Event;
                var eventFromStaging = EditionServiceHelper.PreUpdateExistingEvent((EventEntity)existingEvent.Clone(), stagingEdition);

                // IF MASTERCODE HAS CHANGED:
                if (existingEvent.MasterCode != eventFromStaging.MasterCode)
                {
                    // DELETE THIS WRONGLY UPDATED EDITION FROM CED
                    _editionServices.DeleteEdition(existingEdition.EditionId);
                }
                else
                {
                    var diffOnEvent = GetDiffOnEvent(existingEvent, eventFromStaging);

                    var editionFromStaging = EditionServiceHelper.PreUpdateExistingEdition((EditionEntity)existingEdition.Clone(), stagingEdition, eventStartDate, eventEndDate);
                    var diffOnEdition = existingEdition.Compare<EditionEntity, StagingDbComparableAttribute>(editionFromStaging);

                    var existingEditionTranslation = existingEdition.EditionTranslations.Single(x =>
                        x.EditionId == existingEdition.EditionId &&
                        x.LanguageCode.ToLower() == LanguageHelper.GetBaseLanguageCultureName());
                    var editionTranslationFromStaging = EditionServiceHelper.PreUpdateExistingEditionTranslation((EditionTranslationEntity)existingEditionTranslation.Clone(), stagingEdition);
                    var diffOnEditionTranslation = existingEditionTranslation.Compare<EditionTranslationEntity, StagingDbComparableAttribute>(editionTranslationFromStaging);

                    if (diffOnEvent.Any())
                    {
                        try
                        {
                            _eventServices.UpdateEvent(existingEvent.EventId, eventFromStaging, Constants.AutoIntegrationUserId, true);
                        }
                        catch (Exception exc)
                        {
                            var extraInfo = "Error on UpdateEvent() | EventId=" + existingEvent.EventId + " | StagingEditionEventBEID=" + stagingEdition.EventBEID;
                            var log = CreateInternalLog(exc, extraInfo);
                            ExternalLogHelper.Log(log, LoggingEventType.Error);
                            return;
                        }
                    }

                    if (diffOnEditionTranslation.Any() || diffOnEdition.Any() || diffOnEvent.Any())
                    {
                        try
                        {
                            var startDateChanged = diffOnEdition.SingleOrDefault(x => x.Prop.ToLower().Contains("startdate")) != null;

                            if (startDateChanged)
                            {
                                // UPDATE STARTDATE DIFF for CURRENT EDITION
                                SetEditionStartDateDiff(editionFromStaging);
                            }

                            _editionServices.UpdateEdition(existingEdition.EditionId, editionFromStaging, Constants.AutoIntegrationUserId, true);

                            if (startDateChanged)
                            {
                                // UPDATE STARTDATE DIFF for NEXT EDITION
                                UpdateNextEditionStartDateDiff(existingEdition);
                            }

                            var eventTypeChanged = diffOnEdition.SingleOrDefault(x => x.Prop.ToLower().Contains("eventtype")) != null;
                            if (eventTypeChanged)
                            {
                                if (!Utility.Constants.ValidEventTypesForCed.Contains(existingEdition.EventTypeCode)
                                    && Utility.Constants.ValidEventTypesForCed.Contains(editionFromStaging.EventTypeCode)
                                    && editionFromStaging.StartDate > DateTime.Now)
                                {
                                    SendNotificationEmailForEditionCreation(editionFromStaging, editionTranslationFromStaging, GetEmailTemplateForEditionCreation(editionFromStaging));
                                }
                            }

                            StartNotificationProcessForEditionNameUpdate(existingEdition, existingEditionTranslation, diffOnEdition);
                            StartNotificationProcessForEditionLocationUpdate(existingEdition, existingEditionTranslation, diffOnEdition);
                        }
                        catch (Exception exc)
                        {
                            var extraInfo = "Error on editionServices.UpdateEdition() | EditionId=" + existingEdition.EditionId + " | StagingEditionEventBEID=" + stagingEdition.EventBEID;
                            var log = CreateInternalLog(exc, extraInfo);
                            ExternalLogHelper.Log(log, LoggingEventType.Error);
                            return;
                        }
                    }

                    if (diffOnEditionTranslation.Any())
                    {
                        try
                        {
                            _editionTranslationServices.UpdateEditionTranslation(editionTranslationFromStaging, Constants.AutoIntegrationUserId, true);
                        }
                        catch (Exception exc)
                        {
                            var extraInfo = "EditionTranslationId=" + existingEditionTranslation.EditionTranslationId + " | StagingEditionEventBEID=" + stagingEdition.EventBEID;
                            var log = CreateInternalLog(exc, extraInfo);
                            ExternalLogHelper.Log(log, LoggingEventType.Error);
                            return;
                        }
                    }
                }

                scope.Complete();
            }
        }

        public void CreateEventEditionFromStagingDb(DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate)
        {
            using (var scope = new TransactionScope())
            {
                var existingEvent = _unitOfWork.EventRepository.Get(x => x.MasterCode == stagingEdition.EventMasterCode);
                int eventId;

                if (existingEvent == null)
                {
                    var newEvent = EditionServiceHelper.PrepareNewEvent(stagingEdition);

                    if (!SaveNewEvent(newEvent, stagingEdition))
                        return;

                    eventId = newEvent.EventId;
                }
                else
                {
                    eventId = existingEvent.EventId;
                }

                var axId = Convert.ToInt32(stagingEdition.EventBEID);

                // TODO: Make it faster
                var lastEdition = _editionServices.GetLastEdition(eventId, Utility.Constants.DefaultValidEditionStatusesForCed, Utility.Constants.ValidEventTypesForCed);

                var newEdition = EditionServiceHelper.PrepareNewEdition(axId, eventId, stagingEdition, eventStartDate, eventEndDate, lastEdition);

                EditionTranslationEntity newEditionTranslation = null;

                if (!SaveNewEdition(newEdition, stagingEdition))
                    return;

                UpdateStartDateDiffFieldsOfNewEdition(newEdition.EditionId);

                if (lastEdition != null)
                {
                    foreach (var editionTranslation in lastEdition.EditionTranslations)
                    {
                        newEditionTranslation = EditionServiceHelper.PrepareNewEditionTranslation(newEdition, stagingEdition, editionTranslation);

                        EditionServiceHelper.CopyEditionImages(newEdition.EditionId, newEdition.EditionName, editionTranslation, newEditionTranslation);

                        var newEditionTranslationId = SaveEditionTranslation(newEditionTranslation, stagingEdition);

                        if (!(newEditionTranslationId > 0)) continue;

                        newEditionTranslation.EditionTranslationId = newEditionTranslationId;

                        _editionServiceHelper.CopySocialMediaAccounts(lastEdition.EditionId, editionTranslation.LanguageCode, newEditionTranslation);
                    }
                }
                else
                {
                    // Add a new EditionTranslation (en-gb)
                    newEditionTranslation = EditionServiceHelper.PrepareNewEditionTranslation(newEdition, stagingEdition, null);

                    var newEditionTranslationId = SaveEditionTranslation(newEditionTranslation, stagingEdition);
                }

                scope.Complete();

                if (Utility.Constants.ValidEventTypesForCed.Contains(newEdition.EventTypeCode))
                {
                    var newEditionEntity = Mapper.Map<Edition, EditionEntity>(newEdition);
                    SendNotificationEmailForEditionCreation(newEditionEntity, newEditionTranslation, GetEmailTemplateForEditionCreation(newEditionEntity));
                }
            }
        }

        #endregion

        #region KENTICO

        public bool UpdateEventsFromKentico()
        {
            //UpdateBeIdOfKenticoEvents();
            UpdateCedEventsFromKentico();

            return true;
        }

        private bool UpdateBeIdOfKenticoEvents()
        {
            var kenticoEditions = _unitOfWork.EventRepository.Context.Clnd_KenticoEvents.Where(x => x.EventBEID == null || x.EventBEID == 0).ToList();
            //var kenticoEditions = _unitOfWork.EventRepository.Context.Clnd_KenticoEvents.ToList();

            // Örn: Worldfood Ukraine

            foreach (var kenticoEdition in kenticoEditions)
            {
                var edition = kenticoEdition;
                //var similarEditions = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                //    x.StartDate == edition.EventDate.GetValueOrDefault().Date && x.EndDate == edition.EventEndDate.GetValueOrDefault().Date
                //    // && x.Event.Country.ToLower() == edition.Country.ToLower()
                //    && x.Event.City.ToLower() == edition.City.ToLower())
                //    .ToList();
                var similarEditions = _unitOfWork.EditionRepository.GetManyQueryable(x =>
                        x.StartDate == edition.EventDate.GetValueOrDefault().Date && x.EndDate == edition.EventEndDate.GetValueOrDefault().Date
                        // && x.Event.Country.ToLower() == edition.Country.ToLower()
                        && x.City.ToLower() == edition.City.ToLower())
                    .ToList();
                // Aynı tarih ve aynı şehirde 2 adet edition bulundu:
                // Worldfood Ukraine, WorldFood Tech & WorldFood Pack

                if (similarEditions.Any())
                {
                    if (similarEditions.Count == 1)
                    {
                        var editionFound = similarEditions.First();
                        kenticoEdition.EventBEID = Convert.ToInt32(editionFound.AxEventId);
                        kenticoEdition.UpdatedOn = DateTime.Now;
                        kenticoEdition.Desc = "Tam isabet";

                        editionFound.MatchedKenticoEventId = kenticoEdition.KenticoEventID;
                        editionFound.MatchedOn = DateTime.Now;

                        _unitOfWork.Save();
                    }
                    else
                    {
                        var kenticoEventName = kenticoEdition.EventName;
                        //kenticoEventName = SplitOnCapitals(kenticoEdition.EventName);
                        kenticoEventName = kenticoEventName.RemoveNonAlphaNumerics();
                        var wordsOfKenticoEditionName = kenticoEventName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        Edition mostSimilarEdition = similarEditions.First();
                        var mostSimilarity = 0;

                        foreach (var similarEdition in similarEditions)
                        {
                            // Eğer bu Edition, daha önce başka bir KenticoEdition ile eşleşmişse dikkate alma (continue döndür)
                            if (similarEdition.MatchedKenticoEventId > 0)
                                continue;

                            var similarEventName = similarEdition.Event.MasterName.RemoveNonAlphaNumerics();
                            var wordsOfEventMasterName = similarEventName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var matchedWordCount = 0;
                            foreach (var word in wordsOfEventMasterName)
                            {
                                foreach (var wordKentico in wordsOfKenticoEditionName)
                                {
                                    if (word.ToLower() == wordKentico.ToLower())
                                    {
                                        matchedWordCount++;
                                    }
                                }
                            }

                            if (matchedWordCount > mostSimilarity)
                            {
                                mostSimilarEdition = similarEdition;
                                mostSimilarity = matchedWordCount;
                            }
                        }

                        if (mostSimilarity > 0)
                        {
                            // We found the most similar edition, so we'll update it.
                            kenticoEdition.EventBEID = Convert.ToInt32(mostSimilarEdition.AxEventId);
                            kenticoEdition.UpdatedOn = DateTime.Now;
                            kenticoEdition.Desc = "Ad benzerliği ile bulundu (tarihler tam eşleşti)";

                            mostSimilarEdition.MatchedKenticoEventId = kenticoEdition.KenticoEventID;
                            mostSimilarEdition.MatchedOn = DateTime.Now;

                            _unitOfWork.Save();
                        }
                        else
                        {
                            kenticoEdition.UpdatedOn = DateTime.Now;
                            kenticoEdition.Desc = "Tarih ve lokasyonu eşleşen kayıtlar içinde adı eşleşen kayıt bulunamadı";
                            _unitOfWork.Save();
                        }

                        //    var editionTranslations = _unitOfWork.EditionTranslationRepository
                        //            .GetMany(x =>
                        //                x.EditionName.ToLower() == kenticoEdition.EventName.ToLower()
                        //                && x.LanguageCode == LanguageHelper.GetBaseLanguageCultureName()).ToList();

                        //    if (editionTranslations.Any())
                        //    {
                        //        if (editionTranslations.Count == 1)
                        //        {
                        //            kenticoEdition.EventBEID = Convert.ToInt32(editionTranslations.First().Edition.AxEventId);
                        //            _unitOfWork.Save();
                        //        }
                        //    }

                        //    foreach (var similarEdition in similarEditions)
                        //    {
                        //        var similarEditionName = similarEdition.Event.MasterName;

                        //        if (kenticoEdition.EventName.ToLower() == similarEditionName.ToLower())
                        //        {
                        //            var editionFound = similarEdition;
                        //            kenticoEdition.EventBEID = Convert.ToInt32(editionFound.AxEventId);
                        //            _unitOfWork.Save();
                        //        }

                        //        // TODO: Levenshtein Distance Algorithm
                        //    }
                    }
                }
                else
                {
                    kenticoEdition.UpdatedOn = DateTime.Now;
                    kenticoEdition.Desc = "Tarih ve lokasyonu eşleşen hiçbir kayıt bulunamadı";
                    _unitOfWork.Save();
                }
            }

            return true;
        }

        private void UpdateCedEventsFromKentico()
        {
            try
            {
                // Get all KenticoEditions
                var kenticoEditions = _unitOfWork.EventRepository.Context.Clnd_KenticoEvents.Where(x => x.EventBEID > 0).ToList();
                //var kenticoEditions = _unitOfWork.EventRepository.Context.Clnd_KenticoEvents.Where(x => x.KenticoEventID == 2970).ToList();

                // For every KenticoEdition;
                foreach (var ke in kenticoEditions)
                {
                    var kenticoEdition = ke;
                    // get the CEDEdition corresponding to this KenticoEdition. (1 adet olması gerek.)
                    var cedEditions = _unitOfWork.EditionRepository.GetManyQueryable(x => x.AxEventId == kenticoEdition.EventBEID).ToList();

                    if (cedEditions.Any() && cedEditions.Count == 1)
                    {
                        var cedEdition = cedEditions.First();

                        // If it has not been already updated recently...
                        //if (cedEdition.UpdateTimeByAutoIntegration > new DateTime(2016, 09, 08, 11, 44, 23))
                        //    continue;

                        var allEditionsOfThisEvent = _unitOfWork.EditionRepository.GetManyQueryable(x => x.EventId == cedEdition.EventId);

                        var logoFound = true;
                        foreach (var edition in allEditionsOfThisEvent)
                        {
                            UpdateSingleCedEventFromKentico(edition, kenticoEdition, ref logoFound);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);
            }
        }

        private void UpdateSingleCedEventFromKentico(Edition cedEdition, Clnd_KenticoEvents kenticoEdition, ref bool logoFound)
        {
            try
            {
                var cedEditionTranslations = _unitOfWork.EditionTranslationRepository.GetManyQueryable(x =>
                    x.EditionId == cedEdition.EditionId &&
                    x.LanguageCode == LanguageHelper.GetBaseLanguageCultureName()).ToList();

                if (cedEditionTranslations.Any() && cedEditionTranslations.Count == 1)
                {
                    var cedEditionTranslation = cedEditionTranslations.First();

                    cedEdition.InternationalName = kenticoEdition.EventName;
                    cedEdition.ManagingOfficeName = kenticoEdition.Organiser;
                    cedEdition.ManagingOfficeEmail = kenticoEdition.EmailAddress;
                    cedEdition.ManagingOfficePhone = "+" + kenticoEdition.InternationalDial + " " + kenticoEdition.Telephone;
                    cedEdition.ManagingOfficeWebsite = kenticoEdition.Website;
                    cedEdition.EventWebSite = kenticoEdition.Website;
                    cedEdition.AllDayEvent = Convert.ToBoolean(kenticoEdition.EventAllDay);
                    cedEdition.DisplayOnIteI = Convert.ToBoolean(kenticoEdition.ITEI);
                    cedEdition.DisplayOnIteGermany = Convert.ToBoolean(kenticoEdition.GiMA);
                    cedEdition.DisplayOnIteAsia = Convert.ToBoolean(kenticoEdition.ASIA);
                    cedEdition.DisplayOnIteTurkey = Convert.ToBoolean(kenticoEdition.Turkey);
                    cedEdition.DisplayOnTradeLink = Convert.ToBoolean(kenticoEdition.TradeLink);
                    cedEdition.DisplayOnIteModa = Convert.ToBoolean(kenticoEdition.MODA);
                    cedEdition.UpdateTimeByAutoIntegration = DateTime.Now;

                    cedEditionTranslation.Summary = kenticoEdition.EventSummary.ReplaceBrWithNewLine().StripHtml();
                    cedEditionTranslation.Description = kenticoEdition.EventDetails.ReplaceBrWithNewLine().StripHtml();
                    cedEditionTranslation.BookStandUrl = kenticoEdition.BookTicketLink;

                    // Get venue coordinates
                    if (kenticoEdition.VenueLocation.HasValue)
                    {
                        var venue = _unitOfWork.EditionRepository.Context.Clnd_customtable_Venue.SingleOrDefault(x => x.ItemID == kenticoEdition.VenueLocation);

                        if (venue != null)
                        {
                            cedEdition.VenueCoordinates = venue.VenueLocation.Replace(",", ", ");
                            cedEditionTranslation.VenueName = venue.VenueName;
                        }
                    }

                    if (logoFound)
                        cedEditionTranslation.WebLogoFileName = UploadEventImage(cedEditionTranslation, kenticoEdition.EventImage, true);
                    logoFound = cedEditionTranslation.WebLogoFileName != null;
                    cedEditionTranslation.PeopleImageFileName = UploadEventImage(cedEditionTranslation, kenticoEdition.EventBackGroundImage, false);
                    cedEditionTranslation.UpdateTimeByAutoIntegration = DateTime.Now;

                    _unitOfWork.Save();
                }
            }
            catch (Exception exc)
            {
                var extraInfo = "Error on UpdateSingleCedEventFromKentico() | EditionId=" + cedEdition + " | KenticoEventBEID=" + kenticoEdition.EventBEID;
                var log = CreateInternalLog(exc, extraInfo);
                ExternalLogHelper.Log(log, LoggingEventType.Error);
            }
        }

        #endregion

        public void UpdateEventDirectors()
        {
            using (var scope = new TransactionScope())
            {
                _unitOfWork.EventDirectorRepository.Context.Database.ExecuteSqlCommand("EXEC sp_UpdateEventDirectorTableCED");
                _unitOfWork.EventDirectorRepository.Context.Database.ExecuteSqlCommand("EXEC sp_UpdateEventDirectorTableCRM");

                scope.Complete();
            }
        }

        public IList<Vw_EventInformations> GetEditionsWithMissingImages()
        {
            var editions = _unitOfWork.SqlQuery<Vw_EventInformations>(
                    "SELECT * FROM dbo.Vw_EventInformations WHERE EventBackGroundImage IS NULL OR EventImage IS NULL " +
                    "OR IconFileName IS NULL OR MasterLogoFileName IS NULL OR ProductImageFileName IS NULL " +
                    "ORDER BY EventName")
                .ToList();
            return editions;
        }

        public void ResetEditionStartDateDifferences()
        {
            var editionIds = _unitOfWork.EditionRepository
                .GetManyQueryable(x => true)
                .OrderByDescending(x => x.StartDate)
                .Select(x => x.EditionId)
                .ToList();

            foreach (var editionId in editionIds)
            {
                var editionEntity = _editionServices.GetEditionById(editionId);
                var editionWeekDiffs = GetStartDateDifferencesByPreviousEdition(editionEntity);

                // UPDATE FOR CURRENT EDITION
                editionEntity.StartWeekOfYearDiff = editionWeekDiffs[0];
                editionEntity.StartDayOfYearDiff = editionWeekDiffs[1];

                _editionServices.UpdateEdition(editionId, editionEntity, Helpers.Constants.AutoIntegrationUserId, false);
            }
        }

        #region HELPER METHODS

        private void StartNotificationProcessForEditionNameUpdate(EditionEntity edition, EditionTranslationEntity editionTranslation, List<Variance> diffOnEdition)
        {
            if (IsEditionNameChanged(diffOnEdition))
            {
                if (WebConfigHelper.TrackEditionNameUpdate)
                {
                    var emailTemplate = GetEmailTemplateForEditionUpdateNotification(edition);

                    var recipients = WebConfigHelper.PrimaryDirectorNotificationsUseMockRecipients
                            ? WebConfigHelper.AdminEmails
                            : _eventDirectorServices.GetRecipientEmails(edition);

                    emailTemplate.Recipients = recipients;

                    SendNotificationEmailForEditionNameUpdate(edition, editionTranslation, emailTemplate);
                }
            }
        }

        private void StartNotificationProcessForEditionLocationUpdate(EditionEntity edition, EditionTranslationEntity editionTranslation, List<Variance> diffOnEdition)
        {
            if (IsLocationChanged(diffOnEdition))
            {
                if (WebConfigHelper.TrackEditionLocationUpdate)
                {
                    var emailTemplate = GetEmailTemplateForEditionUpdateNotification(edition);
                    SendNotificationEmailForEditionLocationUpdate(edition, editionTranslation, emailTemplate);
                }
            }
        }

        private EditionMailTemplate GetEmailTemplateForEditionUpdateNotification(EditionEntity edition)
        {
            var recipients = _eventDirectorServices.GetRecipientEmails(edition);
            var recipientFullName = _editionHelper.GetEventDirectorFullName(edition);
            var notificationAttr = NotificationType.EditionLocationUpdated.GetAttribute<NotificationAttribute>();
            var buttonUrl = _editionHelper.GetEditionUrl(edition, notificationAttr.Fragment);
            var unsubscriptionUrl = notificationAttr.Unsubscribable ? EmailNotificationHelper.GetUnsubscriptionUrl(edition) : string.Empty;
            var emailTemplate = new EditionMailTemplate
            {
                Recipients = recipients,
                RecipientFullName = recipientFullName,
                ButtonUrl = buttonUrl,
                UnsubscriptionUrl = unsubscriptionUrl
            };
            return emailTemplate;
        }

        private EditionMailTemplate GetEmailTemplateForEditionCreation(EditionEntity edition)
        {
            var recipients = WebConfigHelper.NewEventNotificationRecipients;
            var recipientFullName = "";
            var notificationAttr = NotificationType.EditionCreated.GetAttribute<NotificationAttribute>();
            var buttonUrl = _editionHelper.GetEditionUrl(edition, notificationAttr.Fragment);
            var unsubscriptionUrl = notificationAttr.Unsubscribable ? EmailNotificationHelper.GetUnsubscriptionUrl(edition) : string.Empty;
            var emailTemplate = new EditionMailTemplate
            {
                Recipients = recipients,
                RecipientFullName = recipientFullName,
                ButtonUrl = buttonUrl,
                UnsubscriptionUrl = unsubscriptionUrl
            };
            return emailTemplate;
        }

        private EmailResult SendNotificationEmailForEditionNameUpdate(EditionEntity edition, EditionTranslationEntity editionTranslation, EditionMailTemplate emailTemplate)
        {
            if (!WebConfigHelper.TrackEditionNameUpdate)
                return new EmailResult { Sent = false, ErrorMessage = "Tracking disabled." };

            // ADDITIONAL RECIPIENTS
            if (!WebConfigHelper.IsLocal && !WebConfigHelper.IsTest)
                emailTemplate.Recipients += "," + WebConfigHelper.TrackEditionNameUpdateAdditionalRecipients;

            if (!string.IsNullOrWhiteSpace(emailTemplate.Recipients))
            {
                const NotificationType notificationType = NotificationType.EditionNameUpdated;

                var recipientFullName = emailTemplate.RecipientFullName;
                var body = $"Dear {recipientFullName} ," +
                "<br/><br/>" +
                           $"This is to inform you that there have been some changes to the event name information for {edition.EditionName}." +
                           @"<br/><br/>
                You may want to log in and double check whether this change causes inconsistency on any information. 
                <br/><br/>
                To do so please click the button below to go to event's page.";

                var buttonUrl = _editionHelper.GetEditionUrl(edition);

                return _emailNotificationHelper.Send(edition, editionTranslation, notificationType, recipientFullName, body, emailTemplate.Recipients,
                    buttonUrl, emailTemplate.UnsubscriptionUrl);
            }

            return new EmailResult { Sent = false, ErrorMessage = "Event doesn't fit the necessary criteria for emailing." };
        }

        private EmailResult SendNotificationEmailForEditionLocationUpdate(EditionEntity edition, EditionTranslationEntity editionTranslation, EditionMailTemplate emailTemplate)
        {
            if (!WebConfigHelper.TrackEditionLocationUpdate)
                return new EmailResult { Sent = false, ErrorMessage = "Tracking disabled." };

            // IF EDITION START DATE IS LATER THAN NOW
            if (edition.HasNotStartedYet() && edition.HasCityOrCountry())
            {
                // SEND NOTIFICATION EMAIL
                var recipients = emailTemplate.Recipients;

                if (!string.IsNullOrWhiteSpace(recipients))
                {
                    const NotificationType notificationType = NotificationType.EditionLocationUpdated;

                    var recipientFullName = emailTemplate.RecipientFullName;
                    var body = $"Dear {recipientFullName} ," +
                    "<br/><br/>" +
                    $"This is to inform you that there have been some changes to the city and/or country information for {edition.EditionName}." +
                    @"<br/><br/>
                    You may want to log in and double check whether this information affects the address / map location of the existing Venue location information.
                    <br/><br/>
                    To do so please click the button below to go to event edition's page and make the changes in Event Venue section of General Info tab shown below if needed.
                    <br/><br/>" +
                    $"<img src='{AzureStorageHelper.AzureStorageContainerUri}event-venue-map.png' />";

                    var buttonUrl = _editionHelper.GetEditionUrl(edition);

                    return _emailNotificationHelper.Send(edition, editionTranslation, notificationType, recipientFullName, body, recipients,
                        buttonUrl, emailTemplate.UnsubscriptionUrl);
                }
            }

            return new EmailResult { Sent = false, ErrorMessage = "Event doesn't fit the necessary criteria for emailing." };
        }

        internal EmailResult SendNotificationEmailForEditionCreation(EditionEntity edition, EditionTranslationEntity editionTranslation, EditionMailTemplate emailTemplate)
        {
            if (!string.IsNullOrWhiteSpace(emailTemplate.Recipients))
            {
                const NotificationType notificationType = NotificationType.EditionCreated;

                var recipientFullName = emailTemplate.RecipientFullName;
                var body = "Hello," +
                           "<br/><br/>" +
                           $"This is to inform you that a new event has been created with a name {edition.EditionName}." +
                           @"<br/><br/>
                Please click the button below to go to event's page.";

                var buttonUrl = _editionHelper.GetEditionUrl(edition);

                return _emailNotificationHelper.Send(edition, editionTranslation, notificationType, recipientFullName, body, emailTemplate.Recipients,
                    buttonUrl, emailTemplate.UnsubscriptionUrl);
            }

            return new EmailResult { Sent = false, ErrorMessage = "Event doesn't fit the necessary criteria for emailing." };
        }

        private int SaveEditionTranslation(EditionTranslationEntity editionTranslation, DWStaging.Edition stagingEdition)
        {
            try
            {
                var et = Mapper.Map<EditionTranslationEntity, EditionTranslation>(editionTranslation);

                _unitOfWork.EditionTranslationRepository.Insert(et);
                _unitOfWork.Save();

                return et.EditionTranslationId;
            }
            catch (Exception exc)
            {
                var extraInfo = "StagingEditionEventBEID=" + stagingEdition.EventBEID;
                var log = CreateInternalLog(exc, extraInfo);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return -1;
            }
        }

        private bool SaveNewEdition(Edition newEdition, DWStaging.Edition stagingEdition)
        {
            try
            {
                _unitOfWork.EditionRepository.Insert(newEdition);
                _unitOfWork.Save();
            }
            catch (Exception exc)
            {
                var extraInfo = "StagingEditionEventBEID=" + stagingEdition.EventBEID;
                var log = CreateInternalLog(exc, extraInfo);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }
            return true;
        }

        private bool SaveNewEvent(Event newEvent, DWStaging.Edition stagingEdition)
        {
            try
            {
                _unitOfWork.EventRepository.Insert(newEvent);
                _unitOfWork.Save();
            }
            catch (Exception exc)
            {
                var extraInfo = "Error on EventRepository.Insert() | StagingEditionEventBEID=" + stagingEdition.EventBEID;
                var log = CreateInternalLog(exc, extraInfo);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }
            return true;
        }

        // TODO: Move to EventServiceHelper.cs?
        //private string GetPrimaryDirectors(int eventId, bool useMockRecipients)
        //{
        //    var recipients = string.Empty;
        //    if (useMockRecipients)
        //        recipients = WebConfigHelper.AdminEmails;
        //    else
        //    {
        //        var primaryDirectors = _eventDirectorServices.GetPrimaryDirectors(eventId, WebConfigHelper.ApplicationIdCed);
        //        if (primaryDirectors.Any())
        //        {
        //            recipients = primaryDirectors.Select(x => x.DirectorEmail).ToCommaSeparatedString();
        //        }
        //    }
        //    return recipients;
        //}

        //private string GetPrimaryDirectorsForLocationUpdate(int eventId)
        //{
        //    return GetPrimaryDirectors(eventId, WebConfigHelper.TrackEditionLocationUpdateUseMockRecipients);
        //}

        private static List<Variance> GetDiffOnEvent(EventEntity existingEvent, EventEntity eventFromStaging)
        {
            var diffOnEvent = existingEvent.Compare<EventEntity, StagingDbComparableAttribute>(eventFromStaging);
            return diffOnEvent;
        }

        private static bool IsEditionNameChanged(List<Variance> diffOnEdition)
        {
            if (diffOnEdition.Any(x => x.Prop.ToLower() == "editionname"))
                return true;
            return false;
        }

        private static bool IsLocationChanged(List<Variance> diffOnEvent)
        {
            if (diffOnEvent.Any(x => x.Prop.ToLower() == "city" || x.Prop.ToLower() == "country"))
                return true;
            return false;
        }

        private string UploadEventImage(EditionTranslation editionTranslation, string imageUrl, bool isWebLogo)
        {
            const string remoteImageDomain = "https://www.hyve.group/";

            try
            {
                if (string.IsNullOrWhiteSpace(imageUrl))
                    return null;

                // D:\PROJECTS\ITE.CED\Ced.Web\content\images\edition\logo
                var newFileName = editionTranslation.EditionId + "-" +
                                  editionTranslation.Edition.EditionName.Trim()
                                      .Replace(" ", "")
                                      .Replace("'", "")
                                      .Replace("&", " ")
                                      .Replace("\"", " ")
                                      .ToLower() + "-" +
                                  editionTranslation.LanguageCode.ToLower() + Path.GetExtension(imageUrl);

                var remoteImgPath = remoteImageDomain + imageUrl.Replace("~/", "");
                remoteImgPath = remoteImgPath.Substring(0, remoteImgPath.IndexOf(".aspx", StringComparison.Ordinal));
                var folderName = isWebLogo
                    ? EditionImageType.WebLogo.GetAttribute<ImageAttribute>().Key
                    : EditionImageType.PeopleImage.GetAttribute<ImageAttribute>().Key;
                var localPath = AppDomain.CurrentDomain.BaseDirectory + @"content\images\edition\" + $@"{folderName}\" + newFileName;
                var webClient = new WebClient();
                try
                {
                    webClient.DownloadFile(remoteImgPath, localPath);
                }
                catch (Exception exc)
                {
                    var extraInfo = "remoteImgPath: " + remoteImgPath + " | EditionId: " + editionTranslation.EditionId;
                    var log = CreateInternalLog(exc, extraInfo);
                    ExternalLogHelper.Log(log, LoggingEventType.Error);

                    if (isWebLogo)
                    {
                        // When the image cannot be found...
                        var kenticoEditions = _unitOfWork.EventRepository.Context.Clnd_KenticoEvents.Where(x => x.EventBEID == editionTranslation.Edition.AxEventId);
                        Clnd_KenticoEvents kenticoEdition = null;
                        if (kenticoEditions.Any())
                            kenticoEdition = kenticoEditions.First();

                        if (!string.IsNullOrWhiteSpace(kenticoEdition?.EventImage))
                        {
                            var lostImageUrl = "";
                            try
                            {
                                // sample: https://www.hyve.group/getmedia/fe18dac0-002d-42a2-b293-2f79a41d8028/anystring.aspx

                                const int guidLength = 36;
                                const string strGetMedia = "/getmedia/";
                                var indexGetMedia = kenticoEdition.EventImage.IndexOf(strGetMedia, StringComparison.Ordinal);
                                var indexStart = indexGetMedia + strGetMedia.Length;

                                if (indexStart == -1) return null;
                                if (kenticoEdition.EventImage.Length < indexStart + guidLength) return null;

                                var guid = kenticoEdition.EventImage.Substring(indexStart, guidLength);
                                lostImageUrl = $"{remoteImageDomain}getmedia/{guid}/anystring.aspx";

                                webClient.DownloadFile(lostImageUrl, localPath);

                                return newFileName;
                            }
                            catch (Exception exc2)
                            {
                                var extraInfo2 = "lostImageUrl: " + lostImageUrl + " | EditionId: " + editionTranslation.EditionId + " | 2nd attempt";
                                var log2 = CreateInternalLog(exc2, extraInfo2);
                                ExternalLogHelper.Log(log2, LoggingEventType.Error);

                                return null;
                            }
                        }
                        return null;
                    }
                    return null;
                }

                return newFileName;
            }
            catch (Exception exc)
            {
                var extraInfo = "EditionId: " + editionTranslation.EditionId;
                var log = CreateInternalLog(exc, extraInfo);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return null;
            }
        }

        #region STARTDATEDIFF

        private List<int?> GetStartDateDifferencesByPreviousEdition(EditionEntity edition)
        {
            var previousEdition = _editionServices.GetPreviousEdition(edition, Utility.Constants.ValidEventTypesForCed);
            if (previousEdition == null) return new List<int?> { null, null };
            var weekDiff = previousEdition.StartDate.GetValueOrDefault().WeekOfYearDiff(edition.StartDate.GetValueOrDefault());
            var dayDiff = previousEdition.StartDate.GetValueOrDefault().DayOfYearDiff(edition.StartDate.GetValueOrDefault());
            return new List<int?> { weekDiff, dayDiff };
        }

        private List<int?> GetNextEditionStartDateDifferences(EditionEntity edition)
        {
            var nextEdition = _editionServices.GetNextEdition(edition, Utility.Constants.ValidEventTypesForCed);
            if (nextEdition == null) return new List<int?> { null, null };
            var weekDiff = edition.StartDate.GetValueOrDefault().WeekOfYearDiff(nextEdition.StartDate.GetValueOrDefault());
            var dayDiff = edition.StartDate.GetValueOrDefault().DayOfYearDiff(nextEdition.StartDate.GetValueOrDefault());
            return new List<int?> { weekDiff, dayDiff };
        }

        // Current: 2017
        // Previous: 2016
        // StartDate'i değişmiş bir edition'ın StartDateDiff değerlerini güncelle.
        public void SetEditionStartDateDiff(EditionEntity edition)
        {
            var startDateDiff = GetStartDateDifferencesByPreviousEdition(edition);
            edition.StartWeekOfYearDiff = startDateDiff[0];
            edition.StartDayOfYearDiff = startDateDiff[1];
        }

        // Current: 2017
        // Next: 2018
        // StartDate'i değişmiş bir edition'dan sonraki edition'ın StartDateDiff değerlerini güncelle.
        public void UpdateNextEditionStartDateDiff(EditionEntity edition)
        {
            // UPDATE FOR NEXT EDITION
            var nextEdition = _editionServices.GetNextEdition(edition, Utility.Constants.ValidEventTypesForCed);
            if (nextEdition != null)
            {
                var startDateDiffsByNextEdition = GetNextEditionStartDateDifferences(edition);
                nextEdition.StartWeekOfYearDiff = startDateDiffsByNextEdition[0];
                nextEdition.StartDayOfYearDiff = startDateDiffsByNextEdition[1];
                _editionServices.UpdateEdition(nextEdition.EditionId, nextEdition, Helpers.Constants.AutoIntegrationUserId, true);
            }
        }

        private void UpdateStartDateDiffFieldsOfNewEdition(int editionId)
        {
            var newEditionEntity = _editionServices.GetEditionById(editionId);
            SetEditionStartDateDiff(newEditionEntity);
            _editionServices.UpdateEdition(newEditionEntity.EditionId, newEditionEntity, Helpers.Constants.AutoIntegrationUserId, true);
        }

        #endregion
        
        #endregion
    }
}
