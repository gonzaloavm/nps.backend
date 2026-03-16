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
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);

            if (existingUser != null)
            {
                // Garantizar la unicidad de la identidad del usuario
                return Result<RegisterResponse>.Failure(new Error(BusinessErrorCodes.DuplicateResource, "El nombre de usuario ya existe."));
            }

            var passwordHash = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                Role = request.Role
            };

            var userId = await _userRepository.AddAsync(user);

            return Result<RegisterResponse>.Success(new RegisterResponse(userId));
        }
    }
}