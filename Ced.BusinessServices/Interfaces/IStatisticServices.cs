using System.Collections.Generic;
using Ced.BusinessServices.Models;

namespace Ced.BusinessServices
{
    public interface IStatisticServices
    {
        IList<EditionStat> GetSqmSales(int eventId, int? count = null, int? maxYear = null);
        // EventId, EditionNo, Year, LocalSqmSold, IntlSqmSold, TotalSqmSold

        IList<EditionStat> GetActualVisitorRatio(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetNumberOfSponsorships(int eventId, int? count = null, int? maxYear = null);
        // EventId, EditionNo, Year, NumberOfSponsors

        IList<EditionStat> GetNumberOfExhibitors(int eventId, int? count = null, int? maxYear = null);
        // EventId, EditionNo, Year, NumberOfLocalExhibitors, NumberOfInternationalExhibitors, TotalNumberOfExhibitors

        IList<EditionStat> GetNumberOfVisitors(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetSqmPerExhibitor(int eventId, int? count = null, int? maxYear = null);
        
        IList<EditionStat> GetNumberOfVisitorsPerSqm(int eventId, int? count = null, int? maxYear = null);
        
        IList<EditionStat> GetNumberOfVisitorsPerExhibitor(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetNumberOfVisitorsPerSqmAndDay(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetNumberOfDelegates(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetNumberOfPaidDelegates(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetTopVisitorCountries(int eventId, int? count = null, int? maxYear = null);
        // EventId, EditionNo, Year, CountryNames

        IList<EditionStat> GetInternationalExhibitorPavilionCounts(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetExhibitorCountryCounts(int eventId, int? count = null, int? maxYear = null);
        
        IList<EditionStat> GetVisitorCountryCounts(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetVisitorNetPromoterScores(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetExhibitorNetPromoterScores(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetVisitorSatisfactionNetPromoterScores(int eventId, int? count = null, int? maxYear = null);

        IList<EditionStat> GetExhibitorSatisfactionNetPromoterScores(int eventId, int? count = null, int? maxYear = null);

        //IList<EditionStat> GetVisitorAverageNetPromoterScores(int eventId, int? count = null, int? maxYear = null);

        //IList<EditionStat> GetExhibitorAverageNetPromoterScores(int eventId, int? count = null, int? maxYear = null);
        
        IList<EditionStat> GetExhibitorRetentionRatesLocal(int eventId, int? count = null, int? maxYear = null);
        
        IList<EditionStat> GetExhibitorRetentionRatesInternational(int eventId, int? count = null, int? maxYear = null);
    }
}