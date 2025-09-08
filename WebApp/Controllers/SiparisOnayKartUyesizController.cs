using Microsoft.AspNetCore.Mvc;
using System.Text;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class SiparisOnayKartUyesizController : BaseController
    {
       
        public async Task<IActionResult> Index( string sipariskodu)
        {
            await LoadCommonData();


            Uyeler sessionUyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            ViewBag.Sepet = sepetList;
            ViewBag.SepetSayi = sepetList.Count;
            ViewBag.Uyeliksiz = sipariskodu;
            // Send email
            var mail = new EMail
            {
                EmailFrom = "2kids2momsiparis@gmail.com",
                EmailTo = "2kids2momsiparis@gmail.com",
                EmailHeader = "2KİDS2MOM ÜYESİZ SİPARİŞ BİLGİLENDİRME",
                EmailSubject = "2KİDS2MOM ÜYESİZ SİPARİŞ BİLGİLENDİRME",
                Html = true,
                Port = 587,
                Host = "smtp.gmail.com",
                Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD") ?? "uraenfbwynetexwe"
            };

            var emailBody = new StringBuilder();
            emailBody.Append("<p style='font-weight:bold'>2KİDS2MOM ÜYESİZ SİPARİŞ  BİLGİLENDİRME</p>");
            emailBody.Append("<p>Üyesiz Siparişiniz oluşturulmuştur.Admin Panelinden kontrol ediniz.</p>");
            emailBody.Append("<p>Sipariş Tarihi: " + DateTime.Now + "</p>");
            emailBody.Append("<hr>");

            mail.EmailBody = emailBody;
            mail.EmailGonder(mail);
            return View();


        }
    }
          

         
    
        }
      
        
    

