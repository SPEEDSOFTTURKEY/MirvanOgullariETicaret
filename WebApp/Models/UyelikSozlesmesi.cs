using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class UyelikSozlesmesi
{
    public int Id { get; set; }
    public string?   Metin { get; set; }

    public int? Durumu { get; set; }

    public DateTime? EklenmeTarihi { get; set; }

    public DateTime? GuncellenmeTarihi { get; set; }

    public string? Baslik { get; set; }
    public string? AltBaslik { get; set; }
}
