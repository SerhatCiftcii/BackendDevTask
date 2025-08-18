using MediatR;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Products.Queries
{
    public class GetProductListQuery : IRequest<IEnumerable<Product>>
    {
    }
}