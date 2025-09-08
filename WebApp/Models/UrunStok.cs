
namespace WebApp.Models
{
    public class UrunStok
    {
        public int Id { get; set; }
        public int UrunAnaKategoriId { get; set; }
        public int UrunAltKategoriId { get; set; }
        public int UrunId { get; set; }
        public int UrunRenkId { get; set; }

        public int BirimlerId { get; set; }
        public int Stok { get; set; }
        public int? RezerveStok { get; set; }
        public DateTime? RezerveBitisZamani { get; set; }
        public int Durumu { get; set; }
        public int StokTuruId { get; set; }
        public string? Barkod {  get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public virtual UrunAnaKategori? UrunAnaKategori { get; set; }
        public virtual UrunAltKategori? UrunAltKategori { get; set; }
        public virtual Urun? Urun { get; set; }
        public virtual Birimler? Birimler { get; set; }
        public StokTuru? StokTuru { get; set; }

    }
}
