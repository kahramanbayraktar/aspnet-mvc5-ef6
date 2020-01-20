using System.ComponentModel;

namespace Ced.BusinessEntities
{
    public enum SocialMediaType
    {
        [Description("facebook")]
        [SocialMedia("http://facebook.com/{0}")]
        Facebook,
        [Description("foursquare")]
        [SocialMedia("http://foursquare.com/v/{0}")]
        Foursquare,
        [Description("google-plus")]
        [SocialMedia("http://plus.google.com/+{0}")]
        GooglePlus,
        [Description("instagram")]
        [SocialMedia("http://instagram.com/{0}")]
        Instagram,
        [Description("linkedin")]
        [SocialMedia("http://linkedin.com/company/{0}")]
        LinkedIn,
        [Description("pinterest")]
        [SocialMedia("http://pinterest.com/{0}")]
        Pinterest,
        [Description("stumbleupon")]
        [SocialMedia("http://stumbleupon.com/stumbler/{0}")]
        Stumbleupon,
        [Description("tumblr")]
        [SocialMedia("http://{0}.tumblr.com/")]
        Tumblr,
        [Description("twitter")]
        [SocialMedia("http://twitter.com/{0}")]
        Twitter,
        [Description("vimeo")]
        [SocialMedia("http://vimeo.com/{0}")]
        Vimeo,
        [Description("vk")]
        [SocialMedia("http://vk.com/{0}")]
        VKontakte,
        [Description("youtube")]
        [SocialMedia("http://youtube.com/user/{0}")]
        Youtube,
        [Description("xing")]
        [SocialMedia("http://xing.com/profile/{0}")]
        Xing
    }
}