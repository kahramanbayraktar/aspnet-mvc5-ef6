using System;

namespace Ced.BusinessEntities
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EditionStatusAttribute : Attribute
    {
        public string FaIcon { get; set; }

        public string IconColor { get; set; }
    }
}