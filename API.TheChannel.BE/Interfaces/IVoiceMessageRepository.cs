using API.TheChannel.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.TheChannel.BE.Interfaces
{
    public interface IVoiceMessageRepository
    {
        void SaveMessage(MessageModel model);
        IEnumerable<MessageViewModel> ViewApprovedMessages();

        IEnumerable<MessageModel> ViewNewMessages();

        IEnumerable<MessageViewModel> ViewAllMessages();
        void ApproveMessage(int id);
        void RemoveMessage(int id);

        Dictionary<string, string> GetAllPhysicalFiles(string language);
    }
}
