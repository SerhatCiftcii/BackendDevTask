using MediatR;
using MyProject.Application.Features.Products.Queries;
using MyProject.Core.Interfaces;
using MyProject.Core.Models;
using System.Text.Json;

namespace MyProject.Application.Features.Products.Handlers
{
    public class GetProductListQueryHandler : IRequestHandler<GetProductListQuery, IEnumerable<Product>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKey = "productList";

        public GetProductListQueryHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<Product>> Handle(GetProductListQuery request, CancellationToken cancellationToken)
        {
            // Önce Redis'te ürün listesi var mı diye kontrol ediyoruz.
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<Product>>(CacheKey);

            if (cachedProducts != null)
            {
                // Varmış, Redis'ten okuyalım.
                return cachedProducts;
            }

            // Yokmuş, o zaman veritabanından çekelim.
            var products = await _productRepository.GetAllAsync();

            // Veritabanından çektikten sonra cache'e kaydedelim ki bir dahaki sefere hızlı olsun. zamanını 10 olarak belirledim.
            await _cacheService.SetAsync(CacheKey, products, TimeSpan.FromMinutes(10));

            return products;
        }
    }
}