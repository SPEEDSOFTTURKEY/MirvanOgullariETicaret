using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class HomeController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            await LoadCommonData();

            
            AnaSayfaResimRepository anaSayfaResimRepository = new AnaSayfaResimRepository();
            List<AnaSayfaResim> anaSayfaResims = new List<AnaSayfaResim>();
            anaSayfaResims = anaSayfaResimRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfa = anaSayfaResims;


            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunler = new List<Urun>();
            List<string> join = new List<string>();
            join.Add("UrunFotograf");
            join.Add("Renk");
            join.Add("UrunAltKategori");
            join.Add("UrunAnaKategori");



            urunler = urunRepository.GetirList(x => x.Durumu == 1, join).OrderByDescending(x => x.Id).Take(16).ToList();

            foreach (var urun in urunler)
            {
                urun.UrunFotograf = urun.UrunFotograf?.Where(f => f.VitrinMi == 1).ToList();
            }

            ViewBag.Urunler = urunler;

            List<Sepet> sepets = new List<Sepet>();
            sepets = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet");
            if (sepets != null)
            {
                ViewBag.SepetSayi = sepets.Count;
                ViewBag.Sepet = sepets;
            }
            else
            {
                ViewBag.SepetSayi = 0;
            }


            return View();
        }

        public IActionResult SignOut()
        {
            HttpContext.Session.Remove("Uyeler");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Search(string searchproduct)
        {
            await LoadCommonData();
            HttpContext.Session.Remove("searchvalue");
            HttpContext.Session.SetString("searchvalue", searchproduct);

            return RedirectToAction("Index", "ProductSearh");
        }

        public async Task<IActionResult> CategoryProducts(int kategoriId)
        {
            await LoadCommonData();
          
            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunler = new List<Urun>();
            urunler = HttpContext.Session.GetObjectFromJsonCollection<Urun>("UrunListesi");

            if (urunler != null)
            {
                if (urunler.Count > 0)
                {
                    List<string> join = new List<string>();
                    join.Add("UrunFotograf");
                    join.Add("Renk");
                    join.Add("UrunAltKategori");
                    join.Add("UrunAnaKategori");

                    urunler = urunRepository.GetirList(x => x.Durumu == 1 && x.UrunAnaKategoriId == kategoriId, join).ToList();

                    foreach (var urun in urunler)
                    {
                        urun.UrunFotograf = urun.UrunFotograf?.Where(f => f.VitrinMi == 1).ToList();
                    }
                    ViewBag.Urunler = urunler;
                }
            }
            else
            {
                List<string> join = new List<string>();
                join.Add("UrunFotograf");
                join.Add("UrunAltKategori");
                join.Add("UrunAnaKategori");
                urunler = urunRepository.GetirList(x => x.Durumu == 1 && x.UrunAnaKategoriId == kategoriId, join).OrderByDescending(x => x.Id).Take(28).ToList();

                //x.UrunFotograf.Any(f => f.VitrinMi == 1)
                ViewBag.Urunler = urunler;
            }
            return View();
        }


    }
}
