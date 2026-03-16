using Application.DTOs;
using Application.Features.Votes.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class VoteValidator : AbstractValidator<CreateVoteCommand>
    {
        public VoteValidator()
        {
            RuleFor(v => v.Score)
                .InclusiveBetween(1, 10)
                .WithMessage("Score must be between 1 and 10.");
        }
    }
}
