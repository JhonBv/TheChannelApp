using System.Collections.Generic;
using System.Threading.Tasks;
using API.TheChannel.BE.Models;

namespace API.TheChannel.BE.Interfaces
{
    public interface IAudiencesRepository
    {
        IEnumerable<Audience> GetAudienceList();
        Audience AddAudience(AudienceBindingModel model);

        //JB. Used by the WebAPI to send details of a given audience upon request.
        AudienceViewModel GetAudienceById(string id);

        //JB. Used by the JWT Formatter
        Audience ReturnAudienceById(string id);
        //JB. Pass vehcile Id to update status to Available
        Task<string> RemoveAudience(string audienceId);
        Task<string> UpdateAudience(AudienceViewModel audienceDtls);

        Audience GetAudienceByClientIdAndClientSecret(string clientId, string clientSecret);
    }
}
