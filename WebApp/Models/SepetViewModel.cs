namespace WebApp.Models
{
    public class SepetViewModel
    {
        public List<Sepet>? SepetListesi { get; set; }
        public decimal GenelToplam { get; set; }
        public decimal IndirimMiktari { get; set; }
        public decimal IndirimliToplam { get; set; }
        public decimal KargoUcreti { get; set; }
        public decimal OdenecekToplam { get; set; }
        public decimal KDVToplam { get; set; }
        public decimal KDVOrani { get; set; } // Completed from "KDVOr"
    }
}