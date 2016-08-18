using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.InternalSendMail.Service
{
    public interface IMailSender
    {
        //bool SendMail(string body = "", string Recipient = "", string RecipientBcc = "", string mysubject = "");
        //bool SendMail(string body = "", string Recipient = "", string RecipientBcc = "", string mysubject = "", string smtpsever = "");
        bool SendMail(string body = "", string Recipient = "", string RecipientBcc = "", string mysubject = "", MailAddress mailAddress = null, List<string> Attachments=null);
    }
}
