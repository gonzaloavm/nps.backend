using Application.Features.Votes.Commands;
using FluentValidation;

namespace Application.Validators
{
    public class CreateVoteCommandValidator : AbstractValidator<CreateVoteCommand>
    {
        public CreateVoteCommandValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("El ID de usuario debe ser un identificador válido.");

            RuleFor(x => x.Score)
                .InclusiveBetween(0, 10).WithMessage("La puntuación NPS debe estar entre 0 y 10.");
        }
    }
}
