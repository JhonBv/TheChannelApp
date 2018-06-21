using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SendGrid;

namespace API.TheChannel.BE.Services
{
    public class EmailService : IIdentityMessageService
    {
        //Implemented from  IIdentityMessageService
        public async Task SendAsync(IdentityMessage message)
        {

            await configSendGridasync(message);

        }

        private async Task configSendGridasync(IdentityMessage message)
        {
            var myMessage = new SendGridMessage();

            myMessage.AddTo(message.Destination);
            //JB. Sender's details
            myMessage.From = new System.Net.Mail.MailAddress(
                ConfigurationManager.AppSettings["RegistrationsAdminEmail"],
                ConfigurationManager.AppSettings["RegistrationsAdminFrom"]);
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body;
            myMessage.Html = message.Body;


            var credentials = new NetworkCredential(
                //JB. Password is taken from the Azure portal.
                ConfigurationManager.AppSettings["mailAccount"],
                ConfigurationManager.AppSettings["mailPassword"]

            );

            // Create a Web transport for sending email.
            var transportWeb = new Web(credentials);

            // Send the email.
            if (transportWeb != null)
            {
                await transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                Trace.TraceError("Failed to create Web transport.");
                await Task.FromResult(0);
            }
        }
    }
}