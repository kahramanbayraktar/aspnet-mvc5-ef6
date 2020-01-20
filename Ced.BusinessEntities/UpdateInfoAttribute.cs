using System;

namespace Ced.BusinessEntities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UpdateInfoAttr : Attribute
    {
        public string DisplayName { get; set; }

        public EditionImageType ImageType { get; set; }

        public UpdateInfoAttr(string displayName, EditionImageType imageType)
        {
            DisplayName = displayName;
            ImageType = imageType;
        }
    }
}
