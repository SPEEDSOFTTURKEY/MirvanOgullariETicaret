using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Repositories;
using System.Text;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Xml.Linq;

namespace WebApp.Controllers
{
    public class AdminPTTController : Controller
    {
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new PazarYeriGirisBilgileriRepository();

        public async Task<IActionResult> Index(int pazarYeriId)
        {
            pazarYeriId = 4; // PttAVM için sabit
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            var products = new List<object>();
            var categories = new List<object>();

            if (girisBilgisi != null)
            {
                products = await GetPTTProducts(girisBilgisi);
                categories = await GetPTTCategories(girisBilgisi); // Kategori bilgilerini al
            }
            ViewBag.Products = products;
            ViewBag.Categories = categories; // Kategorileri ViewBag'e ekle
            ViewBag.PazarYeriId = pazarYeriId;
            ViewBag.ApiType = "ptt";
            return View();
        }
        private async Task<List<object>> GetPTTCategories(PazarYeriGirisBilgileri girisBilgisi)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };

            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;
            await client.OpenAsync();

            try
            {
                var result = await client.GetMainCategoriesAsync();
                var categoryList = new List<object>();

                if (result.success)
                {
                    foreach (var category in result.main_category)
                    {
                        categoryList.Add(new
                        {
                            Id = category.id,
                            Name = category.name
                        });
                    }
                }

                return categoryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Kategori alınırken hata: " + ex.Message);
                return new List<object>();
            }
            finally
            {
                try
                {
                    if (client.State != CommunicationState.Faulted)
                        await client.CloseAsync();
                    else
                        client.Abort();
                }
                catch
                {
                    client.Abort();
                }
            }
        }
        private async Task<List<object>> GetPTTProducts(PazarYeriGirisBilgileri girisBilgisi)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };

            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;
            await client.OpenAsync();
            try
            {
                int page = 1;
                int size = 20;

                var result = await client.GetProductsWithVariantsAsync(page, size);
                var urunListesi = new List<PTTUrun>();

                foreach (var urun in result.ProductsWithVariants)
                {
                    var yeniUrun = new PTTUrun
                    {
                        Name = urun.ProductBarcode,
                        Barcode = urun.ProductBarcode,
                        Price = Convert.ToDecimal(urun.Variants.FirstOrDefault()?.PriceDifference),
                        CategoryId = 0,
                        Desi = 0,
                        Quantity =  0,
                        VATRate = 0,
                        Variants = new List<PTTVaryant>()
                    };

                    if (urun.Variants != null)
                    {
                        foreach (var varyant in urun.Variants)
                        {
                            yeniUrun.Variants.Add(new PTTVaryant
                            {
                                Name = varyant.VariantName,
                                Barcode = varyant.VariantId.ToString(),
                                Price = (decimal)varyant.PriceDifference,
                                Quantity = varyant.Quantity
                            });
                        }
                    }

                    urunListesi.Add(yeniUrun);
                }

                return urunListesi.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata oluştu: " + ex.Message);
                client.Abort();
                return new List<PTTUrun>().Cast<object>().ToList();
            }
            finally
            {
                try
                {
                    if (client.State != CommunicationState.Faulted)
                        await client.CloseAsync();
                    else
                        client.Abort();
                }
                catch
                {
                    client.Abort();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateProduct([FromBody] PTTUrun product, int pazarYeriId)
        {
            pazarYeriId = 4;
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
            {
                return Json(new { Success = false, Message = "Giriş bilgileri bulunamadı." });
            }

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };

            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;
            await client.OpenAsync();

            try
            {
                var productRequest = new PTTServiceReference.ProductV3Request
                {
                    Active = product.Active ?? true,
                    Barcode = product.Barcode,
                    BasketMaxQuantity = product.BasketMaxQuantity ?? 1000,
                    CargoProfileId = product.CargoProfileId ?? 0,
                    CategoryId = product.CategoryId,
                    Desi = Convert.ToInt32(product.Desi),
                    Discount = product.Discount ?? 0,
                    EstimatedCourierDelivery = product.EstimatedCourierDelivery ?? 5,
                    Gtin = product.Gtin,
                    Images = new PTTServiceReference.ProductImageV3[] { new PTTServiceReference.ProductImageV3 { Url = product.ImageUrl } },
                    LongDescription = product.LongDescription,
                    Name = product.Name,
                    NoShippingProduct = product.NoShippingProduct ?? false,
                    PriceWithVat = product.PriceWithVat,
                    PriceWithoutVat = product.Price,
                    ProductCode = product.Barcode,
                    Quantity = product.Quantity,
                    ShortDescription = product.ShortDescription,
                    SingleBox = product.SingleBox ?? false,
                    VATRate = product.VATRate,
                    WarrantyDuration = product.WarrantyDuration ?? 0,
                    WarrantySupplier = product.WarrantySupplier,
                    Variants = product.Variants?.Select(v => new PTTServiceReference.VariantV3
                    {
                        VariantBarcode = v.Barcode,
                        Price = v.Price,
                        Quantity = v.Quantity,
                        Attributes = v.Attributes?.Select(a => new PTTServiceReference.VariantAttrV3
                        {
                            Definition = a.Definition,
                            Value = a.Value
                        }).ToArray()
                    }).ToArray()
                };

                var response = await client.UpdateProductsV3Async(new PTTServiceReference.ProductV3Request[] { productRequest });

                if (response.Success)
                {
                    return Json(new
                    {
                        Success = true,
                        Message = response.Message,
                        TrackingId = response.TrackingId
                    });
                }
                else
                {
                    return Json(new { Success = false, Message = response.Message });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Hata oluştu: " + ex.Message });
            }
            finally
            {
                try
                {
                    if (client.State != CommunicationState.Faulted)
                        await client.CloseAsync();
                    else
                        client.Abort();
                }
                catch
                {
                    client.Abort();
                }
            }
        }
    }
}