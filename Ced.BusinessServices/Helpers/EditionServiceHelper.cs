using Ced.BusinessEntities;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using ITE.AzureStorage;
using ITE.Utility.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ced.BusinessServices.Helpers
{
    public class EditionServiceHelper
    {
        private static CountryServices _countryServices;
        private readonly EditionTranslationSocialMediaServices _editionTranslationSocialMediaServices;

        public EditionServiceHelper(IUnitOfWork unitOfWork)
        {
            _countryServices = new CountryServices(unitOfWork);
            _editionTranslationSocialMediaServices = new EditionTranslationSocialMediaServices(unitOfWork);
        }

        public static bool IsEditionInfoCompleted(object edition, EditionInfoType infoType)
        {
            foreach (var prop in edition.GetType().GetProperties())
            {
                var attrs = prop.GetCustomAttributes<EditionFieldAttribute>(true).ToList();
                if (attrs.Any())
                {
                    var editionField = attrs.First();
                    if ((editionField.InfoType == infoType || infoType == EditionInfoType.AllInfo) && editionField.Required)
                    {
                        var propType = prop.PropertyType;
                        //if (!IsSimpleType(propType))
                        //    continue;

                        var propVal = prop.GetValue(edition);
                        var underlyingPropType =
                            prop.PropertyType.IsGenericType &&
                            prop.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>)
                                ? Nullable.GetUnderlyingType(prop.PropertyType)
                                : null;

                        if (propVal == null)
                            return false;

                        if (propType == typeof (string) && string.IsNullOrWhiteSpace((string) propVal))
                            return false;

                        if (propVal.Equals(propType.GetDefaultValue()))
                            return false;

                        if (underlyingPropType != null && propVal.Equals(underlyingPropType.GetDefaultValue()))
                            return false;
                    }
                }
            }
            return true;
        }

        //private static bool IsSimpleType(Type type)
        //{
        //    return type.IsPrimitive || type.IsValueType || type == typeof (string);
        //}

        public static string ComposeImageName(int editionId, string editionName, string languageCode, string extension)
        {
            var datePart = DateTime.Now.ToUnixTimestamp();
            editionName = editionName.ToLower();
            editionName = editionName.RemoveDuplicateChars(" ");
            editionName = editionName.Replace(" ", "");
            editionName = editionName.RemoveNonAlphaNumerics();
            var fileName = $"{editionId}-{editionName}-{languageCode}-{datePart}{extension}";
            return fileName;
        }

        public static string ComposeFileName(int editionId, string fileName, EditionFileType fileType, string languageCode, string extension)
        {
            fileName = fileName.ToLower();
            fileName = fileName.RemoveNonAlphaNumericsExcept(new[] { '-', '_' });
            fileName = fileName.Replace(" ", "-");
            fileName = fileName.Replace("_", "-");
            fileName = fileName.RemoveDuplicateChars(new [] { "-" });
            if (fileType == EditionFileType.PostShowReport)
                fileName += "-" + languageCode;
            var newFileName = $"{editionId}-{fileName}{extension}";
            return newFileName;
        }

        public static void CopyEditionImages(int editionId, string editionName, EditionTranslationEntity sourceEditionTranslation, EditionTranslationEntity targetEditionTranslation)
        {
            foreach (EditionImageType imageType in Enum.GetValues(typeof(EditionImageType)))
            {
                var sourceFileName = sourceEditionTranslation.GetFileName(imageType);
                var targetFileName = ComposeImageName(editionId, editionName, targetEditionTranslation.LanguageCode, Path.GetExtension(sourceFileName));

                var uploadedFileName = CopyImageOnAzure(sourceFileName, targetFileName, imageType);

                targetEditionTranslation.SetFileName(imageType, uploadedFileName);
            }
        }

        public static void RenameEditionImages(int editionId, string newEditionName, EditionTranslationEntity editionTranslation)
        {
            var azureService = new Service();

            foreach (EditionImageType imageType in Enum.GetValues(typeof(EditionImageType)))
            {
                var oldFileName = editionTranslation.GetFileName(imageType);
                var newFileName = ComposeImageName(editionId, newEditionName, editionTranslation.LanguageCode, Path.GetExtension(oldFileName));

                var oldFileFullName = imageType.BlobFullName(oldFileName);
                var newFileFullName = imageType.BlobFullName(newFileName);

                azureService.RenameFile(oldFileFullName, newFileFullName);

                editionTranslation.SetFileName(imageType, newFileName);
            }
        }

        public static string CopyImageOnAzure(string sourceFileName, string targetFileName, EditionImageType imageType)
        {
            if (string.IsNullOrWhiteSpace(sourceFileName))
                return null;

            var azureService = new Service();

            var sourceFileFullName = imageType.BlobFullName(sourceFileName);
            var targetFileFullName = imageType.BlobFullName(targetFileName);

            var copied = azureService.CopyFile(sourceFileFullName, targetFileFullName);
            return copied ? targetFileName : null;
        }

        public static void ResetEdition(EditionEntity targetEdition)
        {
            targetEdition.Status = EditionStatusType.PreDraft;
            targetEdition.StatusUpdateTime = DateTime.Now;

            targetEdition.EditionId = 0;
            targetEdition.Event = null;

            targetEdition.UpdateTime = null;
            targetEdition.UpdateUser = 0;

            targetEdition.EditionNo = 0;
            targetEdition.StartDate = null;
            targetEdition.EndDate = null;
            //targetEdition.LocalSqmSold = 0;
            //targetEdition.InternationalSqmSold = 0;
            targetEdition.SqmSold = 0;
            targetEdition.SponsorCount = 0;

            //targetEdition.LocalExhibitorCount = 0;
            //targetEdition.InternationalExhibitorCount = 0;
            targetEdition.ExhibitorCount = 0;
            targetEdition.ExhibitorCountryCount = 0;
            targetEdition.NationalGroupCount = 0;

            targetEdition.InternationalVisitorCount = 0;
            targetEdition.VisitorCountryCount = 0;
            targetEdition.LocalVisitorCount = 0;

            targetEdition.NPSAverageExhibitor = 0;
            targetEdition.NPSAverageVisitor = 0;
            targetEdition.NPSSatisfactionExhibitor = 0;
            targetEdition.NPSSatisfactionVisitor = 0;
            targetEdition.NPSScoreExhibitor = 0;
            targetEdition.NPSScoreVisitor = 0;
            targetEdition.NetEasyScoreVisitor = 0;
            targetEdition.NetEasyScoreExhibitor = 0;

            targetEdition.OnlineRegistrationCount = 0;
            targetEdition.OnlineRegisteredVisitorCount = 0;
            targetEdition.OnlineRegisteredBuyerVisitorCount = 0;

            //targetEdition.InternationalExhibitorRetentionRate = 0;
            //targetEdition.LocalExhibitorRetentionRate = 0;
            targetEdition.ExhibitorRetentionRate = 0;

            targetEdition.NPSSatisfactionExhibitor = 0;
            targetEdition.NPSSatisfactionVisitor = 0;
        }

        public void CopySocialMediaAccounts(int sourceEditionId, string languageCode, EditionTranslationEntity targetEditionTranslation)
        {
            var socialMedias = _editionTranslationSocialMediaServices.GetByEdition(sourceEditionId, languageCode);
            foreach (var sm in socialMedias)
            {
                var clonedEntity = (EditionTranslationSocialMediaEntity)sm.Clone();
                clonedEntity.EditionId = targetEditionTranslation.EditionId;
                clonedEntity.EditionTranslationId = targetEditionTranslation.EditionTranslationId;
                _editionTranslationSocialMediaServices.Create(clonedEntity, Constants.AutoIntegrationUserId);
            }
        }


        public static Event PrepareNewEvent(DWStaging.Edition stagingEdition)
        {
            var newEvent = new Event
            {
                MasterCode = stagingEdition.EventMasterCode,
                MasterName = stagingEdition.EventMaster,
                Region = stagingEdition.EventRegion,
                Industry = stagingEdition.EventIndustry,
                SubIndustry = stagingEdition.EventSubIndustry,
                EventType = stagingEdition.EventType,
                EventTypeCode = stagingEdition.EVENTTYPEBEID,
                EventBusinessClassification = stagingEdition.EventBusinessClassification,
                CreateTime = DateTime.Now,
                CreateUser = Constants.AutoIntegrationUserId
            };
            return newEvent;
        }

        public static Edition PrepareNewEdition(int axId, int eventId, DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate, EditionEntity lastEdition)
        {
            var newEdition = new Edition
            {
                AxEventId = axId,
                EventId = eventId,
                DwEventID = stagingEdition.DWEventID,
                PreviousInstanceDwEventId = stagingEdition.PreviousEditionDWEventID.GetValueOrDefault(),
                EditionNo = stagingEdition.EditionNo.GetValueOrDefault(),
                EditionName = stagingEdition.EventName,
                ReportingName =
                    !string.IsNullOrWhiteSpace(stagingEdition.EventNameReporting)
                        ? stagingEdition.EventNameReporting
                        : stagingEdition.EventName,
                LocalName =
                    !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameLocal)
                        ? stagingEdition.EventLongNameLocal
                        : stagingEdition.EventName,
                InternationalName =
                    !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameInternational)
                        ? stagingEdition.EventLongNameInternational
                        : stagingEdition.EventName,
                EventTypeCode = stagingEdition.EVENTTYPEBEID,
                EventActivity = stagingEdition.EventActivity,
                Classification = stagingEdition.EventBandClassification,
                Frequency = stagingEdition.EventFrequency,
                Country = stagingEdition.EventCountryName,
                CountryCode = _countryServices.GetCountryByName(stagingEdition.EventCountryName)?.CountryCode,
                City = stagingEdition.EventCity,
                ManagingOfficeName = stagingEdition.EventManagingOfficeName,
                DirectorManagingOfficeName = stagingEdition.DirectorManagingOfficeName,
                DirectorEmail = stagingEdition.EventDirectorEmail,
                DirectorFullName = stagingEdition.EventDirector,
                StartDate = eventStartDate,
                EndDate = eventEndDate,
                FinancialYearStart = Convert.ToInt32(stagingEdition.EventFYStart),
                FinancialYearEnd = Convert.ToInt32(stagingEdition.EventFYEnd),
                EventOwnershipBEID = stagingEdition.EVENTOWNERSHIPBEID,
                EventOwnership = stagingEdition.EVENTOWNERSHIP,
                Status = (byte)EditionStatusType.Published.GetHashCode(),
                CreateTime = DateTime.Now,
                CreateUser = Constants.AutoIntegrationUserId
            };

            if (lastEdition != null)
            {
                newEdition.VisitStartTime = lastEdition.VisitStartTime.GetValueOrDefault();
                newEdition.VisitEndTime = lastEdition.VisitEndTime.GetValueOrDefault();
                newEdition.AllDayEvent = lastEdition.AllDayEvent;
                newEdition.Promoted = lastEdition.Promoted;
                newEdition.ManagingOfficeEmail = lastEdition.ManagingOfficeEmail;
                newEdition.ManagingOfficePhone = lastEdition.ManagingOfficePhone;
                newEdition.ManagingOfficeWebsite = lastEdition.ManagingOfficeWebsite;
                newEdition.EventWebSite = lastEdition.EventWebSite;
                newEdition.MarketoPreferenceCenterLink = lastEdition.MarketoPreferenceCenterLink;
                newEdition.VenueCoordinates = lastEdition.VenueCoordinates;
                newEdition.DisplayOnIteGermany = lastEdition.DisplayOnIteGermany;
                newEdition.DisplayOnIteAsia = lastEdition.DisplayOnIteAsia;
                newEdition.DisplayOnIteI = lastEdition.DisplayOnIteI;
                newEdition.DisplayOnIteModa = lastEdition.DisplayOnIteModa;
                newEdition.DisplayOnItePoland = lastEdition.DisplayOnItePoland;
                newEdition.DisplayOnIteTurkey = lastEdition.DisplayOnIteTurkey;
                newEdition.DisplayOnIteRussia = lastEdition.DisplayOnIteRussia;
                newEdition.DisplayOnIteEurasia = lastEdition.DisplayOnIteEurasia;
                newEdition.DisplayOnTradeLink = lastEdition.DisplayOnTradeLink;
                newEdition.DisplayOnIteUkraine = lastEdition.DisplayOnIteUkraine;
                newEdition.DisplayOnIteBuildInteriors = lastEdition.DisplayOnIteBuildInteriors;
                newEdition.DisplayOnIteFoodDrink = lastEdition.DisplayOnIteFoodDrink;
                newEdition.DisplayOnIteOilGas = lastEdition.DisplayOnIteOilGas;
                newEdition.DisplayOnIteTravelTourism = lastEdition.DisplayOnIteTravelTourism;
                newEdition.DisplayOnIteTransportLogistics = lastEdition.DisplayOnIteTransportLogistics;
                newEdition.DisplayOnIteFashion = lastEdition.DisplayOnIteFashion;
                newEdition.DisplayOnIteSecurity = lastEdition.DisplayOnIteSecurity;
                newEdition.DisplayOnIteBeauty = lastEdition.DisplayOnIteBeauty;
                newEdition.DisplayOnIteHealthCare = lastEdition.DisplayOnIteHealthCare;
                newEdition.DisplayOnIteMining = lastEdition.DisplayOnIteMining;
                newEdition.DisplayOnIteEngineeringIndustrial = lastEdition.DisplayOnIteEngineeringIndustrial;
                newEdition.TradeShowConnectDisplay = lastEdition.TradeShowConnectDisplay;
            }

            return newEdition;
        }

        public static EditionTranslationEntity PrepareNewEditionTranslation(Edition newEdition, DWStaging.Edition stagingEdition, EditionTranslationEntity lastEditionTranslation)
        {
            if (lastEditionTranslation == null)
            {
                lastEditionTranslation = new EditionTranslationEntity
                {
                    LanguageCode = LanguageHelper.GetBaseLanguageCultureName()
                };
            }

            var venueName = lastEditionTranslation.LanguageCode == LanguageHelper.GetBaseLanguageCultureName()
                ? stagingEdition.EventVenue
                : lastEditionTranslation.VenueName;

            var newEditionTranslation = new EditionTranslationEntity
            {
                EditionId = newEdition.EditionId,
                LanguageCode = lastEditionTranslation.LanguageCode,
                Summary = lastEditionTranslation.Summary,
                Description = lastEditionTranslation.Description,
                ExhibitorProfile = lastEditionTranslation.ExhibitorProfile,
                VisitorProfile = lastEditionTranslation.VisitorProfile,
                VenueName = venueName,
                MapVenueFullAddress = lastEditionTranslation.MapVenueFullAddress,
                WebLogoFileName = lastEditionTranslation.WebLogoFileName,
                PeopleImageFileName = lastEditionTranslation.PeopleImageFileName,
                ProductImageFileName = lastEditionTranslation.ProductImageFileName,
                MasterLogoFileName = lastEditionTranslation.MasterLogoFileName,
                CrmLogoFileName = lastEditionTranslation.CrmLogoFileName,
                IconFileName = lastEditionTranslation.IconFileName,
                BookStandUrl = lastEditionTranslation.BookStandUrl,
                OnlineInvitationUrl = lastEditionTranslation.OnlineInvitationUrl,
                CreateTime = DateTime.Now,
                CreateUser = Constants.AutoIntegrationUserId
            };

            //EditionName = stagingEdition.EventName,
            //ReportingName = !string.IsNullOrWhiteSpace(stagingEdition.EventNameReporting) ? stagingEdition.EventNameReporting : stagingEdition.EventName,
            //LocalName = !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameLocal) ? stagingEdition.EventLongNameLocal : stagingEdition.EventName,
            //InternationalName = !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameInternational) ? stagingEdition.EventLongNameInternational : stagingEdition.EventName,

            return newEditionTranslation;
        }

        public static EventEntity PreUpdateExistingEvent(EventEntity existingEvent, DWStaging.Edition stagingEdition)
        {
            existingEvent.MasterCode = stagingEdition.EventMasterCode; // no conflict
            existingEvent.MasterName = stagingEdition.EventMaster; // no conflict

            existingEvent.Region = stagingEdition.EventRegion; // no conflict

            existingEvent.Industry = stagingEdition.EventIndustry; // no conflict
            existingEvent.SubIndustry = stagingEdition.EventSubIndustry; // no conflict

            existingEvent.EventTypeCode = stagingEdition.EVENTTYPEBEID; // no conflict
            existingEvent.EventType = stagingEdition.EventType; // no conflict

            existingEvent.EventBusinessClassification = stagingEdition.EventBusinessClassification; // no conflict
            return existingEvent;
        }

        public static EditionEntity PreUpdateExistingEdition(EditionEntity existingEdition, DWStaging.Edition stagingEdition, DateTime eventStartDate, DateTime eventEndDate)
        {
            existingEdition.DwEventId = stagingEdition.DWEventID; // no conflict
            existingEdition.PreviousInstanceDwEventId = stagingEdition.PreviousEditionDWEventID.GetValueOrDefault(); // no conflict
            if (existingEdition.EditionNo == 0)
                existingEdition.EditionNo = (short)stagingEdition.EditionNo.GetValueOrDefault();

            existingEdition.EditionName = stagingEdition.EventName; // no conflict
            //existingEdition.ReportingName = !string.IsNullOrWhiteSpace(stagingEdition.EventNameReporting) ? stagingEdition.EventNameReporting : stagingEdition.EventName; // no conflict
            //existingEdition.LocalName = !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameLocal) ? stagingEdition.EventLongNameLocal : stagingEdition.EventName; // no conflict
            //existingEdition.InternationalName = !string.IsNullOrWhiteSpace(stagingEdition.EventLongNameInternational) ? stagingEdition.EventLongNameInternational : stagingEdition.EventName;

            existingEdition.EventActivity = stagingEdition.EventActivity; // no conflict
            existingEdition.EventTypeCode = stagingEdition.EVENTTYPEBEID; // no conflict
            existingEdition.Classification = stagingEdition.EventBandClassification; // no conflict
            existingEdition.Frequency = stagingEdition.EventFrequency; // no conflict

            existingEdition.Country = stagingEdition.EventCountryName;
            existingEdition.CountryCode = _countryServices.GetCountryByName(stagingEdition.EventCountryName)?.CountryCode;
            existingEdition.City = stagingEdition.EventCity;

            if (string.IsNullOrWhiteSpace(existingEdition.ManagingOfficeName))
                existingEdition.ManagingOfficeName = stagingEdition.EventManagingOfficeName;
            existingEdition.DirectorManagingOfficeName = stagingEdition.DirectorManagingOfficeName; // no conflict
            //existingEdition.DirectorEmail = stagingEdition.EventDirectorEmail; // no conflict
            //existingEdition.DirectorFullName = stagingEdition.EventDirector; // no conflict

            existingEdition.StartDate = eventStartDate; // no conflict
            existingEdition.EndDate = eventEndDate; // no conflict
            existingEdition.FinancialYearStart = Convert.ToInt32(stagingEdition.EventFYStart); // no conflict
            existingEdition.FinancialYearEnd = Convert.ToInt32(stagingEdition.EventFYEnd); // no conflict

            existingEdition.EventOwnershipBEID = stagingEdition.EVENTOWNERSHIPBEID;
            existingEdition.EventOwnership = stagingEdition.EVENTOWNERSHIP;
            return existingEdition;
        }

        public static EditionTranslationEntity PreUpdateExistingEditionTranslation(EditionTranslationEntity existingEditionTranslation, DWStaging.Edition stagingEdition)
        {
            existingEditionTranslation.LanguageCode = LanguageHelper.GetBaseLanguageCultureName();
            existingEditionTranslation.VenueName = stagingEdition.EventVenue;
            return existingEditionTranslation;
        }
    }
}
