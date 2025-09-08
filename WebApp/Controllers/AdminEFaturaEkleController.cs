using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.Invoice;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminEFaturaEkleController : AdminBaseController
    {
        private readonly InvoiceService invoiceService;
        private EDMBilgileriRepository edmBilgileriRepository = new EDMBilgileriRepository();
        private InvoiceLineDtoRepository invoiceLineDtoRepository = new InvoiceLineDtoRepository();
        private InvoiceRequestDtoRepository invoiceRequestDtoRepository = new InvoiceRequestDtoRepository();

        private FirmaRepository firmaRepository = new FirmaRepository();
        private SiparisRepository siparisRepository = new SiparisRepository();

        SepetRepository sepetRepository = new SepetRepository();

        public AdminEFaturaEkleController(IHostEnvironment hostEnvironment)
        {
            invoiceService = new InvoiceService(hostEnvironment, edmBilgileriRepository, invoiceLineDtoRepository, invoiceRequestDtoRepository);
        }

        [HttpGet]
        public IActionResult Index(int SiparisId)
        {
            List<string> join = new List<string>();
            join.Add("Sepets");

            Siparis SiparisListesi = siparisRepository.Getir(x => x.Durumu == 1 && x.Id==SiparisId, join);
           
            ViewBag.SiparisListesi = SiparisListesi;

            List<Sepet> sepets = new List<Sepet>();
            SepetRepository sepetRepository = new SepetRepository();
            List<string> join2 = new List<string>();
            join2.Add("Siparis");
            join2.Add("Uye");
            join2.Add("Birim");
            sepets=sepetRepository.GetirList( x=> x.Durumu == 1 && x.SiparisId==SiparisId,join2);
            ViewBag.Sepet=sepets;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> FaturaKes( int SiparisId, string SeriNo )
        {
            int Kdv = 10;
        List<Sepet> sepets = new List<Sepet>();
            sepets=sepetRepository.GetirList(x=> x.SiparisId== SiparisId);
            decimal ToplamTutar = sepets.Sum(x => x.Fiyat ?? 0);
            List<string> join = new List<string>();
            join.Add("Sepets");
            Siparis siparis = siparisRepository.Getir(x => x.Durumu == 1 && x.Id == SiparisId, join);

            // Konaklama kalemi için hesaplamalar

            decimal sepetToplamUcret = ToplamTutar;
            // Net Tutarı, vergiler hariç hale getirmek için:
            decimal sepetNetTutar = Math.Round(sepetToplamUcret / (1 + (Kdv) / 100), 2);

            // KDV ve Konaklama Vergisi tutarlarını net tutar üzerinden hesaplıyoruz:
            decimal konaklamaKdvTutar = Math.Round(sepetNetTutar * Kdv / 100m, 2);

            List<InvoiceLineDto> InvoiceLines = new List<InvoiceLineDto>();

            
            // Adisyon kalemleri için hesaplamalar
            List<string> adisyonJoin = new List<string>();
            adisyonJoin.Add("Siparis");


            foreach (var adisyon in sepets)
            {
             
                decimal adisyonUcret = adisyon.Fiyat ?? 0;
                // adisyonNetTutar, vergiler hariç hale getirmek için:
                decimal adisyonNetTutar = Math.Round(adisyonUcret / (1 + (Kdv / 100m)), 2);

                // adisyonKdvTutar tutarlarını net tutar üzerinden hesaplıyoruz:
                decimal adisyonKdvTutar = Math.Round(adisyonNetTutar * Kdv / 100m, 2);               

                InvoiceLineDto adisyonInvoiceLine = new InvoiceLineDto
                {
                    Hizmet = adisyon.UrunAdi,
                    Miktar = 1,
                    Birim = "Adet",
                    BirimFiyat = Math.Round(adisyonNetTutar, 2),
                    IndirimOrani = 0,
                    IndirimTutar = 0,
                    NetTutar = adisyonNetTutar,
                    KDVOrani = Kdv,
                    KDVTutar = adisyonKdvTutar,                 
                    Tutar = adisyonUcret                 
                };
                InvoiceLines.Add(adisyonInvoiceLine);
            }

            decimal faturaTotalNet = InvoiceLines.Sum(l => l.NetTutar);
            decimal faturaTotalKdv = InvoiceLines.Sum(l => l.KDVTutar);
           // decimal faturaTotalKonaklama = InvoiceLines.Sum(l => l.KonaklamaVergiTutari ?? 0);
            decimal faturaTotalAmount = faturaTotalNet + faturaTotalKdv;
          //  decimal faturaTotalKDVOrani = InvoiceLines.Sum(l => l.KDVOrani);

            var request = new InvoiceRequestDto
            {
                //Fatura kestiğim kişinin bilgileri firma==müşteri
                MusteriVergiNo = siparis.MusteriVergiNumarasi.ToString() ??"11111111111",
                FaturaTuru = "E-FATURA",
                SeriNo = SeriNo.ToUpper(),
                FaturaTarih = DateTime.Now.ToString("yyyy-MM-dd"),
                FaturaNumarasi = "",// InvoiceService.CreateInvoice methodunda ayarlanıyor.
                FaturaTipi = "SATIS",
                FaturaSenaryosu = "EARSIVFATURA",
                ParaBirimi = "TRY",
                DovizKuru = "1",
                SiparisId = siparis.Id,
                Unvan = siparis.MusteriAdiSoyadi,
                SonBakiye = faturaTotalAmount,
                PostaKodu = siparis.MusteriEMail,
                VergiDairesi = siparis.MusteriVergiDairesi,
                Ulke = "Turkiye",
                Sehir = siparis.MusteriVergiDairesi ?? "",
                Ilce = siparis.MusteriVergiDairesi ?? "",
                Adres = siparis.MusteriAdres ?? "",
                Email = siparis.MusteriEMail ?? "",
                Telefon = siparis.MusteriTelefon ?? "",
                SonBakiyeFaturaDurumu = "1",
                OdemeTarihi = DateTime.Now.ToString("dd.MM.yyyy"),
                OdemeTuru = "NAKIT",
                OdemeKanali = "Banka",
                OdemeHesapNo = "124578965",
                OdemeIBAN = "2457965",
                Notlar = "",
                YaziylaTutar = NumberToWordsConverter.ToWords(faturaTotalAmount),
                Toplam = faturaTotalAmount,
                Artirim = 0,
                Iskonto = 0,
                Stopaj = 0,
                Kdv = Kdv,
                OdenecekTutar = faturaTotalAmount,
                TevkifatToplam = 0,
                DigerVergilerToplam = 0,
            };

            var resultDto = await invoiceService.GenerateInvoice(request, InvoiceLines);

            TempData["Message"] = resultDto.ResultMessage;

            // Fatno'yu Index metoduna gönderiyoruz
            return RedirectToAction("Index", "AdminEFatura", new { invoiceNumber = resultDto.Fatno });


        }

    }
}
