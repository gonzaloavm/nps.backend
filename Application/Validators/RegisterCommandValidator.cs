using Application.Features.Auth.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
                .MinimumLength(3).WithMessage("El usuario debe tener al menos 3 caracteres.")
                .MaximumLength(20).WithMessage("El usuario no puede exceder los 20 caracteres.")
                .Must(u => !u.Contains(" ")).WithMessage("El nombre de usuario no puede contener espacios.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("El rol es obligatorio.")
                .Must(r => r == "Voter" || r == "Admin").WithMessage("El rol debe ser 'Voter' o 'Admin'.");
        }
    }
}
