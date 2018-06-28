using API.TheChannel.BE.Factories;
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

namespace API.TheChannel.BE.Controllers
{
    public class MessageUploadController : ApiController
    {
        string accountName = "thechannelstorageacc";
        string accountKey = "x7w+KQ7wsETywh4WvCooSrvZ6kc+MHuiWleyrSDRW7X7NjgTIh3SiSRmTWaHwerxhWfh3xnwtgdRLi/3bQY4Bw==";

        // Interface in place so you can resolve with IoC container of your choice
        private readonly IMessageService _service = new VoiceMessageService();
        private IVoiceMessageRepository _voiceMessage = new VoiceMessageRepository();
        private IMessageFactory _createMessage = new MessageFactory();


        [Authorize]
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

                }
                _voiceMessage.SaveMessage(_createMessage.CreateMessageRecord(m));


                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            
        }

        [Authorize]
        [Route("getem")]
        public Dictionary<string,string> GetMessages(string language)
        {
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/" + language);
            DirectoryInfo d = new DirectoryInfo(root);
            FileInfo[] Files = d.GetFiles();
            Dictionary<string, string> MessageFiles = new Dictionary<string, string>();

            foreach (FileInfo file in Files)
            {
                MessageFiles.Add(file.Name, file.CreationTime.ToString());
            }

            return MessageFiles;
        }

        [Authorize]
        [Route("Messages/New")]
        public IEnumerable<MessageViewModel> GetNewMessages()
        {
            return _voiceMessage.ViewNewMessages().ToList();
        }

        [AllowAnonymous]
        [Route("Messages")]
        public IEnumerable<MessageViewModel> GetApprovedMessages()
        {
            return _voiceMessage.ViewApprovedMessages().ToList();
        }

        [Authorize]
        [Route("Messages/All")]
        public IEnumerable<MessageViewModel> GetAllMessages()
        {
            return _voiceMessage.ViewAllMessages().ToList();
        }
    }
}
