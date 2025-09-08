namespace WebApp.Models
{
    public class MisafirSiparisKargoTakip
    {
        public int Id { get; set; }
        public string? KargoTakipNumarasi { get; set; }
        public string? KargoFirmasi { get; set; }
        public int SiparisGuestId { get; set; }
        public string? Aciklama { get; set; }
        public DateTime? EklenmeTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int? Durumu { get; set; }

        public virtual SiparisGuest? SiparisGuest { get; set; }





    }
}
