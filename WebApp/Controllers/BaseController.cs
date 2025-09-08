using Microsoft.AspNetCore.Mvc;
using WebApp.Helpers;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
     
        protected async Task LoadCommonData()
        {
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
            List<Sepet> sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            sepetList = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            if (sepetList != null)
            {
                ViewBag.SepetSayi = sepetList.Count;
            }
            else
            {
                ViewBag.SepetSayi = 0;
            }

            ViewBag.Sepet = sepetList;
//            List<string> join = new List<string>();
//            join.Add("UrunFotograf");
//            join.Add("UrunBirim.Birimler");
      
//UrunRepository urunRepository = new UrunRepository();
//            List<Urun> urunler = new List<Urun>();

//            foreach (var sepetItem in sepetList)
//            {
//                var urun = urunRepository.Getir(x => x.Durumu == 1 && x.Id == sepetItem.UrunId,join);
//                if (urun != null)
//                {
//                    urunler.Add(urun);
//                }
//            }

//            ViewBag.Urunler = urunler;

            UrunAnaKategoriRepository urunAnaKategoriRepository = new UrunAnaKategoriRepository();
            List<string> join2 = new List<string>();
            join2.Add("UrunAnaKategoriFotograf");
            List<UrunAnaKategori> urunAnaKategori = urunAnaKategoriRepository.GetirList(x => x.Durumu == 1, join2);
            ViewBag.AnaKategori = urunAnaKategori;
            UrunAltKategoriRepository urunAltKategoriRepository = new UrunAltKategoriRepository();
            List<UrunAltKategori> urunAltKategori = urunAltKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AltKategori = urunAltKategori;
            IletisimBilgileriRepository bilgileriRepository = new IletisimBilgileriRepository();
            IletisimBilgileri iletisim = bilgileriRepository.Getir(x => x.Durumu == 1);
            if (iletisim != null)
            {
                ViewBag.Iletisim = iletisim;
            }

            HakkimizdaBilgileriRepository hakkimizdabilgileriRepository = new HakkimizdaBilgileriRepository();
            HakkimizdaBilgileri hakkimizda = hakkimizdabilgileriRepository.Getir(x => x.Durumu == 1);
            if (hakkimizda != null)
            {
                ViewBag.Hakkimizda = hakkimizda;
            }

            HakkimizdaFotografRepository hakkimizdaFotografRepository = new HakkimizdaFotografRepository();
            var hakkimizdafotograf = hakkimizdaFotografRepository.Getir(x => x.Durumu == 1);
            if (hakkimizdafotograf != null)
            {
                ViewBag.HakkimizdaFotograf = hakkimizdafotograf;
            }
            AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository = new AnaSayfaBannerMetinRepository();
            List<AnaSayfaBannerMetin> anaSayfaBannerMetin = new List<AnaSayfaBannerMetin>();
            anaSayfaBannerMetin = anaSayfaBannerMetinRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetin;
            AnaSayfaBannerResimRepository anaSayfaBannerResimRepository = new AnaSayfaBannerResimRepository();
            List<AnaSayfaBannerResim> anasayfa = anaSayfaBannerResimRepository.GetirList(x => x.Durumu == 1);
            if (anasayfa != null)
            {
                ViewBag.AnaSayfaBanner = anasayfa;
            }

        }
    }
}
