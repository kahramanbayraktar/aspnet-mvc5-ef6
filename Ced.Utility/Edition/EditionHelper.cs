using Ced.BusinessEntities;
using Ced.Utility.Web;
using ITE.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ced.Utility.Edition
{
    public class EditionHelper : IEditionHelper
    {
        public string GetEditionUrl(EditionEntity edition, string fragment = null)
        {
            // For integration testing purposes
            if (HttpContext.Current == null || HttpContext.Current.Request.RequestContext == null)
                return "";

            var requestContext = HttpContext.Current.Request.RequestContext;
            var urlHelper = new UrlHelper(requestContext);
            return GetEditionUrl(urlHelper, edition, fragment);
        }

        public string GetEditionListUrl(EventEntity @event, string fragment = null)
        {
            // For integration testing purposes
            if (HttpContext.Current == null || HttpContext.Current.Request.RequestContext == null)
                return "";

            var requestContext = HttpContext.Current.Request.RequestContext;
            var urlHelper = new UrlHelper(requestContext);
            return GetEditionListUrl(urlHelper, @event, fragment);
        }

        /// <summary>
        /// Created for uses outside of the HttpContext like in UnitTest projects.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="edition"></param>
        /// <param name="fragment"></param>
        /// <returns></returns>
        public string GetEditionUrl(UrlHelper urlHelper, EditionEntity edition, string fragment = null)
        {
            string url;

            switch (edition.Status)
            {
                case EditionStatusType.PreDraft:
                case EditionStatusType.Draft:
                case EditionStatusType.WaitingForApproval:
                case EditionStatusType.Approved:
                    url = urlHelper.AbsoluteAction("Draft", "Edition", new { id = edition.EditionId, name = edition.EditionName.ToUrlString() });
                    break;
                default:
                    url = urlHelper.AbsoluteAction("Edit", "Edition", new { id = edition.EditionId, name = edition.EditionName.ToUrlString() });
                    break;
            }

            return url + fragment;
        }

        public string GetEditionListUrl(UrlHelper urlHelper, EventEntity @event, string fragment = null)
        {
            var url = urlHelper.AbsoluteAction("Index", "Edition", new { eventId = @event.EventId, name = @event.MasterName.ToUrlString() });
            return url + fragment;
        }

        public string GetNameWithEditionNo(int editionNo, string editionName)
        {
            var no = editionNo.AddOrdinal();
            var name  = editionNo > 0
                ? no + " edition of " + editionName
                : editionName;
            return name;
        }

        public string GetNameWithEditionNo(EditionEntity edition)
        {
            return GetNameWithEditionNo(edition.EditionNo, edition.EditionName);
        }

        // TODO: Should be moved to a more appropriate class.
        public string GetRecipientFullName(EditionEntity edition)
        {
            string recipientFullName;
            switch (edition.Status)
            {
                case EditionStatusType.WaitingForApproval:
                    recipientFullName = "";
                    break;
                default:
                    var eventDirectorFullName = GetEventDirectorFullName(edition) ?? "";
                    recipientFullName = eventDirectorFullName;
                    break;
            }
            return recipientFullName;
        }

        // TODO: Should be moved to a more appropriate class.
        public string GetEventDirectorFullName(EditionEntity edition)
        {
            if (!edition.Event.Directors.Any())
            {
                //var message = $"The event ({edition.EventId}-{edition.Event.MasterName}) has not got any EventDirector yet.";
                //var log = CreateInternalLog(message, true);
                //ExternalLogHelper.Log(log, LoggingEventType.Fatal);

                return null;
            }

            var directors = edition.Event.Directors.Where(x => x.IsAssistant.GetValueOrDefault()).ToList();

            if (!directors.Any())
                directors = edition.Event.Directors.Where(x => x.IsPrimary.GetValueOrDefault()).ToList();

            var emails = directors.Select(x => new { x.DirectorEmail, x.DirectorFullName }).Distinct().ToList();

            if (!emails.Any())
            {
                //var message = $"A primary EventDirector has not been set for the event {edition.EventId}-{edition.Event.MasterName}";
                //var log = CreateInternalLog(message, true);
                //ExternalLogHelper.Log(log, LoggingEventType.Fatal);

                return null;
            }

            if (emails.Count > 1)
            {
                //var message = $"More than one primary EventDirectors have been set for the event {edition.EventId}-{edition.Event.MasterName}";
                //var log = CreateInternalLog(message, true);
                //ExternalLogHelper.Log(log, LoggingEventType.Fatal);

                return emails.First().DirectorFullName;
            }

            return emails.First().DirectorFullName;
        }

        public List<int> GetImageSizes(string propName)
        {
            propName = propName.Replace(" ", "").ToLower();

            EditionImageType imageType = EditionImageType.WebLogo;

            if (EditionImageType.WebLogo.ToString().ToLower().Contains(propName))
                imageType = EditionImageType.WebLogo;
            else if (EditionImageType.PeopleImage.ToString().ToLower().Contains(propName))
                imageType = EditionImageType.PeopleImage;
            else if (EditionImageType.ProductImage.ToString().ToLower().Contains(propName))
                imageType = EditionImageType.ProductImage;
            else if (EditionImageType.MasterLogo.ToString().ToLower().Contains(propName))
                imageType = EditionImageType.MasterLogo;
            else if (EditionImageType.Icon.ToString().ToLower().Contains(propName))
                imageType = EditionImageType.Icon;

            var width = imageType.GetAttribute<ImageAttribute>().Width;
            var height = imageType.GetAttribute<ImageAttribute>().Height;

            return new List<int> { width, height };
        }

        public bool CorrectLanguageCodeInUrl(ref string lang)
        {
            var corrected = false;

            if (lang == null)
            {
                lang = LanguageHelper.Languages.English.GetAttribute<LanguageHelper.LanguageAttribute>().LanguageCode;
            }
            else
            {
                lang = lang.ToLower();
                try
                {
                    lang.ToEnumFromDescription<LanguageHelper.Languages>();
                }
                catch
                {
                    lang = LanguageHelper.Languages.English.GetAttribute<LanguageHelper.LanguageAttribute>().LanguageCode;
                    corrected = true;
                }
            }
            return corrected;
        }
    }
}