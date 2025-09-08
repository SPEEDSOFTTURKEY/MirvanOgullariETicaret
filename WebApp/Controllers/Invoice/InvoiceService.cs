using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using DocumentFormat.OpenXml.Wordprocessing;
using ServiceReference2;
using WebApp.Models;
using WebApp.Repositories;


namespace WebApp.Controllers.Invoice
{
    public class InvoiceResultDto
    {
        public string ResultMessage { get; set; }
        public string Fatno { get; set; }
    }
    public class InvoiceService
    {
        private readonly IHostEnvironment _hostEnvironment;
        private readonly EDMBilgileriRepository _edmBilgileriRepository;
        private readonly InvoiceLineDtoRepository _invoiceLineDtoRepository;
        private readonly InvoiceRequestDtoRepository _invoiceRequestDtoRepository;

        public InvoiceService(IHostEnvironment hostEnvironment,
            EDMBilgileriRepository edmBilgileriRepository,
            InvoiceLineDtoRepository invoiceLineDtoRepository,
            InvoiceRequestDtoRepository invoiceRequestDtoRepository)
        {
            _hostEnvironment = hostEnvironment;
            _edmBilgileriRepository = edmBilgileriRepository;
            _invoiceLineDtoRepository = invoiceLineDtoRepository;
            _invoiceRequestDtoRepository = invoiceRequestDtoRepository;
        }

        public async Task<InvoiceResultDto> GenerateInvoice(InvoiceRequestDto request, List<InvoiceLineDto> requestLine)
        {
            if (request == null || string.IsNullOrEmpty(request.MusteriVergiNo) || requestLine == null || !requestLine.Any())
                return new InvoiceResultDto { ResultMessage = "Error: Invalid or missing invoice data." };

            try
            {
                EFaturaEDMPortClient eFaturaEDMPortClient = new EFaturaEDMPortClient();
                REQUEST_HEADERType header = CreateRequestHeader();

                LoginResponse loginResponse = await LoginToEFatura(eFaturaEDMPortClient, header);
                header.SESSION_ID = loginResponse.SESSION_ID;

                InvoiceType invoice = CreateInvoice(header, request, requestLine);
                string fatno = invoice.ID.Value;

                string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/WebAdminTheme/E-Faturalar", $"{fatno}.xml");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Overwrite existing file
                }

                SaveInvoiceToFile(invoice, fatno);

                SendInvoiceRequest sendRequest = CreateSendInvoiceRequest(header, fatno, request);
                EDM_SendInvoiceResponseMessage sendResponseMessage = await eFaturaEDMPortClient.SendInvoiceAsync(sendRequest);

                SendInvoiceResponse response = sendResponseMessage.SendInvoiceResponse;

                // Kesilen faturayı veri tabanına kaydediyoruz.
                Context context = new Context();

                context.InvoiceRequestDto.Add(request);
                await context.SaveChangesAsync();
                foreach (var line in requestLine)
                {
                    line.InvoiceRequestDtoId = request.Id;
                    _invoiceLineDtoRepository.Ekle(line);
                }
                return new InvoiceResultDto
                {
                    ResultMessage = $"Success: Invoice {fatno} sent successfully! Response: {response.REQUEST_RETURN.RETURN_CODE}",
                    Fatno = fatno
                };

            }
            catch (Exception ex)
            {
                return new InvoiceResultDto { ResultMessage = $"Error: {ex.Message}" };
            }
        }

        private REQUEST_HEADERType CreateRequestHeader()
        {
            return new REQUEST_HEADERType
            {
                SESSION_ID = string.Empty,
                CLIENT_TXN_ID = Guid.NewGuid().ToString(),
                ACTION_DATE = DateTime.Now,
                ACTION_DATESpecified = true,
                CHANNEL_NAME = "SPEEDSOFT",
                COMPRESSED = "N",
                APPLICATION_NAME = "FaturaPortal",
                HOSTNAME = SanitizeText(Environment.MachineName),
                REASON = "TEST"
            };
        }

        private async Task<LoginResponse> LoginToEFatura(EFaturaEDMPortClient client, REQUEST_HEADERType header)
        {
            EDMBilgileri eDMBilgileri = new EDMBilgileri();
            eDMBilgileri = _edmBilgileriRepository.Getir(x => x.Durumu == 1);
            LoginRequest loginRequest = new LoginRequest
            {
                REQUEST_HEADER = header,
                USER_NAME = eDMBilgileri.KullaniciAdi,
                PASSWORD = eDMBilgileri.Sifre,
                SECRET_KEY = string.Empty
            };
            EDM_LoginResponseMessage responseMessage = await client.LoginAsync(loginRequest);
            return responseMessage.LoginResponse;
        }

        private InvoiceType CreateInvoice(REQUEST_HEADERType header, InvoiceRequestDto request, List<InvoiceLineDto> requestLine)
        {
            EDMBilgileri eDMBilgileri = new EDMBilgileri();
            eDMBilgileri = _edmBilgileriRepository.Getir(x => x.Durumu == 1);

            string series = request.SeriNo; // Türkçe karakter içermeyen bir seri
            string year = DateTime.Now.ToString("yyyy");
            string sequence = DateTime.Now.ToString("HHmmssfff");
            string invoiceId = request.FaturaNumarasi;
            // fatura numarası hazırda var ise tekrar oluşturmasın diye bu if konuldu.Veri tabnından gelen dosyaları oluşturmak için.
            if (request.FaturaNumarasi == "")
            {
                invoiceId = $"{series}{year}{sequence}";
                request.FaturaNumarasi = invoiceId;
            }

            InvoiceType invoice = new InvoiceType
            {
                UBLVersionID = new UBLVersionIDType { Value = "2.1" },
                CustomizationID = new CustomizationIDType { Value = "TR1.2" },
                ProfileID = new ProfileIDType { Value = "EARSIVFATURA" },
                ID = new IDType { Value = invoiceId },
                CopyIndicator = new CopyIndicatorType { Value = false },
                UUID = new UUIDType { Value = Guid.NewGuid().ToString().ToUpper() },
                IssueDate = new IssueDateType { Value = DateTime.Now },
                IssueTime = new IssueTimeType { Value = DateTime.Now.ToLocalTime() },
                InvoiceTypeCode = new InvoiceTypeCodeType { Value = "SATIS" },
                Note = new NoteType[] { new NoteType { Value = $"YALNIZ #{SanitizeText(request.YaziylaTutar ?? "")}#" } },
                DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "TRY" },
                LineCountNumeric = new LineCountNumericType { Value = requestLine.Count() }
            };

            // Supplier Party (Sender) - Değişmedi
            invoice.AccountingSupplierParty = new SupplierPartyType
            {
                Party = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType {
                            ID = new IDType {
                                schemeID = eDMBilgileri.VergiNumarasi.ToString().Length == 11 ? "TCKN" : "VKN",
                                Value = eDMBilgileri.VergiNumarasi.ToString()
                            }
                        }
                    },
                    PartyName = new PartyNameType { Name = new NameType1 { Value = SanitizeText(eDMBilgileri.Unvan) } },
                    PostalAddress = new AddressType
                    {
                        StreetName = new StreetNameType { Value = SanitizeText(eDMBilgileri.Adres) },
                        CitySubdivisionName = new CitySubdivisionNameType { Value = SanitizeText(eDMBilgileri.Ilce) },
                        CityName = new CityNameType { Value = SanitizeText(eDMBilgileri.Il) },
                        Country = new CountryType { Name = new NameType1 { Value = SanitizeText("Turkiye") } }
                    },
                    PartyTaxScheme = new PartyTaxSchemeType
                    {
                        TaxScheme = new TaxSchemeType { Name = new NameType1 { Value = SanitizeText(eDMBilgileri.VergiDairesi) } }
                    },
                    Contact = new ContactType
                    {
                        ElectronicMail = new ElectronicMailType { Value = SanitizeText(eDMBilgileri.Email ?? "") },
                        Telephone = new TelephoneType { Value = eDMBilgileri.Telefon ?? "" }
                    }
                }
            };

            // Customer Party (Receiver) - Güncellendi
            var customerParty = new CustomerPartyType
            {
                Party = new PartyType
                {
                    PartyIdentification = new[]
                    {
                        new PartyIdentificationType
                        {
                            ID = new IDType
                            {
                                schemeID = request.MusteriVergiNo.Length == 11 ? "TCKN" : "VKN",
                                Value = request.MusteriVergiNo
                            }
                        }
                    },
                    PartyName = new PartyNameType { Name = new NameType1 { Value = SanitizeText(request.Unvan ?? "") } },
                    PostalAddress = new AddressType
                    {
                        StreetName = new StreetNameType { Value = SanitizeText(request.Adres ?? "") },
                        CitySubdivisionName = new CitySubdivisionNameType { Value = SanitizeText(request.Ilce ?? "") },
                        CityName = new CityNameType { Value = SanitizeText(request.Sehir ?? "") },
                        Country = new CountryType { Name = new NameType1 { Value = SanitizeText(request.Ulke ?? "Turkiye") } }
                    },
                    Contact = new ContactType
                    {
                        ElectronicMail = new ElectronicMailType { Value = SanitizeText(request.Email ?? "") },
                        Telephone = new TelephoneType { Value = request.Telefon ?? "" }
                    }
                }

            };

            // Eğer schemeID "TCKN" ise, Person elemanı ekle (Dizi yerine tek nesne)
            if (request.MusteriVergiNo.Length == 11)
            {
                customerParty.Party.Person = new PersonType
                {
                    FirstName = new FirstNameType { Value = SanitizeText(SplitName(request.Unvan ?? "").FirstName) },
                    FamilyName = new FamilyNameType { Value = SanitizeText(SplitName(request.Unvan ?? "").FamilyName) }
                };
            }

            invoice.AccountingCustomerParty = customerParty;

            // Invoice Lines (Değişmedi)
            invoice.InvoiceLine = requestLine.Select((line, index) =>
            {
                string unitCode = "C62";
                if (line.Birim != null)
                {
                    if (line.Birim == "Gün") unitCode = "DAY";

                    if (line.Birim == "Adet") unitCode = "C62";

                }
                InvoiceLineType InvoiceLineType = new InvoiceLineType
                {

                    ID = new IDType { Value = (index + 1).ToString() },
                    InvoicedQuantity = new InvoicedQuantityType { unitCode = unitCode, Value = line.Miktar },

                    LineExtensionAmount = new LineExtensionAmountType { currencyID = "TRY", Value = line.NetTutar },
                    Item = new ItemType
                    {
                        Name = new NameType1 { Value = SanitizeText(line.Hizmet ?? "Hizmet") },
                        Description = new DescriptionType { Value = SanitizeText(line.Hizmet ?? "Hizmet") }
                    },
                    Price = new PriceType
                    {
                        PriceAmount = new PriceAmountType { currencyID = "TRY", Value = line.BirimFiyat }
                    },
                    TaxTotal = new TaxTotalType
                    {
                        TaxAmount = new TaxAmountType
                        {
                            currencyID = "TRY",
                            Value = line.KDVTutar
                        },
                        TaxSubtotal = new[]
                        {
                            new TaxSubtotalType
                            {
                                TaxableAmount = new TaxableAmountType { currencyID = "TRY", Value = line.NetTutar },
                                TaxAmount = new TaxAmountType { currencyID = "TRY", Value = line.KDVTutar },
                                Percent = new PercentType1 { Value = line.KDVOrani },
                                TaxCategory = new TaxCategoryType
                                {
                                    TaxScheme = new TaxSchemeType
                                    {
                                        Name = new NameType1 { Value = "KDV" },
                                        TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                                    }
                                }
                            }
                        }
                    }
                };


                return InvoiceLineType;
            }).ToArray();

            // Calculate totals (Değişmedi)
            var totalNet = requestLine.Sum(l => l.NetTutar);
            var totalVat = requestLine.Sum(l => l.KDVTutar);
          //  decimal totalKonaklama = requestLine.Sum(l => l.KonaklamaVergiTutari ?? 0);
            var totalAmount = totalNet + totalVat ;

            invoice.LegalMonetaryTotal = new MonetaryTotalType
            {
                LineExtensionAmount = new LineExtensionAmountType { currencyID = "TRY", Value = totalNet },
                TaxExclusiveAmount = new TaxExclusiveAmountType { currencyID = "TRY", Value = totalNet },
                TaxInclusiveAmount = new TaxInclusiveAmountType { currencyID = "TRY", Value = totalAmount },
                PayableAmount = new PayableAmountType { currencyID = "TRY", Value = totalAmount }
            };

            invoice.TaxTotal = new[]
            {
                new TaxTotalType
                {
                    TaxAmount = new TaxAmountType { currencyID = "TRY", Value = totalVat },
                    TaxSubtotal = new TaxSubtotalType[0] // Null değil, boş dizi
                }
            };

            var kdvGroup = requestLine.GroupBy(x => x.KDVOrani);
            foreach (var group in kdvGroup)
            {
                var groupTotalNet = group.Sum(x => x.NetTutar);
                var groupTotalVat = group.Sum(x => x.KDVTutar);
                decimal groupKDVOrani = group.FirstOrDefault().KDVOrani;
                string nameType = "KDV";

                var TaxSubtotal = invoice.TaxTotal[0].TaxSubtotal;
                Array.Resize(ref TaxSubtotal, TaxSubtotal.Length + 1);
                TaxSubtotal[TaxSubtotal.Length - 1] = new TaxSubtotalType
                {
                    TaxableAmount = new TaxableAmountType { currencyID = "TRY", Value = groupTotalNet },
                    TaxAmount = new TaxAmountType { currencyID = "TRY", Value = groupTotalVat },
                    Percent = new PercentType1 { Value = groupKDVOrani },
                    TaxCategory = new TaxCategoryType
                    {
                        TaxScheme = new TaxSchemeType
                        {
                            Name = new NameType1 { Value = nameType },
                            TaxTypeCode = new TaxTypeCodeType { Value = "0015" }
                        }
                    }
                };
                invoice.TaxTotal[0].TaxSubtotal = TaxSubtotal;
            }


       


            return invoice;
        }

        // Yardımcı metod: İsim ve soyisim ayırma (Değişmedi)
        private (string FirstName, string FamilyName) SplitName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return ("Bilinmeyen", "Kişi");

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return (parts[0], parts[^1]); // İlk kelime isim, son kelime soyisim
            return (parts[0], ""); // Tek kelime varsa sadece isim
        }

        private void SaveInvoiceToFile(InvoiceType invoice, string fatno)
        {
            string directoryPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/WebAdminTheme/E-Faturalar");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true, Encoding = Encoding.UTF8 };
            using (var ms = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(ms, settings))
                {
                    var srl = new XmlSerializer(invoice.GetType());
                    srl.Serialize(writer, invoice, namespacesgetir());
                }

                ms.Seek(0, SeekOrigin.Begin);
                using (var srRead = new StreamReader(ms, Encoding.UTF8, false))
                {
                    string readXml = srRead.ReadToEnd();
                    string path = Path.Combine(directoryPath, $"{fatno}.xml");
                    File.WriteAllText(path, readXml, Encoding.UTF8);
                }
            }
        }

        private SendInvoiceRequest CreateSendInvoiceRequest(REQUEST_HEADERType header, string fatno, InvoiceRequestDto request)
        {
            EDMBilgileri eDMBilgileri = new EDMBilgileri();
            eDMBilgileri = _edmBilgileriRepository.Getir(x => x.Durumu == 1);

            string filePath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/WebAdminTheme/E-Faturalar", $"{fatno}.xml");
            byte[] bytes = File.ReadAllBytes(filePath);

            return new SendInvoiceRequest
            {
                REQUEST_HEADER = header,
                SENDER = new SendInvoiceRequestSENDER { alias = eDMBilgileri.PostaKodu, vkn = eDMBilgileri.VergiNumarasi.ToString() },
                RECEIVER = new SendInvoiceRequestRECEIVER { vkn = request.MusteriVergiNo, alias = SanitizeText(request.Email ?? "mehmetbilen2005@gmail.com") },
                INVOICE = new[]
                {
                    new INVOICE
                    {
                        ID = fatno,
                        UUID = Guid.NewGuid().ToString().ToUpper(),
                        HEADER = new INVOICEHEADER
                        {
                            SENDER = eDMBilgileri.VergiNumarasi.ToString(),
                            RECEIVER = request.MusteriVergiNo,
                            FROM = eDMBilgileri.PostaKodu,
                            EARCHIVE = true,
                            INTERNETSALES = false
                        },
                        CONTENT = new base64Binary
                        {
                            contentType = "XML",
                            Value = bytes
                        }
                    }
                }
            };
        }

        private XmlSerializerNamespaces namespacesgetir()
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return ns;
        }

        // Sanitize text to remove Turkish characters
        private string SanitizeText(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return input
                .Replace("ğ", "g")
                .Replace("Ğ", "G")
                .Replace("ü", "u")
                .Replace("Ü", "U")
                .Replace("ş", "s")
                .Replace("Ş", "S")
                .Replace("ı", "i")
                .Replace("İ", "I")
                .Replace("ö", "o")
                .Replace("Ö", "O")
                .Replace("ç", "c")
                .Replace("Ç", "C");
        }
        public async Task<INVOICE[]> GetInvoicesByDateRange(DateTime startDate, DateTime endDate, string contentType = "XML", string direction = "OUT")
        {
            startDate = startDate.AddDays(1);
            endDate = endDate.AddDays(1);
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

            try
            {
                EFaturaEDMPortClient eFaturaEDMPortClient = new EFaturaEDMPortClient();
                REQUEST_HEADERType header = CreateRequestHeader();

                // Oturum açma
                LoginResponse loginResponse = await LoginToEFatura(eFaturaEDMPortClient, header);
                header.SESSION_ID = loginResponse.SESSION_ID;

                // Fatura alma isteği oluştur
                GetInvoiceRequest getInvoiceRequest = CreateGetInvoiceRequest(header, startDate, endDate, contentType, direction);

                // Faturaları al
                EDM_GetInvoiceResponse response = await eFaturaEDMPortClient.GetInvoiceAsync(getInvoiceRequest);

                return response.GetInvoiceResponse?.Length > 0
                    ? response.GetInvoiceResponse
                    : Array.Empty<INVOICE>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Faturalar alınırken hata oluştu: {ex.Message}", ex);
            }
        }
        private GetInvoiceRequest CreateGetInvoiceRequest(REQUEST_HEADERType header, DateTime startDate, DateTime endDate, string contentType, string direction)
        {
            return new GetInvoiceRequest
            {
                REQUEST_HEADER = header,
                INVOICE_SEARCH_KEY = new GetInvoiceRequestINVOICE_SEARCH_KEY
                {
                    START_DATE = startDate,
                    START_DATESpecified = true,
                    END_DATE = endDate,
                    END_DATESpecified = true,
                    DIRECTION = direction, // "OUT" (giden) veya "IN" (gelen)
                    // CONTENT_TYPE = contentType // Uncomment if needed
                }
            };
        }

  
        public async Task<string> ConvertInvoiceXmlToHtml(string invoiceNumber)
        {
            string directoryPath = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/WebAdminTheme/E-Faturalar");
            string filePath = Path.Combine(directoryPath, $"{invoiceNumber}.xml");

            if (!File.Exists(filePath))
            {
                EFaturaEDMPortClient eFaturaEDMPortClient = new EFaturaEDMPortClient();
                REQUEST_HEADERType header = CreateRequestHeader();

                LoginResponse loginResponse = await LoginToEFatura(eFaturaEDMPortClient, header);
                header.SESSION_ID = loginResponse.SESSION_ID;

                InvoiceRequestDto request = _invoiceRequestDtoRepository.Getir(x => x.FaturaNumarasi == invoiceNumber);
                if (request == null)
                {
                    throw new FileNotFoundException($"Fatura dosyası bulunamadı: {invoiceNumber}");
                }

                List<InvoiceLineDto> requestLine = _invoiceLineDtoRepository.GetirList(x => x.InvoiceRequestDtoId == request.Id);
                if (requestLine == null || requestLine.Count == 0)
                {
                    throw new FileNotFoundException($"Fatura dosyası bulunamadı: {invoiceNumber}");
                }
                InvoiceType invoice = CreateInvoice(header, request, requestLine);
                string fatno = invoice.ID.Value;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Overwrite existing file
                }

                SaveInvoiceToFile(invoice, fatno);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Fatura dosyası bulunamadı: {invoiceNumber}");
                }
            }

            XDocument doc = XDocument.Load(filePath);

            StringBuilder html = new StringBuilder();

            // HTML Başlangıcı ve Stil (Orijinal şablon korundu)
            html.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">");
            html.AppendLine("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-16\">");
            html.AppendLine("<style type=\"text/css\">");
            html.AppendLine("body { background-color: #FFFFFF; font-family: 'Tahoma', \"Times New Roman\", Times, serif; font-size: 11px; color: #666666; }");
            html.AppendLine("h1, h2 { padding-bottom: 3px; padding-top: 3px; margin-bottom: 5px; text-transform: uppercase; font-family: Arial, Helvetica, sans-serif; }");
            html.AppendLine("h1 { font-size: 1.4em; text-transform:none; }");
            html.AppendLine("h2 { font-size: 1em; color: brown; }");
            html.AppendLine("h3 { font-size: 1em; color: #333333; text-align: justify; margin: 0; padding: 0; }");
            html.AppendLine("h4 { font-size: 1.1em; font-style: bold; font-family: Arial, Helvetica, sans-serif; color: #000000; margin: 0; padding: 0; }");
            html.AppendLine("hr { height:2px; color: #000000; background-color: #000000; border-bottom: 1px solid #000000; }");
            html.AppendLine("p, ul, ol { margin-top: 1.5em; }");
            html.AppendLine("ul, ol { margin-left: 3em; }");
            html.AppendLine("blockquote { margin-left: 3em; margin-right: 3em; font-style: italic; }");
            html.AppendLine("a { text-decoration: none; color: #70A300; }");
            html.AppendLine("a:hover { border: none; color: #70A300; }");
            html.AppendLine("#despatchTable { border-collapse:collapse; font-size:11px; float:right; border-color:gray; }");
            html.AppendLine("#ettnTable { border-collapse:collapse; font-size:11px; border-color:gray; }");
            html.AppendLine("#customerPartyTable { border-width: 0px; border-spacing:; border-style: inset; border-color: gray; border-collapse: collapse; background-color: }");
            html.AppendLine("#customerIDTable { border-width: 2px; border-spacing:; border-style: inset; border-color: gray; border-collapse: collapse; background-color: }");
            html.AppendLine("#customerIDTableTd { border-width: 2px; border-spacing:; border-style: inset; border-color: gray; border-collapse: collapse; background-color: }");
            html.AppendLine("#lineTable { border-width:2px; border-spacing:; border-style: inset; border-color: black; border-collapse: collapse; background-color:; }");
            html.AppendLine("td.lineTableTd { border-width: 1px; padding: 1px; border-style: inset; border-color: black; background-color: white; }");
            html.AppendLine("tr.lineTableTr { border-width: 1px; padding: 0px; border-style: inset; border-color: black; background-color: white; -moz-border-radius:; }");
            html.AppendLine("#lineTableDummyTd { border-width: 1px; border-color:white; padding: 1px; border-style: inset; border-color: black; background-color: white; }");
            html.AppendLine("td.lineTableBudgetTd { border-width: 2px; border-spacing:0px; padding: 1px; border-style: inset; border-color: black; background-color: white; -moz-border-radius:; }");
            html.AppendLine("#notesTable { border-width: 2px; border-spacing:; border-style: inset; border-color: black; border-collapse: collapse; background-color: }");
            html.AppendLine("#notesTableTd { border-width: 0px; border-spacing:; border-style: inset; border-color: black; border-collapse: collapse; background-color: }");
            html.AppendLine("table { border-spacing:0px; }");
            html.AppendLine("#budgetContainerTable { border-width: 0px; border-spacing: 0px; border-style: inset; border-color: black; border-collapse: collapse; background-color:; }");
            html.AppendLine("td { border-color:gray; }");
            html.AppendLine("</style><title>e-Fatura</title></head>");

            // Body Başlangıcı
            html.AppendLine("<body style=\"margin-left=0.6in; margin-right=0.6in; margin-top=0.79in; margin-bottom=0.79in;\">");

            // Ana Tablo
            html.AppendLine("<table style=\"border-color:blue;\" border=\"0\" cellspacing=\"0px\" width=\"800\" cellpadding=\"0px\"><tbody>");

            // Gönderici ve Logo Bölümü
            html.AppendLine("<tr valign=\"top\">");
            html.AppendLine("<td width=\"40%\"><hr>");
            html.AppendLine("<table align=\"center\" border=\"0\" width=\"100%\"><tbody>");
            var supplier = doc.Root.Element(XName.Get("AccountingSupplierParty", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Party", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
            html.AppendLine($"<tr align=\"left\"><td align=\"left\">{(supplier?.Element(XName.Get("PartyName", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}<br></td></tr>");
            html.AppendLine($"<tr align=\"left\"><td align=\"left\">{(supplier?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("CitySubdivisionName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} {(supplier?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("CityName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}<br> {(supplier?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Country", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} </td></tr>");
            html.AppendLine($"<tr align=\"left\"><td align=\"left\">Tel: {(supplier?.Element(XName.Get("Contact", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Telephone", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} Fax: </td></tr>");
            html.AppendLine($"<tr align=\"left\"><td>E-Posta: <a href=\"mailto:{(supplier?.Element(XName.Get("Contact", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("ElectronicMail", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}\">{(supplier?.Element(XName.Get("Contact", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("ElectronicMail", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</a></td></tr>");
            html.AppendLine($"<tr align=\"left\"><td align=\"left\">Vergi Dairesi: {(supplier?.Element(XName.Get("PartyTaxScheme", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("TaxScheme", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} </td></tr>");
            var supplierId = supplier?.Elements(XName.Get("PartyIdentification", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                .FirstOrDefault()?.Element(XName.Get("ID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            html.AppendLine($"<tr align=\"left\"><td>VKN: {(supplierId?.Value ?? "")}</td></tr>");
            html.AppendLine("</tbody></table><hr></td>");
            // Logo Bölümü
            html.AppendLine("<td width=\"20%\" align=\"center\" valign=\"middle\"><br><br>");
            html.AppendLine("<img style=\"width:91px;\" align=\"middle\" alt=\"E-Fatura Logo2\" src=\"https://www.orkestra.com.tr/usr_img/usr/contents/fatura_tasarim/gib_400px.png\" />");
            html.AppendLine("<h1 align=\"center\"><span style=\"font-weight:bold;\">e-FATURA</span></h1></td><td width=\"40%\"></td></tr>");

            // Alıcı ve Fatura Bilgileri Bölümü
            html.AppendLine("<tr style=\"height:118px;\" valign=\"top\">");
            html.AppendLine("<td width=\"40%\" align=\"right\" valign=\"bottom\">");
            html.AppendLine("<table id=\"customerPartyTable\" align=\"left\" border=\"0\"><tbody><tr style=\"height:71px;\"><td><hr>");
            html.AppendLine("<table align=\"center\" border=\"0\"><tbody>");
            var customer = doc.Root.Element(XName.Get("AccountingCustomerParty", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Party", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
            html.AppendLine("<tr><td style=\"width:469px;\" align=\"left\"><span style=\"font-weight:bold;\">SAYIN</span></td></tr>");
            var customerName = customer?.Element(XName.Get("PartyName", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value;
            if (customer?.Element(XName.Get("Person", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")) != null)
            {
                var person = customer.Element(XName.Get("Person", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
                customerName = $"{(person?.Element(XName.Get("FirstName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} {(person?.Element(XName.Get("FamilyName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}".Trim();
            }
            html.AppendLine($"<tr><td style=\"width:469px;\" align=\"left\">{(customerName ?? "")}<br></td></tr>");
            html.AppendLine($"<tr><td style=\"width:469px;\" align=\"left\">{(customer?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("StreetName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} {(customer?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("CityName", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}<br><br>{(customer?.Element(XName.Get("PostalAddress", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Country", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} </td></tr>");
            html.AppendLine($"<tr align=\"left\"><td>E-Posta: {(customer?.Element(XName.Get("Contact", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("ElectronicMail", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</td></tr>");
            html.AppendLine($"<tr align=\"left\"><td style=\"width:469px;\" align=\"left\">Tel: {(customer?.Element(XName.Get("Contact", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Telephone", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")} Fax: </td></tr>");
            var customerId = customer?.Elements(XName.Get("PartyIdentification", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                .FirstOrDefault()?.Element(XName.Get("ID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));
            html.AppendLine($"<tr align=\"left\"><td>VKN: {(customerId?.Value ?? "")}</td></tr>");
            html.AppendLine("</tbody></table><hr></td></tr></tbody></table><br></td>");
            html.AppendLine("<td width=\"60%\" align=\"right\" valign=\"bottom\" colspan=\"2\">");
            html.AppendLine("<table border=\"1\" id=\"despatchTable\"><tbody>");
            html.AppendLine($"<tr><td style=\"width:105px;\" align=\"left\"><span style=\"font-weight:bold;\">Özelleştirme No:</span></td><td style=\"width:110px;\" align=\"left\">{(doc.Root.Element(XName.Get("CustomizationID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "TR1.2")}</td></tr>");
            html.AppendLine($"<tr style=\"height:13px;\"><td align=\"left\"><span style=\"font-weight:bold;\">Senaryo:</span></td><td align=\"left\">{(doc.Root.Element(XName.Get("ProfileID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "TICARIFATURA")}</td></tr>");
            html.AppendLine($"<tr style=\"height:13px;\"><td align=\"left\"><span style=\"font-weight:bold;\">Fatura Tipi:</span></td><td align=\"left\">{(doc.Root.Element(XName.Get("InvoiceTypeCode", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "SATIS")}</td></tr>");
            html.AppendLine($"<tr style=\"height:13px;\"><td align=\"left\"><span style=\"font-weight:bold;\">Fatura No:</span></td><td align=\"left\">{(doc.Root.Element(XName.Get("ID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</td></tr>");
            html.AppendLine($"<tr style=\"height:13px;\"><td align=\"left\"><span style=\"font-weight:bold;\">Fatura Tarihi:</span></td><td align=\"left\">{(doc.Root.Element(XName.Get("IssueDate", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</td></tr>");
            html.AppendLine("</tbody></table></td></tr>");
            html.AppendLine($"<tr align=\"left\"><td align=\"left\" valign=\"top\" id=\"ettnTable\"><span style=\"font-weight:bold;\">ETTN: </span>{(doc.Root.Element(XName.Get("UUID", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</td></tr>");
            html.AppendLine("</tbody></table>");

            // Satır Tablosu
            html.AppendLine("<div id=\"lineTableAligner\"><span> </span></div>");
            html.AppendLine("<table border=\"1\" id=\"lineTable\" width=\"800\"><tbody>");
            html.AppendLine("<tr class=\"lineTableTr\">");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:3%\" align=\"center\"><span style=\"font-weight:bold;\">Sıra No</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:20%\" align=\"center\"><span style=\"font-weight:bold;\">Mal Hizmet</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:7.4%\" align=\"center\"><span style=\"font-weight:bold;\">Miktar</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:9%\" align=\"center\"><span style=\"font-weight:bold;\">Birim Fiyat</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:7%\" align=\"center\"><span style=\"font-weight:bold;\">İskonto/ Arttırım Oranı</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:9%\" align=\"center\"><span style=\"font-weight:bold;\">İskonto/ Arttırım Tutarı</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:9%\" align=\"center\"><span style=\"font-weight:bold;\">İskonto/ Arttırım Nedeni</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:7%\" align=\"center\"><span style=\"font-weight:bold;\">KDV Oranı</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:10%\" align=\"center\"><span style=\"font-weight:bold;\">KDV Tutarı</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:17%;\" align=\"center\"><span style=\"font-weight:bold;\">Diğer Vergiler</span></td>");
            html.AppendLine("<td class=\"lineTableTd\" style=\"width:10.6%\" align=\"center\"><span style=\"font-weight:bold;\">Mal Hizmet Tutarı</span></td>");
            html.AppendLine("</tr>");

            // Satır Bilgileri
            var invoiceLines = doc.Root.Elements(XName.Get("InvoiceLine", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
            int lineNumber = 1;
            foreach (var line in invoiceLines)
            {
                var taxTotalLine = line.Element(XName.Get("TaxTotal", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
                var taxSubtotalsLine = taxTotalLine?.Elements(XName.Get("TaxSubtotal", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
                var kdvSubtotal = taxSubtotalsLine?.FirstOrDefault(s => s.Element(XName.Get("TaxCategory", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                    ?.Element(XName.Get("TaxScheme", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                    ?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value == "KDV");
            

                html.AppendLine("<tr class=\"lineTableTr\">");
                html.AppendLine($"<td class=\"lineTableTd\"> {lineNumber++}</td>");
                html.AppendLine($"<td class=\"lineTableTd\"> {(line.Element(XName.Get("Item", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "")}</td>");
                string lineUnitCode = (line.Element(XName.Get("InvoicedQuantity", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Attribute("unitCode")?.Value ?? "");
                string lineUnitCodeTR = "";
                if (lineUnitCode != null)
                {
                    if (lineUnitCode == "DAY") lineUnitCodeTR = "Gün";
                    if (lineUnitCode == "C62") lineUnitCodeTR = "Adet";
                }

                html.AppendLine($"<td class=\"lineTableTd\" align=\"right\"> {(line.Element(XName.Get("InvoicedQuantity", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} {lineUnitCodeTR}</td>");
                html.AppendLine($"<td class=\"lineTableTd\" align=\"right\"> {(line.Element(XName.Get("Price", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))?.Element(XName.Get("PriceAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td>");
                html.AppendLine("<td class=\"lineTableTd\" align=\"right\"> %0,00</td>");
                html.AppendLine("<td class=\"lineTableTd\" align=\"right\"> 0,00 TL</td>");
                html.AppendLine("<td class=\"lineTableTd\" align=\"right\"> </td>");
                html.AppendLine($"<td class=\"lineTableTd\" align=\"right\"> %{(kdvSubtotal?.Element(XName.Get("Percent", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")},00</td>");
                html.AppendLine($"<td class=\"lineTableTd\" align=\"right\"> {(kdvSubtotal?.Element(XName.Get("TaxAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td>");
    
               
                    html.AppendLine("<td class=\"lineTableTd\" align=\"right\"> </td>");
            

                html.AppendLine($"<td class=\"lineTableTd\" align=\"right\"> {(line.Element(XName.Get("LineExtensionAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td>");
                html.AppendLine("</tr>");
            }

            // Boş Satırlar (Toplam 20 satır olacak şekilde)
            for (int i = lineNumber; i <= 20; i++)
            {
                html.AppendLine("<tr class=\"lineTableTr\">");
                html.AppendLine("<td class=\"lineTableTd\"> </td><td class=\"lineTableTd\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td><td class=\"lineTableTd\" align=\"right\"> </td>");
                html.AppendLine("</tr>");
            }
            html.AppendLine("</tbody></table>");

            // Toplam Tutar Tablosu
            var legalMonetary = doc.Root.Element(XName.Get("LegalMonetaryTotal", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
            var taxTotal = doc.Root.Elements(XName.Get("TaxTotal", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")).FirstOrDefault();
            var taxSubtotals = taxTotal?.Elements(XName.Get("TaxSubtotal", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"));
            var kdvTotal = taxSubtotals?.FirstOrDefault(s => s.Element(XName.Get("TaxCategory", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                ?.Element(XName.Get("TaxScheme", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2"))
                ?.Element(XName.Get("Name", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value == "KDV");                    
            html.AppendLine("<table id=\"budgetContainerTable\" width=\"800px\">");
            html.AppendLine($"<tr align=\"right\"><td></td><td class=\"lineTableBudgetTd\" align=\"right\" width=\"200px\"><span style=\"font-weight:bold;\">Mal Hizmet Toplam Tutarı</span></td><td class=\"lineTableBudgetTd\" style=\"width:81px;\" align=\"right\">{(legalMonetary?.Element(XName.Get("LineExtensionAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td></tr>");
            html.AppendLine($"<tr align=\"right\"><td></td><td class=\"lineTableBudgetTd\" align=\"right\" width=\"200px\"><span style=\"font-weight:bold;\">Toplam İskonto</span></td><td class=\"lineTableBudgetTd\" style=\"width:81px;\" align=\"right\">{(legalMonetary?.Element(XName.Get("AllowanceTotalAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td></tr>");
            html.AppendLine($"<tr align=\"right\"><td></td><td class=\"lineTableBudgetTd\" width=\"211px\" align=\"right\"><span style=\"font-weight:bold;\">Hesaplanan KDV(%{(kdvTotal?.Element(XName.Get("Percent", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")})</span></td><td class=\"lineTableBudgetTd\" style=\"width:82px;\" align=\"right\">{(kdvTotal?.Element(XName.Get("TaxAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td></tr>");
            html.AppendLine($"<tr align=\"right\"><td></td><td class=\"lineTableBudgetTd\" width=\"200px\" align=\"right\"><span style=\"font-weight:bold;\">Vergiler Dahil Toplam Tutar</span></td><td class=\"lineTableBudgetTd\" style=\"width:82px;\" align=\"right\">{(legalMonetary?.Element(XName.Get("TaxInclusiveAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td></tr>");
            html.AppendLine($"<tr align=\"right\"><td></td><td class=\"lineTableBudgetTd\" width=\"200px\" align=\"right\"><span style=\"font-weight:bold;\">Ödenecek Tutar</span></td><td class=\"lineTableBudgetTd\" style=\"width:82px;\" align=\"right\">{(legalMonetary?.Element(XName.Get("PayableAmount", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? "0")} TL</td></tr>");
            html.AppendLine("</table><br><br>");

            // Notlar
            html.AppendLine("<table id=\"notesTable\" width=\"800\" align=\"left\"><tbody>");
            var notes = doc.Root.Elements(XName.Get("Note", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"));

            foreach (var note in notes)
            {
                html.AppendLine($"<tr align=\"left\"><td id=\"notesTableTd\"><br><b>      Not: </b>{(note.Value ?? "")}<br></td></tr>");
            }
            html.AppendLine($"<tr align=\"left\"><td id=\"notesTableTd\"><b>      Not: </b>--- Fatura Vadesi : {doc.Root.Element(XName.Get("IssueDate", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2"))?.Value ?? ""} dir.<br><br></td></tr>");
            html.AppendLine("</tbody></table>");

            // HTML Kapanış
            html.AppendLine("</body></html>");

            return html.ToString();
        }

    }
}

