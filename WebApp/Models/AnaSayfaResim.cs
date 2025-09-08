namespace WebApp.Models
{
    public class AnaSayfaResim
    {
        public int Id { get; set; }
        public string? FotografBuyuk { get; set; }
        public string? FotografKucuk { get; set; }
        public string? Link { get; set; }
        public int? Durumu { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }

    }
}
