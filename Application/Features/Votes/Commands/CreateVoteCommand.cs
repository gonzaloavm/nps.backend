using Application.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Votes.Commands
{
    public record CreateVoteCommand(int UserId, int Score) : IRequest<Result<CreateVoteResponse>>;
}
