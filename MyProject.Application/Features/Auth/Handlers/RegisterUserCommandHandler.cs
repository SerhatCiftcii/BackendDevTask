using MediatR;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Core.Interfaces;
using MyProject.Core.Models;

namespace MyProject.Application.Features.Auth.Handlers
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // Şifreyi hash'lemek için basit bir yöntem bu Bcry kütüphanesini indirdim. - kütüphane ismi, BCrypt.Net-Next
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            };

            await _userRepository.AddAsync(user);
            return true;
        }
    }
}