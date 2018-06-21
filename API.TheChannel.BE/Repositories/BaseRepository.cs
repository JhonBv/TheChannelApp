using API.TheChannel.BE.Infrastructure;

namespace API.TheChannel.BE.Repositories
{
    public class BaseRepository
    {
        protected ApplicationDbContext Ctx;

        public BaseRepository()
        {
            Ctx = new ApplicationDbContext();
        }
    }
}