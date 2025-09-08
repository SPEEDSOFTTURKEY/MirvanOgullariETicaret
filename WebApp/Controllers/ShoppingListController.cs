using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class ShoppingListController : BaseController
	{
		public async Task<IActionResult> Index()
        {
            await LoadCommonData();


            Uyeler uyeler = new Uyeler();
            uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            if (uyeler != null)
            {
                FavorilerRepository favorilerRepository = new FavorilerRepository();
                List<Favoriler> favorilers = new List<Favoriler>();
                List<string> join = new List<string>();
                join.Add("Urun");
                join.Add("Urun.UrunFotograf");
                favorilers = favorilerRepository.GetirList(x => x.Durumu == 1 && x.UyeId == uyeler.Id,join);
                UrunStokRepository urunStokRepository = new UrunStokRepository();
                foreach (var favori in favorilers)
                {
                    var stok = urunStokRepository.Getir(x => x.UrunId == favori.UrunId && x.Durumu == 1);
                    favori.Urun.UrunStok = stok; // favori ürününe stok bilgisini setle
                }

                ViewBag.Favoriler = favorilers;
                ViewBag.Uyeler = uyeler;
            }
            else
            {
                ViewBag.Uyeler = null;
            }

            return View();
		}
	}
}
