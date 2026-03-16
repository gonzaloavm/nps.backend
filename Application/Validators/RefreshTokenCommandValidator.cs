using Application.Features.Auth.Commands;
using FluentValidation;

namespace Application.Validators
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El token de refresco es obligatorio.")
                .Must(token => !token.Contains(" ")).WithMessage("El token de refresco no puede contener espacios.");

            RuleFor(x => x.Ip)
                .NotEmpty().WithMessage("La dirección IP es obligatoria para validar el origen de la petición.")
                .Must(ip => ip.Contains(".") || ip.Contains(":")).WithMessage("El formato de la IP no es válido.");
        }
    }
}
