using Application.Features.NPS.Queries;
using Domain.Common;
using Domain.Entities.DTOs;
using Domain.Repositories;
using MediatR;

namespace Application.Features.NPS.Handlers
{
    public class GetVoterListHandler : IRequestHandler<GetVoterListQuery, Result<IEnumerable<VoterDto>>>
    {
        private readonly IUserRepository _userRepository;

        public GetVoterListHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<IEnumerable<VoterDto>>> Handle(GetVoterListQuery request, CancellationToken ct)
        {
            // Recuperar usuarios junto con su estado de participación actual
            var voters = await _userRepository.GetUsersWithVoteStatusAsync();

            return Result<IEnumerable<VoterDto>>.Success(voters);
        }
    }
}