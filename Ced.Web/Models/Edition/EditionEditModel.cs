using System;
using System.Web.Mvc;
using Ced.BusinessEntities;
using Ced.Web.Models.File;

namespace Ced.Web.Models.Edition
{
    public class EditionEditModel : EditionBaseModel
    {
        public EditionEditGeneralInfoModel EditionEditGeneralInfoModel { get; set; }

        public EditionEditSalesMetricsModel EditionEditSalesMetricsModel { get; set; }

        public EditionEditExhibitorVisitorStatsModel EditionEditExhibitorVisitorStatsModel { get; set; }
        
        public FilesEditModel EditionEditFilesModel { get; set; }

        public EditionEditImagesModel EditionEditImagesModel { get; set; }

        public EditionEditPostShowMetricsModel EditionEditPostShowMetricsModel { get; set; }

        public int AxEventId { get; set; }
        
        public string Classification { get; set; }

        public string Frequency { get; set; }

        public string DirectorFullName { get; set; }

        public EditionStatusType Status { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime UpdateTimeByAutoIntegration { get; set; }

        public SelectList CountrySelectList { get; set; }

        public bool IsUserSubscribed { get; set; }
    }
}