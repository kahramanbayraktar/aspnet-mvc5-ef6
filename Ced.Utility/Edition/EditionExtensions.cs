using System;
using System.Collections.Generic;
using System.Linq;
using Ced.BusinessEntities;
using Ced.Utility.Azure;
using Ced.Utility.Web;
using ITE.Utility.Extensions;

namespace Ced.Utility.Edition
{
    public static class EditionExtensions
    {
        public static bool IsAlive(this EditionEntity edition)
        {
            return (DateTime.Now - edition.EndDate.GetValueOrDefault()).Days < WebConfigHelper.EditionLifeSpan;
        }

        public static bool IsCancelled(this EditionEntity edition)
        {
            return edition.EventActivity == EventActivity.ShowCancelled.GetDescription();
        }

        public static bool IsClonable(this EditionEntity edition, CedUser user)
        {
            if (!WebConfigHelper.CloningAllowed)
                return false;

            if (!Constants.ValidEventActivitiesToClone.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                return false;

            if (!Constants.ValidEditionStatusesToClone.Contains(edition.Status.GetHashCode()))
                return false;

            if (user.IsSuperAdmin)
                return true;

            if (edition.DirectorEmails.Contains(user.CurrentUser.Email.ToLower()))
                return true;

            return false;
        }

        public static bool IsClonable(this EditionEntityLight edition, CedUser user)
        {
            if (!WebConfigHelper.CloningAllowed)
                return false;

            if (!Constants.ValidEventActivitiesToClone.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                return false;

            if (!Constants.ValidEditionStatusesToClone.Contains(edition.Status))
                return false;

            if (user.IsSuperAdmin)
                return true;

            if (edition.DirectorEmails.Contains(user.CurrentUser.Email.ToLower()))
                return true;

            return false;
        }

        public static List<string> ReasonsForNotBeingClonable(this EditionEntityLight edition, CedUser user)
        {
            var reasons = new List<string>();

            if (!edition.IsClonable(user))
            {
                if (!WebConfigHelper.CloningAllowed)
                {
                    reasons.Add("Cloning not allowed.");
                    return reasons;
                }

                if (!Constants.ValidEventActivitiesToClone.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                    reasons.Add(edition.EventActivity);

                if (!Constants.ValidEditionStatusesToClone.Contains(edition.Status))
                    reasons.Add(edition.Status.ToEnum<EditionStatusType>().GetDescription());

                if (!edition.DirectorEmails.Contains(user.CurrentUser.Email.ToLower()))
                    reasons.Add("You don't have permission.");
            }

            return reasons;
        }

        public static bool IsEditable(this EditionEntity edition, CedUser user)
        {
            if (!Constants.ValidEventActivitiesToEdit.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                return false;

            if (!Constants.ValidEditionStatusesToEdit.Contains(edition.Status.GetHashCode()))
                return false;

            return true;
        }

        public static bool IsEditable(this EditionEntityLight edition, CedUser user)
        {
            if (!Constants.ValidEventActivitiesToEdit.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                return false;

            if (edition.Status == EditionStatusType.WaitingForApproval.GetHashCode() && user.IsApprover)
                return true;

            if (!Constants.ValidEditionStatusesToEdit.Contains(edition.Status))
                return false;

            return true;
        }

        public static List<string> ReasonsForNotBeingEditable(this EditionEntityLight edition, CedUser user)
        {
            var reasons = new List<string>();

            if (!edition.IsEditable(user))
            {
                if (!Constants.ValidEventActivitiesToEdit.Select(x => x.ToLower()).ToArray().Contains(edition.EventActivity.ToLower()))
                    reasons.Add(edition.EventActivity);

                if (!Constants.ValidEditionStatusesToEdit.Contains(edition.Status))
                    reasons.Add(edition.Status.ToEnum<EditionStatusType>().GetDescription());
            }

            return reasons;
        }

        public static bool ImagesEditable(this EditionEntity edition, CedUser user)
        {
            if (user.IsViewer)
                return false;

            if (user.IsAdmin || user.IsSuperAdmin)
                return true;

            if (edition.EventActivity == EventActivity.ShowCancelled.GetDescription())
                return false;

            return true;
        }

        public static bool IsConferenceType(this EventType eventType)
        {
            return eventType == EventType.Conference || eventType == EventType.ConferenceExhibition;
        }

        public static bool CanHaveDelegates(this EventType eventType)
        {
            return Constants.EventTypesWithDelegates.Contains(eventType);
        }

        public static string GetFileName(this EditionTranslationEntity editionTranslation, EditionImageType imageType)
        {
            switch (imageType)
            {
                case EditionImageType.WebLogo:
                    return editionTranslation.WebLogoFileName;
                case EditionImageType.PeopleImage:
                    return editionTranslation.PeopleImageFileName;
                case EditionImageType.MasterLogo:
                    return editionTranslation.MasterLogoFileName;
                case EditionImageType.CrmLogo:
                    return editionTranslation.CrmLogoFileName;
                case EditionImageType.ProductImage:
                    return editionTranslation.ProductImageFileName;
                case EditionImageType.Icon:
                    return editionTranslation.IconFileName;
                case EditionImageType.PromotedLogo:
                    return editionTranslation.PromotedLogoFileName;
                case EditionImageType.DetailsImage:
                    return editionTranslation.DetailsImageFileName;
                default:
                    throw new Exception("Unexpected edition image type!");
            }
        }

        public static void SetFileName(this EditionTranslationEntity editionTranslation, EditionImageType imageType, string fileName)
        {
            switch (imageType)
            {
                case EditionImageType.WebLogo:
                    editionTranslation.WebLogoFileName = fileName;
                    break;
                case EditionImageType.PeopleImage:
                    editionTranslation.PeopleImageFileName = fileName;
                    break;
                case EditionImageType.MasterLogo:
                    editionTranslation.MasterLogoFileName = fileName;
                    break;
                case EditionImageType.CrmLogo:
                    editionTranslation.CrmLogoFileName = fileName;
                    break;
                case EditionImageType.ProductImage:
                    editionTranslation.ProductImageFileName = fileName;
                    break;
                case EditionImageType.Icon:
                    editionTranslation.IconFileName = fileName;
                    break;
                case EditionImageType.PromotedLogo:
                    editionTranslation.PromotedLogoFileName = fileName;
                    break;
                case EditionImageType.DetailsImage:
                    editionTranslation.DetailsImageFileName = fileName;
                    break;
                default:
                    throw new Exception("Unexpected edition image type!");
            }
        }

        public static void SetFullUrlOfImages(this EditionTranslationEntity editionTranslation)
        {
            if (!string.IsNullOrWhiteSpace(editionTranslation.WebLogoFileName))
                editionTranslation.WebLogoFileName = EditionImageType.WebLogo.BlobFullUrl(editionTranslation); //.WebLogoFileName);

            if (!string.IsNullOrWhiteSpace(editionTranslation.PeopleImageFileName))
                editionTranslation.PeopleImageFileName = EditionImageType.PeopleImage.BlobFullUrl(editionTranslation); //.PeopleImageFileName);

            if (!string.IsNullOrWhiteSpace(editionTranslation.ProductImageFileName))
                editionTranslation.ProductImageFileName = EditionImageType.ProductImage.BlobFullUrl(editionTranslation); //.ProductImageFileName);

            if (!string.IsNullOrWhiteSpace(editionTranslation.MasterLogoFileName))
                editionTranslation.MasterLogoFileName = EditionImageType.MasterLogo.BlobFullUrl(editionTranslation); //.MasterLogoFileName);

            if (!string.IsNullOrWhiteSpace(editionTranslation.CrmLogoFileName))
                editionTranslation.CrmLogoFileName = EditionImageType.CrmLogo.BlobFullUrl(editionTranslation); //.CrmLogoFileName);

            if (!string.IsNullOrWhiteSpace(editionTranslation.IconFileName))
                editionTranslation.IconFileName = EditionImageType.Icon.BlobFullUrl(editionTranslation); //.IconFileName);
        }

        public static bool HasNotStartedYet(this EditionEntity edition)
        {
            return edition.StartDate.GetValueOrDefault() > DateTime.Now;
        }

        public static bool HasCityOrCountry(this EditionEntity edition)
        {
            return edition.Country != null || edition.City != null;
        }
    }
}