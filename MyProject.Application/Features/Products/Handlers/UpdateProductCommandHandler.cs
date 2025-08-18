using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Features.Products.Commands;
using MyProject.Core.Interfaces;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Products.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<UpdateProductCommandHandler> _logger;
        private const string CacheKey = "productList";

        public UpdateProductCommandHandler(IProductRepository productRepository, ICacheService cacheService, ILogger<UpdateProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ürün güncelleme isteği geldi. Ürün ID: {ProductId}", request.Id);

            var product = await _productRepository.GetByIdAsync(request.Id);

            if (product == null)
            {
                _logger.LogWarning("Güncelleme başarısız: Ürün bulunamadı. ID: {ProductId}", request.Id);
                return false;
            }

            // Ürün bilgilerini güncelleyelim
            product.Name = request.Name;
            product.Price = request.Price;
            product.Stock = request.Stock;

            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Ürün başarıyla veritabanında güncellendi: {ProductName}", product.Name);

            // Veri değiştiği için cache'i temizleyelim.
            await _cacheService.RemoveAsync(CacheKey);
            _logger.LogWarning("Ürün güncellendiği için cache temizlendi.");

            return true;
        }
    }
}