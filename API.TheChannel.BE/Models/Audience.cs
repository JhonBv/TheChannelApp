﻿using System.ComponentModel.DataAnnotations;

namespace API.TheChannel.BE.Models
{
    public class Audience
    {
        /// <summary>
        /// Not passed by the clients, instead is generated automatically by the DB
        /// </summary>
        [Key]
        [MaxLength(32)]
        public string ClientId { get; set; }

        /// <summary>
        /// Very important part. The Secret generated by the DB is Base64
        /// </summary>
        [MaxLength(80)]
        [Required]
        public string Base64Secret { get; set; }

        /// <summary>
        /// This is passed by the client with a name that will identify the App.
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Important: This is where the Scope will be formed for any OIDC integration.
        /// </summary>
        [Required]
        [DataType(DataType.Url)]
        public string URL { get; set; }
    }
}