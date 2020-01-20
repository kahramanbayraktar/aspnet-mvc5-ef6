using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace Ced.Web.Helpers
{
    public class JwtHelper
    {
        // Just for checking if the jwt mechanism works and setting current user.
        public static ClaimsPrincipal ValidateJwt(string jwt)
        {
            //string symmetricKeyAsBase64 = ApiHelpers.Constants.AuthApiClientSecret;
            //var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            //var handler = new JwtSecurityTokenHandler();
            //var validationParameters = new TokenValidationParameters
            //{
            //    IssuerSigningToken = new JwtSecurityToken(jwt),
            //    ValidAudience = ApiHelpers.Constants.AuthApiClientId,
            //    ValidateAudience = true,
            //    ValidIssuer = ApiHelpers.Constants.AuthApiUrl,
            //    ValidateIssuer = true,
            //    CertificateValidator = X509CertificateValidator.None,
            //    RequireExpirationTime = true,
            //    IssuerSigningKey = new InMemorySymmetricSecurityKey(keyByteArray)
            //};

            //try
            //{
            //    SecurityToken validatedToken;
            //    var principal = handler.ValidateToken(jwt, validationParameters, out validatedToken);
            //    return principal;
            //}
            //catch (Exception e)
            //{

            //}

            return null;
        }
    }
}