using API.TheChannel.BE.Infrastructure;
using API.TheChannel.BE.Interfaces;
using API.TheChannel.BE.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace API.TheChannel.BE.Repositories
{
    public class VoiceMessageRepository : IVoiceMessageRepository
    {
       
        ApplicationDbContext Ctx = new ApplicationDbContext();
        public void ApproveMessage(int id)
        {
            var daMess = Ctx.Message.FirstOrDefault(m => m.Id == id);
            daMess.isApproved = true;
            Ctx.Entry(daMess).State = System.Data.Entity.EntityState.Modified;
            Ctx.SaveChanges();
        }

        public void RemoveMessage(int id)
        {
            var daMess = Ctx.Message.FirstOrDefault(m => m.Id == id);


            var daName = Ctx.Message.Where(i => i.Id == id).Select(n => n.FileName).FirstOrDefault();
            
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/english/" + daName);
            var uri = new Uri(root, UriKind.Absolute);
            System.IO.File.Delete(uri.LocalPath);

            Ctx.Message.Remove(daMess);
            Ctx.SaveChanges();
        }

        public void UpdateMessage(MessageModel model)
        {
            var daMess = Ctx.Message.FirstOrDefault(m => m.Id == model.Id);
            if (daMess != null)
            {
                daMess.isApproved = model.isApproved;
                daMess.FileName = model.FileName;
                Ctx.Entry(daMess).State = System.Data.Entity.EntityState.Modified;
            }

            Ctx.SaveChanges();

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
        public IEnumerable<MessageModel> ViewNewMessages()
        {
            List<MessageModel> list = new List<MessageModel>();
            foreach (var i in Ctx.Message.Where(m => m.isApproved == false))
            {
                list.Add(new MessageModel { Id=i.Id, FileName = i.FileName, FileUrl = i.FileUrl, Location = i.Location, DateAdded = i.DateAdded, FileSizeInBytes = i.FileSizeInBytes });
            }
            return list;
        }

        public IEnumerable<MessageModel> ViewAllMessages()
        {
            return Ctx.Message.ToList();
        }

        public Dictionary<string, string> GetAllPhysicalFiles(string language)
        {
            string root = HttpContext.Current.Server.MapPath("~/Content/Messages/" + language);
            DirectoryInfo d = new DirectoryInfo(root);
            FileInfo[] Files = d.GetFiles();
            Dictionary<string, string> messageFiles = new Dictionary<string, string>();

            foreach (FileInfo file in Files)
            {
                messageFiles.Add(file.Name, file.CreationTime.ToString());
            }

            return messageFiles;
        }

        
    }
}