using Application.DTOs;
using Application.Features.Votes.Commands;
using Application.Features.Votes.Queries;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VoteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize (Policy = "VoterOnly")]
        [ProducesResponseType(typeof(ApiResponse<CreateVoteResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateVote([FromBody] CreateVoteRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ProblemDetailsMapper.ToProblemDetails(new[] { new Error(BusinessErrorCodes.Generic, "Usuario no identificado.") }, StatusCodes.Status401Unauthorized, "Unauthorized"));
            }

            var command = new CreateVoteCommand(userId, request.Score);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<CreateVoteResponse>.Ok(result.Value));
        }

        [HttpGet("has-voted")]
        [Authorize(Policy = "VoterOnly")]
        [ProducesResponseType(typeof(ApiResponse<HasVotedResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> HasVoted()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(ProblemDetailsMapper.ToProblemDetails(new[] { new Error(BusinessErrorCodes.Generic, "Usuario no identificado.", "userId") }, StatusCodes.Status401Unauthorized, "Unauthorized"));
            }

            var result = await _mediator.Send(new HasUserVotedQuery(userId));

            if (!result.IsSuccess)
            {
                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<HasVotedResponse>.Ok(result.Value));
        }
    }
}
