using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Features.Products.Commands;
using MyProject.Core.Interfaces;

namespace MyProject.Application.Features.Products.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<DeleteProductCommandHandler> _logger;
        private const string CacheKey = "productList";

        public DeleteProductCommandHandler(IProductRepository productRepository, ICacheService cacheService, ILogger<DeleteProductCommandHandler> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ürün silme isteği geldi. Ürün ID: {ProductId}", request.Id);

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                _logger.LogWarning("Silme başarısız: Ürün bulunamadı. ID: {ProductId}", request.Id);
                return false;
            }

            await _productRepository.DeleteAsync(request.Id);
            _logger.LogInformation("Ürün başarıyla veritabanından silindi. ID: {ProductId}", request.Id);

            // Ürün silindiği için cache'i temizleyelim.
            await _cacheService.RemoveAsync(CacheKey);
            _logger.LogWarning("Ürün silindiği için cache temizlendi.");

            return true;
        }
    }
}