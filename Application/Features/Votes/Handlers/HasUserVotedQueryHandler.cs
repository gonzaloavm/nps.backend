using Application.Features.Votes.Queries;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Votes.Handlers
{
    public class HasUserVotedQueryHandler : IRequestHandler<HasUserVotedQuery, bool>
    {
        private readonly IVoteRepository _voteRepository;

        public HasUserVotedQueryHandler(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<bool> Handle(HasUserVotedQuery request, CancellationToken cancellationToken)
        {
            var vote = await _voteRepository.GetByUserIdAsync(request.UserId);
            return vote != null;
        }
    }
}
