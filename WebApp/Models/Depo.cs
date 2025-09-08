namespace WebApp.Models
{
    public class Depo
    {
        public int Id { get; set; }

        public int UrunId { get; set; }
        public int UrunAltKategoriId { get; set; }
        public int UrunAnaKategoriId { get; set; }
        public int Stok { get; set; }
        public int Durumu { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public Urun Urun { get; set; }
        public UrunAltKategori UrunAltKategori { get; set; }
        public UrunAnaKategori UrunAnaKategori { get; set; }
    }
}
