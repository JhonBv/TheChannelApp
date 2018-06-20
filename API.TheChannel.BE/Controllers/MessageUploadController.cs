using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using API.TheChannel.BE.Providers;
using API.TheChannel.BE.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
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
using System.Web.Http.Description;

namespace API.TheChannel.BE.Controllers
{
    public class MessageUploadController : ApiController
    {
        string accountName = "thechannelstorageacc";
        string accountKey = "x7w+KQ7wsETywh4WvCooSrvZ6kc+MHuiWleyrSDRW7X7NjgTIh3SiSRmTWaHwerxhWfh3xnwtgdRLi/3bQY4Bw==";

        // Interface in place so you can resolve with IoC container of your choice
        private readonly IMessageService _service = new VoiceMessageService();

        //[System.Web.Mvc.HttpPost]
        [Route("postit")]
        public async Task<HttpResponseMessage> PostFormData(string language, string location)
        {

            //BinaryReader binread = new BinaryReader(myfile);

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
                CustomMultipartFormDataStreamProvider ppr = new CustomMultipartFormDataStreamProvider(root);

                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> files = new List<string>();

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName + ".mp3");
                    files.Add(Path.GetFileName(file.LocalFileName));
                    var medate = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_");
                    File.Move(file.LocalFileName, root + "\\VoiceMessage_" + location + "@" + medate + ".mp3");
                    //System.IO.File.Move(file.LocalFileName, "newMessage_english"+DateTime.Now.ToShortDateString()+"_.mp3");
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("getem")]
        public List<string> GetMessages(string language)
        {
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/" + language);
            DirectoryInfo d = new DirectoryInfo(root);
            FileInfo[] Files = d.GetFiles("*.MP3");
            List<string> MessageFiles = new List<string>();

            foreach (FileInfo file in Files)
            {
                MessageFiles.Add(file.Name);
            }

            return MessageFiles;
        }
    }
}
