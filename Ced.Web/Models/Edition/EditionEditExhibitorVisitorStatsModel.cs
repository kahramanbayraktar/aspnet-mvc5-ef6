using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ced.BusinessEntities;

namespace Ced.Web.Models.Edition
{
    public class EditionEditExhibitorVisitorStatsModel : EditionBaseModel
    {
        // EXHIBITOR
        [Display(Name = "NumberOfLocalExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? LocalExhibitorCount { get; set; }

        [Display(Name = "NumberOfInternationalExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? InternationalExhibitorCount { get; set; }

        //[Display(Name = "NumberOfExhibitors", ResourceType = typeof(Resources.Resources))]
        public int? ExhibitorCount { get; set; }

        [Display(Name = "NumberOfExhibitingCountries", ResourceType = typeof(Resources.Resources))]
        public short? ExhibitorCountryCount { get; set; }

        [Display(Name = "TopExhibitorCountries", ResourceType = typeof(Resources.Resources))]
        public string[] TopExhibitorCountries { get; set; }

        [Display(Name = "LocalExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        public decimal? LocalExhibitorRetentionRate { get; set; }

        [Display(Name = "InternationalExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        public decimal? InternationalExhibitorRetentionRate { get; set; }

        //[Display(Name = "ExhibitorRetentionRate", ResourceType = typeof(Resources.Resources))]
        public decimal? ExhibitorRetentionRate { get; set; }

        // VISITOR
        [Display(Name = "LocalVisitorCount", ResourceType = typeof(Resources.Resources))]
        public int? LocalVisitorCount { get; set; }

        [Display(Name = "InternationalVisitorCount", ResourceType = typeof(Resources.Resources))]
        public int? InternationalVisitorCount { get; set; }

        //[Display(Name = "LocalRepeatVisitCount", ResourceType = typeof(Resources.Resources))]
        public int? LocalRepeatVisitCount { get; set; }

        //[Display(Name = "InternationalRepeatVisitCount", ResourceType = typeof(Resources.Resources))]
        public int? InternationalRepeatVisitCount { get; set; }

        public int? RepeatVisitCount { get; set; }

        [Display(Name = "NumberOfVisitorCountries", ResourceType = typeof(Resources.Resources))]
        public int? VisitorCountryCount { get; set; }

        [Display(Name = "NumberOfNationalGroups", ResourceType = typeof(Resources.Resources))]
        public short? NationalGroupCount { get; set; }

        [Display(Name = "OnlineRegisteredCount", ResourceType = typeof(Resources.Resources))]
        public int? OnlineRegistrationCount { get; set; }

        [Display(Name = "OnlineRegisteredVisitorCount", ResourceType = typeof(Resources.Resources))]
        public int? OnlineRegisteredVisitorCount { get; set; }

        public int? OnlineRegisteredBuyerVisitorCount { get; set; }

        [Display(Name = "TopVisitorCountries", ResourceType = typeof(Resources.Resources))]
        public string[] TopVisitorCountries { get; set; }

        // DELEGATE
        [Display(Name = "NumberOfLocalDelegates", ResourceType = typeof(Resources.Resources))]
        public int? LocalDelegateCount { get; set; }

        [Display(Name = "NumberOfInternationalDelegates", ResourceType = typeof(Resources.Resources))]
        public int? InternationalDelegateCount { get; set; }

        [Display(Name = "NumberOfLocalPaidDelegates", ResourceType = typeof(Resources.Resources))]
        public int? LocalPaidDelegateCount { get; set; }

        [Display(Name = "NumberOfInternationalPaidDelegates", ResourceType = typeof(Resources.Resources))]
        public int? InternationalPaidDelegateCount { get; set; }

        [Display(Name = "DelegateCountries", ResourceType = typeof(Resources.Resources))]
        public string[] DelegateCountries { get; set; }

        [Display(Name = "CohostedEvent", ResourceType = typeof(Resources.Resources))]
        public bool CohostedEvent { get; set; }
        
        public int? CohostedEventCount { get; set; }

        public IEnumerable<EditionCohostEntity> Cohosts { get; set; }

        public IEnumerable<EditionKeyVisitorEntity> EditionKeyVisitors { get; set; }

        public IEnumerable<EditionVisitorEntity> EditionVisitors { get; set; }

        public int[] DailyVisitorCounts { get; set; }

        public int[] DailyRepeatVisits { get; set; }

        public int[] DailyOldVisitorCounts { get; set; }

        public int[] DailyNewVisitorCounts { get; set; }
        

        public IList<KeyVisitorEntity> KeyVisitors { get; set; }
    }
}