using AutoMapper;
using Ced.BusinessEntities;
using Ced.BusinessServices.Helpers;
using Ced.Data.Models;
using Ced.Data.UnitOfWork;
using ITE.Logger;
using ITE.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ced.BusinessServices
{
    public class FileServices : ServiceBase, IFileServices
    {
        private readonly UnitOfWork _unitOfWork;

        public FileServices(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = (UnitOfWork)unitOfWork;
        }

        public FileEntity GetFileById(int fileId)
        {
            var file = _unitOfWork.FileRepository.GetById(fileId);
            if (file != null)
            {
                var fileModel = Mapper.Map<File, FileEntity>(file);
                return fileModel;
            }
            return null;
        }

        public IList<FileEntity> GetAllFiles()
        {
            var files = _unitOfWork.FileRepository.GetAll().ToList();
            if (files.Any())
            {
                var filesModel = Mapper.Map<List<File>, List<FileEntity>>(files);
                return filesModel;
            }
            return new List<FileEntity>();
        }

        public IList<FileEntity> GetFilesByEntity(int entityId, string entityType, EditionFileType? editionFileType, string langCode)
        {
            var fileTypeName = editionFileType != null ? editionFileType.GetDescription() : null;

            var files = _unitOfWork.FileRepository
                .GetManyQueryable(x => x.EntityId == entityId
                              && x.EntityType == entityType
                              && (fileTypeName == null || x.FileType == fileTypeName)
                              && (x.LanguageCode == null || x.LanguageCode == langCode))
                .ToList();

            if (files.Any())
            {
                var filesModel = Mapper.Map<List<File>, List<FileEntity>>(files);
                return filesModel;
            }
            return new List<FileEntity>();
        }

        public int GetFileCount(int entityId, string entityType, EditionFileType? editionFileType, string langCode)
        {
            var fileTypeName = editionFileType?.GetDescription();

            var count = _unitOfWork.FileRepository
                .GetManyQueryable(x => x.EntityId == entityId
                              && x.EntityType == entityType
                              && (fileTypeName == null || x.FileType == fileTypeName)
                              && (x.LanguageCode == null || x.LanguageCode == langCode))
                .Count();

            return count;
        }

        public bool CreateFile(FileEntity fileEntity, int userId)
        {
            try
            {
                var file = new File
                {
                    FileName = fileEntity.FileName,
                    EntityId = fileEntity.EntityId,
                    EntityType = fileEntity.EntityType,
                    FileType = fileEntity.EditionFileType.GetDescription(),
                    LanguageCode = fileEntity.LanguageCode,

                    CreatedOn = DateTime.Now,
                    CreatedBy = userId,
                    CreatedByFullName = fileEntity.CreatedByFullName,
                    CreatedByEmail = fileEntity.CreatedByEmail
                };

                _unitOfWork.FileRepository.Insert(file);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }
        }

        public bool DeleteFile(int fileId)
        {
            try
            {
                var file = _unitOfWork.FileRepository.GetById(fileId);
                _unitOfWork.FileRepository.Delete(file);
                _unitOfWork.Save();

                return true;
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }
        }
    }
}
