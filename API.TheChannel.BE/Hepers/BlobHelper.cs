using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace API.TheChannel.BE.Hepers
{
    /// <summary>
    /// JB. Helper class to obtain the Container to which Upload/Download messages
    /// </summary>
    public static class BlobHelper
    {
        /// <summary>
        /// JB. Get the CloudBlobContainer from Azure
        /// </summary>
        /// <returns></returns>
        public static CloudBlobContainer GetBlobContainer()
        {
            // Pull these from config
            var blobStorageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var blobStorageContainerName = ConfigurationManager.AppSettings["BlobStorageContainerName"];

            // Create blob client and return reference to the container
            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
    }
}