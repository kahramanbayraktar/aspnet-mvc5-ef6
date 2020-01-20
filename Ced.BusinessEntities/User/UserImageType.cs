using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum UserImageType
    {
        [Image(
            AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" },
            MinMaxLengths = new[] { 0, 25600 },
            //Width = 214,
            //Height = 111,
            Key = "ProfilePic")]
        [Description("profilepic")]
        ProfilePic
    }
}