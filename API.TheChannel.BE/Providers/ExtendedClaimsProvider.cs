using System.Security.Claims;
using API.TheChannel.BE.Infrastructure;

namespace API.TheChannel.BE.Providers
{
    /// <summary>
    /// JB. This class will set and retrieve Claims for users ;) 
    /// </summary>
    public class ExtendedClaimsProvider
    {
        private ApplicationDbContext _ctx;

        /// <summary>
        /// Instantiate this class
        /// </summary>
        /// <param name="context"></param>
        public ExtendedClaimsProvider(ApplicationDbContext context)
        {
            _ctx = context;
        }

        /// <summary>
        /// Add a new claim for a user
        /// </summary>
        /// <param name="type">What claim? i.e. Email</param>
        /// <param name="value">Such as elJuano@bacano.com</param>
        /// <returns></returns>
        public static System.Security.Claims.Claim CreateClaim(string type, string value)
        {
            return new System.Security.Claims.Claim(type, value, ClaimValueTypes.String);
        }
    }
}