namespace WebApp.Models
{
    public class Sepet
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public string? UrunAdi { get; set; }
        public decimal? Fiyat { get; set; }
        public int Miktar { get; set; }
        public decimal? Toplam { get; set; }
        public string? Birim { get; set; }
        public int BirimlerId { get; set; }
        public Birimler? Birimler { get; set; }
        public string? UrunResmi { get; set; }
        public string? KolBoyu { get; set; }
        public string? ImagePath { get; set; }
        public string? EkTasarim { get; set; }
        public string? EkTasarimMetni { get; set; }
        public int? UyeId { get; set; }
        public Uyeler? Uye { get; set; }
        public Siparis? Siparis { get; set; }
        public int? SiparisId { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int Durumu { get; set; }

        // Constructor used in Add action
        public Sepet(Urun urun, int miktar, string birim, int bedenId, Uyeler uyeler, Birimler bedenler)
        {
            UrunId = urun.Id;
            UrunAdi = urun.Adi;
            Fiyat = urun.Fiyat;
            Miktar = miktar;
            Toplam = miktar * urun.Fiyat;
            Birim = birim;
            BirimlerId = bedenId;
            Birimler = bedenler;
            UrunResmi = urun.UrunFotograf?.FirstOrDefault()?.FotografBuyuk;
            UyeId = uyeler?.Id;
            Uye = uyeler;
            EklenmeTarihi = DateTime.Now;
            GuncellenmeTarihi = DateTime.Now;
            Durumu = 1;
        }

        public Sepet() { }
    }
}