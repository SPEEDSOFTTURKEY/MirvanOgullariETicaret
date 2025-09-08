using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUyelerGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            Uyeler guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            UyelerRepository uyelerRepository = new UyelerRepository();
            List<string> join = new List<string>();
            join.Add("UyeAdres");
          
            guncellemeBilgisi = uyelerRepository.Getir(x => x.Durumu == 1 && x.Id == guncellemeBilgisi.Id, join);
            ViewBag.Uyeler=guncellemeBilgisi;
          

            return View();
        }
        public IActionResult Kaydet(Uyeler uyeler)
        {
         
                UyelerRepository repository = new UyelerRepository();
                Uyeler existingEntity = repository.Getir(uyeler.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = uyeler.Adi;
                    existingEntity.Sifre = uyeler.Sifre;
                    existingEntity.Soyadi = uyeler.Soyadi;
                    existingEntity.Telefon = uyeler.Telefon;
                    existingEntity.EMail = uyeler.EMail;
                    existingEntity.Turu = uyeler.Turu;
                    existingEntity.EBulten = uyeler.EBulten;
                    existingEntity.TCKimlikNo = uyeler.TCKimlikNo;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);

                    }
                return RedirectToAction("Index", "AdminUyeler");


            
        }
        public IActionResult AdresKaydet(UyeAdres uyeAdres) {
        
        UyeAdresRepository repository = new UyeAdresRepository();
            UyeAdres existingEntity= repository.Getir(uyeAdres.Id);
            if (existingEntity != null) {
            
            existingEntity.Adres=uyeAdres.Adres;
                existingEntity.il=uyeAdres.il;
                existingEntity.ilce=uyeAdres.ilce;
                existingEntity.Adi= uyeAdres.Adi;
                existingEntity.Soyadi= uyeAdres.Soyadi;
                existingEntity.AdresBasligi= uyeAdres.AdresBasligi;
                existingEntity.Telefon= uyeAdres.Telefon;
                existingEntity.FaturaTuru= uyeAdres.FaturaTuru;
                existingEntity.GuncellenmeTarihi=DateTime.Now;
                repository.Guncelle(existingEntity);

            }


            return RedirectToAction("Index", "AdminUyelerGuncelle");


        }

        //public IActionResult IlceGetir(int id) {
         
        //    List<ilceler> ilceler   = new List<ilceler>();
        //    ilcelerRepository ilcelerRepository = new ilcelerRepository();
        //    ilceler=ilcelerRepository.Getir(x => x.Durumu==1 && x.il_id ==id);

        //    ViewBag.ilceler = ilceler;



        //}
    }
}
