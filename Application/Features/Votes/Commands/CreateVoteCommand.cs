using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Votes.Commands
{
    public record CreateVoteCommand(int UserId, int Score) : IRequest<bool>;
}
