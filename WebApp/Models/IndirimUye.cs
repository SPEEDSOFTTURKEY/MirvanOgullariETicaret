namespace WebApp.Models
{
    public class IndirimUye
    {
        public int Id { get; set; }
        public int? IndirimId { get; set; }
        public int? UyeId { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public int Durumu { get; set; }
    }
}
