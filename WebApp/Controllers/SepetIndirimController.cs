using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class SepetIndirimController : Controller
	{
		public IActionResult Index()
        {
      

            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler != null)
            {
                ViewBag.Uyeler = uyeler;
            }
            else
            {
                ViewBag.Uyeler = null;
            }

            SepetIndirimRepository repository = new SepetIndirimRepository();
			List<SepetIndirim> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
			ViewBag.SepetIndirimListesi = modelListesi;
			return View();
		}
		public IActionResult Guncelleme(int id)
		{
			SepetIndirimRepository repository = new SepetIndirimRepository();
			SepetIndirim sepetIndirim = repository.Getir(id);
			if(sepetIndirim != null)
			{
				HttpContext.Session.SetObjectAsJson("SepetIndirim", sepetIndirim);
			}
			return RedirectToAction("Index", "SepetIndirimGuncelle");
		}

		public IActionResult Sil(int id)
		{
			SepetIndirimRepository repository = new SepetIndirimRepository();
			SepetIndirim sepetIndirim = repository.Getir(id);
			if (sepetIndirim != null)
			{
				sepetIndirim.Durumu = 0;
				sepetIndirim.GuncellemeTarihi = DateTime.Now;
				repository.Guncelle(sepetIndirim);
			}
			return RedirectToAction("Index", "SepetIndirim");
		}
	}
}
