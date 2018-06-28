using System.Web.Http;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using API.TheChannel.BE.Repositories;

namespace API.TheChannel.BE.Controllers
{
    public class AudienceController : ApiController
    {
        private IAudiencesRepository _audiences = new AudienceRepository();
        /// <summary>
        /// Register a new App with the BSI AUth.
        /// </summary>
        /// <param name="audienceModel"></param>
        /// <returns>Confirmation String</returns>
        [Route("api/audiences/addAudience")]
        public IHttpActionResult Post(AudienceBindingModel audienceModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //JB. Check if the audience already exists. 
            if (audienceModel.Name.Equals(_audiences.ReturnAudienceByName(audienceModel.Name)) || audienceModel.URL.Equals(_audiences.ReturnAudienceByUrl(audienceModel.URL)))
            {

                return (BadRequest("Application Name and/or Url already exists"));
            }

            Audience newAudience = _audiences.AddAudience(audienceModel);
            return Ok<Audience>(newAudience);
        }
    }
}
