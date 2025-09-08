using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Video
{
    public int Id { get; set; }
    public int UrunId { get; set; }
    public int RenkId { get; set; }

    public string? DosyaYolu { get; set; }

    public int? Durumu { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public DateTime? GuncellenmeTarihi { get; set; }
    public Urun Urun { get; set; }
    public Renkler Renk { get; set; }
}
