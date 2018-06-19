using API.TheChannel.BE.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.TheChannel.BE.Interfaces
{
    /// <summary>
    /// Interface to use as a Service Upload/Download
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Uploads a Message to the server
        /// </summary>
        /// <param name="httpContent">Binary</param>
        /// <returns></returns>
        Task<List<MessageUploadModel>> UploadMessageAsync(HttpContent httpContent);
        /// <summary>
        /// Downloads a message from the server
        /// </summary>
        /// <param name="blobId">int</param>
        /// <param name="language">Strin</param>
        /// <returns>MP3 file</returns>
        Task<MessageDownloadModel> DownloadMessageAsync(int blobId, string language);
    }
}
