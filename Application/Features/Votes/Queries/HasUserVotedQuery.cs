using Application.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Votes.Queries
{
    public record HasUserVotedQuery(int UserId) : IRequest<Result<HasVotedResponse>>;
}
