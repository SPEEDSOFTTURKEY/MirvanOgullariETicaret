using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.ServiceModel;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminPTTUrunEkleController : Controller
    {
       
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository = new();
        private readonly PazarYerleriRepository _pazarYerleriRepository = new();
        private static readonly ConcurrentDictionary<int, List<PTTCategory>> _categoryCache = new();

        public IActionResult Index()
        {
            int? pazarYeriId = 4; // PTT için sabit ID
            var pazarYerleris = _pazarYerleriRepository.GetirList(x => x.Durumu == 1);
            ViewBag.Pazaryerleri = pazarYerleris;
            ViewBag.SelectedPazarYeriId = pazarYeriId;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPTTCategories(int pazarYeriId)
        {
            pazarYeriId = 4; // PTT için sabit ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            // Check cache first
            if (_categoryCache.TryGetValue(pazarYeriId, out var cachedCategories))
            {
                var treeCategories = BuildCategoryTree(cachedCategories);
                return Json(new { success = true, categories = treeCategories });
            }
      
            // Configure SOAP client
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };

            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;

            try
            {
                await client.OpenAsync();
                var result = await client.GetCategoryTreeAsync("0", "2025");

                if (!result.success)
                    return Json(new { success = false, message = "Failed to retrieve category tree from API." });

                // Convert flat category list to PTTCategory objects
                var convertedList = result.category_tree?.Select(x => new PTTCategory
                {
                    Id = Convert.ToInt32(x.id),
                    Name = x.name,
                    ParentId = Convert.ToInt32(x.parent_id),
                    UpdatedAt = x.updated_at,
                    Children = new List<PTTCategory>()
                }).ToList() ?? new List<PTTCategory>();

                // Build hierarchical tree
                var rootCategories = BuildHierarchicalTree(convertedList);

                // Cache the result
                _categoryCache[pazarYeriId] = rootCategories;
                var treeCategories = BuildCategoryTree(rootCategories);
                return Json(new { success = true, categories = treeCategories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error retrieving categories: {ex.Message}" });
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

        private List<PTTCategory> BuildHierarchicalTree(List<PTTCategory> flatCategories)
        {
            var categoryMap = flatCategories.ToDictionary(c => c.Id, c => c);
            var rootCategories = new List<PTTCategory>();

            foreach (var category in flatCategories)
            {
                if (category.ParentId == 0) // Root categories have parent_id = 0
                {
                    rootCategories.Add(category);
                }
                else if (categoryMap.TryGetValue(category.ParentId ?? 0, out var parent))
                {
                    parent.Children = parent.Children ?? new List<PTTCategory>();
                    parent.Children.Add(category);
                }
            }

            return rootCategories.OrderBy(c => c.Name).ToList(); // Sort for consistent display
        }

        private List<object> BuildCategoryTree(List<PTTCategory> categories)
        {
            var result = new List<object>();
            foreach (var category in categories)
            {
                var node = new
                {
                    id = category.Id,
                    name = category.Name,
                    fullPath = GetCategoryPath(category, new List<PTTCategory>()),
                    isLeaf = category.Children == null || !category.Children.Any(),
                    subCategories = category.Children?.Any() == true
                        ? BuildCategoryTree(category.Children)
                        : null
                };
                result.Add(node);
            }
            return result;
        }

        private string GetCategoryPath(PTTCategory category, List<PTTCategory> path)
        {
            path.Insert(0, category);
            if (category.ParentId != 0 && _categoryCache.TryGetValue(4, out var allCategories))
            {
                var parent = allCategories.FirstOrDefault(c => c.Id == category.ParentId);
                if (parent != null)
                {
                    return GetCategoryPath(parent, path);
                }
            }
            return string.Join(" > ", path.Select(c => c.Name));
        }

        [HttpPost]
        public async Task<IActionResult> AddPTTProduct([FromForm] UnifiedMarketplaceModel request, int pazarYeriId)
        {
            pazarYeriId = 4; // PTT için sabit ID
            var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
            if (girisBilgisi == null)
                return Json(new { success = false, message = "API credentials not found." });

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential)
            {
                Security = { Message = { ClientCredentialType = BasicHttpMessageCredentialType.UserName } },
                MaxReceivedMessageSize = 20000000
            };

            var endpoint = new EndpointAddress("https://ws.pttavm.com:93/service.svc");
            var client = new PTTServiceReference.ServiceClient(binding, endpoint);
            client.ClientCredentials.UserName.UserName = girisBilgisi.KullaniciAdi;
            client.ClientCredentials.UserName.Password = girisBilgisi.Sifre;

            try
            {
                await client.OpenAsync();
                var productRequest = new PTTServiceReference.ProductV3Request
                {
                    Active =  true,
                    Barcode = request.Barcode,
                    BasketMaxQuantity = request.PTT.BasketMaxQuantity ?? 1000,
                    CargoProfileId = request.PTT.CargoProfileId ?? 0,
                    CategoryId = (int?)request.PTT.CategoryId,
                    Desi = (double)request.PTT.Desi,
                    Discount = request.PTT.Discount ?? 0,
                    EstimatedCourierDelivery = request.PTT.EstimatedCourierDelivery ?? 5,
                    Images = request.ImgFiles.Select(file =>
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine("wwwroot/uploads", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var fileUrl = "/uploads/" + fileName;

                        return new PTTServiceReference.ProductImageV3
                        {
                            Url = fileUrl
                        };
                    }).ToArray(),
                    LongDescription = request.Description,
                    Name = request.Title,
                    NoShippingProduct = false,
                    PriceWithVat = request.PTT.PriceWithVat ?? 0,
                    PriceWithoutVat = request.PTT.SalePrice,
                    ProductCode = request.Barcode,
                    Quantity = request.PTT.StockQuantity,
                    SingleBox = false,
                    VATRate = request.VatRate,
                    WarrantyDuration = request.PTT.WarrantyDuration ?? 0,
                    Variants = request.PTT.Variants?.Select(v => new PTTServiceReference.VariantV3
                    {
                        VariantBarcode = v.Barcode,
                        Price = v.Price ??0,
                        Quantity = v.Quantity??0,
                        Attributes = v.Attributes?.Select(a => new PTTServiceReference.VariantAttrV3
                        {
                            Definition = a.Definition,
                            Value = a.Value
                        }).ToArray()
                    }).ToArray()
                };

                var response = await client.UpdateProductsV3Async(new PTTServiceReference.ProductV3Request[] { productRequest });

                return Json(new
                {
                    success = response.Success,
                    message = response.Message,
                    trackingId = response.TrackingId
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error adding product: {ex.Message}" });
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