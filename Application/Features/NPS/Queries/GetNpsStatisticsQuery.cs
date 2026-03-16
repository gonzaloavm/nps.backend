using Application.DTOs;
using Domain.Common;
using MediatR;

namespace Application.Features.NPS.Queries
{
    public record GetNpsStatisticsQuery() : IRequest<Result<NpsStatisticsResponse>>;
}
