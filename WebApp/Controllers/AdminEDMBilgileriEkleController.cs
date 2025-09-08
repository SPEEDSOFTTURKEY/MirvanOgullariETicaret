using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
    public class AdminEDMBilgileriEkleController : AdminBaseController
    {
        EDMBilgileriRepository EDMBilgileriRepository = new EDMBilgileriRepository();
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Kaydet(EDMBilgileri EDMBilgileri)
        {
            if (EDMBilgileri != null)
            {
                if (EDMBilgileri.Unvan != string.Empty && EDMBilgileri.Unvan != null)
                {
                    EDMBilgileri EDMBilgileriKontrol = new EDMBilgileri();
                    EDMBilgileriKontrol = EDMBilgileriRepository.Listele().FirstOrDefault(x => x.Unvan == EDMBilgileri.Unvan && x.VergiNumarasi == EDMBilgileri.VergiNumarasi && x.Durumu == 1);
                    if (EDMBilgileriKontrol == null)
                    {                       
                        EDMBilgileri.Unvan = EDMBilgileri.Unvan.ToUpper();
                        EDMBilgileri.VergiDairesi = EDMBilgileri.VergiDairesi.ToUpper();
                        EDMBilgileri.VergiNumarasi = EDMBilgileri.VergiNumarasi;
                        EDMBilgileri.Adres = EDMBilgileri.Adres;                      
                        EDMBilgileri.Il = EDMBilgileri.Il;
                        EDMBilgileri.Ilce = EDMBilgileri.Ilce;
                        EDMBilgileri.PostaKodu = EDMBilgileri.PostaKodu;
                        EDMBilgileri.Email = EDMBilgileri.Email;
                        EDMBilgileri.Telefon = EDMBilgileri.Telefon;                      
                        EDMBilgileri.KullaniciAdi = EDMBilgileri.KullaniciAdi;
                        EDMBilgileri.Sifre = EDMBilgileri.Sifre;
                        EDMBilgileri.EklenmeTarihi = DateTime.Now;
                        EDMBilgileri.GuncellenmeTarihi = DateTime.Now;
                        EDMBilgileri.Durumu = 1;
                        EDMBilgileriRepository.Ekle(EDMBilgileri);
                    }
                }
            }

            return RedirectToAction("Index", "AdminEDMBilgileri");
        }
    }
}
