using AutoMapper;
using ImageResizer;
using ITE.AzureStorage;
using Ced.BusinessEntities;
using Ced.BusinessServices;
using Ced.BusinessServices.Auth;
using Ced.BusinessServices.Helpers;
using Ced.Utility;
using Ced.Utility.Azure;
using Ced.Utility.Email;
using Ced.Utility.MVC;
using Ced.Utility.Web;
using Ced.Web.Filters;
using Ced.Web.Models.Select2;
using Ced.Web.Models.User;
using ITE.Logger;
using ITE.Utility.Extensions;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Ced.Web.Controllers
{
    public class UserController : GlobalController
    {
        public UserController(
            IUserServices authUserServices,
            IRoleServices roleServices,
            IApplicationServices applicationServices,
            IIndustryServices industryServices,
            IRegionServices regionServices,
            IEventServices eventServices,
            IEventDirectorServices eventDirectorServices,
            ILogServices logServices,
            INotificationServices notificationServices,
            IUserServices userServices,
            IUserRoleServices userRoleServices):
            base(authUserServices, roleServices, applicationServices, industryServices, regionServices,
                eventServices, eventDirectorServices, logServices, notificationServices, userServices, userRoleServices)
        {
        }

        [CedAuthorize(Roles = "Super Admin")]
        public ActionResult Index()
        {
            var users = UserServices.GetAllUsers().ToList();
            var model = Mapper.Map<List<UserEntity>, List<UserListModel>>(users);
            return View(model);
        }

        [CedAuthorize]
        [CedAction(ActionType = ActionType.Logout, Loggable = true)]
        public ActionResult Logout()
        {
            return RedirectToAction("SignOut", "Auth");
        }

        [AllowAnonymous]
        public ActionResult RequestAccess()
        {
            if (CurrentCedUser != null)
                return View("NotFound");
            return View("AccessRequest");
        }

        [AllowAnonymous]
        [AjaxOnly]
        [HttpGet]
        public ActionResult _RequestAccess()
        {
            return PartialView("_AccessRequest");
        }

        [AllowAnonymous]
        [AjaxOnly]
        [HttpPost]
        [CedAction(Loggable = true, ActionType = ActionType.AccessRequest)]
        public ActionResult _RequestAccess(AccessRequestModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.GetErrors() });

            const string subject = "CED Access Request";
            var recipient = WebConfigHelper.AdminEmails; // WebConfigHelper.HelpDeskUserName;
            var body = "Message: " + model.AdditionalNotes;
            body += "<br><br>Events: " + model.EventNames;
            body += "<br>Office: " + model.OfficeName;
            body += "<br>Sender: " + model.FirstName + " " + model.LastName;
            
            try
            {
                new EmailHelper().SendEmail(subject, body, recipient);

                // EMAIL LOGGING
                LogEmail(null, recipient, body, null, "_RequestAccess");

                return Json(new { success = true });
            }
            catch (Exception exc)
            {
                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);

                return Json(new { success = false, message = exc.Message });
            }
        }

        [AjaxOnly]
        [CedAction(Loggable = true, ActionType = ActionType.ProfilePictureUpload)]
        public ActionResult _SaveProfilePicture()
        {
            var imageType = UserImageType.ProfilePic;
            var oldFileName = UserServices.GetProfilePictureName(CurrentCedUser.CurrentUser.UserId);
            
            try
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    // Resizing & changing format(to jpg)
                    using (var outStream = new MemoryStream())
                    {
                        using (var inStream = new MemoryStream(file.InputStream.ReadAsBytes()))
                        {
                            var settings = new ResizeSettings("maxwidth=76&maxheight=76&format=jpg");
                            ImageBuilder.Current.Build(inStream, outStream, settings);

                            var newFileName = $"{CurrentCedUser.CurrentUser.UserId }.jpg";

                            //// CHECK EXTENSION
                            //var extension = Path.GetExtension(file.FileName);
                            //var allowedExtensions = imageType.GetAttribute<ImageAttribute>().AllowedExtensions;
                            //if (!allowedExtensions.Contains(extension))
                            //{
                            //    var errorMessage = "Invalid file extension!";
                            //    return Json(new { success = false, message = errorMessage });
                            //}

                            //// CHECK LENGTH
                            //var minMaxLengths = imageType.GetAttribute<ImageAttribute>().MinMaxLengths;
                            //if (file.ContentLength < minMaxLengths[0] || file.ContentLength > minMaxLengths[1])
                            //{
                            //    var errorMessage = "Invalid file size!";
                            //    return Json(new { success = false, message = errorMessage });
                            //}

                            //// CHECK SIZES (WIDTH AND HEIGHT)
                            //var allowedWidth = imageType.GetAttribute<ImageAttribute>().Width;
                            //var allowedHeight = imageType.GetAttribute<ImageAttribute>().Height;
                            //if (allowedWidth > 0 && allowedHeight > 0)
                            //{
                            //    var imgFile = System.Drawing.Image.FromStream(file.InputStream);

                            //    if (imgFile.PhysicalDimension.Width > allowedWidth ||
                            //        imgFile.PhysicalDimension.Width < allowedWidth
                            //        || imgFile.PhysicalDimension.Height > allowedHeight ||
                            //        imgFile.PhysicalDimension.Height < allowedHeight)
                            //    {
                            //        var errorMessage = "Invalid file dimension!";
                            //        return Json(new { success = false, message = errorMessage });
                            //    }
                            //}

                            var azureStorageService = new Service();

                            // DELETE OLD FILE
                            try
                            {
                                azureStorageService.DeleteFile(imageType.BlobFullName(oldFileName));
                            }
                            catch (Exception exc)
                            {
                                var extraInfo = "Error while deleting file on Azure Storage.";
                                var extLog = CreateInternalLog(exc, extraInfo);
                                ExternalLogHelper.Log(extLog, LoggingEventType.Error);
                            }

                            var result = azureStorageService.UploadFile(
                                "user/images/" + imageType.GetAttribute<ImageAttribute>().Key.ToLower() + "/" +
                                newFileName, file.ContentType, outStream);

                            if (result.Result == OperationResult.Failed)
                            {
                                var intLog = CreateInternalLog(result.Message);
                                ExternalLogHelper.Log(intLog, LoggingEventType.Error);

                                return Json(new { success = false, message = result.Message });
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                return Json(new
                {
                    success = false,
                    message = Utility.Constants.ErrorWhileSavingFile + (CurrentCedUser.IsSuperAdmin ? " Reason: " + exc.InnerException.GetFullMessage() : "")
                });
            }

            return Json(new { success = true, fileName = "" });
        }

        [AjaxOnly]
        [CedAction(Loggable = true, ActionType = ActionType.ProfilePictureDelete)]
        public ActionResult _DeleteProfilePicture()
        {
            var succeeded = true;

            try
            {
                var azureStorageServce = new Service();
                var success = azureStorageServce.DeleteFile("user/images/profilepic/" + UserServices.GetProfilePictureName(CurrentCedUser.CurrentUser.UserId));

                if (!success)
                    return Json(new { success = false, message = "Error while deleting profile picture" });
            }
            catch (Exception exc)
            {
                succeeded = false;

                var log = CreateInternalLog(exc);
                ExternalLogHelper.Log(log, LoggingEventType.Error);
            }

            return succeeded
                ? Json(new { success = true })
                : Json(new { success = false, message = "Error while deleting profile picture." });
        }

        [AjaxOnly]
        public ActionResult _SearchUsers(string searchTerm) //, int pageSize, int pageNum)
        {
            var users = UserServices.GetUsersByEmail(searchTerm.Trim());
            var pagedEvents = UsersToSelect2Format(users, 15);

            return new JsonResult
            {
                Data = pagedEvents,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static Select2PagedResult UsersToSelect2Format(IEnumerable<UserEntity> users, int totalUsers)
        {
            var jsonUsers = new Select2PagedResult
            {
                Results = new List<Select2Result>()
            };

            foreach (var u in users)
            {
                jsonUsers.Results.Add(new Select2Result { id = u.UserId + "|" + u.Email, text = u.Email + " (" + u.FullName + ")" });
            }

            jsonUsers.Total = totalUsers;
            return jsonUsers;
        }
    }
}