using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.TheChannel.BE.Models
{
    public class MessageModel
    {
        /// <summary>
        /// Not passed by the clients, instead is generated automatically by the DB
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }

        public string DateAdded { get; set; }

        //public bool IsNew { get; set; }


        public bool isApproved { get; set; }
        //JB. As per isobel, this is where the message was sent from
        public string Location { get; set; }
        public long FileSizeInBytes { get; set; }
        

    }
}