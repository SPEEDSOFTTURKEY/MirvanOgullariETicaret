using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging; // Görüntü formatları için eklendi
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminAnaSayfaBannerResimEkleController : AdminBaseController
    {
        AnaSayfaBannerResimRepository repository = new AnaSayfaBannerResimRepository();
        [Obsolete]
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _HostEnvironment;

        [Obsolete]
        public AdminAnaSayfaBannerResimEkleController(Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
        {
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Obsolete]
        public IActionResult Kaydet(List<IFormFile> imagefile)
        {

            if (imagefile != null && imagefile.Count != 0)
            {
                string serverpath = _HostEnvironment.ContentRootPath;
                AnaSayfaBannerResim fotograf = new AnaSayfaBannerResim();
                foreach (IFormFile item in imagefile)
                {
                    fotograf = new AnaSayfaBannerResim();
                    string extension = Path.GetExtension(item.FileName);
                    string newimagename = Guid.NewGuid() + extension;
                    string buyukFolderPath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "AnaSayfa", "Buyuk");
                    string kucukFolderPath = Path.Combine(serverpath, "wwwroot", "WebAdminTheme", "AnaSayfa", "Kucuk");

                    if (!Directory.Exists(buyukFolderPath))
                    {
                        Directory.CreateDirectory(buyukFolderPath);
                    }
                    if (!Directory.Exists(kucukFolderPath))
                    {
                        Directory.CreateDirectory(kucukFolderPath);
                    }

                    try
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            item.CopyTo(memoryStream);
                            memoryStream.Position = 0; // Akış konumunu başa sıfırla

                            using (Bitmap orjinal = new Bitmap(memoryStream))
                            {
                                using (Bitmap buyuk = new Bitmap(orjinal, new Size(1844, 1037)))
                                {
                                    buyuk.Save(Path.Combine(buyukFolderPath, newimagename), GetImageFormat(extension));
                                }
                                using (Bitmap kucuk = new Bitmap(orjinal, new Size(1429, 864)))
                                {
                                    kucuk.Save(Path.Combine(kucukFolderPath, newimagename), GetImageFormat(extension));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Hata günlüğü tutulabilir (örneğin, bir logger kullanarak)
                        ModelState.AddModelError("", "Resim işlenirken bir hata oluştu: " + ex.Message);
                        return View("Index"); // Veya belirli bir hata görünümü döndürülebilir
                    }

                    fotograf.Durumu = 1;
                    fotograf.EklenmeTarihi = DateTime.Now;
                    fotograf.GuncellenmeTarihi = DateTime.Now;
                    fotograf.FotografBuyuk = "/WebAdminTheme/AnaSayfa/Buyuk/" + newimagename;
                    fotograf.FotografKucuk = "/WebAdminTheme/AnaSayfa/Kucuk/" + newimagename;
                    repository.Ekle(fotograf);
                }
            }
            return RedirectToAction("Index", "AdminAnaSayfaBannerResim");
        }

        // Dosya uzantısından ImageFormat almak için yardımcı metot
        private ImageFormat GetImageFormat(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return ImageFormat.Jpeg;
                case ".png":
                    return ImageFormat.Png;
                case ".gif":
                    return ImageFormat.Gif;
                case ".bmp":
                    return ImageFormat.Bmp;
                case ".tiff":
                    return ImageFormat.Tiff;
                default:
                    return ImageFormat.Jpeg; // Bilinmeyen uzantılar için varsayılan olarak JPEG
            }
        }
    }
}
