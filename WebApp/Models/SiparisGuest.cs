namespace WebApp.Models
{
    public class SiparisGuest
    {
        public int Id { get; set; }
        public string? SiparisKodu { get; set; } // Örn: SG20250414-001
        public string? MusteriAdiSoyadi { get; set; }
        public string? MusteriAdres { get; set; }
        public string? MusteriTelefon { get; set; }
        public string? MusteriVergiNumarasi { get; set; }
        public string? MusteriVergiDairesi { get; set; }
        public string? MusteriEmail { get; set; }
        public decimal? ToplamTutar { get; set; }
        public decimal? IndirimMiktari { get; set; }
        public decimal? KargoUcreti { get; set; }
        public decimal? OdenecekTutar { get; set; }
        public string? OdemeTipi { get; set; }

        public int? DurumId { get; set; } // SiparişDurumlari tablosuna FK
        public int? Durumu { get; set; } // SiparişDurumlari tablosuna FK
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellenmeTarihi { get; set; }

        public string IPAdresi { get; set; }
        public string Notlar { get; set; }

        // Navigation properties
        public SiparisDurumlari Durum { get; set; }
        public List<GuestSepet> GuestSepet { get; set; } // Eklenen kısım
    }
}