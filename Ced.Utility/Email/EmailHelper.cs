using Ced.Utility.Web;
using System.Net.Mail;

namespace Ced.Utility.Email
{
    public class EmailHelper : IEmailHelper
    {
        public void SendEmail(string subject, string body, string to)
        {
            using (var msg = new MailMessage())
            {
                msg.To.Add(to);
                msg.Bcc.Add(WebConfigHelper.AdminEmails);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                using (var client = new SmtpClient())
                {
                    client.Send(msg);
                }
            }
        }
    }
}