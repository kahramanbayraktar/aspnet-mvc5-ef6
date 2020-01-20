using Ced.BusinessEntities;
using Ced.Utility.Azure;

namespace Ced.Utility.File
{
    public static class FileExtensions
    {
        public static string GetFileTypeIcon(this Ced.Data.Models.File file)
        {
            switch (System.IO.Path.GetExtension(file.FileName))
            {
                case ".xls":
                    return "file-excel-o";
                case ".xlsx":
                    return "file-excel-o";
                case ".ppt":
                    return "file-powerpoint-o";
                case ".pdf":
                    return "file-pdf-o";
                case ".doc":
                    return "file-word-o";
                case ".docx":
                    return "file-word-o";
                case ".jpeg":
                    return "file-image-o";
                case ".jpg":
                    return "file-image-o";
                case ".png":
                    return "file-image-o";
                default:
                    return "file-o";
            }
        }

        public static bool Downloadable(this FileEntity file, CedUser user)
        {
            switch (file.EditionFileType)
            {
                case EditionFileType.VisitorData:
                    if (user.IsSuperAdmin)
                        return true;
                    return false;
                case EditionFileType.PostShowReport:
                    return true;
                case EditionFileType.ExhibitionPhoto:
                    return true;
                case EditionFileType.PriceList:
                    return true;
                default:
                    return false;
            }
        }

        public static bool Deletable(this FileEntity file, CedUser user)
        {
            if (user.IsViewer)
                return false;

            switch (file.EditionFileType)
            {
                case EditionFileType.VisitorData:
                    if (user.IsSuperAdmin)
                        return true;
                    return false;
                case EditionFileType.PostShowReport:
                    return true;
                case EditionFileType.ExhibitionPhoto:
                    return true;
                case EditionFileType.PriceList:
                    return true;
                default:
                    return false;
            }
        }

        public static void SetFullUrl(this FileEntity file)
        {
            file.FileName = EditionFileType.VisitorData.BlobFullUrl(file.FileName);
        }
    }
}