using Application.Features.Auth.Commands;
using FluentValidation;
using System.Net;

namespace Application.Validators
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .MaximumLength(50).WithMessage("El nombre de usuario no puede exceder los 50 caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no puede exceder los 100 caracteres.");

            RuleFor(x => x.Ip)
                .NotEmpty().WithMessage("La dirección IP es obligatoria.")
                .Must(BeAValidIpAddress).WithMessage("La dirección IP no tiene un formato válido (IPv4 o IPv6).");

            RuleFor(x => x.Device)
                .NotEmpty().WithMessage("El dispositivo es obligatorio.")
                .MaximumLength(200).WithMessage("El dispositivo no puede exceder los 200 caracteres.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("La ubicación es obligatoria.")
                .MaximumLength(200).WithMessage("La ubicación no puede exceder los 200 caracteres.");
        }

        private bool BeAValidIpAddress(string ip)
        {
            // IPAddress.TryParse devuelve true si el string es una dirección IP válida (IPv4 o IPv6)
            return IPAddress.TryParse(ip, out _);
        }
    }
}
