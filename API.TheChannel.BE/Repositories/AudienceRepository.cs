using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using Microsoft.Owin.Security.DataHandler.Encoder;

namespace API.TheChannel.BE.Repositories
{
    public class AudienceRepository : BaseRepository, IAudiencesRepository
    {

        /// <summary>
        /// Adds a new entry in the database
        /// </summary>
        /// <param name="model">AudienceBindingModel</param>
        /// <returns>Newly created entry upon successful operation.</returns>
        public Audience AddAudience(AudienceBindingModel model)
        {
            var clientId = Guid.NewGuid().ToString("N");


            var key = new byte[32];
            RandomNumberGenerator.Create().GetBytes(key);
            var base64Secret = TextEncodings.Base64Url.Encode(key);

            Audience newAudience = new Audience { ClientId = clientId, Base64Secret = base64Secret, Name = model.Name, URL = model.URL };

            Ctx.Audience.Add(newAudience);
            Ctx.SaveChanges();

            return newAudience;
        }

        /// <summary>
        /// Used by the CustomJWFormat class.
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <returns>Full Audience to protect</returns>
        public Audience ReturnAudienceById(string id)
        {
            var myAudience = Ctx.Audience.Where(a => a.ClientId.Equals(id)).FirstOrDefault();

            if (myAudience == null)
            {
                return null;
            }

            return myAudience;
        }

        /// <summary>
        /// Used by the AudiencesController.
        /// </summary>
        /// <param name="name">Client Name</param>
        /// <returns>If Audience Exists, return Audience Name</returns>
        public Audience ReturnAudienceByName(string name)
        {
            var myAudience = Ctx.Audience.Where(a => a.Name.Equals(name)).FirstOrDefault();

            if (myAudience == null)
            {
                return null;
            }

            return myAudience;
        }

        /// <summary>
        /// Used by the AudiencesController.
        /// </summary>
        /// <param name="url">Client's URL</param>
        /// <returns>If Audience Exists, return Audience URL</returns>
        public Audience ReturnAudienceByUrl(string url)
        {
            var myAudience = Ctx.Audience.Where(a => a.URL.Equals(url)).FirstOrDefault();

            if (myAudience == null)
            {
                return null;
            }

            return myAudience;
        }

        public Task<string> RemoveAudience(string audienceId)
        {
            throw new NotImplementedException();
        }

        public Task<string> UpdateAudience(AudienceViewModel audienceDtls)
        {
            throw new NotImplementedException();
        }


        public AudienceViewModel GetAudienceById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Audience> GetAudienceList()
        {
            return Ctx.Audience.ToList();
        }

        public Audience GetAudienceByClientIdAndClientSecret(string clientId, string clientSecret)
        {
            var audience = Ctx.Audience.FirstOrDefault(a => a.ClientId.Equals(clientId) && a.Base64Secret.Equals(clientSecret));
            return audience;
        }
    }
}