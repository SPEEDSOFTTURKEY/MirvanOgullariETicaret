namespace WebApp.Models
{
    public class PazarYeriGirisBilgileri
    {
        public int Id { get; set; }
        public int? PazarYerleriId { get; set; }
        public int? Durumu { get; set; }
        public string? ApiKey { get; set; }
        public string? SecretKey { get; set; }
        public string? KullaniciAdi { get; set; }
        public string? Sifre { get; set; }
        public PazarYerleri PazarYerleri { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }

    }
}
