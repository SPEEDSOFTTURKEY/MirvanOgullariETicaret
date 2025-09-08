using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminBirimGuncelleController : AdminBaseController
    {
        public IActionResult Index()
        {
            Birimler guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Birimler>("Birimler");
            ViewBag.Birimler=guncellemeBilgisi;
            return View();
        }
        public IActionResult Kaydet(Birimler birim)
        {
           
                BirimlerRepository birimlerRepository = new BirimlerRepository(); Birimler existingEntity = birimlerRepository.Getir(birim.Id);
                if (existingEntity != null)
                {
                    existingEntity.Adi = birim.Adi;
                    existingEntity.GuncellenmeTarihi = DateTime.Now;
                    birimlerRepository.Guncelle(existingEntity);

                }
                return RedirectToAction("Index", "AdminBirim");

            
            }
        }
    }
