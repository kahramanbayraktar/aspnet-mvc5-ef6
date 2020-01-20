namespace Ced.BusinessEntities
{
    public enum ActionType
    {
        [ActionType(EntityType = EntityType.Edition)] EditionDetails,

        [ActionType(EntityType = EntityType.Edition)] EditionEdit,

        [ActionType(EntityType = EntityType.Edition)] EditionClone,

        [ActionType(EntityType = EntityType.Edition)] EditionPdf,

        [ActionType(EntityType = EntityType.Edition)] EditionDraftEdit,

        [ActionType(EntityType = EntityType.Edition)] EditionDraftSendForApproval,

        [ActionType(EntityType = EntityType.Edition)] EditionDraftApprove,

        [ActionType(EntityType = EntityType.Edition)] EditionDraftReject,

        [ActionType(EntityType = EntityType.Edition)] EditionSubscribe,

        [ActionType(EntityType = EntityType.Edition)] TaskEditionUpdateFromStaging,

        [ActionType(EntityType = EntityType.Edition)] TaskNotifyAboutMissingEditionImages,

        [ActionType(EntityType = EntityType.Edition)] TaskEditionUpdateFromKentico,

        [ActionType(EntityType = EntityType.Event)] EventDashboard,

        [ActionType(EntityType = EntityType.Event)] EventEdit,

        [ActionType(EntityType = EntityType.EventDirector)] EventDirectorAdd,

        [ActionType(EntityType = EntityType.EventDirector)] EventDirectorEdit,

        [ActionType(EntityType = EntityType.EventDirector)] EventDirectorDelete,

        [ActionType(EntityType = EntityType.EventDirector)] TaskEventDirectorUpdate,

        [ActionType(EntityType = EntityType.File)] FileUpload,

        [ActionType(EntityType = EntityType.File)] FileDelete,

        [ActionType(EntityType = EntityType.Log)] LogDelete,

        [ActionType(EntityType = EntityType.Notification)] TaskNotificationUpdate,

        [ActionType(EntityType = EntityType.Notification)] NotificationEmailSend,

        [ActionType(EntityType = EntityType.Notification)] NotificationEmailSendFailure,

        [ActionType(EntityType = EntityType.User)] Login,
        
        [ActionType(EntityType = EntityType.User)] Logout,

        [ActionType(EntityType = EntityType.User)] ProfilePictureUpload,

        [ActionType(EntityType = EntityType.User)] ProfilePictureDelete,

        [ActionType(EntityType = EntityType.UserRole)] UserRoleAdd,

        [ActionType(EntityType = EntityType.None)] HelpRequest,

        [ActionType(EntityType = EntityType.None)] AccessRequest
    }
}