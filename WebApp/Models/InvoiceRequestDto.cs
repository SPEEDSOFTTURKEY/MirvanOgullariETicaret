namespace WebApp.Models
{
    public class InvoiceRequestDto
    {
        public int Id { get; set; }
        public string MusteriVergiNo { get; set; } // musteriVergiNo
        public string FaturaTuru { get; set; } // faturaTuru
        public string SeriNo { get; set; } // seriNumaralar
        public string FaturaTarih { get; set; } // tarih
        public string FaturaNumarasi { get; set; } // faturaNumarasi
        public string FaturaTipi { get; set; } // faturaSenaryosu
        public string FaturaSenaryosu { get; set; } // paraBirimi
        public string ParaBirimi { get; set; }
        public string DovizKuru { get; set; }
        public int SiparisId { get; set; }
        public string Unvan { get; set; }
        public decimal SonBakiye { get; set; }
        public string PostaKodu { get; set; }
        public string VergiDairesi { get; set; }
        public string Ulke { get; set; }
        public string Sehir { get; set; }
        public string Ilce { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string SonBakiyeFaturaDurumu { get; set; }
        public string OdemeTarihi { get; set; }
        public string OdemeTuru { get; set; }
        public string OdemeKanali { get; set; }
        public string OdemeHesapNo { get; set; }
        public string OdemeIBAN { get; set; }
        public string Notlar { get; set; }
        public string YaziylaTutar { get; set; }
        public decimal Toplam { get; set; }
        public decimal Artirim { get; set; }
        public decimal Iskonto { get; set; }
        public decimal Stopaj { get; set; }
        public decimal Kdv { get; set; }
        public decimal OdenecekTutar { get; set; }
        public decimal TevkifatToplam { get; set; }
        public decimal DigerVergilerToplam { get; set; }
        public List<InvoiceLineDto> InvoiceLines { get; set; } // faturaKalemleri

        public virtual Siparis Siparis { get; set; }
    }
    public class InvoiceLineDto
    {
        public int Id { get; set; }
        public int InvoiceRequestDtoId { get; set; }
        public string Hizmet { get; set; } // aciklama
        public int Miktar { get; set; } // miktar
        public string Birim { get; set; } // birim
        public decimal BirimFiyat { get; set; } // birimFiyat
        public decimal IndirimOrani { get; set; } // indirimOrani
        public decimal IndirimTutar { get; set; } // indirimTutar
        public decimal NetTutar { get; set; } // netTutar
        public decimal KDVOrani { get; set; } // kdv
        public decimal KDVTutar { get; set; } // kdvTutar
       // public decimal? KonaklamaVergiOrani { get; set; } // kdv
        //public decimal? KonaklamaVergiTutari { get; set; } // kdvTutar
        public decimal Tutar { get; set; } // tutar
        public string? KDVsizlikKodu { get; set; }
        public string? TevkifatKodu { get; set; }
        public string? TevkifatOrani { get; set; }
        public decimal? TevkifatTutar { get; set; } // String yerine decimal
    }

}