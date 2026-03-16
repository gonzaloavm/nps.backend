using Application.DTOs;
using Application.Features.NPS.Queries;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Features.NPS.Handlers
{
    public class GetNpsStatisticsQueryHandler : IRequestHandler<GetNpsStatisticsQuery, Result<NpsStatisticsResponse>>
    {
        private readonly IVoteRepository _voteRepository;

        public GetNpsStatisticsQueryHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<Result<NpsStatisticsResponse>> Handle(GetNpsStatisticsQuery request, CancellationToken ct)
        {
            var data = await _voteRepository.GetNpsStatisticsAsync();

            if (data.TotalVotes == 0)
                return Result<NpsStatisticsResponse>.Success(new NpsStatisticsResponse());

            // Calculamos el Score
            double npsScore = ((double)(data.Promoters - data.Detractors) / data.TotalVotes) * 100;

            var response = new NpsStatisticsResponse
            {
                TotalVotes = data.TotalVotes,
                Promoters = data.Promoters,
                Detractors = data.Detractors,
                Neutrals = data.Neutrals,
                NpsScore = Math.Round(npsScore, 2),
                Classification = npsScore switch
                {
                    > 75 => "Excelente",
                    > 50 => "Muy Bueno",
                    > 0 => "Bueno",
                    _ => "Necesita Mejorar"
                }
            };

            return Result<NpsStatisticsResponse>.Success(response);
        }
    }
}
