using System;

namespace Ced.BusinessEntities
{
    public class SocialMediaAttribute : Attribute
    {
        public string AccountUrlRoot { get; set; }

        public SocialMediaAttribute(string accountUrlRoot)
        {
            AccountUrlRoot = accountUrlRoot;
        }
    }
}
