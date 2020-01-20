namespace Ced.Web.Models.Token
{
    public class JwtTokenResponse
    {
        public string Access_Token { get; set; }
        
        public string Token_Type { get; set; }
        
        public int Expires_In { get; set; }
    }
}