using Application.DTOs;
using Application.Features.NPS.Queries;
using Domain.Entities.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NpsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NpsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("nps-statistics")]
        [Authorize(Policy = "AdminOnly")]
        [Tags("Métricas")]
        [EndpointDescription("Calcula el índice NPS actual basándose en los votos registrados, clasificándolos en Promotores, Pasivos y Detractores.")]
        [ProducesResponseType(typeof(ApiResponse<NpsStatisticsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetNpsStatistics(CancellationToken ct)
        {
            // Enviamos el Query al Handler que usa el DTO con la lógica matemática
            var result = await _mediator.Send(new GetNpsStatisticsQuery(), ct);

            if (!result.IsSuccess)
            {
                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Bad Request"));
            }

            return Ok(ApiResponse<NpsStatisticsResponse>.Ok(result.Value));
        }

        [HttpGet("voters-list")]
        [Authorize(Policy = "AdminOnly")]
        [Tags("Administración")]
        [EndpointDescription("Obtiene la lista detallada de todos los votantes registrados y su estado de participación.")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<VoterDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetVotersList(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetVoterListQuery(), ct);

            if (!result.IsSuccess)
            {
                return BadRequest(ProblemDetailsMapper.ToProblemDetails(result.Errors, StatusCodes.Status400BadRequest, "Error al obtener la lista de votantes"));
            }

            return Ok(ApiResponse<IEnumerable<VoterDto>>.Ok(result.Value));
        }

    }
}
