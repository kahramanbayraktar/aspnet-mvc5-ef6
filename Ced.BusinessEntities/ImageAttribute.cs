using System;

namespace Ced.BusinessEntities
{
    public class ImageAttribute : Attribute
    {
        public string[] AllowedExtensions { get; set; }

        public int[] MinMaxLengths { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string Key { get; set; }
    }
}