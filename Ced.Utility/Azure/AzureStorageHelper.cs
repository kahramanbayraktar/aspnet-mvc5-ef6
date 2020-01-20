using System.Configuration;
using Ced.BusinessEntities;
using Ced.Utility.Edition;
using ITE.Utility.Extensions;

namespace Ced.Utility.Azure
{
    public static class AzureStorageHelper
    {
        private static readonly string ContainerName = ConfigurationManager.AppSettings["ContainerName"];
        private static readonly string AzureStorageUri = ConfigurationManager.AppSettings["AzureStorageUri"];
        public static readonly string AzureStorageContainerUri = AzureStorageUri + ContainerName + "/";

        private static readonly string FileBlobSubdirectory = ConfigurationManager.AppSettings["FileBlobSubdirectory"];
        private static readonly string FileBlobDirectory = $"{AzureStorageContainerUri}{FileBlobSubdirectory}";

        private static readonly string ImageBlobSubdirectory = ConfigurationManager.AppSettings["ImageBlobSubdirectory"];
        public static readonly string ImageBlobDirectory = $"{AzureStorageContainerUri}{ImageBlobSubdirectory}";

        private static readonly string ProfilePicBlobSubdirectory = ConfigurationManager.AppSettings["ProfilePicBlobSubdirectory"];
        private static readonly string ProfilePicBlobDirectory = $"{AzureStorageContainerUri}{ProfilePicBlobSubdirectory}";

        #region FILES

        // 1) https://itedata.blob.core.windows.net/ced/edition/files/visitordata/
        private static string BlobSubDirectory(this EditionFileType editionFileType)
        {
            return FileBlobDirectory + editionFileType.GetAttribute<FileTypeAttribute>().FolderName.ToLower() + "/";
        }

        // 2) edition/files/visitordata/688-ADUsersAndEvents_MA_20160621.xlsx
        public static string BlobFullName(this EditionFileType editionFileType, string fileName)
        {
            return FileBlobSubdirectory + editionFileType.GetAttribute<FileTypeAttribute>().FolderName.ToLower() + "/" + fileName;
        }

        // 3) https://itedata.blob.core.windows.net/ced/edition/files/visitordata/688-ADUsersAndEvents_MA_20160621.xlsx
        public static string BlobFullUrl(this EditionFileType editionFileType, string fileName)
        {
            return editionFileType.BlobSubDirectory() + fileName;
        }

        #endregion

        #region IMAGES

        // 1) https://itedata.blob.core.windows.net/ced/edition/images/weblogo/
        public static string BlobSubDirectory(this EditionImageType editionImageType)
        {
            return ImageBlobDirectory + editionImageType.GetAttribute<ImageAttribute>().Key.ToLower() + "/";
        }

        // 2) edition/images/weblogo/688-turkeybuildistanbul2016-en-gb.png
        public static string BlobFullName(this EditionImageType editionImageType, string fileName)
        {
            return ImageBlobSubdirectory + editionImageType.GetAttribute<ImageAttribute>().Key.ToLower() + "/" + fileName;
        }

        // 3) https://itedata.blob.core.windows.net/ced/edition/images/weblogo/688-turkeybuildistanbul2016-en-gb.png
        public static string BlobFullUrl(this EditionImageType editionImageType, string fileName)
        {
            return editionImageType.BlobSubDirectory() + fileName;
        }

        public static string BlobFullUrl(this EditionImageType editionImageType, EditionTranslationEntity editionTranslation)
        {
            var fileName = editionTranslation.GetFileName(editionImageType);
            if (string.IsNullOrWhiteSpace(fileName))
                return editionImageType.EditionDefaultImageUrl();
            return editionImageType.BlobSubDirectory() + fileName;
        }

        public static string EditionDefaultImageUrl(this EditionImageType editionImageType)
        {
            return ImageBlobDirectory + "no-file-" + editionImageType.GetAttribute<ImageAttribute>().Key.ToLower() + ".png";
        }

        #endregion

        #region PROFILE PICS

        // 1) https://itedata.blob.core.windows.net/ced/user/profilepic/
        private static string BlobSubDirectory(this UserImageType userImageType)
        {
            return ProfilePicBlobDirectory + userImageType.GetAttribute<ImageAttribute>().Key.ToLower() + "/";
        }

        // 2) user/profilepic/1.jpg
        public static string BlobFullName(this UserImageType userFileType, string fileName)
        {
            return ProfilePicBlobDirectory + userFileType.GetAttribute<FileTypeAttribute>().FolderName.ToLower() + "/" + fileName;
        }

        // 3) https://itedata.blob.core.windows.net/ced/user/profilepic/1.jpg
        public static string BlobFullUrl(this UserImageType userFileType, string fileName)
        {
            return userFileType.BlobSubDirectory() + fileName;
        }

        #endregion
    }
}