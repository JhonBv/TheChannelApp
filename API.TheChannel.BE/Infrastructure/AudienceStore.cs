using API.TheChannel.BE.Models;
using API.TheChannel.BE.Repositories;

namespace API.TheChannel.BE.Infrastructure
{
    public static class AudienceStore
    {
        static AudienceStore()
        {

        }

        public static Audience FindAudience(string clientId)
        {
            AudienceRepository repo = new AudienceRepository();

            return repo.ReturnAudienceById(clientId);
        }
    }
}