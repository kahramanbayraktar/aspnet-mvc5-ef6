using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum EditionFileType
    {
        [FileType(AllowedExtensions = new[] { ".xls", ".xlsx" }, FolderName = "visitordata", Private = true)]
        [Description("visitordata")]
        VisitorData,

        [FileType(AllowedExtensions = new[] { ".ppt", ".pptx", ".pdf" }, FolderName = "postshowreport")]
        [Description("postshowreport")]
        PostShowReport,

        [FileType(AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" }, FolderName = "exhibitionphoto")]
        [Description("exhibitionphoto")]
        ExhibitionPhoto,

        [FileType(AllowedExtensions = new[] { ".xls", ".xlsx" }, FolderName = "pricelist")]
        [Description("pricelist")]
        PriceList
    }
}