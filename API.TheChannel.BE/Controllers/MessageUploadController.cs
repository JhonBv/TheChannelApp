﻿using API.TheChannel.BE.Factories;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using API.TheChannel.BE.Repositories;
using API.TheChannel.BE.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;


namespace API.TheChannel.BE.Controllers
{
    public class MessageUploadController : ApiController
    {
   
        // Interface in place so you can resolve with IoC container of your choice
        private readonly IMessageService _service = new VoiceMessageService();
        private readonly IVoiceMessageRepository _voiceMessage = new VoiceMessageRepository();
        private readonly IMessageFactory _createMessage = new MessageFactory();
        private ValidUserService _checkUser = new ValidUserService();

        [AllowAnonymous]
        [Route("postit")]
        public async Task<HttpResponseMessage> PostFormData(string language, string location)
        {

            MessageViewModel m = new MessageViewModel();
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            //JB. Forms the URL for the corresponding directory (i.e. English -> english)
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/" + language);
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> files = new List<string>();

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    var originalFileName = file.Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });
                    
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName + ".mp3");
                    
                    files.Add(Path.GetFileName(file.LocalFileName));
                    
                    //var medate = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_");
                    File.Move(file.LocalFileName, root +"/"+ originalFileName);
                    m.FileName = originalFileName;
                    m.FileUrl = ConfigurationManager.AppSettings["MessageUrlRoot"] + originalFileName;
                    m.DateAdded = DateTime.Now.ToLongDateString();
                    m.Location = location;
                    m.FileSizeInBytes = file.Headers.ContentDisposition.Size.GetValueOrDefault();

                   //_voiceMessage.AddToAzure(root+"/"+originalFileName);
                }
                _voiceMessage.SaveMessage(_createMessage.CreateMessageRecord(m));

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            
        }

        /// <summary>
        /// Gets All files in the physical directory.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        [Authorize]
        [Route("getem")]
        public Dictionary<string,string> GetMessages(string language)
        {
            var userId = User.Identity.GetUserId();
            if (_checkUser.userIsActive(userId) == false)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }
            return _voiceMessage.GetAllPhysicalFiles(language);
        }

        /// <summary>
        /// Gets a list of all new messages (not yet approved)
        /// </summary>
        /// <returns>IEnumerable&lt;MessageModel&gt;</returns>
        [Authorize]
        [Route("Messages/New")]
        public IEnumerable<MessageModel> GetNewMessages()
        {
            return _voiceMessage.ViewNewMessages().ToList();
        }

        /// <summary>
        /// Returns a list of mssages (approved) so that users can listen to.
        /// </summary>
        /// <returns>IEnumerable&lt;MessageViewModel&gt;</returns>
        [AllowAnonymous]
        [Route("Messages/approved")]
        public IEnumerable<MessageViewModel> GetApprovedMessages()
        {
            return _voiceMessage.ViewApprovedMessages().ToList();
        }        

        /// <summary>
        /// Gets all messages from the Database.
        /// </summary>
        /// <returns>LIst of messages</returns>
        [Authorize]
        [Route("Messages/All")]
        public IEnumerable<MessageModel> GetAllMessages()
        {
            return _voiceMessage.ViewAllMessages().ToList();
        }

        /// <summary>
        /// Allows an administratior to approve a message
        /// </summary>
        /// <param name="id">integer Id</param>
        /// <returns></returns>
        //[Authorize]
        [Route("Message/Approve")]
        public IHttpActionResult ApproveMessage(int id)
        {
             _voiceMessage.ApproveMessage(id);
            return Ok("Message Approved");
        }

        /// <summary>
        /// Allows an administrator to remove a message
        /// </summary>
        /// <param name="id">integer Id</param>
        /// <returns></returns>
        [Authorize]
        [Route("Message/Remove")]
        public IHttpActionResult RemoveMessage(int id)
        {
            _voiceMessage.RemoveMessage(id);
            return Ok("Message Removed");
        }

        /// <summary>
        /// Allows an administrator to remove a message
        /// </summary>
        /// <param name="mod">Model</param>
        /// <returns></returns>
        [Authorize]
        [Route("Message/update")]
        public IHttpActionResult UpdateMessage(MessageModel mod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _voiceMessage.UpdateMessage(mod);
            return Ok("Message Updated");
        }
    }
}
