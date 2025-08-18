using MediatR;
using MyProject.Application.Features.Products.Commands;
using MyProject.Core.Interfaces;

namespace MyProject.Application.Features.Products.Handlers
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKey = "productList";

        public DeleteProductCommandHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return false;
            }

            await _productRepository.DeleteAsync(request.Id);

            // Ürün silindiği için cache'i temizleyelimm Ürünün silindiğini bilmesi gerekir çünkü.
            await _cacheService.RemoveAsync(CacheKey);

            return true;
        }
    }
}