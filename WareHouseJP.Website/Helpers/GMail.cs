using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
namespace WareHouseJP.Website.Helpers
{
    public class GMail
    {
        public static String email = "cntt@beterlifejp.com";
        public static String password = "A01qa2ws3ed";

        public static void Send(String to, String subject, String body)
        {
            String from = "BLF-JP <" + email + ">";
            GMail.Send(from, to, "", "", subject, body, "");
        }
        public static void Send(String from, String to, String subject, String body)
        {
            GMail.Send(from, to,"","", subject, body,"");
        }
        public static void Send(List<String> to, String subject, String body)
        {
            String from = "BLF-JP <" + email + ">";
            foreach (var item in to)
            {
                Send(item, subject, body);
            }
        }
        public static void Send(String from, String to, String cc, String bcc, String subject, String body, String attachments)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.ReplyToList.Add(new MailAddress(from));
            mail.To.Add(new MailAddress(to));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            if (!String.IsNullOrEmpty(cc))
            {
                mail.CC.Add(cc);
            }
            if (!String.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(bcc);
            }
            if (!String.IsNullOrEmpty(attachments))
            {
                String[] fileNames = attachments.Split(";,".ToCharArray());
                foreach (String fileName in fileNames)
                {
                    if (fileName.Trim().Length > 0)
                    {
                        mail.Attachments.Add(new Attachment(fileName.Trim()));
                    }
                }
            }

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(email, password);
            client.Send(mail);
        }
        
    }
}