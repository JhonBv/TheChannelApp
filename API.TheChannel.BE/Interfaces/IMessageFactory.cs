using API.TheChannel.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.TheChannel.BE.Interfaces
{
    public interface IMessageFactory
    {
        MessageModel CreateMessageRecord(MessageViewModel m);
    }
}
