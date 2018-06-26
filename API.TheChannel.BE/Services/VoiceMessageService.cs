using API.TheChannel.BE.Hepers;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using API.TheChannel.BE.Providers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace API.TheChannel.BE.Services
{
    /// <summary>
    /// Implementation of the IMessageService interface to Upload/Download Messages
    /// </summary>
    public class VoiceMessageService : IMessageService
    {
        /// <summary>
        /// Download a Message
        /// </summary>
        /// <param name="blobId">Int</param>
        /// <param name="language">String</param>
        /// <returns>MP3 File</returns>
        public async Task<MessageDownloadModel> DownloadMessageAsync(int blobId, string language)
        {
            // blobName, which you should return as the result of this helper method.
            var blobName = GetBlobName(blobId, language);

            if (!String.IsNullOrEmpty(blobName))
            {
                var container = BlobHelper.GetBlobContainer();
                var blob = container.GetBlockBlobReference(blobName);

                // JB. Download the blob into a memory stream. Notice that we're not putting the memory
                // stream in a using statement. This is because we need the stream to be open for the
                // API controller in order for the file to actually be downloadable. The closing and
                // disposing of the stream is handled by the Web API framework.
                var ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);

                // JB. Strip off any folder structure so the file name is just the file name
                var lastPos = blob.Name.LastIndexOf('/');
                var fileName = blob.Name.Substring(lastPos + 1, blob.Name.Length - lastPos - 1);

                // JB. Build and return the download model with the blob stream and its relevant info
                var download = new MessageDownloadModel
                {
                    BlobStream = ms,
                    BlobFileName = fileName,
                    BlobLength = blob.Properties.Length,
                    BlobContentType = blob.Properties.ContentType
                };

                return download;
            }

            // Otherwise
            return null;
        }

        /// <summary>
        /// JB. Upload a new Message to the server
        /// </summary>
        /// <param name="httpContent">File name and location</param>
        /// <returns>Server response</returns>
        public async Task<List<MessageModel>> UploadMessageAsync(HttpContent httpContent)
        {
            var blobUploadProvider = new BlobStorageUploadProvider();

            var list = await httpContent.ReadAsMultipartAsync(blobUploadProvider)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        throw task.Exception;
                    }

                    var provider = task.Result;
                    return provider.Uploads.ToList();
                });

            // TODO: Use data in the list to store blob info in your
            // database so that you can always retrieve it later.

            return list;
        }

        /// <summary>
        /// Obtain the Container name of the Blob Storage
        /// </summary>
        /// <param name="blobId">Int</param>
        /// <param name="language">String</param>
        /// <returns>String{Blob name}</returns>
        private string GetBlobName(int blobId, string language)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudBlobContainer blobContainer = storageAccount.CreateCloudBlobClient().GetContainerReference(language);
            var blobs = blobContainer.ListBlobs(null, true, BlobListingDetails.All).Cast<CloudBlockBlob>();

            return "";
        }
    }
}