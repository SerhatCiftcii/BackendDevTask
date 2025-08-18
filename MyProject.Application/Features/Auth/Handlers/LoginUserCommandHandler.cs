using MediatR;
using Microsoft.Extensions.Logging;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Core.Interfaces;
using MyProject.Core.Models; // User modelini kullanmak için gerekli

namespace MyProject.Application.Features.Auth.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ILogger<LoginUserCommandHandler> _logger;

        public LoginUserCommandHandler(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, ILogger<LoginUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _logger = logger;
        }

        public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login isteği geldi: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogWarning("Login başarısız: Kullanıcı bulunamadı. E-posta: {Email}", request.Email);
                return null;
            }

            // Şifre doğrulama için BCrypt.Net kütüphanesini kullanıyoruz
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login başarısız: Şifre yanlış. E-posta: {Email}", request.Email);
                return null;
            }

            // Başarılı durum: Giriş yapıldı. Kullanıcının e-postasını logluyoruz.
            _logger.LogInformation("Kullanıcı başarıyla giriş yaptı: {Email}", user.Email);

            // Kullanıcı doğrulandı, JWT token'ı üretip geri dönüyoruz.
            return _tokenGenerator.GenerateToken(user);
        }
    }
}