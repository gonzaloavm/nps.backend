using Application.DTOs;
using Application.Features.Votes.Commands;
using Application.Features.Votes.Queries;
using Application.Validators;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> CreateVote([FromBody] VoteDto voteDto)
        {
            var validator = new VoteValidator();
            var validationResult = await validator.ValidateAsync(voteDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var hasVoted = await _mediator.Send(new HasUserVotedQuery (userId));
            if (hasVoted)
                return BadRequest(new { message = "User has already voted." });

            var command = new CreateVoteCommand (userId, voteDto.Score);
            var result = await _mediator.Send(command);
            if (result)
                return Ok(new { message = "Vote registered successfully." });
            else
                return BadRequest(new { message = "Unable to register vote." });
        }

        [HttpGet("has-voted")]
        public async Task<IActionResult> HasVoted()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var hasVoted = await _mediator.Send(new HasUserVotedQuery (userId));
            return Ok(new { hasVoted });
        }
    }
}
