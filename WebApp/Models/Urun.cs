using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Urun
{
    public int Id { get; set; }

    public decimal? Fiyat { get; set; }
    public int? IndirimYuzdesi { get; set; }

    public string? Adi { get; set; }
    public string? Kodu { get; set; }

    public string? Aciklama { get; set; }
    public string? Ozellikler { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public DateTime? GuncellenmeTarihi { get; set; }
    public int RenkId { get; set; }
    public int? Durumu { get; set; }
    public int? UrunAnaKategoriId { get; set; }
    public int? UrunAltKategoriId { get; set; }

    //public virtual ICollection<UrunFotograf> UrunFotografs { get; set; } = new List<UrunFotograf>();
    public virtual UrunAnaKategori? UrunAnaKategori { get; set; }
    public virtual UrunAltKategori? UrunAltKategori { get; set; }
	public List<UrunFotograf>? UrunFotograf { get; set; }
	public List<Video>? Video { get; set; }

	public virtual UrunGaleri? UrunGaleri { get; set; }
    public virtual UrunBirim? UrunBirim { get; set; }
    public virtual Renkler? Renk { get; set; }
    public virtual UrunStok? UrunStok { get; set; }


}
