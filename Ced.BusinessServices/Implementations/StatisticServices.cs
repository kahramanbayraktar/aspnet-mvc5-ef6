using Ced.BusinessEntities;
using Ced.BusinessServices.Models;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class StatisticServices : IStatisticServices
    {
        private readonly UnitOfWork _unitOfWork;

        public StatisticServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        private IQueryable<Edition> StatsQuery(int eventId, int? count = null, int? maxYear = null)
        {
            if (count == null) count = 3;
            if (maxYear == null) maxYear = DateTime.Now.Year;
            var statusId = EditionStatusType.Published.GetHashCode();

            var query = _unitOfWork.EditionRepository
                .GetManyQueryable(edition =>
                    edition.EventId == eventId
                    && DateTime.Now > (edition.EndDate ?? DateTime.MinValue)
                    && (edition.StartDate.HasValue ? edition.StartDate.Value.Year : DateTime.MinValue.Year) <= maxYear
                    && edition.Status == statusId)
                .OrderByDescending(edition => edition.StartDate.Value.Year)
                .Take(count.Value);
            return query;
        }

        public IList<EditionStat> GetSqmSales(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var sqmSalesList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalSqmSold ?? 0,
                    InternationalNumber = edition.InternationalSqmSold ?? 0,
                    TotalNumber = (edition.LocalSqmSold ?? 0) + (edition.InternationalSqmSold ?? 0)
                }).ToList();

            return sqmSalesList;
        }

        public IList<EditionStat> GetActualVisitorRatio(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var exhibitorList = query
                .AsEnumerable()
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate?.Year ?? 0),
                    LocalNumber = 0,
                    InternationalNumber = 0,
                    TotalNumber = edition.OnlineRegistrationCount > 0 ? Math.Round((Convert.ToDecimal(edition.OnlineRegisteredVisitorCount) / edition.OnlineRegistrationCount) ?? 0, 2) : 0
                }).ToList();

            return exhibitorList;
        }

        public IList<EditionStat> GetNumberOfSponsorships(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var sponsorshipList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = 0,
                    InternationalNumber = 0,
                    TotalNumber = edition.SponsorCount ?? 0
                }).ToList();

            return sponsorshipList;
        }

        public IList<EditionStat> GetNumberOfExhibitors(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var exhibitorList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalExhibitorCount ?? 0,
                    InternationalNumber = edition.InternationalExhibitorCount ?? 0,
                    TotalNumber = (edition.LocalExhibitorCount ?? 0) + (edition.InternationalExhibitorCount ?? 0)
                }).ToList();

            return exhibitorList;
        }

        public IList<EditionStat> GetNumberOfVisitors(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalVisitorCount ?? 0,
                    InternationalNumber = edition.InternationalVisitorCount ?? 0,
                    TotalNumber = (edition.LocalVisitorCount ?? 0) + (edition.InternationalVisitorCount ?? 0)
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetSqmPerExhibitor(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var exhibitorList = query
                .AsEnumerable()
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate?.Year ?? 0),
                    LocalNumber = edition.LocalExhibitorCount > 0 ? Convert.ToInt32((edition.LocalSqmSold / edition.LocalExhibitorCount) ?? 0) : 0,
                    InternationalNumber = edition.InternationalExhibitorCount > 0 ? Convert.ToInt32((edition.InternationalSqmSold / edition.InternationalExhibitorCount) ?? 0) : 0,
                    TotalNumber = edition.LocalExhibitorCount + edition.InternationalExhibitorCount > 0 ? Convert.ToInt32(((edition.LocalSqmSold + edition.InternationalSqmSold) / (edition.LocalExhibitorCount + edition.InternationalExhibitorCount)) ?? 0) : 0
                }).ToList();

            return exhibitorList;
        }

        public IList<EditionStat> GetNumberOfVisitorsPerSqm(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalSqmSold > 0 ? Math.Round((edition.LocalVisitorCount / edition.LocalSqmSold) ?? 0, 2) : 0,
                    InternationalNumber = edition.InternationalSqmSold > 0 ? Math.Round((edition.InternationalVisitorCount / edition.InternationalSqmSold) ?? 0, 2) : 0,
                    TotalNumber = edition.LocalSqmSold + edition.InternationalSqmSold > 0 ? Math.Round(((edition.LocalVisitorCount + edition.InternationalVisitorCount) / (edition.LocalSqmSold + edition.InternationalSqmSold)) ?? 0, 2) : 0
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetNumberOfVisitorsPerExhibitor(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .AsEnumerable()
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate?.Year ?? 0),
                    LocalNumber = edition.LocalExhibitorCount > 0 ? Convert.ToInt32((edition.LocalVisitorCount / edition.LocalExhibitorCount) ?? 0) : 0,
                    InternationalNumber = edition.InternationalExhibitorCount > 0 ? Convert.ToInt32((edition.InternationalVisitorCount / edition.InternationalExhibitorCount) ?? 0) : 0,
                    TotalNumber = edition.LocalExhibitorCount + edition.InternationalExhibitorCount > 0 ? Convert.ToInt32(((edition.LocalVisitorCount + edition.InternationalVisitorCount) / (edition.LocalExhibitorCount + edition.InternationalExhibitorCount)) ?? 0) : 0
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetNumberOfVisitorsPerSqmAndDay(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .AsEnumerable()
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate?.Year ?? 0),
                    LocalNumber = edition.LocalExhibitorCount > 0 ? Math.Round(Convert.ToDecimal(edition.LocalVisitorCount) / edition.LocalSqmSold / ((edition.EndDate - edition.StartDate != null ? (edition.EndDate - edition.StartDate).Value.Days : 0) + 1) ?? 0, 2) : 0,
                    InternationalNumber = edition.InternationalExhibitorCount > 0 ? Math.Round(Convert.ToDecimal(edition.InternationalVisitorCount) / edition.InternationalSqmSold / ((edition.EndDate - edition.StartDate != null ? (edition.EndDate - edition.StartDate).Value.Days : 0) + 1) ?? 0, 2) : 0,
                    TotalNumber = edition.LocalExhibitorCount + edition.InternationalExhibitorCount > 0 ? Math.Round(Convert.ToDecimal(edition.LocalVisitorCount + edition.InternationalVisitorCount) / (edition.LocalSqmSold + edition.InternationalSqmSold) / ((edition.EndDate - edition.StartDate != null ? (edition.EndDate - edition.StartDate).Value.Days : 0) + 1) ?? 0, 2) : 0
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetNumberOfDelegates(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalDelegateCount ?? 0,
                    InternationalNumber = edition.InternationalDelegateCount ?? 0,
                    TotalNumber = (edition.LocalDelegateCount ?? 0) + (edition.InternationalDelegateCount ?? 0)
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetNumberOfPaidDelegates(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var visitorList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    LocalNumber = edition.LocalPaidDelegateCount ?? 0,
                    InternationalNumber = edition.InternationalPaidDelegateCount ?? 0,
                    TotalNumber = (edition.LocalPaidDelegateCount ?? 0) + (edition.InternationalPaidDelegateCount ?? 0)
                }).ToList();

            return visitorList;
        }

        public IList<EditionStat> GetTopVisitorCountries(int eventId, int? count = null, int? maxYear = null)
        {
            //if (count == null) count = 3;
            //if (maxYear == null) maxYear = DateTime.Now.Year;

            //var countryList = _unitOfWork.EditionRepository
            //    .GetManyQueryable(edition => edition.EventId == eventId
            //          && DateTime.Now > edition.EndDate.GetValueOrDefault()
            //          && edition.Year <= maxYear)
            //    .OrderByDescending(edition => edition.Year)
            //    .Take(count.Value)
            //    .Select(edition => new EditionStat
            //    {
            //        EventId = edition.EventId,
            //        EditionNo = edition.EditionNo,
            //        Year = edition.Year.GetValueOrDefault(),
            //        Countries = edition.TopVisitorCountries
            //    }).ToList();

            //return countryList;
            return new List<EditionStat>();
        }

        public IList<EditionStat> GetInternationalExhibitorPavilionCounts(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            //var query = _unitOfWork.EditionRepository
            //    .GetManyQueryable(edition => edition.EventId == eventId
            //                                 && DateTime.Now > edition.EndDate.GetValueOrDefault()
            //                                 && edition.StartDate.GetValueOrDefault().Year <= maxYear);
            //query = query
            //    .OrderByDescending(edition => edition.StartDate.GetValueOrDefault().Year);
            //var scoreList = query
            //    .Take(count.Value)
            //    .Select(edition => new EditionStat
            //    {
            //        EventId = edition.EventId,
            //        EditionNo = edition.EditionNo,
            //        Year = (short)edition.StartDate.GetValueOrDefault().Year,
            //        TotalNumber = edition.NationalGroupCount ?? 0
            //    }).ToList();

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.NationalGroupCount ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetExhibitorCountryCounts(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.ExhibitorCountryCount ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetVisitorCountryCounts(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.VisitorCountryCount ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetVisitorNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.NPSScoreVisitor ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetExhibitorNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.NPSScoreExhibitor ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetVisitorSatisfactionNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.NPSSatisfactionVisitor ?? 0
                }).ToList();

            return scoreList;
        }

        public IList<EditionStat> GetExhibitorSatisfactionNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var scoreList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.NPSSatisfactionExhibitor ?? 0
                }).ToList();

            return scoreList;
        }

        //public IList<EditionStat> GetVisitorAverageNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        //{
        //    if (count == null) count = 3;
        //    if (maxYear == null) maxYear = DateTime.Now.Year;

        //    var scoreList = UnitOfWork.EditionRepository
        //        .GetManyQueryable(edition => edition.EventId == eventId && DateTime.Now > edition.EndDate.GetValueOrDefault() && edition.Year <= maxYear)
        //        .OrderByDescending(edition => edition.Year)
        //        .Take(count.Value)
        //        .Select(edition => new EditionStat
        //        {
        //            EventId = edition.EventId,
        //            EditionNo = edition.EditionNo,
        //            Year = edition.Year.GetValueOrDefault(),
        //            TotalNumber = edition.NPSAverageVisitor.GetValueOrDefault()
        //        }).ToList();

        //    return scoreList;
        //}

        //public IList<EditionStat> GetExhibitorAverageNetPromoterScores(int eventId, int? count = null, int? maxYear = null)
        //{
        //    if (count == null) count = 3;
        //    if (maxYear == null) maxYear = DateTime.Now.Year;

        //    var scoreList = UnitOfWork.EditionRepository
        //        .GetManyQueryable(edition => edition.EventId == eventId && DateTime.Now > edition.EndDate.GetValueOrDefault() && edition.Year <= maxYear)
        //        .OrderByDescending(edition => edition.Year)
        //        .Take(count.Value)
        //        .Select(edition => new EditionStat
        //        {
        //            EventId = edition.EventId,
        //            EditionNo = edition.EditionNo,
        //            Year = edition.Year.GetValueOrDefault(),
        //            TotalNumber = edition.NPSAverageExhibitor.GetValueOrDefault()
        //        }).ToList();

        //    return scoreList;
        //}

        public IList<EditionStat> GetExhibitorRetentionRatesLocal(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var rateList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.LocalExhibitorRetentionRate ?? 0
                }).ToList();

            return rateList;
        }

        public IList<EditionStat> GetExhibitorRetentionRatesInternational(int eventId, int? count = null, int? maxYear = null)
        {
            var query = StatsQuery(eventId, count, maxYear);

            var rateList = query
                .Select(edition => new EditionStat
                {
                    EventId = edition.EventId,
                    EditionNo = edition.EditionNo,
                    Year = (short)(edition.StartDate.HasValue ? edition.StartDate.Value.Year : 0),
                    TotalNumber = edition.InternationalExhibitorRetentionRate ?? 0
                }).ToList();

            return rateList;
        }
    }
}