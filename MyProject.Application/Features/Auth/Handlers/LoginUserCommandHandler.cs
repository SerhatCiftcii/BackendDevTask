using MediatR;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Core.Interfaces; 

namespace MyProject.Application.Features.Auth.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator; 

        public LoginUserCommandHandler(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Hata yönetimi burada yapılacak,
                return null;
            }

            // Kullanıcı doğrulandı, JWT token'ı üret ve dön 
            return _tokenGenerator.GenerateToken(user);
        }
    }
}