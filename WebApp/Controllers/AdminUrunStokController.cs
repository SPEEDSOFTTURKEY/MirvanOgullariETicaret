using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminUrunStokController : AdminBaseController
    {
        public IActionResult Index()
        {
            UrunStokRepository urunStokRepository = new UrunStokRepository();
            List<UrunStok> urunStok=new List<UrunStok>();
            urunStok = HttpContext.Session.GetObjectFromJsonCollection<UrunStok>("StokListesi");
            if (urunStok != null && urunStok.Count > 0)
            {
                ViewBag.UrunStok = urunStok;
            }
            else 
            {
                List<string> join = new List<string>
            {
                "Urun",
                "Urun.Renk",
                "UrunAnaKategori",
                "UrunAltKategori",
                "Birimler",
                "Urun.UrunFotograf"
            };
                urunStok = urunStokRepository.GetirList(x => x.Durumu == 1, join).ToList();

                ViewBag.UrunStok = urunStok;
            }
            return View();

        }
        public IActionResult Guncelleme(int id)
        {
            UrunStokRepository repository = new UrunStokRepository();
            UrunStok stok = repository.Getir(id);
            if(stok != null) {
                HttpContext.Session.SetObjectAsJson("UrunStok",stok);

        }
            
                return RedirectToAction("Index", "AdminUrunStokGuncelle");
            
        }
        [HttpPost]
        public IActionResult TopluSil(List<int> idList)
        {
            UrunStokRepository repository = new UrunStokRepository();

            foreach (int id in idList)
            {
                UrunStok stok = repository.Getir(id);
                if (stok != null)
                {
                    stok.Durumu = 0;
                    stok.GuncellenmeTarihi = DateTime.Now;
                    repository.Guncelle(stok);
                }
            }

            return RedirectToAction("Index", "AdminUrunStok");
        }
        public IActionResult Sil(int id)
        {
        
        UrunStokRepository repository=new UrunStokRepository();
            UrunStok stok =repository.Getir(id);
            if (stok != null)
            {
                stok.Durumu = 0;
                stok.GuncellenmeTarihi=DateTime.Now;
                repository.Guncelle(stok);
            }
            return RedirectToAction("Index", "AdminUrunStok");


        }
        public IActionResult Search(string adi)
        {
            HttpContext.Session.Remove("StokListesi");
            List<UrunStok> urunStok = new List<UrunStok>();
            UrunStokRepository urunStokRepository = new UrunStokRepository();
            List<string> join = new List<string>
            {
                "Urun",
                "UrunAnaKategori",
                "UrunAltKategori",
                "Birimler",
                "Urun.UrunFotograf"
            };
            urunStok = urunStokRepository.GetirList(x => x.Durumu == 1 && x.Urun.UrunFotograf != null&&x.Urun.Adi.Contains(adi), join);
            HttpContext.Session.SetObjectAsJson("StokListesi", urunStok);


            return RedirectToAction("Index", "AdminUrunStok");
        }
        public IActionResult GetAllData()
        {

            HttpContext.Session.Remove("StokListesi");
            return RedirectToAction("Index", "AdminUrunStok");
        }




    }
}
