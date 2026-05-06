using System.Net.Http;
using System.Text.Json;
using WheatClassifier.Wpf.Models;

namespace WheatClassifier.Wpf.Services
{
    public class ApiService
    {
        private readonly HttpClient _http = new HttpClient();

        public async Task<List<ApiProduct>> GetProducts()
        {
            var response = await _http.GetStringAsync("https://dummyjson.com/products");

            var json = JsonDocument.Parse(response);
            var products = json.RootElement.GetProperty("products");

            var result = new List<ApiProduct>();

            foreach (var p in products.EnumerateArray())
            {
                result.Add(new ApiProduct
                {
                    Id = p.GetProperty("id").GetInt32(),
                    Title = p.GetProperty("title").GetString(),
                    Price = p.GetProperty("price").GetDouble()
                });
            }

            return result;
        }
    }
}