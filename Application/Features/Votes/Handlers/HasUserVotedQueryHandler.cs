using Application.DTOs;
using Application.Features.Votes.Queries;
using Domain.Common;
using Domain.Repositories;
using MediatR;

namespace Application.Features.Votes.Handlers
{
    public class HasUserVotedQueryHandler : IRequestHandler<HasUserVotedQuery, Result<HasVotedResponse>>
    {
        private readonly IVoteRepository _voteRepository;

        public HasUserVotedQueryHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<Result<HasVotedResponse>> Handle(HasUserVotedQuery request, CancellationToken cancellationToken)
        {
            var vote = await _voteRepository.HasUserVotedAsync(request.UserId);
            return Result<HasVotedResponse>.Success(new HasVotedResponse(vote));
        }
    }
}
