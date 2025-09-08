using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
namespace WebApp.Controllers
{
	public class AdminUyelerController : AdminBaseController
    {
		UyelerRepository repository = new UyelerRepository();
		public IActionResult Index()
		{
			List<string> join = new List<string>();
			join.Add("UyeAdres");
			List<Uyeler> modelListesi = repository.GetirList(x => x.Durumu == 1 && x.Adi != null && x.Adi != "1",join).ToList();
			ViewBag.UyelerList = modelListesi;
			
			return View();
		}

		public IActionResult Guncelleme(int id)
		{
			Uyeler uyeler = repository.Getir(id);
			if(uyeler != null)
			{
				HttpContext.Session.SetObjectAsJson("Uyeler", uyeler);
			}
			return RedirectToAction("Index", "AdminUyelerGuncelle");
		}
		public IActionResult Sil(int id)
		{
			Uyeler uyeler =repository.Getir(id);
			if(uyeler != null)
			{
				uyeler.Durumu = 0;
				uyeler.GuncellenmeTarihi = DateTime.Now;
				repository.Guncelle(uyeler);
			}
			return RedirectToAction("Index", "AdminUyeler");
		}
	}
}
