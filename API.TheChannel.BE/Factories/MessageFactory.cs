using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.TheChannel.BE.Factories
{
    /// <summary>
    /// Build the message SQL record
    /// </summary>
    public class MessageFactory:IMessageFactory
    {
        /// <summary>
        /// JB. Take a MessageViewModel from client, and convert into full Model so it can be persisted
        /// </summary>
        /// <param name="m">MessageViewModel</param>
        /// <returns>MessageModel</returns>
        public MessageModel CreateMessageRecord(MessageViewModel m)
        {
            MessageModel daModel = new MessageModel {
                FileName = m.FileName,
                FileUrl = m.FileUrl,
                DateAdded = m.DateAdded,
                Location = m.Location,
                FileSizeInBytes = m.FileSizeInBytes
            };

            return daModel;
        }
    }
}