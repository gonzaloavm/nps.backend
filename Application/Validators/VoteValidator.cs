using Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class VoteValidator : AbstractValidator<VoteDto>
    {
        public VoteValidator()
        {
            RuleFor(v => v.Score)
                .InclusiveBetween(1, 5)
                .WithMessage("Score must be between 1 and 10.");
        }
    }
}
