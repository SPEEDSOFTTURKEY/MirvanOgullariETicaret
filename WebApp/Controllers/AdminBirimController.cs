using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBirimController : AdminBaseController
    {
        public IActionResult Index()
        {
            BirimlerRepository birimlerRepository = new BirimlerRepository();
            List<Birimler> birimler = birimlerRepository.GetirList(x => x.Durumu == 1);
           ViewBag.Birimler = birimler;
            return View();
        }
        public IActionResult Guncelle ( int id )
        {
            BirimlerRepository birimlerRepository = new BirimlerRepository();
            Birimler birimler = birimlerRepository.Getir(id);
            if (birimler != null) 
            {
            HttpContext.Session.SetObjectAsJson("Birimler",birimler);
            
            }
            return RedirectToAction("Index", "AdminBirimGuncelle");
    }
        public IActionResult Sil(int id) 
        {
        BirimlerRepository birimlerRepository=new BirimlerRepository();
            Birimler birimler = birimlerRepository.Getir(id);
            if (birimler != null)
            {
                birimler.Durumu = 0;
                birimler.GuncellenmeTarihi = DateTime.Now;
                birimlerRepository.Guncelle(birimler);

            }
            return RedirectToAction("Index", "AdminBirim");
        
        }
        }
}
