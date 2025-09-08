namespace WebApp.Models
{
    public class UrunAnaKategoriFotograf
    {
        public int Id { get; set; }

        public string? FotografBuyuk { get; set; }

        public string? FotografKucuk { get; set; }

        public int? Durumu { get; set; }

        public int? UrunAnaKategoriId { get; set; }

        public DateTime? EklenmeTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }

        public virtual UrunAnaKategori? UrunAnaKategori { get; set; }
    }
}
