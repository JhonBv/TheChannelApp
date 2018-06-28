using API.TheChannel.BE.Infrastructure;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace API.TheChannel.BE.Repositories
{
    public class VoiceMessageRepository : IVoiceMessageRepository
    {
        ApplicationDbContext Ctx = new ApplicationDbContext();
        public void ApproveMessage(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveMessage(int id)
        {
            throw new NotImplementedException();
        }

        public void SaveMessage(MessageModel model)
        {
            Ctx.Message.Add(model);
            Ctx.SaveChanges();
        }

        public IEnumerable<MessageViewModel> ViewApprovedMessages()
        {
            List<MessageViewModel> list = new List<MessageViewModel>();
            foreach (var i in Ctx.Message.Where(m => m.isApproved))
            {
                list.Add(new MessageViewModel {FileName = i.FileName, FileUrl = i.FileUrl, Location = i.Location, DateAdded = i.DateAdded, FileSizeInBytes = i.FileSizeInBytes });
            }
            return list;
        }

        /// <summary>
        /// Returns a lsit of New Messages (unapproved)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MessageViewModel> ViewNewMessages()
        {
            List<MessageViewModel> list = new List<MessageViewModel>();
            foreach (var i in Ctx.Message.Where(m => m.isApproved))
            {
                list.Add(new MessageViewModel { FileName = i.FileName, FileUrl = i.FileUrl, Location = i.Location, DateAdded = i.DateAdded, FileSizeInBytes = i.FileSizeInBytes });
            }
            return list;
        }

        public IEnumerable<MessageViewModel> ViewAllMessages()
        {
            List<MessageViewModel> list = new List<MessageViewModel>();
            foreach (var i in Ctx.Message)
            {
                list.Add(new MessageViewModel { FileName = i.FileName, FileUrl = i.FileUrl, Location = i.Location, DateAdded = i.DateAdded, FileSizeInBytes = i.FileSizeInBytes });
            }
            return list;
        }

    }
}