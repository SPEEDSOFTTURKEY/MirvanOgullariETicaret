namespace WebApp.Models
{
    public class UrunAnaKategori
    {
        public int Id { get; set; }
        public string? Adi { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int? Durumu { get; set; }
       public List<UrunAltKategori>? UrunAltKategoriList {  get; set; }
       public List<UrunAnaKategoriFotograf>? UrunAnaKategoriFotograf {  get; set; }
        public virtual List<UrunStok> UrunStoklari { get; set; }

    }
}
