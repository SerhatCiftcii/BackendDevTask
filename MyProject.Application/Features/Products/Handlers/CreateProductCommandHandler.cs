using MediatR;
using Microsoft.Extensions.Logging; // Bunu unutma!
using MyProject.Application.Features.Products.Commands;
using MyProject.Core.Interfaces;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Products.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CreateProductCommandHandler> _logger; // Tanımla!
        private const string CacheKey = "productList";

        public CreateProductCommandHandler(IProductRepository productRepository, ICacheService cacheService, ILogger<CreateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ürün ekleme isteği geldi: {Name}", request.Name);

            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            await _productRepository.AddAsync(product);
            _logger.LogInformation("Ürün başarıyla veritabanına eklendi: {ProductName}", product.Name);

            await _cacheService.RemoveAsync(CacheKey);
            _logger.LogWarning("Ürün eklendiği için cache temizlendi."); // Neden warning? Cache temizlemek önemli bir durum, takip etmeliyiz.

            return true;
        }
    }
}