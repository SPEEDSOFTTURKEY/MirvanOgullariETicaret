using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class SepetController : Controller
    {
        private readonly UrunRepository _urunRepository;
        private readonly UrunStokRepository _urunStokRepository;
        private readonly BirimlerRepository _birimlerRepository;
        private readonly UrunAnaKategoriRepository _urunAnaKategoriRepository;
        private readonly UrunAltKategoriRepository _urunAltKategoriRepository;
        private readonly AnaSayfaBannerMetinRepository _anaSayfaBannerMetinRepository;

        public SepetController(
            UrunRepository urunRepository,
            UrunStokRepository urunStokRepository,
            BirimlerRepository birimlerRepository,
            UrunAnaKategoriRepository urunAnaKategoriRepository,
            UrunAltKategoriRepository urunAltKategoriRepository,
            AnaSayfaBannerMetinRepository anaSayfaBannerMetinRepository)
        {
            _urunRepository = urunRepository;
            _urunStokRepository = urunStokRepository;
            _birimlerRepository = birimlerRepository;
            _urunAnaKategoriRepository = urunAnaKategoriRepository;
            _urunAltKategoriRepository = urunAltKategoriRepository;
            _anaSayfaBannerMetinRepository = anaSayfaBannerMetinRepository;
        }

        public IActionResult Index()
        {
            Console.WriteLine("Index metodu TETİKLENDİ!");
            HttpContext.Session.Remove("UrunListesi");

            // Ana Kategori
            List<string> join2 = new List<string> { "UrunAnaKategoriFotograf" };
            List<UrunAnaKategori> urunAnaKategori = _urunAnaKategoriRepository.GetirList(x => x.Durumu == 1, join2);
            ViewBag.AnaKategori = urunAnaKategori;

            // Alt Kategori
            List<UrunAltKategori> urunAltKategori = _urunAltKategoriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AltKategori = urunAltKategori;

            // Banner Metin
            List<AnaSayfaBannerMetin> anaSayfaBannerMetin = _anaSayfaBannerMetinRepository.GetirList(x => x.Durumu == 1);
            ViewBag.AnaSayfaBannerMetin = anaSayfaBannerMetin;

            // Üye Bilgisi
            Uyeler uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
            ViewBag.Uyeler = uyeler;

            // Sepet Listesi
            List<Sepet> list = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
            SepetViewModel model = new()
            {
                SepetListesi = list,
                GenelToplam = list.Sum(x => x.Miktar * x.Fiyat ?? 0)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] AddToCartDto model)
        {
            try
            {
                Console.WriteLine("Add metodu TETİKLENDİ!");
                Console.WriteLine($"Gelen veri: {JsonSerializer.Serialize(model)}");

                // Model doğrulama
                if (model == null)
                {
                    Console.WriteLine("Hata: Model null.");
                    return Json(new { success = false, message = "Geçersiz veri." });
                }

                // Doğrulamalar
                if (model.Id <= 0)
                {
                    Console.WriteLine("Hata: Geçersiz ürün ID'si.");
                    return Json(new { success = false, message = "Geçersiz ürün ID'si." });
                }

                int birimId;
                if (string.IsNullOrEmpty(model.BirimId) || !int.TryParse(model.BirimId, out birimId))
                {
                    Console.WriteLine("Hata: Geçersiz birim ID'si.");
                    return Json(new { success = false, message = "Lütfen geçerli bir birim seçiniz." });
                }

                int intMiktar = 1;
                if (!string.IsNullOrEmpty(model.Miktar))
                {
                    if (!int.TryParse(model.Miktar, out intMiktar) || intMiktar < 1)
                    {
                        Console.WriteLine("Hata: Geçersiz miktar.");
                        return Json(new { success = false, message = "Geçersiz miktar." });
                    }
                }

                // Ürün kontrolü
                Console.WriteLine($"Ürün sorgulanıyor: Id={model.Id}");
                var urun = _urunRepository.Getir(x => x.Id == model.Id, new List<string> { "UrunFotograf" });
                if (urun == null)
                {
                    Console.WriteLine("Hata: Ürün bulunamadı.");
                    return Json(new { success = false, message = "Ürün bulunamadı!" });
                }
                Console.WriteLine($"Ürün bulundu: {urun.Adi}");

                // Stok kontrolü
                Console.WriteLine($"Stok sorgulanıyor: UrunId={urun.Id}, BirimId={birimId}");
                var stokItems = _urunStokRepository.GetirList(x => x.UrunId == urun.Id && x.Durumu == 1 && x.BirimlerId == birimId);
                int stokMiktari = stokItems != null ? stokItems.Sum(x => x.Stok) : 0;
                Console.WriteLine($"Stok miktarı: {stokMiktari}");

                if (intMiktar > stokMiktari && stokMiktari > 0)
                {
                    Console.WriteLine($"Hata: Stok yetersiz, talep edilen: {intMiktar}, mevcut: {stokMiktari}");
                    return Json(new { success = false, message = $"Stok yetersiz! Mevcut stok: {stokMiktari}" });
                }

                // Session kontrolü
                Console.WriteLine("Session'dan sepet alınıyor...");
                var list = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                Console.WriteLine($"Mevcut sepet öğe sayısı: {list.Count}");

                var uyeler = HttpContext.Session.GetObjectFromJson<Uyeler>("Uyeler");
                Console.WriteLine($"Üye: {(uyeler != null ? uyeler.Id.ToString() : "Yok")}");

                // Birim bilgisi
                Console.WriteLine($"Birim sorgulanıyor: BirimId={birimId}");
                var birimler = _birimlerRepository.Getir(birimId);
                if (birimler == null)
                {
                    Console.WriteLine("Hata: Birim bulunamadı.");
                    return Json(new { success = false, message = "Geçersiz birim!" });
                }
                Console.WriteLine($"Birim bulundu: {birimler.Adi}");

                decimal? fiyat = model.IndirimliFiyat ?? urun.Fiyat;
                decimal? fiyatVirgul = fiyat / 100;

                // Sepette aynı ürün kontrolü
                Console.WriteLine("Sepette aynı ürün kontrol ediliyor...");
                var urunStok = list.FirstOrDefault(s => s.UrunId == model.Id && s.BirimlerId == birimId);
                if (urunStok != null)
                {
                    urunStok.Miktar += intMiktar;
                    Console.WriteLine($"Mevcut ürünün miktarı güncellendi. Yeni miktar: {urunStok.Miktar}");
                }
                else
                {
                    var sepetItem = new Sepet(urun, intMiktar, model.Birim ?? "Adet", birimId, uyeler, birimler)
                    {
                        Fiyat = fiyatVirgul
                    };
                    list.Add(sepetItem);
                    Console.WriteLine("Yeni ürün sepete eklendi");
                    Console.WriteLine($"Yeni sepet öğesi: UrunId={sepetItem.UrunId}, BirimId={sepetItem.BirimlerId}");
                }

                // Session'a kaydet
                try
                {
                    Console.WriteLine("Sepet Session'a kaydediliyor...");
                    HttpContext.Session.SetObjectAsJson("Sepet", list);
                    Console.WriteLine("Sepet Session'a başarıyla kaydedildi");
                    ViewBag.Sepet = list;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Session kayıt hatası: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return Json(new { success = false, message = $"Session kaydedilemedi: {ex.Message}" });
                }

                Console.WriteLine("İşlem başarılı, yanıt dönülüyor.");
                return Json(new { success = true, message = "Ürün sepete eklendi!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KRİTİK HATA: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"İç hata: {ex.InnerException.Message}");
                }
                return Json(new { success = false, message = $"Sepete ekleme sırasında bir hata oluştu: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Decrease(int id)
        {
            try
            {
                var list = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                var item = list.FirstOrDefault(x => x.UrunId == id);
                if (item == null)
                {
                    return RedirectToAction("Index");
                }

                if (item.Miktar > 1)
                {
                    item.Miktar -= 1;
                }
                else
                {
                    list.RemoveAll(x => x.UrunId == id);
                }

                HttpContext.Session.SetObjectAsJson("Sepet", list);
                TempData["Mesaj"] = "Ürün miktarı azaltıldı.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                TempData["Mesaj"] = "Bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                var list = HttpContext.Session.GetObjectFromJsonCollection<Sepet>("Sepet") ?? new List<Sepet>();
                list.RemoveAll(x => x.UrunId == id);

                HttpContext.Session.SetObjectAsJson("Sepet", list);
                TempData["Mesaj"] = "Ürün sepetten kaldırıldı.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                TempData["Mesaj"] = "Bir hata oluştu.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            try
            {
                HttpContext.Session.Remove("Sepet");
                TempData["Mesaj"] = "Sepet temizlendi.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
                TempData["Mesaj"] = "Bir hata oluştu.";
                return RedirectToAction("Index");
            }
        }
    }

    public class AddToCartDto
    {
        public int Id { get; set; }
        public string? UrunAdi { get; set; }
        public decimal? IndirimliFiyat { get; set; }
        public string? Birim { get; set; }
        public int? UyeId { get; set; }
        public string? BirimId { get; set; }
        public string? Miktar { get; set; }
    }
}