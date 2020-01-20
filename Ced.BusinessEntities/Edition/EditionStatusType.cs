using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum EditionStatusType
    {
        [EditionStatus(FaIcon = "fa-circle-o", IconColor = "primary")]
        [Description("Draft")]
        Draft = 0,

        [EditionStatus(FaIcon = "fa-check-circle-o", IconColor = "success")]
        [Description("Published")]
        Published = 1,

        [EditionStatus(FaIcon = "fa-hourglass-2", IconColor = "warning")]
        [Description("Waiting For Approval")]
        WaitingForApproval = 2,

        [EditionStatus(FaIcon = "fa-certificate", IconColor = "info")]
        [Description("Approved")]
        Approved = 3,

        [EditionStatus(FaIcon = "fa-circle-thin", IconColor = "danger")]
        [Description("PreDraft")]
        PreDraft = 4
    }
}