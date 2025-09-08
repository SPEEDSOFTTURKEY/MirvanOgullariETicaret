using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
    public class AdminEDMBilgileriGuncelleController : AdminBaseController
    {
        EDMBilgileriRepository EDMBilgileriRepository = new EDMBilgileriRepository();
        public IActionResult Index()
        {
            EDMBilgileri EDMBilgileri = new EDMBilgileri();
            EDMBilgileri = SessionHelper.GetObjectFromJson<EDMBilgileri>(HttpContext.Session, "EDMBilgileri");
            return View(EDMBilgileri);
        }

        public IActionResult Kaydet(int Id, string Unvan, string VergiDairesi, Int64 VergiNumarasi,
            string Adres, string Il, string Ilce, string PostaKodu, string Email, string Telefon,
            string KullaniciAdi, string Sifre, int Durumu)
        {
            if (Unvan != string.Empty && Unvan != null)
            {
                EDMBilgileri EDMBilgileriKontrol = new EDMBilgileri();
                EDMBilgileriKontrol = EDMBilgileriRepository.Listele().Where(x => x.Id == Id).FirstOrDefault();
                if (EDMBilgileriKontrol != null)
                {
                    EDMBilgileri EDMBilgileri = new EDMBilgileri();
                    EDMBilgileri = EDMBilgileriRepository.Getir(Id);
                    EDMBilgileri.Unvan = Unvan.ToUpper();
                    EDMBilgileri.VergiDairesi = VergiDairesi.ToUpper();
                    EDMBilgileri.VergiNumarasi = VergiNumarasi;
                    EDMBilgileri.Adres = Adres;
                    EDMBilgileri.Il = Il;
                    EDMBilgileri.Ilce = Ilce;
                    EDMBilgileri.PostaKodu = PostaKodu;
                    EDMBilgileri.Email = Email;
                    EDMBilgileri.Telefon = Telefon;
                    EDMBilgileri.KullaniciAdi = KullaniciAdi;
                    EDMBilgileri.Sifre = Sifre;
                    EDMBilgileri.EklenmeTarihi = EDMBilgileri.EklenmeTarihi;
                    EDMBilgileri.GuncellenmeTarihi = DateTime.Now;
                    EDMBilgileri.Durumu = Durumu;

                    EDMBilgileriRepository.Guncelle(EDMBilgileri);
                }
            }

            return RedirectToAction("Index", "AdminEDMBilgileri");
        }
    }
}
