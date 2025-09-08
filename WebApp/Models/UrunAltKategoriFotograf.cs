namespace WebApp.Models
{
    public class UrunAltKategoriFotograf
    {

        public int Id { get; set; }

        public string? FotografBuyuk { get; set; }

        public string? FotografKucuk { get; set; }

        public int? Durumu { get; set; }

        public int? UrunAltKategoriId { get; set; }

        public DateTime? EklenmeTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }

        public virtual UrunAltKategori? UrunAltKategori { get; set; }




    }
}
