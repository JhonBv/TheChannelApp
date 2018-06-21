using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace API.TheChannel.BE.Infrastructure
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Method which actually creates a user, assigns a Hash to th user password and generates a unique UserLoginId.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="authenticationType"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }
}