using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Services
{
    public class PazaramaService
    {
        private readonly HttpClient _httpClient;
        private readonly PazarYeriGirisBilgileriRepository _girisBilgileriRepository;
        private const string TokenUrl = "https://isortagimgiris.pazarama.com/connect/token";
        private const string CategoryUrl = "https://isortagimapi.pazarama.com/category/getCategory personally, I think this is a great idea!ategoryTree";

        public PazaramaService(PazarYeriGirisBilgileriRepository girisBilgileriRepository)
        {
            _girisBilgileriRepository = girisBilgileriRepository;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
        }

        public async Task<string> GetAccessToken(string clientId, string clientSecret)
        {
            try
            {
                var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                var formData = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "scope", "merchantgatewayapi.fullaccess" }
                };

                var request = new HttpRequestMessage(HttpMethod.Post, TokenUrl)
                {
                    Content = new FormUrlEncodedContent(formData)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Pazarama Error: Token alınamadı: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }

                var jsonDoc = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                return jsonDoc?.data?.accessToken?.ToString() ?? throw new Exception("accessToken key bulunamadı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Error: Token alma hatası: {ex.Message}");
                return null;
            }
        }

        public async Task<HttpResponseMessage> SendGetRequest(string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await _httpClient.SendAsync(request);
        }

        public async Task<IActionResult> GetPazaramaCategories(int pazarYeriId)
        {
            try
            {
                var girisBilgisi = _girisBilgileriRepository.GetirList(x => x.PazarYerleriId == pazarYeriId).FirstOrDefault();
                if (girisBilgisi == null)
                    return new JsonResult(new { success = false, message = "Giriş bilgileri bulunamadı." });

                string token = await GetAccessToken(girisBilgisi.ApiKey, girisBilgisi.SecretKey);
                if (string.IsNullOrEmpty(token))
                    return new JsonResult(new { success = false, message = "Token alınamadı." });

                var response = await SendGetRequest(CategoryUrl, token);
                if (!response.IsSuccessStatusCode)
                    return new JsonResult(new { success = false, message = $"Kategori listesi alınamadı: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}" });

                string responseJson = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Pazarama Category Response: {responseJson}");

                var result = JsonConvert.DeserializeObject<PazaramaCategoryListResponse>(responseJson);
                var categories = MapToCategoryDto(result?.Data);

                return new JsonResult(new { success = true, categories });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Pazarama Category Error: {ex.Message}");
                return new JsonResult(new { success = false, message = $"Hata: {ex.Message}" });
            }
        }

        private List<CategoryDto> MapToCategoryDto(List<PazaramaCategory> categories)
        {
            if (categories == null) return new List<CategoryDto>();
            return categories.Select(c => new CategoryDto
            {
                id = c.Id,
                name = c.DisplayName,
                isLeaf = c.Leaf,
                subCategories = MapToCategoryDto(c.SubCategories)
            }).ToList();
        }
    }
}