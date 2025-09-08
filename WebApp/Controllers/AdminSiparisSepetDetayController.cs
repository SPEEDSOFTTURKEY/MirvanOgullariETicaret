using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminSiparisSepetDetayController : AdminBaseController
    {
        public IActionResult Index()
        {
            int SiparisId=Convert.ToInt32(HttpContext.Session.GetInt32("SiparisSepetDetayId"));
            List<Sepet> sepets = new List<Sepet>();
            SepetRepository sepetRepository = new SepetRepository();
            List<string>join=new List<string>();
           // join.Add("Uye");
            join.Add("Birim");
            sepets=sepetRepository.GetirList(x => x.Durumu == 1 && x.SiparisId == SiparisId, join).ToList();
            ViewBag.Sepet=sepets;

            UrunRepository urunRepository = new UrunRepository();
            List<Urun> urunler = new List<Urun>();

            RenklerRepository renkRepository = new RenklerRepository();

            List<string> renkAdlari = new List<string>();

            foreach (var sepet in sepets)
            {
                // Sepet'teki her UrunId'ye göre Urunler tablosundan ürünleri alıyoruz
                Urun urun = urunRepository.Getir(x => x.Id == sepet.UrunId); // UrunId'ye göre ürünü buluyoruz
                if (urun != null)
                {
                    int renkId = urun.RenkId;

                    Renkler renk = renkRepository.Getir(x => x.Id == renkId);
                    if (renk != null)
                    {
                        // RenkAdı'nı ViewBag ile gönderiyoruz
                        //ViewBag.RenkAdi = renk.Adi;
                        renkAdlari.Add(renk.Adi);
                    }
                }
            }
            ViewBag.RenkAdlari = renkAdlari;


            return View();
        }
    }
}
