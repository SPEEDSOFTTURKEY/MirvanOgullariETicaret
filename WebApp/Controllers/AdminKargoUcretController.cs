using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AdminKargoUcretController : AdminBaseController
    {
		public IActionResult Index()
		{
			KargoUcretRepository repository = new KargoUcretRepository();
			List<KargoUcret> modelListesi = repository.GetirList(x => x.Durumu == 1).ToList();
			ViewBag.KargoUcretListesi = modelListesi;
			return View();
		}
		public IActionResult Guncelleme(int id)
		{
			KargoUcretRepository repository = new KargoUcretRepository();
			KargoUcret kargoUcret = repository.Getir(id);
			if (kargoUcret != null)
			{
				HttpContext.Session.SetObjectAsJson("KargoUcret", kargoUcret);
			}
			return RedirectToAction("Index", "AdminKargoUcretGuncelle");
		}

		public IActionResult Sil(int id)
		{
			KargoUcretRepository repository = new KargoUcretRepository();
			KargoUcret kargoUcret = repository.Getir(id);
			if (kargoUcret != null)
			{
				kargoUcret.Durumu = 0;
				kargoUcret.GuncellemeTarihi = DateTime.Now;
				repository.Guncelle(kargoUcret);
			}
			return RedirectToAction("Index", "AdminKargoUcret");
		}
	}
}
