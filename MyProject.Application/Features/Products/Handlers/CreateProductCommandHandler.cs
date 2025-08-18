using MediatR;
using MyProject.Application.Features.Products.Commands;
using MyProject.Core.Interfaces;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Products.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private const string CacheKey = "productList";

        public CreateProductCommandHandler(IProductRepository productRepository, ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Stock = request.Stock
            };

            await _productRepository.AddAsync(product);

            // Ürün eklendiği için cache'deki eski veriyi siliyoruz.
            // Bu, bir sonraki listeleme işleminde güncel verinin gelmesini sağlar.
            await _cacheService.RemoveAsync(CacheKey);

            return true;
        }
    }
}