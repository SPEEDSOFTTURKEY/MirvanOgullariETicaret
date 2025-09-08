using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaRakamlariController : AdminBaseController
    {
        public IActionResult Index()
        {
            AnaSayfaRakamlariRepository repository = new AnaSayfaRakamlariRepository();
            List<AnaSayfaRakamlari> hizmetlerimizBilgileriList = new List<AnaSayfaRakamlari>();
            hizmetlerimizBilgileriList = repository.Listele().Where(x => x.Durumu == 1).ToList();
            ViewBag.Title = "Ana Sayfa Rakamları";
            ViewBag.RakamlarListesi = hizmetlerimizBilgileriList;
            return View();
        }
        public IActionResult Guncelleme(int id)
        {
            AnaSayfaRakamlariRepository repository = new AnaSayfaRakamlariRepository();

            AnaSayfaRakamlari rakamlari = repository.Getir(id);

            if (rakamlari != null)
            {
                HttpContext.Session.SetObjectAsJson("AnaSayfaRakamlari", rakamlari);
            }
            return RedirectToAction("Index", "AdminAnaSayfaRakamlariGuncelle");
        }


        public IActionResult Sil(int id)
        {
            AnaSayfaRakamlariRepository repository = new AnaSayfaRakamlariRepository();

            AnaSayfaRakamlari hizmetlerimizBilgileri = repository.Getir(id);

            if (hizmetlerimizBilgileri != null)
            {
                hizmetlerimizBilgileri.Durumu = 0;
                hizmetlerimizBilgileri.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(hizmetlerimizBilgileri);
            }

            return RedirectToAction("Index", "AdminAnaSayfaRakamlari");
        }
    }
}
