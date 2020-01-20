using Ced.BusinessEntities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Ced.Utility.Edition
{
    public interface IEditionHelper
    {
        string GetEditionUrl(EditionEntity edition, string fragment = null);
        string GetEditionListUrl(EventEntity @event, string fragment = null);

        /// <summary>
        /// Created for uses outside of the HttpContext like in UnitTest projects.
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="edition"></param>
        /// <param name="fragment"></param>
        /// <returns></returns>
        string GetEditionUrl(UrlHelper urlHelper, EditionEntity edition, string fragment = null);

        string GetEditionListUrl(UrlHelper urlHelper, EventEntity @event, string fragment = null);
        string GetNameWithEditionNo(int editionNo, string editionName);
        string GetNameWithEditionNo(EditionEntity edition);
        string GetRecipientFullName(EditionEntity edition);
        string GetEventDirectorFullName(EditionEntity edition);
        List<int> GetImageSizes(string propName);
        bool CorrectLanguageCodeInUrl(ref string lang);
    }
}