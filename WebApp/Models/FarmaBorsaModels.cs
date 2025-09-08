namespace WebApp.Models
{
    public class FarmaBorsaProductRequest
    {
        public string urunAd { get; set; }
        public int adet { get; set; }
        public int maxAdet { get; set; }
        public int op { get; set; }
        public double tutar { get; set; }
        public bool borsadaGoster { get; set; }
        public string miad { get; set; }
        public int miadTip { get; set; }
        public string barkod { get; set; }
        public string resimUrl { get; set; }
        public int? kod { get; set; }
    }

    public class FarmaBorsaProductResponse
    {
        public bool hata { get; set; }
        public string mesaj { get; set; }
    }

    public class InputModel
    {
        public string nick { get; set; }
        public string parola { get; set; }
        public string apiKey { get; set; }
        public int pasif { get; set; }
        public int ilan { get; set; }
        public bool tumIlanlarGelsin { get; set; }
    }

    public class Ilan
    {
        public int sira { get; set; }
        public int kod { get; set; }
        public string urunAd { get; set; }
        public int adet { get; set; }
        public int maxAdet { get; set; }
        public int op { get; set; }
        public double tutar { get; set; }
        public bool borsadaGoster { get; set; }
        public int urun { get; set; }
        public string miad { get; set; }
        public string miadText { get; set; }
        public int miadTip { get; set; }
        public string miadTipAd { get; set; }
        public string barkod { get; set; }
        public object psf { get; set; }
        public string tutarAd { get; set; }
        public string resimUrl { get; set; }
    }

    public class UrunTalepOutput
    {
        public bool hata { get; set; }
        public string mesaj { get; set; }
        public List<Ilan> ilanlarimList { get; set; }
    }
}