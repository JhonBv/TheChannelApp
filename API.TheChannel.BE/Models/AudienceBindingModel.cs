using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace API.TheChannel.BE.Models
{
    public class AudienceBindingModel
    {
        /// <summary>
        /// The name of the App. i.e. My BSI Shop
        /// </summary>
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Application's URL. i.e. http://www.myBsiShop.com
        /// </summary>
        [Required]
        public string URL { get; set; }
    }
}