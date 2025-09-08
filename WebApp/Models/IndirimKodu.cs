using Microsoft.CodeAnalysis.Options;

namespace WebApp.Models
{
    public class IndirimKodu
    {
        public int Id { get; set; }
        public string? Kodu { get; set; }
        public decimal? AltLimit { get; set; }   
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public int Durumu { get; set; }
        public int Orani { get; set; }
    }
}
