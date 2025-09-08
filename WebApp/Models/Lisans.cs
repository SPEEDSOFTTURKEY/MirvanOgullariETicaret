namespace WebApp.Models
{
   //veritabanında yok sadce veri çekmek için

    public partial class Lisans
    {
        public int Id { get; set; }
        public string LisansAnahtari { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public DateTime? OlusturulmaTarihi { get; set; }
        public DateTime? GuncellenmeTarihi { get; set; }
        public int? Durumu { get; set; }
        public int MusteriId { get; set; }
        public int ProgramId { get; set; }
        public DateTime? SunucuTarihi { get; set; }
    }
}
