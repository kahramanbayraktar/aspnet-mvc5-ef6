namespace Ced.BusinessEntities
{
    public enum TaskType
    {
        UpdateEventsDirectorsNotifications = 1,
        UpdateApprovedEditionsFromStagingDb = 2,
        UpdateEventsFromStagingDb = 3,
        UpdateEventDirectors = 4,
        UpdateNotifications = 5,
        UpdateEventsFromKentico = 6,
        NotifyAboutMissingEditionImages = 7
    }
}
