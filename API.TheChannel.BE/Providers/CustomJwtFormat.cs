using System;

using API.TheChannel.BE.Models;
using API.TheChannel.BE.Repositories;

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.IdentityModel.Tokens;
using Thinktecture.IdentityModel.Tokens;

namespace API.TheChannel.BE.Providers
{
    public class CustomJwtFormat : ISecureDataFormat<Microsoft.Owin.Security.AuthenticationTicket>
    {
        private readonly AudienceRepository _repository;

        private const string AudiencePropertyKey = "audience";

        private readonly string _issuer = string.Empty;

        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
            _repository = new AudienceRepository();
        }

        /// <summary>
        /// JB. Preparing the raw data for the JSON Web Token which will be issued to the requester by providing the issuer, audience, user claims, issue date, expiry date, and the signing Key which will sign the JWT payload.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            string audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) ? data.Properties.Dictionary[AudiencePropertyKey] : null;

            if (string.IsNullOrWhiteSpace(audienceId)) throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");


            //Audience audience = AudienceStore.FindAudience(audienceId);
            Audience audience = _repository.ReturnAudienceById(audienceId);

            string symmetricKeyAsBase64 = audience.Base64Secret;

            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);

            //JB. This is IdentityServer (Thinktecture) working.
            var signingKey = new HmacSigningCredentials(keyByteArray);

            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, signingKey);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;
        }

        /// <summary>
        /// JB. Not needed now but could be in future to remove an App from the protected list.
        /// </summary>
        /// <param name="protectedText"></param>
        /// <returns></returns>
        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}