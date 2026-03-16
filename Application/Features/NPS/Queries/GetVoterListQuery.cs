using Domain.Common;
using Domain.Entities.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.NPS.Queries
{
    public record GetVoterListQuery() : IRequest<Result<IEnumerable<VoterDto>>>;
}
