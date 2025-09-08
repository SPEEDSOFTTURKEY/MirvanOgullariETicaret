namespace WebApp.Models
{
    public class GuestSepet
    {
        public int Id { get; set; }
        public int? UrunId { get; set; }
        public decimal? Fiyat { get; set; }
        public string UrunAdi { get; set; }
        public string Birim { get; set; }
        public int? Miktar { get; set; }
        public string UrunResmi { get; set; }
        public decimal? Toplam { get; set; }
        public int? SiparisGuestId { get; set; }
        public int? Durumu { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public string SiparisKodu { get; set; }
        public int? BirimlerId { get; set; }

        // Navigation property (eğer ihtiyaç varsa)
        public SiparisGuest SiparisGuest { get; set; }
        public Birimler Birimler { get; set; }
    }
}
