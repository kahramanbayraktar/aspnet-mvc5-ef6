using Ced.BusinessEntities;
using Ced.Utility;
using Ced.Utility.Edition;
using Ced.Web.Models.Edition;
using ITE.Utility.Extensions;

namespace Ced.Web.Helpers
{
    public static class EditionControllerHelper
    {
        public static EditionEditGeneralInfoModel ComposeEditionEditGeneralInfoModel(
            CedUser currentUser,
            EditionEntity edition,
            EditionTranslationEntity editionTranslation,
            string lang, bool isEditableForImages)
        {
            var model = new EditionEditGeneralInfoModel
            {
                EditionTranslationId = editionTranslation.EditionTranslationId,
                EditionId = edition.EditionId,
                EditionName = edition.EditionName,
                EditionNo = edition.EditionNo,
                EventId = edition.EventId,
                Country = edition.Country,
                City = edition.City,
                CountryLocalName = edition.CountryLocalName,
                CityLocalName = edition.CityLocalName,
                ReportingName = edition.ReportingName,
                LocalName = edition.LocalName,
                InternationalName = edition.InternationalName,
                LanguageCode = lang,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),

                StartDate = edition.StartDate,
                EndDate = edition.EndDate,
                VisitEndTime = edition.VisitEndTime,
                VisitStartTime = edition.VisitStartTime,
                CoolOffPeriodStartDate = edition.CoolOffPeriodStartDate,
                CoolOffPeriodEndDate = edition.CoolOffPeriodEndDate,
                InternationalDate = edition.InternationalDate,
                LocalDate = edition.LocalDate,
                Summary = editionTranslation.Summary,
                Description = editionTranslation.Description,
                ExhibitorProfile = editionTranslation.ExhibitorProfile,
                VisitorProfile = editionTranslation.VisitorProfile,
                WebLogoFileName = editionTranslation.WebLogoFileName,
                PeopleImageFileName = editionTranslation.PeopleImageFileName,
                EventFlagPictureFileName = edition.EventFlagPictureFileName,
                AllDayEvent = edition.AllDayEvent,
                Promoted = edition.Promoted,
                TradeShowConnectDisplay = edition.TradeShowConnectDisplay,
                ManagingOfficeEmail = edition.ManagingOfficeEmail,
                ManagingOfficePhone = edition.ManagingOfficePhone,
                EventWebSite = edition.EventWebSite,
                MarketoPreferenceCenterLink = edition.MarketoPreferenceCenterLink,
                VenueName = editionTranslation.VenueName,
                MapVenueFullAddress = editionTranslation.MapVenueFullAddress,
                VenueCoordinates = edition.VenueCoordinates,
                BookStandUrl = editionTranslation.BookStandUrl,
                OnlineInvitationUrl = editionTranslation.OnlineInvitationUrl,
                EventActivity = edition.EventActivity,

                DisplayOnIteGermany = edition.DisplayOnIteGermany,
                DisplayOnIteAsia = edition.DisplayOnIteAsia,
                DisplayOnIteI = edition.DisplayOnIteI,
                DisplayOnItePoland = edition.DisplayOnItePoland,
                DisplayOnIteModa = edition.DisplayOnIteModa,
                DisplayOnIteTurkey = edition.DisplayOnIteTurkey,
                DisplayOnIteRussia = edition.DisplayOnIteRussia,
                DisplayOnIteEurasia = edition.DisplayOnIteEurasia,
                DisplayOnTradeLink = edition.DisplayOnTradeLink,
                DisplayOnIteUkraine = edition.DisplayOnIteUkraine,
                DisplayOnIteBuildInteriors = edition.DisplayOnIteBuildInteriors,
                DisplayOnIteFoodDrink = edition.DisplayOnIteFoodDrink,
                DisplayOnIteOilGas = edition.DisplayOnIteOilGas,
                DisplayOnIteTravelTourism = edition.DisplayOnIteTravelTourism,
                DisplayOnIteTransportLogistics = edition.DisplayOnIteTransportLogistics,
                DisplayOnIteFashion = edition.DisplayOnIteFashion,
                DisplayOnIteSecurity = edition.DisplayOnIteSecurity,
                DisplayOnIteBeauty = edition.DisplayOnIteBeauty,
                DisplayOnIteHealthCare = edition.DisplayOnIteHealthCare,
                DisplayOnIteMining = edition.DisplayOnIteMining,
                DisplayOnIteEngineeringIndustrial = edition.DisplayOnIteEngineeringIndustrial,
                HiddenFromWebSites = edition.HiddenFromWebSites,

                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                IsEditableForImages = isEditableForImages,
                CurrentUser = currentUser
            };
            return model;
        }

        public static EditionEditSalesMetricsModel ComposeEditionEditSalesMetricsModel(CedUser currentUser, EditionEntity edition)
        {
            var model = new EditionEditSalesMetricsModel
            {
                EditionId = edition.EditionId,
                EventId = edition.EventId,
                EventType = edition.Event.EventTypeCode.ToEnumFromDescription<EventType>(),
                LocalSqmSold = edition.LocalSqmSold,
                InternationalSqmSold = edition.InternationalSqmSold,
                SqmSold = edition.SqmSold,
                SponsorCount = edition.SponsorCount,
                EventActivity = edition.EventActivity,
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                CurrentUser = currentUser
            };
            return model;
        }

        public static EditionEditImagesModel ComposeEditionEditImagesModel(
            CedUser currentUser,
            EditionEntity edition,
            EditionTranslationEntity editionTranslation,
            bool isEditableForImages)
        {
            var model = new EditionEditImagesModel
            {
                EditionTranslationId = editionTranslation.EditionTranslationId,
                EditionId = edition.EditionId,
                EditionName = edition.EditionName,
                ProductImageFileName = editionTranslation.ProductImageFileName,
                MasterLogoFileName = editionTranslation.MasterLogoFileName,
                CrmLogoFileName = editionTranslation.CrmLogoFileName,
                IconFileName = editionTranslation.IconFileName,
                PromotedLogoFileName = editionTranslation.PromotedLogoFileName,
                DetailsImageFileName = editionTranslation.DetailsImageFileName,
                LanguageCode = editionTranslation.LanguageCode,
                IsAlive = edition.IsAlive(),
                IsCancelled = edition.IsCancelled(),
                IsEditableForImages = isEditableForImages,
                CurrentUser = currentUser
            };
            return model;
        }
    }
}