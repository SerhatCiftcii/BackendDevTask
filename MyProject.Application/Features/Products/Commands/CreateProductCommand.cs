using MediatR;

namespace MyProject.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}