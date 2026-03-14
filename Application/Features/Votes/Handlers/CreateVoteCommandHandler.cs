using Application.Features.Votes.Commands;
using Domain.Entities;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Votes.Handlers
{
    public class CreateVoteCommandHandler : IRequestHandler<CreateVoteCommand, bool>
    {
        private readonly IVoteRepository _voteRepository;
        private readonly IUserRepository _userRepository;

        public CreateVoteCommandHandler(IVoteRepository voteRepository, IUserRepository userRepository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(CreateVoteCommand request, CancellationToken cancellationToken)
        {
            var alreadyVoted = await _voteRepository.GetByUserIdAsync(request.UserId);
            if (alreadyVoted != null)
                return false;

            var vote = new Vote
            {
                UserId = request.UserId,
                Score = request.Score,
                VotedAt = DateTime.UtcNow
            };

            await _voteRepository.AddAsync(vote);
            return true;
        }
    }
}
