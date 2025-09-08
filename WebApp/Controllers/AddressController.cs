using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AddressController : Controller
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
                Uyeler uyeler1 = new Uyeler();
                UyelerRepository uyelerRepository = new UyelerRepository();
                uyeler1 = uyelerRepository.Getir(uyeler.Id);
                ViewBag.Uyeler = uyeler1;
                List<UyeAdres> uyeAdres = new List<UyeAdres>();
                UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
                uyeAdres = uyeAdresRepository.GetirList(x => x.UyeId == uyeler1.Id);
                ViewBag.Adres = uyeAdres;
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
        public IActionResult Sil(int Id)
        {
            UyeAdres uyeAdres = new UyeAdres();
            UyeAdresRepository uyeAdresRepository = new UyeAdresRepository();
            uyeAdres = uyeAdresRepository.Getir(Id);
            uyeAdres.Durumu = 0;
            uyeAdresRepository.Guncelle(uyeAdres);
            return RedirectToAction("Index", "Account");
        }
	}
}
