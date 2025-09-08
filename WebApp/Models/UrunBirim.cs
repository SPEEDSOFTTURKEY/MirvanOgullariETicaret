using System;
using System.Collections.Generic; // HashSet için gerekli namespace

namespace WebApp.Models
{
    public class UrunBirim
    {


        public int Id { get; set; }
        public int UrunId { get; set; }
        public int BirimlerId { get; set; }
        public int Durumu { get; set; }
        public DateTime EklenmeTarihi { get; set; }
        public DateTime GuncellenmeTarihi { get; set; }

        public virtual Urun? Urun { get; set; }
		public Birimler? Birimler { get; set; } // Birden fazla birim



    }
}
