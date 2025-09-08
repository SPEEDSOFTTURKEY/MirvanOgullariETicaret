using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
	public class AboutController : Controller
	{
		public IActionResult Index()
		{
            AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
            List<AnaSayfaBannerMetin> anaSayfaBannerMetin = new List<AnaSayfaBannerMetin>();
            anaSayfaBannerMetin = anaSayfaBannerMetinRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetin;

            HakkimizdaBilgileriRepository hakkimizdaBilgileriRepository = new HakkimizdaBilgileriRepository();
            HakkimizdaBilgileri hakkimizdaBilgileri = new HakkimizdaBilgileri();
            hakkimizdaBilgileri = hakkimizdaBilgileriRepository.Getir(x => x.Durumu == 1);
            ViewBag.Hakkimizda= hakkimizdaBilgileri;
            UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
            List<string> join2 = new List<string>();
            join2.Add("UrunAnaKategoriFotograf");
            List<UrunAnaKategori> urunAnaKategori = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1, join2);
            ViewBag.AnaKategori = urunAnaKategori;

            UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
            List<UrunAltKategori> urunAltKategori = urunAltKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AltKategori = urunAltKategori;

            HakkimizdaFotografRepository hakkimizdaFotografRepository = new HakkimizdaFotografRepository();
            HakkimizdaFotograf hakkimizdaFotograf = new HakkimizdaFotograf();
            hakkimizdaFotograf = hakkimizdaFotografRepository.Getir(x => x.Durumu == 1);
            ViewBag.HakkimizdaFotograf = hakkimizdaFotograf;

            IletisimBilgileriRepository ıletisimBilgileriRepository = new IletisimBilgileriRepository();
            IletisimBilgileri ıletisimBilgileri = new IletisimBilgileri();
            ıletisimBilgileri = ıletisimBilgileriRepository.Getir(x => x.Durumu == 1);
            ViewBag.Iletisim = ıletisimBilgileri;
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

