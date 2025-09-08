using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using WebApp.Models;
using WebApp.Repositories;
using System.Globalization;

namespace WebApp.Controllers
{
    public class AdminUrunBilgileriGuncelleController : AdminBaseController
    {
        private readonly UrunAltKategoriRepository _urunAltKategoriRepository = new UrunAltKategoriRepository();
        private readonly UrunBirimRepository _urunBirimRepository = new UrunBirimRepository();
        private readonly UrunStokRepository _urunStokRepository = new UrunStokRepository();
        private readonly UrunRepository _urunRepository = new UrunRepository();
        private readonly UrunAnaKategoriRepository _urunAnaKategoriRepository = new UrunAnaKategoriRepository();
        private readonly RenklerRepository _renklerRepository = new RenklerRepository();
        private readonly BirimlerRepository _birimlerRepository = new BirimlerRepository();

        public IActionResult Index()
        {
            var guncellemeBilgisi = HttpContext.Session.GetObjectFromJson<Urun>("Urun");
            if (guncellemeBilgisi == null)
            {
                return RedirectToAction("Index", "AdminUrunBilgileri");
            }

            // Ensure Fiyat and IndirimYuzdesi are initialized
            if (guncellemeBilgisi.Fiyat == null)
            {
                guncellemeBilgisi.Fiyat = 0m; // Default to 0 if null
            }
            if (guncellemeBilgisi.IndirimYuzdesi == null)
            {
                guncellemeBilgisi.IndirimYuzdesi = 0; // Default to 0 if null
            }

            var urunStok = _urunStokRepository.Getir(x => x.UrunId == guncellemeBilgisi.Id && x.Durumu == 1);
            guncellemeBilgisi.UrunStok = urunStok;

            ViewBag.UrunTipListesi = _urunAltKategoriRepository.GetirList(x => x.Durumu == 1 && x.UrunAnaKategoriId == guncellemeBilgisi.UrunAnaKategoriId).ToList();
            ViewBag.Urun = guncellemeBilgisi;
            var join = new List<string> { "Birimler", "Urun" };
            ViewBag.BirimListesi = _urunBirimRepository.GetirList(x => x.UrunId == guncellemeBilgisi.Id && x.Durumu == 1 && x.Urun != null, join).ToList();
            ViewBag.YeniBirimListesi = _birimlerRepository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.UrunAnaKategoriListesi = _urunAnaKategoriRepository.GetirList(x => x.Durumu == 1).ToList();
            ViewBag.RenkListesi = _renklerRepository.GetirList(x => x.Durumu == 1).ToList();

            return View();
        }

        public JsonResult GetirAltKategori(int AnaKategoriId)
        {
            var urunAltKategori = _urunAltKategoriRepository.GetirList(x => x.UrunAnaKategoriId == AnaKategoriId && x.Durumu == 1).ToList();
            return Json(urunAltKategori);
        }

        public JsonResult GetirUrun(int AltKategoriId)
        {
            var urun = _urunRepository.GetirList(x => x.UrunAltKategoriId == AltKategoriId && x.Durumu == 1).ToList();
            return Json(urun);
        }

        public JsonResult GetirRenk(int UrunId)
        {
            var urun = _urunRepository.Getir(x => x.Id == UrunId);
            if (urun != null)
            {
                var renk = _renklerRepository.GetirList(x => x.Durumu == 1 && x.Id == urun.RenkId).ToList();
                return Json(renk);
            }
            return Json(null);
        }

        [HttpPost]
        public JsonResult Kaydet(Urun urun, int[] birimIds, int stok)
        {
            try
            {
                // Validate input
                if (urun == null || urun.Id <= 0)
                {
                    return Json(new { success = false, message = "Geçersiz ürün bilgisi." });
                }
                if (urun.Fiyat <= 0)
                {
                    return Json(new { success = false, message = "Fiyat sıfırdan büyük olmalıdır." });
                }
                if (urun.IndirimYuzdesi < 0)
                {
                    return Json(new { success = false, message = "İndirim yüzdesi negatif olamaz." });
                }
                if (string.IsNullOrWhiteSpace(urun.Adi))
                {
                    return Json(new { success = false, message = "Ürün adı boş olamaz." });
                }
                if (birimIds == null || birimIds.Length == 0)
                {
                    return Json(new { success = false, message = "En az bir birim seçilmelidir." });
                }

                var existingEntity = _urunRepository.Getir(x => x.Id == urun.Id);
                if (existingEntity == null)
                {
                    return Json(new { success = false, message = "Ürün bulunamadı." });
                }

            
                decimal parsedFiyat;
                string fiyatStr = urun.Fiyat.ToString();

                // Eğer sayı 100x büyük geldiyse (örneğin 49999), düzelt
                if (urun.Fiyat > 1000 && fiyatStr.All(char.IsDigit)) // Basit bir kontrol
                {
                    // 49999 => 499,99 gibi düzelt
                    fiyatStr = fiyatStr.Insert(fiyatStr.Length - 2, ",");
                }

                // Türkçe kültürde parse etmeye çalış
                if (!decimal.TryParse(fiyatStr, NumberStyles.Number, new CultureInfo("tr-TR"), out parsedFiyat))
                {
                    return Json(new { success = false, message = "Geçersiz fiyat formatı." });
                }


                // Update entity fields
                existingEntity.Adi = urun.Adi;
                existingEntity.Fiyat = parsedFiyat; // Use parsed value
                existingEntity.IndirimYuzdesi = urun.IndirimYuzdesi;
                existingEntity.Aciklama = urun.Aciklama;
                existingEntity.Ozellikler = urun.Ozellikler;
                existingEntity.RenkId = urun.RenkId;
                existingEntity.UrunAnaKategoriId = urun.UrunAnaKategoriId;
                existingEntity.UrunAltKategoriId = urun.UrunAltKategoriId;
                existingEntity.Durumu = urun.Durumu;
                existingEntity.GuncellenmeTarihi = DateTime.Now;
                existingEntity.EklenmeTarihi = urun.EklenmeTarihi;

                // Update the product in the repository
                _urunRepository.Guncelle(existingEntity);

                // Fetch related entities for session update
                var anaKategori = _urunAnaKategoriRepository.Getir(x => x.Id == urun.UrunAnaKategoriId);
                var altKategori = _urunAltKategoriRepository.Getir(x => x.Id == urun.UrunAltKategoriId);
                var renk = _renklerRepository.Getir(x => x.Id == urun.RenkId);

                existingEntity.UrunAnaKategori = anaKategori;
                existingEntity.UrunAltKategori = altKategori;
                existingEntity.Renk = renk;

                // Update session
                HttpContext.Session.SetObjectAsJson("Urun", existingEntity);

                // Handle stock if provided
                if (stok > 0)
                {
                    var urunStokListesi = _urunStokRepository.GetirList(x => x.UrunId == urun.Id).ToList();
                    foreach (var item in urunStokListesi)
                    {
                        _urunStokRepository.Sil(item);
                    }
                    var newStock = new UrunStok
                    {
                        UrunId = urun.Id,
                        Stok = stok,
                        Durumu = 1,
                        EklenmeTarihi = DateTime.Now,
                        GuncellenmeTarihi = DateTime.Now
                    };
                    _urunStokRepository.Ekle(newStock);
                }

                // Handle birimler (sizes)
                birimIds = birimIds?.Distinct().ToArray() ?? new int[0];
                var urunBirimler = _urunBirimRepository.GetirList(x => x.UrunId == urun.Id).ToList();

                // Remove unselected birimler
                var currentBirimIds = urunBirimler.Select(x => x.BirimlerId).ToList();
                var removeBirimIds = currentBirimIds.Except(birimIds).ToList();
                foreach (var removeBirimId in removeBirimIds)
                {
                    var birimToRemove = urunBirimler.FirstOrDefault(x => x.BirimlerId == removeBirimId);
                    if (birimToRemove != null)
                    {
                        _urunBirimRepository.Sil(birimToRemove);
                    }
                }

                // Add new birimler
                foreach (var birimId in birimIds)
                {
                    if (!currentBirimIds.Contains(birimId))
                    {
                        var urunBirim = new UrunBirim
                        {
                            Durumu = 1,
                            UrunId = urun.Id,
                            BirimlerId = birimId,
                            EklenmeTarihi = DateTime.Now,
                            GuncellenmeTarihi = DateTime.Now
                        };
                        _urunBirimRepository.Ekle(urunBirim);
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Kayıt sırasında bir hata oluştu: " + ex.Message });
            }
        }
    }
}
