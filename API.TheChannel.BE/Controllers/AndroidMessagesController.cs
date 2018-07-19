using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.TheChannel.BE.Models;

namespace API.TheChannel.BE.Controllers
{
    
    public class AndroidMessagesController : Controller
    {

        private readonly IVoiceMessageRepository _voiceMessage = new VoiceMessageRepository();

        // GET: AndroidMessages
        public ActionResult Index()
        {
           
            return View(_voiceMessage.ViewApprovedMessages());
        }
        
    }
}
