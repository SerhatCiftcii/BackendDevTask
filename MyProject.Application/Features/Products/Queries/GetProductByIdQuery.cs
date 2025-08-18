using MediatR;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Products.Queries
{
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
    }
}