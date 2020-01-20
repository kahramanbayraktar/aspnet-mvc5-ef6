using System;

namespace Ced.BusinessEntities
{
    public class EditionFieldAttribute : Attribute
    {
        public EditionInfoType InfoType { get; set; }

        public bool Required { get; set; }

        public EditionFieldAttribute(EditionInfoType infoType, bool required = true)
        {
            InfoType = infoType;
            Required = required;
        }
    }
}
