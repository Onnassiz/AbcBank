using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AbcBank.Controllers
{
    public class Mailer
    {
        public static void Send(string email, string subject, string messageBody)
        {
            var message = new MimeMessage();
            message.From.Add (new MailboxAddress ("ABC Bank", "no-reply@abc.co.uk"));
            message.To.Add (new MailboxAddress ("User", email));
            message.Subject = subject;

            message.Body = new TextPart ("html") {
                Text = "<br/><br/><br/>" + messageBody
            };

            message.WriteTo("wwwroot/emails/messages.html");

//            using (var client = new SmtpClient())
//            {
//                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
//                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
//
//                client.Connect("smtp.gmail.com", 587, false);
//
//                // Note: since we don't have an OAuth2 token, disable
//                // the XOAUTH2 authentication mechanism.
//                client.AuthenticationMechanisms.Remove("XOAUTH2");
//
//                // Note: only needed if the SMTP server requires authentication
//                client.Authenticate("cos317codes@gmail.com", "unncyber1");
//
//                client.Send(message);
//                client.Disconnect(true);
//            }
        }
    }
}