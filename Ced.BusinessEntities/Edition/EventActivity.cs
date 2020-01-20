using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum EventActivity
    {
        [Description("Show Held")]
        ShowHeld,

        [Description("Show Cancelled")]
        ShowCancelled,

        [Description("Business Unit Error")]
        BusinessUnitError,

        [Description("Unspecified")]
        Unspecified,

        //[Description(null)]
        //Null
    }
}
