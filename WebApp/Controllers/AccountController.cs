using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Index()
        {
            IletisimBilgileri ıletisimBilgileri = new IletisimBilgileri();
            IletisimBilgileriRepository ıletisimBilgileriRepository = new IletisimBilgileriRepository();
            ıletisimBilgileri = ıletisimBilgileriRepository.Getir(x => x.Durumu == 1);
            ViewBag.Iletisim = ıletisimBilgileri;
            UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
            List<string> join2 = new List<string>();
            join2.Add("UrunAnaKategoriFotograf");
            List<UrunAnaKategori> urunAnaKategori = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1, join2);
            ViewBag.AnaKategori = urunAnaKategori;

            UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
            List<UrunAltKategori> urunAltKategori = urunAltKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AltKategori = urunAltKategori;

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

            AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
            List<AnaSayfaBannerMetin> anaSayfaBannerMetin = new List<AnaSayfaBannerMetin>();
            anaSayfaBannerMetin = anaSayfaBannerMetinRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetin;
            return View();
        }
	}
}
