namespace Ced.BusinessEntities
{
    public enum EditionImageType
    {
        [Image(
            AllowedExtensions = new[] { ".png" },
            MinMaxLengths = new[] { 0, 25 },
            Width = 214,
            Height = 111,
            Key = "WebLogo")]
        WebLogo,

        [Image(
            AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" },
            MinMaxLengths = new[] { 0, 300 },
            Width = 1140,
            Height = 283,
            Key = "PeopleImage")]
        PeopleImage,

        [Image(
            AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" },
            MinMaxLengths = new[] { 0, 300 },
            Width = 794,
            Height = 293,
            Key = "ProductImage")]
        ProductImage,

        [Image(
            AllowedExtensions = new[] { ".png" },
            MinMaxLengths = new[] { 0, 500 },
            Width = 0,
            Height = 0,
            Key = "MasterLogo")]
        MasterLogo,

        [Image(
            AllowedExtensions = new[] { ".png" },
            MinMaxLengths = new[] { 0, 500 },
            Width = 580,
            Height = 190,
            Key = "CrmLogo")]
        CrmLogo,

        [Image(
            AllowedExtensions = new[] { ".png" },
            MinMaxLengths = new[] { 0, 25 },
            Width = 64,
            Height = 64,
            Key = "Icon")]
        Icon,

        [Image(
            AllowedExtensions = new[] { ".png" },
            MinMaxLengths = new[] { 0, 25 },
            Width = 0,
            Height = 0,
            Key = "PromotedLogo")]
        PromotedLogo,

        [Image(
            AllowedExtensions = new[] { ".jpg", ".jpeg", ".png" },
            MinMaxLengths = new[] { 0, 300 },
            Width = 500,
            Height = 300,
            Key = "DetailsImage")]
        DetailsImage
    }
}