using Application.DTOs;
using Application.Features.Votes.Commands;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.VoteAggregate;
using Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
            // Validar escala (0-10) - Podrías usar FluentValidation también
            if (request.Score < 0 || request.Score > 10)
                return Result<CreateVoteResponse>.Failure(new Error(BusinessErrorCodes.Generic, "La puntuación debe estar entre 0 y 10."));

            // Validar si ya votó
            var hasVoted = await _voteRepository.HasUserVotedAsync(request.UserId);
            if (hasVoted)
                return Result<CreateVoteResponse>.Failure(new Error(BusinessErrorCodes.Generic, "El usuario ya ha realizado su clasificación."));

            // Guardar en DB
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
