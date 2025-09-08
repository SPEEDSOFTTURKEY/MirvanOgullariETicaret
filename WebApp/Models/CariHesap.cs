namespace WebApp.Models
{
    public class CariHesap
    {
        public int Id { get; set; }
        public int UrunId { get; set; }
        public int UrunAltKategoriId { get; set; }
        public int UrunAnaKategoriId { get; set; }
        public int Adet { get; set; }
        public decimal? ToplamFiyat { get; set; }
        public decimal? Fiyat { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int Durumu { get; set; }
        public Urun Urun {  get; set; }
        public UrunAltKategori UrunAltKategori { get; set; }
        public UrunAnaKategori UrunAnaKategori { get; set; }
    }
}
