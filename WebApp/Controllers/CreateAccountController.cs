using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class CreateAccountController : BaseController
	{
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            return View();
        }

            [HttpPost]
        public async Task<IActionResult> Register(Uyeler model, string Sifre2)
        {
            await LoadCommonData();

            if (model.Sifre != Sifre2)
            {
                TempData["Error"] = "Şifreler uyuşmuyor.";
                return RedirectToAction("Index", "LoginMember"); 
            }

            UyelerRepository repository = new UyelerRepository();
            var mevcutUye = repository.Getir(x => x.EMail == model.EMail && x.Durumu == 1);

            if (mevcutUye != null)
            {
                TempData["Error"] = "Bu e-posta adresi ile kayıtlı bir kullanıcı zaten var.";
                return RedirectToAction("Index", "LoginMember");
            }

            model.Durumu = 1;
            model.EklenmeTarihi = DateTime.Now;
            model.GuncellenmeTarihi = DateTime.Now;

            repository.Ekle(model);
            HttpContext.Session.SetObjectAsJson("Uyeler", model);

            return RedirectToAction("Index", "Home");
        }


    }
}
