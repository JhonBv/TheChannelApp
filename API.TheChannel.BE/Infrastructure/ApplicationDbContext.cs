using System.Data.Entity;
using API.TheChannel.BE.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace API.TheChannel.BE.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("theChannelAppCon")
        {

        }
        public DbSet<Audience> Audience { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

    }
}