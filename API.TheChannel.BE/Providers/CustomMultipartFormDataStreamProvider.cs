using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace API.TheChannel.BE.Providers
{
    public class CustomMultipartFormDataStreamProvider: MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path)
        { }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {

            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";

            return name; //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped

        }
    }
}