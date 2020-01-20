using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum NotificationType
    {
        [CompletenessNotificationType]
        [Notification(sendEmail: true, privateEmail: false, eventTypes: new[] { EventType.Exhibition, EventType.Conference, EventType.ConferenceExhibition }, checkDaysType: NotificationAttribute.CheckDaysTypes.Coming, viewName: "EditionExistence", buttonText: "Set up Event", faIcon: "unlink", textClass: "text-danger", fragment: "#newedition")]
        [Description("Next Edition of {0} must be set up in CED")]
        EditionExistence = 1,

        [CompletenessNotificationType]
        [Notification(sendEmail: true, privateEmail: false, eventTypes: new[] { EventType.Exhibition, EventType.Conference, EventType.ConferenceExhibition }, checkDaysType: NotificationAttribute.CheckDaysTypes.Coming, viewName: "GeneralInfoCompleteness", buttonText: "Get Started", faIcon: "info-circle", textClass: "text-danger")]
        [Description("Critical Event Edition Information is needed for {0}")]
        GeneralInfoCompleteness = 2,

        [CompletenessNotificationType]
        [Notification(sendEmail: true, privateEmail: false, eventTypes: new[] { EventType.Exhibition }, checkDaysType: NotificationAttribute.CheckDaysTypes.Passed, viewName: "PostShowMetricsInfoCompleteness", buttonText: "Get Started", faIcon: "bar-chart", textClass: "text-warning", fragment: "#step4")]
        [Description("Post Show Statistics needed for {0}")]
        PostShowMetricsInfoCompleteness = 3,

        [CompletenessNotificationType]
        [Notification(sendEmail: true, privateEmail: false, eventTypes: new[] { EventType.Exhibition }, checkDaysType: NotificationAttribute.CheckDaysTypes.Passed, viewName: "PostShowMetricsInfoCompleteness2", buttonText: "Got it. Take me to CED!", faIcon: "bar-chart", textClass: "text-danger", fragment: "#step4")]
        [Description("Post Show Statistics needed for {0}. Last 20 days")]
        PostShowMetricsInfoCompleteness2 = 4,



        [Notification(sendEmail: true, privateEmail: false, viewName: "EditionApproval", buttonText: "View event", faIcon: "certificate", textClass: "text-warning")]
        [Description("{0} is pending your approval")]
        DraftEditionWaitingForApproval = 5,

        [Notification(sendEmail: true, privateEmail: false, viewName: "EditionApproval", buttonText: "Take me to my event", faIcon: "check", textClass: "text-success")]
        [Description("{0} has been approved")]
        DraftEditionApproved = 6,

        [Notification(sendEmail: true, privateEmail: false, viewName: "EditionApproval", buttonText: "Take me back to the event setup screen", faIcon: "remove", textClass: "text-danger")]
        [Description("{0} has been rejected")]
        DraftEditionRejected = 7,
        


        [Notification(sendEmail: true, privateEmail: true, viewName: "EditionUpdate", buttonText: "View event", unsubscribable: true, faIcon: "bullseye", textClass: "text-warning")]
        [Description("{0} has been updated")]
        EditionUpdated = 8,

        [Notification(sendEmail: true, privateEmail: true, viewName: "EditionUpdate", buttonText: "Review venue location", faIcon: "map-marker", textClass: "text-warning")]
        [Description("Important information about location change for {0}")]
        EditionLocationUpdated = 9,

        [Notification(sendEmail: true, privateEmail: true, viewName: "EditionUpdate", buttonText: "Review edition", faIcon: "map-marker", textClass: "text-warning")]
        [Description("Event edition name change for {0}")]
        EditionNameUpdated = 11,

        [Notification(sendEmail: true, privateEmail: true, viewName: "EditionUpdate", buttonText: "View event edition", unsubscribable: true, faIcon: "bullseye", textClass: "text-warning")]
        [Description("{0} has been created")]
        EditionCreated = 12,



        [Notification(sendEmail: true, privateEmail: false, viewName: "MissingEditionImages", buttonText: "", unsubscribable: false, faIcon: "edit", textClass: "text-warning")]
        [Description("CED - Editions with Missing Images Found")]
        MissingEditionImages = 10
    }
}
