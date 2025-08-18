using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Features.Products.Commands;
using MyProject.Application.Features.Products.Queries;

namespace MyProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Bu controller'daki tüm endpoint'ler için JWT token'ı gereklidir.
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetProductListQuery();
            var products = await _mediator.Send(query);
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok("Ürün başarıyla eklendi.");
            }
            return BadRequest("Ürün ekleme başarısız.");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok("Ürün başarıyla güncellendi.");
            }
            return NotFound("Ürün bulunamadı veya güncelleme başarısız.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok("Ürün başarıyla silindi.");
            }
            return NotFound("Ürün bulunamadı veya silme başarısız.");
        }
    }
}