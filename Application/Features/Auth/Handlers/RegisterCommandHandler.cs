using Application.Contracts;
using Application.DTOs;
using Application.Features.Auth.Commands;
using Domain.Common;
using Domain.Entities.UserAggregate;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Validar que el usuario no exista
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return Result<RegisterResponse>.Failure(new Error(BusinessErrorCodes.Generic, "El nombre de usuario ya existe."));
            }

            // Hashear contraseña
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Crear nuevo usuario
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = request.Role
            };

            var userId = await _userRepository.AddAsync(user);

            return Result<RegisterResponse>.Success(new RegisterResponse (userId));
        }
    }
}
