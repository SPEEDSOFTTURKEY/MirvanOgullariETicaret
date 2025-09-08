namespace WebApp.Models
{
    public class Bayilik
    {
        public int Id { get; set; }
        public string? Metin { get; set; }

        public int? Durumu { get; set; }

        public DateTime? EklenmeTarihi { get; set; }

        public DateTime? GuncellenmeTarihi { get; set; }

        public string? Baslik { get; set; }
        public string? AltBaslik { get; set; }
    }
}
