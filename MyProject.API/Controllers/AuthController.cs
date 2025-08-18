using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyProject.Application.Features.Auth.Commands;

namespace MyProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            // MediatR ile kayıt komutunu gönderiyoruz.
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok("Kullanıcı başarıyla kaydedildi.");
            }
            return BadRequest("Kullanıcı kaydı başarısız.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            // MediatR ile login komutunu gönderiyoruz.
            var token = await _mediator.Send(command);
            if (token == null)
            {
                return Unauthorized("Giriş başarısız. Lütfen bilgilerinizi kontrol edin.");
            }
            return Ok(new { token });
        }
    }
}
