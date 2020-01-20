using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.Utility;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Models;
using Ced.Web.Models.Dashboard;
using ITE.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Constants = Ced.Utility.Constants;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class DashboardController : GlobalController
    {
        public DashboardController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEditionCountryServices editionCountryServices,
            IEditionTranslationServices editionTranslationServices,
            IEventDirectorServices eventDirectorServices,
            IEventServices eventServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IStatisticServices statisticServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionCountryServices, editionServices, editionTranslationServices, eventDirectorServices, eventServices, logServices,
                notificationServices, statisticServices, userServices, userRoleServices)
        {
        }

        [CedAction(ActionType = ActionType.EventDashboard, Loggable = true)]
        public ActionResult Index(int? id = null)
        {
            var anyEventSelected = id.HasValue;

            if (id.HasValue)
            {
                if (!IsDirectorAuthorizedOnEvent(id.Value))
                    return View("Unauthorized");
            }

            var pastEvents = EventServices.GetPastEventsByDirector(
                CurrentCedUser.CurrentUser.Email,
                WebConfigHelper.MinFinancialYear,
                Constants.DefaultValidEditionStatusesForCed,
                Constants.ValidEventTypesForCed,
                3,
                20)
                .OrderBy(x => x.MasterName).ToList();

            foreach (var @event in pastEvents)
            {
                @event.DisplayDate = DateHelper.GetDisplayDate(@event.StartDate, @event.EndDate);
            }

            if (!pastEvents.Any())
            {
                var allEvents = EventServices.GetEventLightsByDirector(
                    CurrentCedUser.CurrentUser.Email,
                    Constants.ValidEventTypesForCed,
                    WebConfigHelper.MinFinancialYear)
                    .ToList();

                if (allEvents.Any())
                {
                    id = allEvents.First().EventId;
                }
                else
                {
                    return View("Unauthorized");
                }
            }

            if (id == null)
                id = pastEvents.First().EventId;

            var selectedEvent = EventServices.GetEventById(id.Value);
            if (selectedEvent == null)
                return View("NotFound");

            var lastEdition = EditionServices.GetLastFinishedEdition(selectedEvent.EventId);
            if (lastEdition == null)
            {
                var nextEdition = EditionServices.GetClosestEdition(selectedEvent.EventId, Constants.ValidEventTypesForCed);
                if (nextEdition == null)
                    return View("Error", new ErrorModel { Message = $"Next edition not found for {selectedEvent.EventId} | {selectedEvent.MasterName}!" });

                return View(new IndexModel
                {
                    EventId = selectedEvent.EventId,
                    EventName = selectedEvent.MasterName,
                    EventType = selectedEvent.EventTypeCode.ToEnumFromDescription<EventType>(),
                    EventDisplayDate = selectedEvent.DisplayDate,
                    SelectedEditionId = nextEdition.EditionId,
                    SelectedEditionName = nextEdition.EditionName,
                    NextEditionStartDate = nextEdition.StartDate,
                    EventEditionList = pastEvents
                });
            }

            selectedEvent.DisplayDate = DateHelper.GetDisplayDate(lastEdition.StartDate, lastEdition.EndDate);

            var lastEditionTranslation = EditionTranslationServices.GetEditionTranslationLight(lastEdition.EditionId, LanguageHelper.GetBaseLanguageCultureName());

            var internationalExhibitorPavilionCounts = StatisticServices.GetInternationalExhibitorPavilionCounts(selectedEvent.EventId, 3);
            var exhibitorCountryCounts = StatisticServices.GetExhibitorCountryCounts(selectedEvent.EventId, 3);
            var visitorCountryCounts = StatisticServices.GetVisitorCountryCounts(selectedEvent.EventId, 3);

            var localExhibitorRetentionRates = StatisticServices.GetExhibitorRetentionRatesLocal(selectedEvent.EventId, 3);
            var internationalExhibitorRetentionRates = StatisticServices.GetExhibitorRetentionRatesInternational(selectedEvent.EventId, 3);

            var editionCountriesExhibitor = EditionCountryServices.GetEditionCountriesByEdition(lastEdition.EditionId, EditionCountryRelationType.Exhibitor);
            var editionCountriesVisitor = EditionCountryServices.GetEditionCountriesByEdition(lastEdition.EditionId, EditionCountryRelationType.Visitor);

            var exhibitorCountryNames =
                Countries.Where(c => editionCountriesExhibitor.Any(ec => ec.CountryCode == c.CountryCode))
                    .Select(c => c.CountryName)
                    .ToList();

            var visitorCountryNames =
                Countries.Where(c => editionCountriesVisitor.Any(ec => ec.CountryCode == c.CountryCode))
                    .Select(c => c.CountryName)
                    .ToList();

            var delegateCountryNames = new List<string>();
            var lastEditionEvent = lastEdition.Event ?? EventServices.GetEventById(lastEdition.EventId);
            if (Constants.ConferenceEventTypes.Contains(lastEditionEvent.EventTypeCode))
            {
                var editionCountriesDelegate = EditionCountryServices.GetEditionCountriesByEdition(lastEdition.EditionId, EditionCountryRelationType.Delegate);
                delegateCountryNames = editionCountriesDelegate.Any()
                    ? Countries.Where(c => editionCountriesExhibitor.Any(ec => ec.CountryCode == c.CountryCode))
                        .Select(c => c.CountryName)
                        .ToList()
                    : new List<string>();
            }

            var model = new IndexModel
            {
                EventId = selectedEvent.EventId,
                EventName = selectedEvent.MasterName,
                EventType = selectedEvent.EventTypeCode.ToEnumFromDescription<EventType>(),
                EventDisplayDate = selectedEvent.DisplayDate,
                SelectedEditionId = lastEdition.EditionId,
                SelectedEditionName = lastEdition.EditionName,
                EventEditionList = pastEvents,
                EditionStatModel = new EditionStatModel
                {
                    ExhibitorCountries = exhibitorCountryNames,
                    VisitorCountries = visitorCountryNames,
                    DelegateCountries = delegateCountryNames,
                    LocalExhibitorRetentionRates = localExhibitorRetentionRates,
                    InternationalExhibitorRetentionRates = internationalExhibitorRetentionRates,
                    NumberOfInternationalPavilions = internationalExhibitorPavilionCounts,
                    NumberOfExhibitorCountries = exhibitorCountryCounts,
                    NumberOfVisitorCountries = visitorCountryCounts,
                    BookAStandUrl = lastEditionTranslation.BookStandUrl,
                    VisitorETicketUrl = lastEditionTranslation.OnlineInvitationUrl
                }
            };

            // Additions to LOG object
            if (TempData["Log"] is LogEntity log)
            {
                if (log.EntityId == null || log.EntityId == 0)
                {
                    log.EntityId = selectedEvent.EventId;
                    log.EntityName = selectedEvent.MasterName;
                    log.EventId = selectedEvent.EventId;
                    log.EventName = selectedEvent.MasterName;
                }
                if (anyEventSelected) // If an event selected intentionally.
                    log.AdditionalInfo = "{ 'WebLogoFileName': '" + lastEditionTranslation.WebLogoFileName + "' }";
            }

            return View(model);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _SqmSales(int id)
        {
            var salesList = StatisticServices.GetSqmSales(id).OrderBy(x => x.Year).ToList();

            var barChart = new
            {
                Years = salesList.Select(x => x.Year).ToArray(),
                LocalSqms = salesList.Select(x => x.LocalNumber).ToArray(),
                IntlSqms = salesList.Select(x => x.InternationalNumber).ToArray(),
                TotalSqms = salesList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _ActualVisitorRatio(int id)
        {
            var exhibitorList = StatisticServices.GetActualVisitorRatio(id).OrderBy(x => x.Year).ToList();

            if (!exhibitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = exhibitorList.Select(x => x.Year).ToArray(),
                TotalNumbers = exhibitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfSponsorships(int id)
        {
            var sponsorshipList = StatisticServices.GetNumberOfSponsorships(id).OrderBy(x => x.Year).ToList();

            var barChart = new
            {
                Years = sponsorshipList.Select(x => x.Year).ToArray(),
                TotalNumbers = sponsorshipList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly] 
        [NoNeedForUserData]
        public ActionResult _NumberOfExhibitors(int id)
        {
            var exhibitorList = StatisticServices.GetNumberOfExhibitors(id).OrderBy(x => x.Year).ToList();

            var barChart = new
            {
                Years = exhibitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = exhibitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = exhibitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = exhibitorList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfVisitors(int id)
        {
            var visitorList = StatisticServices.GetNumberOfVisitors(id).OrderBy(x => x.Year).ToList();

            if (!visitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _SqmPerExhibitor(int id)
        {
            var exhibitorList = StatisticServices.GetSqmPerExhibitor(id).OrderBy(x => x.Year).ToList();

            if (!exhibitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = exhibitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = exhibitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = exhibitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = exhibitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfVisitorsPerSqm(int id)
        {
            var visitorList = StatisticServices.GetNumberOfVisitorsPerSqm(id).OrderBy(x => x.Year).ToList();

            if (!visitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                //LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                //IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfVisitorsPerExhibitor(int id)
        {
            var visitorList = StatisticServices.GetNumberOfVisitorsPerExhibitor(id).OrderBy(x => x.Year).ToList();

            if (!visitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfVisitorsPerSqmAndDay(int id)
        {
            var visitorList = StatisticServices.GetNumberOfVisitorsPerSqmAndDay(id).OrderBy(x => x.Year).ToList();

            if (!visitorList.Any())
                return Json(new { Success = false, JsonRequestBehavior.AllowGet });

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                //LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                //IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray(),
                Success = true
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfDelegates(int id)
        {
            var visitorList = StatisticServices.GetNumberOfDelegates(id).OrderBy(x => x.Year).ToList();

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _NumberOfPaidDelegates(int id)
        {
            var visitorList = StatisticServices.GetNumberOfPaidDelegates(id).OrderBy(x => x.Year).ToList();

            var barChart = new
            {
                Years = visitorList.Select(x => x.Year).ToArray(),
                LocalNumbers = visitorList.Select(x => x.LocalNumber).ToArray(),
                IntlNumbers = visitorList.Select(x => x.InternationalNumber).ToArray(),
                TotalNumbers = visitorList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _VisitorNetPromoterScores(int id)
        {
            var scoreList = StatisticServices.GetVisitorNetPromoterScores(id).OrderByDescending(x => x.Year).ToList();

            var barChart = new
            {
                Years = scoreList.Select(x => x.Year).ToArray(),
                TotalNumbers = scoreList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _ExhibitorNetPromoterScores(int id)
        {
            var scoreList = StatisticServices.GetExhibitorNetPromoterScores(id).OrderByDescending(x => x.Year).ToList();

            var barChart = new
            {
                Years = scoreList.Select(x => x.Year).ToArray(),
                TotalNumbers = scoreList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _VisitorSatisfactionNetPromoterScores(int id)
        {
            var scoreList = StatisticServices.GetVisitorSatisfactionNetPromoterScores(id).OrderByDescending(x => x.Year).ToList();

            var barChart = new
            {
                Years = scoreList.Select(x => x.Year).ToArray(),
                TotalNumbers = scoreList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }

        [AjaxOnly]
        [NoNeedForUserData]
        public ActionResult _ExhibitorSatisfactionNetPromoterScores(int id)
        {
            var scoreList = StatisticServices.GetExhibitorSatisfactionNetPromoterScores(id).OrderByDescending(x => x.Year).ToList();

            var barChart = new
            {
                Years = scoreList.Select(x => x.Year).ToArray(),
                TotalNumbers = scoreList.Select(x => x.TotalNumber).ToArray()
            };

            return Json(barChart, JsonRequestBehavior.AllowGet);
        }
    }
}