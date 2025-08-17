using MediatR;

namespace MyProject.Application.Features.Auth.Commands
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}