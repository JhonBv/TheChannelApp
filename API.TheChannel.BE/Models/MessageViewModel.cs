using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.TheChannel.BE.Models
{
    public class MessageViewModel
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public string DateAdded { get; set; }

        //JB. As per isobel, this is where the message was sent from
        public string Location { get; set; }
        public long FileSizeInBytes { get; set; }

        public bool isApproved { get; set; }
    }
}