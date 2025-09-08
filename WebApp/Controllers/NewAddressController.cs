using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class NewAddressController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();
            return View();
		}

        public async Task<IActionResult> Add(UyeAdres model)
        {
            await LoadCommonData();

            UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
            model.EklenmeTarihi = DateTime.Now;
            model.GuncellenmeTarihi = DateTime.Now;
            model.Durumu = 1;
            uyeAdresRepository.Ekle(model);
            return RedirectToAction("Index", "Address");

        }
	}
}
