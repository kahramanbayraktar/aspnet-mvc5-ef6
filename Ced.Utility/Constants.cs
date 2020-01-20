using System.Collections.Generic;
using Ced.BusinessEntities;
using ITE.Utility.Extensions;

namespace Ced.Utility
{
    public static class Constants
    {
        public static readonly EditionStatusType[] DefaultValidEditionStatusesForCed =
        {
            EditionStatusType.Published
        };

        public static readonly List<EditionStatusType> ValidEditionStatusesToList = new List<EditionStatusType>
        {
            EditionStatusType.Published,
            EditionStatusType.Approved
        };

        public static readonly int[] ValidEditionStatusesToClone =
        {
            EditionStatusType.Published.GetHashCode()
        };

        public static readonly int[] ValidEditionStatusesToEdit =
        {
            EditionStatusType.Published.GetHashCode(),
            EditionStatusType.Approved.GetHashCode()
        };

        public static readonly string[] ValidEventTypesForCed =
        {
            EventType.Exhibition.GetDescription(),
            EventType.ConferenceExhibition.GetDescription(),
            EventType.Conference.GetDescription()
        };

        public static readonly string[] ValidEventActivitiesForCed =
        {
            EventActivity.ShowHeld.GetDescription(),
            EventActivity.ShowCancelled.GetDescription()
        };

        public static readonly string[] ValidEventActivitiesToClone =
        {
            EventActivity.ShowHeld.GetDescription(),
            EventActivity.Unspecified.GetDescription(),
            EventActivity.ShowCancelled.GetDescription()
        };

        public static readonly string[] ValidEventActivitiesToEdit =
        {
            EventActivity.ShowHeld.GetDescription(),
            EventActivity.BusinessUnitError.GetDescription(),
            EventActivity.Unspecified.GetDescription()
        };

        public static readonly string[] ValidEventActivitiesToNotify =
        {
            EventActivity.ShowHeld.GetDescription(),
        };

        public static readonly string[] ConferenceEventTypes =
        {
            EventType.Conference.GetDescription(),
            EventType.ConferenceExhibition.GetDescription(),
            EventType.ExhibitionConference.GetDescription()
        };

        public static readonly EventType[] EventTypesWithDelegates =
        {
            EventType.Conference,
            EventType.ConferenceExhibition,
            EventType.ExhibitionConference
        };

        public static readonly string[] PostShowMetricsEventTypes =
        {
            EventType.ExhibitionConference.GetDescription()
        };

        public const string NoProfilePicFileName = "no-profile-pic.png";

        public const string WarningMessageEventCancelled = "This event's status has been set to <b>Cancelled</b>, therefore further data entry/modification is disabled.<br/>If you believe the event is still active, please contact:<br/><a href='mailto:event-setup@hyve.group'><i class='fa fa-envelope'></i> Reporting Department</a>";

        public const string EditionNotFound = "Edition not found!";

        public const string EditionTranslationNotFound = "Edition Translation not found!";

        public const string ErrorWhileSavingFile = "Error while saving file!";

        public const char EmailAddressSeparator = ',';

        public const string FileNotFound = "File not found!";

        public const string FileSaved = "File saved!";

        public const string FileDeleted = "File deleted!";

        public const string FileNotDeleted = "File not deleted!";

        public const string FileAlreadyExistsWithTheSameName = "Another file with the same file name already exists. Please rename the file you're trying to upload or select another file.";

        public const string InvalidFileExtension = "Invalid file extension!";

        public const string InvalidFileSize = "Invalid file size!";

        public const string InvalidFileDimension = "Invalid file dimension!";

        public const string ErrorWhileDeletingFileOnAzureStorage = "Error while deleting file on Azure Storage!";

        public const string NotAuthorizedToEditionsOfWaitingForApproval = "You are not authorized to see editions of status Waiting For Approval.";

        public const string NoEventFoundWithThisId = "No event has been found with this id.";
    }
}
