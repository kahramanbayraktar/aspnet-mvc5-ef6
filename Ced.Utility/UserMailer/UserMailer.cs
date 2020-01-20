using Ced.Utility.Email;
using Ced.Utility.Web;
using Mvc.Mailer;

namespace Ced.Utility.UserMailer
{
    public class UserMailer : MailerBase
    {
        public UserMailer()
        {
            MasterName = "_Layout";
        }

        public virtual MvcMailMessage Send(EditionMailTemplate mailTemplate, bool isPrivate)
        {
            ViewBag.EditionId = mailTemplate.EditionId;
            ViewBag.EventId = mailTemplate.EventId;
            ViewBag.AxEventId = mailTemplate.AxEventId;
            ViewBag.EditionName = mailTemplate.EditionName;
            ViewBag.EventName = mailTemplate.EventName;
            ViewBag.Title = mailTemplate.Subject;
            ViewBag.EventDirectorFullName = mailTemplate.RecipientFullName;
            ViewBag.CedLogoUrl = WebConfigHelper.CedLogoUrl;
            ViewBag.WebLogoUrl = mailTemplate.WebLogoUrl;
            ViewBag.VenueName = mailTemplate.VenueName;
            ViewBag.DisplayDate = mailTemplate.DisplayDate;
            ViewBag.Body = mailTemplate.Body;
            ViewBag.ButtonText = mailTemplate.ButtonText;
            ViewBag.Url = mailTemplate.ButtonUrl;
            ViewBag.UnsubscriptionUrl = mailTemplate.UnsubscriptionUrl;
            ViewBag.GuideUrl = WebConfigHelper.ApplicationAbsolutePath + WebConfigHelper.QuickStartGuideFilePath;

            var msg = Populate(x =>
            {
                x.Subject = mailTemplate.Subject;
                x.ViewName = mailTemplate.PartialViewName;
                if (!isPrivate)
                    x.To.Add(mailTemplate.Recipients);
                else
                    x.Bcc.Add(mailTemplate.Recipients);
                x.Bcc.Add(WebConfigHelper.AdminEmails);
                x.ReplyToList.Add(WebConfigHelper.AdminEmails);
            });

            msg.Send();
            return msg;
        }

        public virtual MvcMailMessage Welcome()
        {
            ViewBag.Name = "Sohan";
            var msg = Populate(x => {
                x.Subject = "Welcome to MvcMailer";
                x.To.Add("kahramanb@ite-turkey.com");
                x.ViewName = "Welcome";
                x.Body = "test!";
            });

            msg.Send();
            return msg;
        }
    }
}