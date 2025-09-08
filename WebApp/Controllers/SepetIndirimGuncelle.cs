using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class SepetIndirimGuncelle : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();


            SepetIndirim sepetIndirim = HttpContext.Session.GetObjectFromJson<SepetIndirim>("SepetIndirim");
			ViewBag.SepetIndirim = sepetIndirim;
			return View();
		}
		public async Task<IActionResult> Kaydet(SepetIndirim sepetIndirim)
        {
            await LoadCommonData();


            SepetIndirimRepository repository = new SepetIndirimRepository();
			SepetIndirim existingEntity = repository.Getir(sepetIndirim.Id);
			if (existingEntity != null)
			{
				existingEntity.SepetTutari = sepetIndirim.SepetTutari;
				existingEntity.IndirimMiktari = sepetIndirim.IndirimMiktari;
				existingEntity.GuncellemeTarihi = DateTime.Now;
				repository.Guncelle(existingEntity);
			}
			return RedirectToAction("Index", "SepetIndirim");

		}
	}
}
