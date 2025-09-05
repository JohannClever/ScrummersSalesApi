// ScrummersSalesApi.Backend.Orders.Infrastructure/Http/ProductCatalogClient.cs
using Polly;
using Polly.Extensions.Http;
using ScrummersSalesApi.Backend.Orders.Domain.Ports;
using ScrummersSalesApi.Backend.Orders.Domain.ReadModel.Products;
using System.Net.Http.Json;


namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Adapters.Http
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient _http;
        public ProductCatalogClient(HttpClient http) => _http = http;

        public async Task<IEnumerable<ProductRead>> GetProductsByIdsAsync(List<Guid> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                return Enumerable.Empty<ProductRead>();

            var resp = await _http.PostAsJsonAsync("/Products/ByIds", productIds);
            resp.EnsureSuccessStatusCode();

            var products = await resp.Content.ReadFromJsonAsync<IEnumerable<ProductRead>>();
            return products ?? Enumerable.Empty<ProductRead>();
        }

        public static IAsyncPolicy<HttpResponseMessage> RetryPolicy =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .OrResult(r => (int)r.StatusCode == 429)
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)));

        public static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =>
            HttpPolicyExtensions.HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        public static IAsyncPolicy<HttpResponseMessage> TimeoutPolicy =>
            Policy.TimeoutAsync<HttpResponseMessage>(5);
    }
}


