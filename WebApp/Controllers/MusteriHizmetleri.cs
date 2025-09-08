using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class MusteriHizmetleri : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

			UrunRepository urunRepository = new UrunRepository();
			List<Urun> urunler = new List<Urun>();
			List<string> join = new List<string>();
			join.Add("UrunFotograf");
			join.Add("Renk");
			join.Add("UrunAltKategori");
			join.Add("UrunAnaKategori");

			urunler = urunRepository.GetirList(x => x.Durumu == 1, join);
			ViewBag.Urunler = urunler;
			return View();
        }
    }
}
