using Application.Features.Auth.Commands;
using FluentValidation;

namespace Application.Validators
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("El token de refresco es obligatorio para cerrar la sesión.")
                .Must(token => !token.Contains(" ")).WithMessage("El token de refresco no es válido (contiene espacios).");
        }
    }
}
