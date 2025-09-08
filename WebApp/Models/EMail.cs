using System.Net.Mail;
using System.Net;
using System.Text;

namespace WebApp.Models
{
    public class EMail
    {
        public StringBuilder? EmailBody { get; set; }
        public string? EmailSubject { get; set; }
        public string? EmailFrom { get; set; }
        public string? EmailTo { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Html { get; set; }
        public int Port { get; set; }
        public string? Host { get; set; }
        public string? EmailHeader { get; set; }

        public void EmailGonder(EMail email)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient client = new SmtpClient();
                mail.From = new MailAddress(email.EmailFrom, EmailHeader);
                mail.To.Add(email.EmailTo);
                mail.Subject = email.EmailSubject;
                mail.IsBodyHtml = email.Html;
                mail.Body = email.EmailBody.ToString();
                client.Port = 587;
                client.Host = email.Host; // Burada doğru SMTP sunucusu ayarlandığından emin olun
                client.EnableSsl = true;
				client.UseDefaultCredentials = false;
			    client.Credentials = new NetworkCredential(email.EmailFrom, email.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
            }
            catch (SmtpException ex)
            {
                // Hata mesajını burada loglayabilirsiniz
                Console.WriteLine("SMTP Hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Diğer hataları yakalamak için
                Console.WriteLine("Hata: " + ex.Message);
            }
        }

    }
}
