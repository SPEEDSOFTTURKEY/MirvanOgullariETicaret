using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunStokGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunStok urunStok = new UrunStok();
            urunStok= HttpContext.Session.GetObjectFromJson<UrunStok>("UrunStok");
            ViewBag.UrunStok = urunStok;
            return View();
        }

        public IActionResult Kaydet(UrunStok urunStok)
        {
         
            UrunStokRepository repository = new UrunStokRepository();
             
            UrunStok existingEntity = repository.Getir(urunStok.Id);
                if (existingEntity != null)
                {
                    existingEntity.Stok = urunStok.Stok;
                    existingEntity.Barkod = urunStok.Barkod;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(existingEntity);

		     	}
                return RedirectToAction("Index", "AdminUrunStok");
                
                }
          
    }
}
