using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum EntityType
    {
        [Description("none")]
        None,

        [Description("edition")]
        Edition,

        [Description("editiontranslation")]
        EditionTranslation,

        [Description("email")]
        Email,

        [Description("event")]
        Event,

        [Description("event")]
        EventDirector,

        [Description("file")]
        File,

        [Description("log")]
        Log,

        [Description("notification")]
        Notification,

        [Description("user")]
        User,

        [Description("userrole")]
        UserRole
    }
}
