namespace WebApp.Models
{
    public class Siparis
    {
       public int Id { get; set; }
       public int KargoAdresiId { get; set; }
       public int KargoId { get; set; }
       public decimal ToplamTutar { get; set; }
       public decimal IndirimMiktari { get; set; }
       public decimal KargoUcreti { get; set; }
       public decimal OdenecekTutar { get; set; }
        public string? MusteriAdiSoyadi {  get; set; }
        public string? MusteriAdres{ get; set; }
        public string? OdemeTipi { get; set; }
        public int DurumId { get; set; } // SiparişDurumlari tablosuna FK

        public string? MusteriFaturaAdres { get; set; } 
        public string? MusteriTelefon {  get; set; }
        public string? MusteriVergiNumarasi {  get; set; }
        public string? SiparisNotu {  get; set; }
        public string? SiparisKodu {  get; set; }
        public string? MusteriVergiDairesi { get; set; }
        public int? Durumu { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int UyelerId {  get; set; }
        public int? IndirimId {  get; set; }    
        public string? MusteriEMail {  get; set; }
        public virtual Uyeler? Uyeler { get; set; }
		public ICollection<Sepet>? Sepets { get; set; }
        public SiparisDurumlari? Durum { get; set; }

    }
}
