using Application.DTOs;
using Application.Features.Votes.Commands;
using Domain.Common;
using Domain.Entities.VoteAggregate;
using Domain.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Votes.Handlers
{
    public class CreateVoteCommandHandler : IRequestHandler<CreateVoteCommand, Result<CreateVoteResponse>>
    {
        private readonly IVoteRepository _voteRepository;

        public CreateVoteCommandHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<Result<CreateVoteResponse>> Handle(CreateVoteCommand request, CancellationToken ct)
        {
            // Garantizar la integridad de la encuesta permitiendo un solo voto por usuario
            var hasVoted = await _voteRepository.HasUserVotedAsync(request.UserId);

            if (hasVoted)
                return Result<CreateVoteResponse>.Failure(new Error(BusinessErrorCodes.AlreadyProcessed, "El usuario ya ha realizado su clasificación."));

            var vote = new Vote
            {
                UserId = request.UserId,
                Score = request.Score
            };

            var voteId = await _voteRepository.AddAsync(vote);

            return Result<CreateVoteResponse>.Success(new CreateVoteResponse(voteId));
        }
    }
}