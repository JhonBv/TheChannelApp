using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                List<string> files = new List<string>();

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName + ".mp3");
                    // "\"hindu23 Sep 2016 16_32_58.mp3\""
                    files.Add(Path.GetFileName(file.LocalFileName));
                    
                                        
                    var originalFileName = file.Headers.ContentDisposition.FileName.Trim(new char[] { '\"' });
                    
                    //var medate = DateTime.Now.ToString().Replace("/", "_").Replace(" ", "_").Replace(":", "_");
                    File.Move(file.LocalFileName, root +"/"+ originalFileName); //"\\VoiceMessage_" + location + "@" + medate + ".mp3");

                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("getem")]
        public Dictionary<string,string> GetMessages(string language)
        {
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/" + language);
            DirectoryInfo d = new DirectoryInfo(root);
            FileInfo[] Files = d.GetFiles("*.MP3");
            Dictionary<string, string> MessageFiles = new Dictionary<string, string>();

            foreach (FileInfo file in Files)
            {
                MessageFiles.Add(file.Name, file.CreationTime.ToString());
            }

            return MessageFiles;
        }
    }
}
