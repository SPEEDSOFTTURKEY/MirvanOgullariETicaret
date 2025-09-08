using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class SiparisOnaylanmadiController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            ViewBag.Hata = HttpContext.Session.GetString("OdemeHataMesaj");
            return View();
        }
    }
}
