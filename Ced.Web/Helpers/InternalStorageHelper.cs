using Ced.BusinessEntities;
using ITE.Utility.Extensions;

namespace Ced.Web.Helpers
{
    public static class InternalStorageHelper
    {
        #region FILES

        // content/files/visitordata/688-ADUsersAndEvents_MA_20160621.xlsx
        public static string FileFullNameInternal(this EditionFileType editionFileType, string fileName)
        {
            return "content/files/" + editionFileType.GetAttribute<FileTypeAttribute>().FolderName.ToLower() + "/" + fileName;
        }

        #endregion
    }
}