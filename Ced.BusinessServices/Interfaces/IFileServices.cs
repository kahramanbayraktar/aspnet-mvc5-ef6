using System.Collections.Generic;
using Ced.BusinessEntities;

namespace Ced.BusinessServices
{
    public interface IFileServices
    {
        FileEntity GetFileById(int fileId);

        IList<FileEntity> GetAllFiles();

        IList<FileEntity> GetFilesByEntity(int entityId, string entityType, EditionFileType? editionFileType, string langCode);

        int GetFileCount(int entityId, string entityType, EditionFileType? editionFileType, string langCode);

        bool CreateFile(FileEntity fileEntity, int userId);

        bool DeleteFile(int fileId);
    }
}
