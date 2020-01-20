using System;

namespace Ced.BusinessEntities.Auth
{
    public class TokenEntity
    {
        public int TokenId { get; set; }
        
        public int UserId { get; set; }
        
        public string TokenString { get; set; }
        
        public DateTime IssuedOn { get; set; }
        
        public DateTime ExpiresOn { get; set; }
    }
}
