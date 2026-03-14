using Application.DTOs;
using Application.Features.NPS.Queries;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.NPS.Handlers
{
    public class GetNPSResultQueryHandler : IRequestHandler<GetNPSResultQuery, NPSResultDto>
    {
        private readonly IVoteRepository _voteRepository;

        public GetNPSResultQueryHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<NPSResultDto> Handle(GetNPSResultQuery request, CancellationToken cancellationToken)
        {
            var promoters = await _voteRepository.GetPromotersCountAsync();
            var detractors = await _voteRepository.GetDetractorsCountAsync();
            var neutrals = await _voteRepository.GetNeutralsCountAsync();
            var total = await _voteRepository.GetTotalVotesAsync();

            double nps = total > 0 ? ((double)(promoters - detractors) / total) * 100 : 0;

            return new NPSResultDto(promoters, detractors, neutrals, total, Math.Round(nps, 2));
        }
    }
}
