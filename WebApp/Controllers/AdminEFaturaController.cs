using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Controllers.Invoice;
using WebApp.Repositories;

namespace WebApp.Controllers
{
    public class AdminEFaturaController : AdminBaseController
    {
        private readonly InvoiceService invoiceService;
        private EDMBilgileriRepository edmBilgileriRepository = new EDMBilgileriRepository();
        private InvoiceLineDtoRepository invoiceLineDtoRepository = new InvoiceLineDtoRepository();


        private InvoiceRequestDtoRepository invoiceRequestDtoRepository = new InvoiceRequestDtoRepository();

        public AdminEFaturaController(IHostEnvironment hostEnvironment)
        {
            invoiceService = new InvoiceService(hostEnvironment, edmBilgileriRepository, invoiceLineDtoRepository, invoiceRequestDtoRepository);
        }
        [HttpGet]
        public IActionResult Index(string invoiceNumber="")
        {
            CultureInfo trCulture = new CultureInfo("tr-TR");
            ViewBag.EFaturaListesi = invoiceRequestDtoRepository.Listele("Siparis")
                .OrderByDescending(x => DateTime.ParseExact(x.FaturaTarih, "yyyy-MM-dd", trCulture))
                .ThenByDescending(x => Convert.ToInt64(x.FaturaNumarasi.Replace(x.SeriNo, "")))
                .ToList();
            ViewBag.SelectInvoiceNumber = invoiceNumber;
            return View();
        }
  
        public async Task<IActionResult> GetInvoiceHtml(string invoiceNumber)
        {
            try
            {
                var html = await invoiceService.ConvertInvoiceXmlToHtml(invoiceNumber);
                return Ok(new { htmlContent = html });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
