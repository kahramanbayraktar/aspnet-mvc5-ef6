using ITE.AzureStorage;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.BusinessServices.Helpers;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Edition;
using Ced.Utility.File;
using Ced.Web.Filters;
using Ced.Web.Helpers;
using Ced.Web.Models;
using Ced.Web.Models.File;
using ITE.Logger;
using ITE.Utility.Extensions;
using ITE.Utility.ObjectComparison;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = Ced.Utility.Constants;

namespace Ced.Web.Controllers
{
    [CedAuthorize]
    public class FileController : GlobalController
    {
        private readonly Service _azureStorageService = new Service();

        public FileController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEditionServices editionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            IEditionTranslationServices editionTranslationServices,
            IFileServices fileServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            ISubscriptionServices subscriptionServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices) :
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                editionServices, editionTranslationServices, eventServices, eventDirectorServices, fileServices, logServices,
                notificationServices, subscriptionServices, userServices, userRoleServices)
        {
        }

        [AjaxOnly]
        [HttpPost]
        [CedAction(ActionType = ActionType.FileUpload, Loggable = true)]
        public ActionResult _Upload()
        {
            var entityId = Convert.ToInt32(Request.Params["EntityId"]);
            var fileType = Request.Params["FileType"].ToEnum<EditionFileType>();
            var langCode = Request.Params["LanguageCode"];
            var isSaved = true;
            var newFileName = "";

            var edition = EditionServices.GetEditionById(entityId);
            if (edition == null)
                return View("NotFound", new ErrorModel { Message = Constants.EditionNotFound });
            if (edition.IsCancelled())
                return Json(new { success = false, message = Constants.WarningMessageEventCancelled });

            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);

                    // CHECK EXTENSION
                    var extension = Path.GetExtension(file.FileName);
                    var allowedExtensions = fileType.GetAttribute<FileTypeAttribute>().AllowedExtensions;

                    if (!allowedExtensions.Contains(extension))
                    {
                        return Json(new { success = false, message = Constants.InvalidFileExtension });
                    }

                    newFileName = EditionServiceHelper.ComposeFileName(edition.EditionId, fileName, fileType, langCode, extension);

                    // The way of saving changes depending on fileType's Private property.
                    var result = fileType.GetAttribute<FileTypeAttribute>().Private
                        ? SaveFileToLocalDisk(fileType, newFileName, file)
                        : SaveFileToAzureStorage(fileType, newFileName, file);

                    switch (result.Result)
                    {
                        case OperationResult.AlreadyExists:
                            return Json(new { success = false, message = Constants.FileAlreadyExistsWithTheSameName });
                        case OperationResult.Failed:
                                return Json(new { success = false, message = result.Message });
                    }

                    // CREATE FILE
                    if (!SaveFileInfo(entityId, newFileName, fileType, extension, langCode))
                        return Json(new { success = false, message = Constants.ErrorWhileSavingFile });
                }
            }
            catch (Exception exc)
            {
                isSaved = false;

                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);
            }

            if (!isSaved)
                return Json(new {success = false, message = Constants.ErrorWhileSavingFile});

            // UPDATE EDITION
            UpdateEditionUpdateInfo(edition);

            // UPDATE LOG
            var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson($"File ({fileType})", new List<Variance> { new Variance { Prop = fileType.ToString() } });
            UpdateLogInMemory(edition, updatedFields);

            //UpdateLogInMemory(currentEdition, edition, currentEditionTranslation, editionTranslation);

            // UPDATEDCONTENT
            var currentFileForComparison = new FileEntity();
            var fileForComparison = new FileEntity { FileName = newFileName };
            fileForComparison.SetFullUrl();
            var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentFileForComparison, fileForComparison);

            PushEditionUpdateNotifications(edition, updatedContent);

            return Json(new { success = true, message = Constants.FileSaved });
        }

        public FileResult Download(int fileId)
        {
            var file = FileServices.GetFileById(fileId);
            if (file != null)
            {
                if (file.EditionFileType.GetAttribute<FileTypeAttribute>().Private)
                    return DownloadFileFromLocalDisk(file);
                return DownloadFileFromAzureStorage(file);
            }
            throw new FileNotFoundException("File not found!");
        }

        [AjaxOnly]
        [CedAction(ActionType = ActionType.FileDelete, Loggable = true)]
        public ActionResult _Delete(int fileId, string fileType)
        {
            var editionFileType = fileType.ToLower().ToEnumFromDescription<EditionFileType>();

            var file = FileServices.GetFileById(fileId);
            if (file == null)
                return Json(new { success = false, message = Constants.FileNotFound });

            var edition = EditionServices.GetEditionById(file.EntityId);
            if (edition.IsCancelled())
                return Json(new { success = false, message = Constants.WarningMessageEventCancelled });

            var success = FileServices.DeleteFile(file.FileId);
            if (success)
            {
                bool success2;
                if (file.EditionFileType.GetAttribute<FileTypeAttribute>().Private)
                {
                    success2 = DeleteFileFromLocalDisk(file.FileName, file.EditionFileType);
                }
                else
                {
                    var blobName = editionFileType.BlobFullName(file.FileName);

                    try
                    {
                        success2 = _azureStorageService.DeleteFile(blobName);
                    }
                    catch (Exception exc)
                    {
                        var log = CreateInternalLog(exc);
                        ExternalLogHelper.Log(log, LoggingEventType.Error);

                        success2 = false;
                    }
                }

                //if (!success2)
                //    return Json(new {success = false, message = "File could not be deleted!"});

                // UPDATE EDITON
                UpdateEditionUpdateInfo(edition);

                //// DIFF
                //var diff = new List<Variance> { new Variance { Prop = $"File ({file.EditionFileType})", ValA = file.FileName, ValB = null } };

                //OnEditionUpdated(edition, diff);

                // UPDATE LOG
                var updatedFields = NotificationControllerHelper.GetUpdatedFieldsAsJson($"File ({file.EditionFileType})", new List<Variance> { new Variance { Prop = editionFileType.ToString() } });
                UpdateLogInMemory(edition, updatedFields);

                // UPDATEDCONTENT
                var currentFileForComparison = new FileEntity { FileName = file.FileName };
                var fileForComparison = new FileEntity();
                fileForComparison.SetFullUrl();
                var updatedContent = NotificationControllerHelper.GetUpdatedContent(currentFileForComparison, fileForComparison);

                PushEditionUpdateNotifications(edition, updatedContent);

                return Json(new { success = true, message = Constants.FileDeleted, fileType = editionFileType.GetDescription() });
            }
            return Json(new { success = false, message = Constants.FileNotDeleted });
        }

        [AjaxOnly]
        public ActionResult _GetFiles(int entityId, string entityType, string fileType, string langCode)
        {
            var files = FileServices.GetFilesByEntity(entityId, entityType, null, langCode).OrderByDescending(x => x.CreatedOn).ToList();

            var isCancelled = false;
            if (entityType == EntityType.Edition.GetDescription())
            {
                var edition = EditionServices.GetEditionById(entityId, Constants.ValidEventTypesForCed);
                isCancelled = edition.IsCancelled();
            }

            var model = new FilesEditModel
            {
                EntityId = entityId,
                EntityType = entityType.ToEnumFromDescription<EntityType>(),
                EditionFileType = fileType.ToEnumFromDescription<EditionFileType>(),
                LanguageCode = langCode,
                Files = files,
                CurrentUser = CurrentCedUser,
                IsCancelled = isCancelled
            };

            return PartialView("_EditionFiles", model);
        }

        #region HELPER METHODS

        private bool SaveFileInfo(int entityId, string fileName, EditionFileType editionFileType, string extension, string langCode)
        {
            var fileEntity = new FileEntity
            {
                FileName = fileName,
                FileExtension = extension,
                EntityId = entityId,
                EntityType = EntityType.Edition.GetDescription(),
                EditionFileType = editionFileType,
                LanguageCode = langCode,
                CreatedByFullName = CurrentCedUser.CurrentUser.FullName,
                CreatedByEmail = CurrentCedUser.CurrentUser.Email
            };

            return FileServices.CreateFile(fileEntity, 1);
        }

        private FileOperationResult SaveFileToAzureStorage(EditionFileType fileType, string newFileName, HttpPostedFileBase file)
        {
            var blobName = fileType.BlobFullName(newFileName);
            if (_azureStorageService.FileExists(blobName))
            {
                return new FileOperationResult
                {
                    Result = OperationResult.AlreadyExists
                };
            }

            return _azureStorageService.UploadFile(blobName, file.ContentType, file.InputStream);
        }

        private FileOperationResult SaveFileToLocalDisk(EditionFileType fileType, string newFileName, HttpPostedFileBase file)
        {
            var path = Server.MapPath("~/" + fileType.FileFullNameInternal(newFileName));
            if (System.IO.File.Exists(path))
            {
                return new FileOperationResult
                {
                    Result = OperationResult.AlreadyExists
                };
            }

            try
            {
                file.SaveAs(path);
                return new FileOperationResult
                {
                    Result = OperationResult.Succeeded,
                    Message = path
                };
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return new FileOperationResult
                {
                    Result = OperationResult.Failed,
                    Message = exc.Message
                };
            }
        }

        private FileContentResult DownloadFileFromAzureStorage(FileEntity file)
        {
            var blobName = file.EditionFileType.BlobFullName(file.FileName);
            var stream = _azureStorageService.DownloadFile(blobName);
            if (stream == null)
                throw new FileNotFoundException("File not found!");

            var contents = ((MemoryStream)stream).ToArray();
            var contentType = MimeMapping.GetMimeMapping(file.FileName);
            return File(contents, contentType, file.FileName);
        }

        private FilePathResult DownloadFileFromLocalDisk(FileEntity file)
        {
            var path = Server.MapPath("~/" + file.EditionFileType.FileFullNameInternal(file.FileName));
            var mimeType = MimeMapping.GetMimeMapping(file.FileName);
            return File(path, mimeType, file.FileName);
        }

        private bool DeleteFileFromLocalDisk(string fileName, EditionFileType editionFileType)
        {
            var path = Server.MapPath("~/" + editionFileType.FileFullNameInternal(fileName));
            if (!System.IO.File.Exists(path))
            {
                var log = CreateInternalLog($"File not found on {path}!");
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }

            try
            {
                System.IO.File.Delete(path);
                return true;
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return false;
            }
        }

        #endregion
    }
}