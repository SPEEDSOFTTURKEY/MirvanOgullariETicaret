using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

public class DovizServisi
{
    private static readonly HttpClient _httpClient = new HttpClient();
    public enum DovizCinsi
    {
        TL,
        EUR,
        USD
    }

    public static async Task<(decimal, decimal)> GetDovizKurlariAsync()
    {
        try
        {
            string url = "https://www.tcmb.gov.tr/kurlar/today.xml";
            var response = await _httpClient.GetStringAsync(url);

            XDocument xml = XDocument.Parse(response);

            string euroStr = xml.Descendants("Currency")
                .Where(x => (string)x.Attribute("Kod") == DovizCinsi.EUR.ToString())
                .Select(x => x.Element("ForexSelling")?.Value)
                .FirstOrDefault();

            string dolarStr = xml.Descendants("Currency")
                .Where(x => (string)x.Attribute("Kod") == DovizCinsi.USD.ToString())
                .Select(x => x.Element("ForexSelling")?.Value)
                .FirstOrDefault();

            // Eğer değer boş gelirse, hata almamak için varsayılan değer atıyoruz
            decimal euroKur = !string.IsNullOrEmpty(euroStr) ? Convert.ToDecimal(euroStr, System.Globalization.CultureInfo.InvariantCulture) : 1.00m;
            decimal dolarKur = !string.IsNullOrEmpty(dolarStr) ? Convert.ToDecimal(dolarStr, System.Globalization.CultureInfo.InvariantCulture) : 1.00m;

            return (euroKur, dolarKur);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata oluştu: " + ex.Message);
            return (1.00m, 1.00m); // Hata olursa varsayılan değerleri kullan
        }
    }

    public static async Task<decimal> DovizDonusturAsync(decimal tutar, string kaynakDoviz, string hedefDoviz)
    {
        // Eğer kaynak ve hedef döviz aynıysa, direkt döndür
        if (kaynakDoviz == hedefDoviz) return tutar;

        var (euroKur, dolarKur) = await GetDovizKurlariAsync(); // Döviz kurlarını al

        decimal sonucTutar = (kaynakDoviz, hedefDoviz) switch
        {
            // TL'den diğer dövizlere çevirme
            (nameof(DovizCinsi.TL), nameof(DovizCinsi.EUR)) => tutar / euroKur, // TL -> EUR
            (nameof(DovizCinsi.TL), nameof(DovizCinsi.USD)) => tutar / dolarKur, // TL -> USD

            // EUR'dan diğer dövizlere çevirme
            (nameof(DovizCinsi.EUR), nameof(DovizCinsi.TL)) => tutar * euroKur, // EUR -> TL
            (nameof(DovizCinsi.EUR), nameof(DovizCinsi.USD)) => (tutar * euroKur) / dolarKur, // EUR -> USD

            // USD'den diğer dövizlere çevirme
            (nameof(DovizCinsi.USD), nameof(DovizCinsi.TL)) => tutar * dolarKur, // USD -> TL
            (nameof(DovizCinsi.USD), nameof(DovizCinsi.EUR)) => (tutar * dolarKur) / euroKur, // USD -> EUR

            _ => throw new ArgumentException("Geçersiz döviz türleri.") // Geçersiz durum
        };

        return sonucTutar;
    }


}
