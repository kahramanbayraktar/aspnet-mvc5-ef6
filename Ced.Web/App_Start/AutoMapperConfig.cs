using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessEntities.Auth;
using Ced.BusinessEntities.Event;
using Ced.BusinessEntities.ExternalEntities;
using Ced.Data.Models;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using Ced.Utility.File;
using Ced.Utility.Web;
using Ced.Web.Models;
using Ced.Web.Models.AdminEdition;
using Ced.Web.Models.Edition;
using Ced.Web.Models.EditionTranslation;
using Ced.Web.Models.Event;
using Ced.Web.Models.EventDirector;
using Ced.Web.Models.Log;
using Ced.Web.Models.Notification;
using Ced.Web.Models.User;
using Ced.Web.Models.UserRole;
using ITE.Utility.Extensions;
using System;
using System.Linq;
using System.Web;
using Constants = Ced.Utility.Constants;
using NotificationType = Ced.BusinessEntities.NotificationType;

namespace Ced.Web
{
    public class AutoMapperConfig
    {
        public class TestProfile : Profile
        {
            public TestProfile()
            {
                CreateMap<Edition, EditionEntity>();
            }
        }

        public static void Register()
        {
            Mapper.Initialize(cfg =>
            {
                // DTO <-> POCO
                cfg.CreateMap<Event, EventEntity>()
                    .ForMember(dest => dest.Directors, src => src.MapFrom(x => x.EventDirectors.Where(ed => ed.ApplicationId == WebConfigHelper.ApplicationIdCed)));

                cfg.CreateMap<Event, EventEntityLight>();

                cfg.CreateMap<Edition, EditionEntity>()
                    .ForMember(dest => dest.DirectorEmail, src => src.MapFrom(x => x.Event.EventDirectors.FirstOrDefault(ed =>
                        ed.IsPrimary == true && ed.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorEmail))
                    .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status.ToEnum<EditionStatusType>()))
                    .ForMember(dest => dest.DirectorEmails, src => src.MapFrom(x => x.Event.EventDirectors.Select(d => d.DirectorEmail.ToLower()).Distinct()));
                cfg.CreateMap<Edition, EditionEntityLight>()
                    .ForMember(dest => dest.DirectorEmail, src => src.MapFrom(x =>
                        x.Event.EventDirectors.FirstOrDefault(d =>
                            d.IsPrimary == true &&
                            d.DirectorFullName.ToLower() == x.DirectorFullName.ToLower() &&
                            d.ApplicationId == WebConfigHelper.ApplicationIdCed).DirectorEmail))
                    .ForMember(dest => dest.EventId, src => src.MapFrom(x => x.Event.EventId))
                    .ForMember(dest => dest.EventName, src => src.MapFrom(x => x.Event.MasterName))
                    .ForMember(dest => dest.EventTypeCode, src => src.MapFrom(x => x.Event.EventTypeCode))
                    .ForMember(dest => dest.DirectorEmails, src => src.MapFrom(x => x.Event.EventDirectors.Select(d => d.DirectorEmail.ToLower()).Distinct()));

                cfg.CreateMap<EditionCohost, EditionCohostEntity>()
                    .ForMember(dest => dest.FirstEdition, src => src.MapFrom(x => x.Edition))
                    .ForMember(dest => dest.SecondEdition, src => src.MapFrom(x => x.Edition1));

                cfg.CreateMap<EditionCohostEntity, EditionCohost>()
                    .ForMember(dest => dest.Edition, src => src.MapFrom(x => x.FirstEdition))
                    .ForMember(dest => dest.Edition1, src => src.MapFrom(x => x.SecondEdition));

                cfg.CreateMap<EditionKeyVisitor, EditionKeyVisitorEntity>();
                cfg.CreateMap<EditionKeyVisitorEntity, EditionKeyVisitor>();

                // STAGING
                cfg.CreateMap<DWStaging.Edition, EventEntity>()
                    .ForMember(dest => dest.MasterCode, src => src.MapFrom(x => x.EventMasterCode))
                    .ForMember(dest => dest.MasterName, src => src.MapFrom(x => x.EventMaster))
                    .ForMember(dest => dest.Region, src => src.MapFrom(x => x.EventRegion))
                    .ForMember(dest => dest.Industry, src => src.MapFrom(x => x.EventIndustry))
                    .ForMember(dest => dest.SubIndustry, src => src.MapFrom(x => x.EventSubIndustry))
                    .ForMember(dest => dest.EventTypeCode, src => src.MapFrom(x => x.EVENTTYPEBEID));

                cfg.CreateMap<DWStaging.Edition, EditionEntity>()
                    .ForMember(dest => dest.EditionName, src => src.MapFrom(x => x.EventName))
                    // TODO: PATCH#001
                    .ForMember(dest => dest.EventTypeCode, src => src.MapFrom(x => x.EVENTTYPEBEID))
                    .ForMember(dest => dest.Classification, src => src.MapFrom(x => x.EventBandClassification))
                    .ForMember(dest => dest.Frequency, src => src.MapFrom(x => x.EventFrequency))
                    .ForMember(dest => dest.ManagingOfficeName, src => src.MapFrom(x => x.EventManagingOfficeName))
                    .ForMember(dest => dest.FinancialYearStart, src => src.MapFrom(x => Convert.ToInt32(x.EventFYStart)))
                    .ForMember(dest => dest.FinancialYearEnd, src => src.MapFrom(x => Convert.ToInt32(x.EventFYEnd)))
                    //.ForMember(dest => dest.DirectorEmail, src => src.MapFrom(x => x.EventDirectorEmail))
                    //.ForMember(dest => dest.DirectorFullName, src => src.MapFrom(x => x.EventDirector))
                    .ForMember(dest => dest.StartDate, src => src.MapFrom(x => x.EventStartDate.ToDate()))
                    .ForMember(dest => dest.EndDate, src => src.MapFrom(x => x.EventEndDate.ToDate()));

                cfg.CreateMap<DWStaging.Edition, EditionTranslationEntity>()
                    .ForMember(dest => dest.VenueName, src => src.MapFrom(x => x.EventVenue));

                // SELF MAPPING
                cfg.CreateMap<EventEntity, EventEntity>()
                    .ForMember(dest => dest.EventId, opt => opt.Ignore());

                cfg.CreateMap<EditionEntity, EditionEntity>()
                    .ForMember(dest => dest.AxEventId, opt => opt.Ignore())
                    .ForMember(dest => dest.EditionId, opt => opt.Ignore())
                    .ForMember(dest => dest.EventId, opt => opt.Ignore())
                    .ForMember(dest => dest.LocalName, opt => opt.Ignore())
                    .ForMember(dest => dest.InternationalName, opt => opt.Ignore())
                    .ForMember(dest => dest.ReportingName, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.Event, opt => opt.Ignore())
                    .ForMember(dest => dest.EditionTranslations, opt => opt.Ignore());

                cfg.CreateMap<EditionTranslationEntity, EditionTranslationEntity>()
                    .ForMember(dest => dest.EditionTranslationId, opt => opt.Ignore())
                    .ForMember(dest => dest.EditionId, opt => opt.Ignore())
                    .ForMember(dest => dest.LanguageCode, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    // TODO: Sadece Staging<->CED mapping'inde kullanılabilir!
                    .ForMember(dest => dest.BookStandUrl, opt => opt.Ignore())
                    .ForMember(dest => dest.Description, opt => opt.Ignore())
                    .ForMember(dest => dest.ExhibitorProfile, opt => opt.Ignore())
                    .ForMember(dest => dest.IconFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.MapVenueFullAddress, opt => opt.Ignore())
                    .ForMember(dest => dest.MasterLogoFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.CrmLogoFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.OnlineInvitationUrl, opt => opt.Ignore())
                    .ForMember(dest => dest.PeopleImageFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductImageFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.SocialMedias, opt => opt.Ignore())
                    .ForMember(dest => dest.Summary, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdateTime, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.VisitorProfile, opt => opt.Ignore())
                    .ForMember(dest => dest.WebLogoFileName, opt => opt.Ignore());

                cfg.CreateMap<Country, CountryEntity>();

                cfg.CreateMap<EditionEntity, Edition>()
                    // DO NOT REMOVE! The reason for this: The relationship could not be changed because one or more of the foreign-key properties is non-nullable.
                    .ForMember(dest => dest.Event, opt => opt.Ignore())
                    // Because these entities (Edition, Event, EditionTranslations) are seperately/one bu one updated. They must not be updated all in one/at once.
                    .ForMember(dest => dest.EditionTranslations, opt => opt.Ignore())
                    .ForMember(dest => dest.EventOwnershipBEID, src => src.MapFrom(x => x.EventOwnershipBEID))
                    .ForMember(dest => dest.EventOwnership, src => src.MapFrom(x => x.EventOwnership))
                    .ForMember(dest => dest.Status, src => src.MapFrom(x => (byte) x.Status.GetHashCode()));

                cfg.CreateMap<EditionCountryEntity, EditionCountry>()
                    .ForMember(dest => dest.EditionId, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
                cfg.CreateMap<EditionCountryEntity, EditionCountry>()
                    .ForMember(dest => dest.EditionId, src => src.MapFrom(x => x.EditionId))
                    .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

                cfg.CreateMap<EditionCountry, EditionCountryEntity>()
                    .ForMember(x => x.RelationType,
                        src => src.MapFrom(x => x.RelationType.ToEnum<BusinessEntities.EditionCountryRelationType>()));

                cfg.CreateMap<EventDirector, EventEntity>()
                    .ForMember(dest => dest.EventId, src => src.MapFrom(x => x.Event.EventId))
                    .ForMember(dest => dest.MasterCode, src => src.MapFrom(x => x.Event.MasterCode))
                    .ForMember(dest => dest.MasterName, src => src.MapFrom(x => x.Event.MasterName))
                    .ForMember(dest => dest.EventType, src => src.MapFrom(x => x.Event.EventType))
                    .ForMember(dest => dest.EventTypeCode, src => src.MapFrom(x => x.Event.EventTypeCode))
                    .ForMember(dest => dest.Industry, src => src.MapFrom(x => x.Event.Industry))
                    .ForMember(dest => dest.Region, src => src.MapFrom(x => x.Event.Region));

                cfg.CreateMap<EventDirector, EventDirectorEntity>()
                    .ForMember(dest => dest.EventName, src => src.MapFrom(x => x.Event.MasterName));

                cfg.CreateMap<EventDirectorEntity, EventDirectorListItemModel>();

                cfg.CreateMap<EditionDiscountApprover, EditionDiscountApproverEntity>();

                cfg.CreateMap<EditionPaymentSchedule, EditionPaymentScheduleEntity>();

                cfg.CreateMap<EditionSection, EditionSectionEntity>();

                cfg.CreateMap<EditionTranslationEntity, EditionTranslation>()
                    .ForMember(dest => dest.Edition, opt => opt.Ignore());
                cfg.CreateMap<EditionTranslation, EditionTranslationEntity>()
                    .ForMember(dest => dest.Summary, src => src.MapFrom(x => HttpUtility.HtmlDecode(x.Summary)))
                    .ForMember(dest => dest.Description, src => src.MapFrom(x => HttpUtility.HtmlDecode(x.Description)));

                cfg.CreateMap<EditionTranslation, EditionTranslationEntityLight>();

                cfg.CreateMap<EditionTranslation, EditionVenue>();

                cfg.CreateMap<EditionTranslationSocialMedia, EditionTranslationSocialMediaEntity>();

                cfg.CreateMap<EditionVisitor, EditionVisitorEntity>();
                cfg.CreateMap<EditionVisitorEntity, EditionVisitor>()
                    .ForMember(dest => dest.EditionVisitorId, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

                cfg.CreateMap<EditionEditExhibitorVisitorStatsModel, EditionTranslationEntity>()
                    .ForMember(dest => dest.EditionTranslationId, opt => opt.Ignore())
                    .ForMember(dest => dest.EditionId, opt => opt.Ignore());

                cfg.CreateMap<EventEntity, Event>()
                    .ForMember(dest => dest.Editions, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore());

                cfg.CreateMap<File, FileEntity>()
                    .ForMember(dest => dest.FileExtension,
                        src => src.MapFrom(x => System.IO.Path.GetExtension(x.FileName)))
                    .ForMember(dest => dest.FileTypeIcon, src => src.MapFrom(x => x.GetFileTypeIcon()))
                    .ForMember(dest => dest.EditionFileType,
                        src => src.MapFrom(x => x.FileType.ToLower().ToEnumFromDescription<EditionFileType>()));

                cfg.CreateMap<KeyVisitor, KeyVisitorEntity>();
                cfg.CreateMap<KeyVisitorEntity, KeyVisitor>();

                cfg.CreateMap<Log, LogEntity>()
                    .ForMember(dest => dest.EntityType, src => src.MapFrom(x => x.EntityType.ToEnum<EntityType>()))
                    .ForMember(dest => dest.ActionType, src => src.MapFrom(x => x.ActionType.ToEnum<ActionType>()));
                cfg.CreateMap<LogEntity, Log>()
                    .ForMember(dest => dest.EntityType, src => src.MapFrom(x => x.EntityType.ToString()))
                    .ForMember(dest => dest.ActionType, src => src.MapFrom(x => x.ActionType.ToString()));

                cfg.CreateMap<Log, LogEntityLight>();

                cfg.CreateMap<Notification, NotificationEntity>()
                    .ForMember(dest => dest.NotificationType, src => src.MapFrom(x => x.NotificationType.ToEnum<NotificationType>()));
                cfg.CreateMap<NotificationEntity, Notification>()
                    .ForMember(dest => dest.NotificationType, src => src.MapFrom(x => (byte) x.NotificationType));

                cfg.CreateMap<SocialMedia, SocialMediaEntity>();

                cfg.CreateMap<Subscription, SubscriptionEntity>();

                // DTO <-> DTO
                cfg.CreateMap<EditionTranslationEntityLight, EditionTranslationEntity>();

                cfg.CreateMap<EditionCloneModel, EditionApprovalModel>()
                    .ForMember(dest => dest.StartDate, src => src.MapFrom(x => x.StartDate.GetValueOrDefault().ToShortDateString()))
                    .ForMember(dest => dest.EndDate, src => src.MapFrom(x => x.EndDate.GetValueOrDefault().ToShortDateString()))
                    .ForMember(dest => dest.VisitStartTime, src => src.MapFrom(x => x.VisitStartTime.GetValueOrDefault().ToString("g")))
                    .ForMember(dest => dest.VisitEndTime, src => src.MapFrom(x => x.VisitEndTime.GetValueOrDefault().ToString("g")));

                // POCO <-> ViewModel
                cfg.CreateMap<EditionCloneModel, EditionEntity>()
                    .ForMember(dest => dest.Frequency, opt => opt.Ignore());
                cfg.CreateMap<EditionEntity, EditionCloneModel>()
                    .ForMember(dest => dest.EventType, src => src.MapFrom(x => x.Event.EventType))
                    .ForMember(dest => dest.Industry, src => src.MapFrom(x => x.Event.Industry));

                cfg.CreateMap<EditionCloneModel, EditionTranslationEntity>()
                    .ForMember(dest => dest.Description, opt => opt.Ignore())
                    .ForMember(dest => dest.Summary, opt => opt.Ignore());

                cfg.CreateMap<EditionEntityLight, EditionListModel>()
                    .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status.ToEnum<EditionStatusType>()))
                    .ForMember(dest => dest.EventActivity, src => src.MapFrom(x => x.EventActivity.ToEnumFromDescription<EventActivity>()))
                    .ForMember(d => d.IsClonable, o => o.ResolveUsing((src, dest, destMember, resContext) => dest.IsClonable = src.IsClonable((CedUser) resContext.Items["CurrentUser"])))
                    .ForMember(d => d.IsClonableDesc, o => o.ResolveUsing((src, dest, destMember, resContext) => dest.IsClonableDesc = src.ReasonsForNotBeingClonable((CedUser)resContext.Items["CurrentUser"]).ToCommaSeparatedString()))
                    .ForMember(d => d.IsEditable, o => o.ResolveUsing((src, dest, destMember, resContext) => dest.IsEditable = src.IsEditable((CedUser) resContext.Items["CurrentUser"])))
                    .ForMember(d => d.IsEditableDesc, o => o.ResolveUsing((src, dest, destMember, resContext) => dest.IsEditableDesc = src.ReasonsForNotBeingEditable((CedUser) resContext.Items["CurrentUser"]).ToCommaSeparatedString()));

                cfg.CreateMap<EditionEntityLight, AdminEditionListItemModel>()
                    .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status.ToEnum<EditionStatusType>().GetDescription()))
                    .ForMember(dest => dest.EventActivity, src => src.MapFrom(x => x.EventActivity.ToEnum<EventActivity>().GetDescription()));

                cfg.CreateMap<EditionEntity, AdminEditionDetailsModel>()
                    .ForMember(dest => dest.EventName, src => src.MapFrom(x => x.Event.MasterName))
                    .ForMember(dest => dest.Status, src => src.MapFrom(x => x.Status.ToEnum<EditionStatusType>().GetDescription()))
                    .ForMember(dest => dest.EventActivity, src => src.MapFrom(x => x.EventActivity.ToEnum<EventActivity>().GetDescription()))
                    .ForMember(dest => dest.EventType, src => src.MapFrom(x => x.Event.EventType.ToEnum<NotificationType>()));

                cfg.CreateMap<EditionTranslationEditModel, EditionTranslationEntity>();
                cfg.CreateMap<EditionTranslationEntity, EditionTranslationEditModel>();

                cfg.CreateMap<EditionEditGeneralInfoModel, EditionEntity>()
                    .ForMember(dest => dest.DisplayOnIteAsia, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteI, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteGermany, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteTurkey, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnTradeLink, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnItePoland, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteModa, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteUkraine, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteBuildInteriors, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteFoodDrink, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteOilGas, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteTravelTourism, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteTransportLogistics, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteFashion, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteSecurity, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteBeauty, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteHealthCare, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteMining, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.DisplayOnIteEngineeringIndustrial, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.HiddenFromWebSites, src => src.Condition(x => x.DisplayOnTypeFieldsMappable))
                    .ForMember(dest => dest.TradeShowConnectDisplay, src => src.Condition(x => x.DisplayOnTypeFieldsMappable));
                cfg.CreateMap<EditionEditSalesMetricsModel, EditionEntity>();
                cfg.CreateMap<EditionEditExhibitorVisitorStatsModel, EditionEntity>()
                    .ForMember(dest => dest.TopExhibitorCountries, src => src.MapFrom(x => x.TopExhibitorCountries != null ? string.Join(",", x.TopExhibitorCountries) : ""))
                    .ForMember(dest => dest.TopVisitorCountries, src => src.MapFrom(x => x.TopVisitorCountries != null ? string.Join(",", x.TopVisitorCountries) : ""))
                    .ForMember(dest => dest.DelegateCountries, src => src.MapFrom(x => x.DelegateCountries != null ? string.Join(",", x.DelegateCountries) : ""))
                    .ForMember(dest => dest.LocalDelegateCount, src => src.Condition(x => Constants.EventTypesWithDelegates.Contains(x.EventType)))
                    .ForMember(dest => dest.InternationalDelegateCount, src => src.Condition(x => Constants.EventTypesWithDelegates.Contains(x.EventType)))
                    .ForMember(dest => dest.LocalPaidDelegateCount, src => src.Condition(x => Constants.EventTypesWithDelegates.Contains(x.EventType)))
                    .ForMember(dest => dest.InternationalPaidDelegateCount, src => src.Condition(x => Constants.EventTypesWithDelegates.Contains(x.EventType)))
                    ;
                cfg.CreateMap<EditionEditPostShowMetricsModel, EditionEntity>();

                cfg.CreateMap<EditionEditGeneralInfoModel, EditionTranslationEntity>()
                    .ForMember(dest => dest.EditionTranslationId, opt => opt.Ignore())
                    .ForMember(dest => dest.WebLogoFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.PeopleImageFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductImageFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.MasterLogoFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.CrmLogoFileName, opt => opt.Ignore())
                    .ForMember(dest => dest.IconFileName, opt => opt.Ignore());

                cfg.CreateMap<EventEntity, EventListModel>()
                    .ForMember(dest => dest.EventType, src => src.MapFrom(x => x.EventType.ToEnum<NotificationType>()))
                    .ForMember(dest => dest.EventTypeName, src => src.MapFrom(x => x.EventType));
                cfg.CreateMap<EventEntity, EventEditModel>();
                cfg.CreateMap<EventEditModel, EventEntity>()
                    .ForMember(dest => dest.MasterCode, opt => opt.Ignore())
                    .ForMember(dest => dest.MasterName, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                    .ForMember(dest => dest.AxRecId, opt => opt.Ignore())
                    //.ForMember(dest => dest.CityId, opt => opt.Ignore())
                    //.ForMember(dest => dest.City, opt => opt.Ignore())
                    //.ForMember(dest => dest.CountryId, opt => opt.Ignore())
                    //.ForMember(dest => dest.Country, opt => opt.Ignore())
                    .ForMember(dest => dest.CreateTime, opt => opt.Ignore())
                    .ForMember(dest => dest.EventType, opt => opt.Ignore())
                    .ForMember(dest => dest.IndustryId, opt => opt.Ignore())
                    .ForMember(dest => dest.RegionId, opt => opt.Ignore());

                cfg.CreateMap<LogEntity, LogDetailsModel>()
                    .ForMember(dest => dest.ActionType, src => src.MapFrom(x => x.ActionType.ToString()));

                cfg.CreateMap<NotificationEntity, NotificationListItemModel>()
                    .ForMember(dest => dest.Type, src => src.MapFrom(x => x.NotificationType.ToEnum<NotificationType>()))
                    .ForMember(dest => dest.FaIcon, src => src.MapFrom(x => x.NotificationType.ToEnum<NotificationType>().GetAttribute<NotificationAttribute>().FaIcon))
                    .ForMember(dest => dest.TextClass, src => src.MapFrom(x => x.NotificationType.ToEnum<NotificationType>().GetAttribute<NotificationAttribute>().TextClass));

                cfg.CreateMap<EventEditionCustomType, RecentViewListModel>()
                    .ForMember(dest => dest.Title, src => src.MapFrom(x => x.MasterName))
                    .ForMember(dest => dest.EntityId, src => src.MapFrom(x => x.EventId));

                cfg.CreateMap<UserEntity, UserListModel>();
                cfg.CreateMap<UserListModel, UserEntity>();

                cfg.CreateMap<UserRoleEntity, UserRoleListItemModel>();
                cfg.CreateMap<UserRoleListModel, UserRoleEntity>();

                cfg.CreateMap<UserRoleEntity, UserRoleEditModel>()
                    .ForMember(dest => dest.UserRoleId, src => src.MapFrom(x => x.UserRoleId));
                cfg.CreateMap<UserRoleEditModel, UserRoleEntity>()
                   .ForMember(dest => dest.UserRoleId, src => src.MapFrom(x => x.UserRoleId));

                // API
                cfg.CreateMap<EditionTranslation, Wb365ApiEventEntity>()
                    .ForMember(dest => dest.EventId, src => src.MapFrom(x => x.Edition.EventId))
                    .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Edition.EditionName))
                    .ForMember(dest => dest.LangCode, src => src.MapFrom(x => x.LanguageCode))
                    .ForMember(dest => dest.LangId, src => src.MapFrom(x => x.LanguageCode.ToLower() == LanguageHelper.Languages.English.GetDescription().ToLower() ? "1" : 
                        x.LanguageCode.ToLower() == LanguageHelper.Languages.Russian.GetDescription().ToLower() ? "2" : null))
                    .ForMember(dest => dest.ShortDescription, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.Summary) ? x.Summary : ""))
                    .ForMember(dest => dest.Country, src => src.MapFrom(x => x.Edition.Country))
                    .ForMember(dest => dest.CountryCode, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.Edition.CountryCode) ? x.Edition.Country1.CountryCode2 : ""))
                    .ForMember(dest => dest.City, src => src.MapFrom(x => x.Edition.City))
                    .ForMember(dest => dest.Venue, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.VenueName) ? x.VenueName : ""))
                    .ForMember(dest => dest.WebsiteLink, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.Edition.EventWebSite) ? x.Edition.EventWebSite : ""))
                    .ForMember(dest => dest.HeroImage, src => src.MapFrom(x => !string.IsNullOrWhiteSpace(x.PeopleImageFileName) ? EditionImageType.PeopleImage.BlobSubDirectory() + x.PeopleImageFileName : ""))
                    .ForMember(dest => dest.StartDate, src => src.MapFrom(x => "/Date(" + x.Edition.StartDate.GetValueOrDefault().ToUnixTimestamp() + ")/"))
                    .ForMember(dest => dest.EndDate, src => src.MapFrom(x => "/Date(" + x.Edition.EndDate.GetValueOrDefault().ToUnixTimestamp() + ")/"))
                    .ForMember(dest => dest.TcDisplay, src => src.MapFrom(x => x.Edition.TradeShowConnectDisplay));

                cfg.CreateMap<Edition, WisentApiEventEntity>()
                    .ForMember(dest => dest.EventBEID, src => src.MapFrom(x => x.AxEventId))
                    .ForMember(dest => dest.EventName, src => src.MapFrom(x => x.InternationalName))
                    .ForMember(dest => dest.Country, src => src.MapFrom(x => x.Country))
                    .ForMember(dest => dest.CountryCode, src => src.MapFrom(x => x.CountryCode))
                    .ForMember(dest => dest.City, src => src.MapFrom(x => x.City))
                    .ForMember(dest => dest.StartDate, src => src.MapFrom(x => x.StartDate.GetValueOrDefault().ToShortDateString()))
                    .ForMember(dest => dest.EndDate, src => src.MapFrom(x => x.EndDate.GetValueOrDefault().ToShortDateString()))
                    .ForMember(dest => dest.FinancialYearStart, src => src.MapFrom(x => x.FinancialYearStart));

                // INTEGRATION TEST
                cfg.CreateMap<Edition, EditionEditGeneralInfoModel>();

                // AUTH
                cfg.CreateMap<Application, ApplicationEntity>();
                cfg.CreateMap<Industry, IndustryEntity>();
                cfg.CreateMap<Region, RegionEntity>();
                cfg.CreateMap<Role, RoleEntity>();
                cfg.CreateMap<UserRole, UserRoleEntity>()
                    .ForMember(dest => dest.CountryName, src => src.MapFrom(x => x.Country.CountryName));
                cfg.CreateMap<User, UserEntity>();
            });
        }
    }
}