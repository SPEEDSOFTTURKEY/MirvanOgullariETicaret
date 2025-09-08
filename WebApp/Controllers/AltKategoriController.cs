using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AltKategoriController : Controller
	{
		public IActionResult Index(int Id)
		{
			AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
			AnaSayfaBannerMetin anaSayfaBannerMetin = new AnaSayfaBannerMetin();
			anaSayfaBannerMetin = anaSayfaBannerMetinRepository.Getir(x => x.Durumu == 1);
			ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetin;

			UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
			List<UrunAltKategori> altKategori = new List<UrunAltKategori>();
			List<string> join = new List<string>();
			join.Add("UrunAltKategoriFotograf");
			altKategori = urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == Id&&x.Durumu==1,join);
			ViewBag.AltKAtegori = altKategori;
			Uyeler uyeler = new Uyeler();
			uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
			if (uyeler != null)
			{
				ViewBag.Uyeler = uyeler;
			}
			else
			{
				ViewBag.Uyeler = null;
			}
			List<Sepet> sepets = new List<Sepet>();
			sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
			if (sepets != null)
			{
				ViewBag.SepetSayi = sepets.Count;
			}
			else
			{
				ViewBag.SepetSayi = 0;
			}
			return View();
		}
	}
}
