using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAcilKargoUcretGuncelleController : Controller
    {
        public IActionResult Index()
        {
            AcilKargoUcret kargoUcret = HttpContext.Session.GetObjectFromJson<AcilKargoUcret>("AcilKargoUcret");
            ViewBag.KargoUcret = kargoUcret;
            return View();
        }
        public IActionResult Kaydet(AcilKargoUcret kargoUcret)
        {

            AcilKargoUcretRepository repository = new AcilKargoUcretRepository();
            AcilKargoUcret existingEntity = repository.Getir(kargoUcret.Id);
            if (existingEntity != null)
            {
                existingEntity.Fiyat = kargoUcret.Fiyat;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                repository.Guncelle(existingEntity);
            }
            return RedirectToAction("Index", "AdminAcilKargoUcret");

        }
    }
}
