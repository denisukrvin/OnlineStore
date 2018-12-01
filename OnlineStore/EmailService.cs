using System;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;

namespace OnlineStore.Models
{
    public static class EmailService
    {
        private static readonly string emailServiceLogin = WebConfigurationManager.AppSettings["EmailServiceLogin"];
        private static readonly string emailServicePassword = WebConfigurationManager.AppSettings["EmailServicePassword"];
        private static readonly string emailServiceSMTP = WebConfigurationManager.AppSettings["EmailServiceSMTP"];
        private static readonly int emailServicePort = Int32.Parse(WebConfigurationManager.AppSettings["EmailServicePort"]);


        public static void SendEmail(string recipient, string subject, string body)
        {
            MailAddress from = new MailAddress(emailServiceLogin, "Online Store");
            MailAddress to = new MailAddress(recipient);
            MailMessage m = new MailMessage(from, to);
            m.Subject = subject;
            m.Body = body;
            SmtpClient smtp = new SmtpClient(emailServiceSMTP, emailServicePort);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(emailServiceLogin, emailServicePassword);
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}