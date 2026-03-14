using Application.Features.NPS.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NPSController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NPSController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("result")]
        public async Task<IActionResult> GetNPSResult()
        {
            var result = await _mediator.Send(new GetNPSResultQuery());
            return Ok(result);
        }
    }
}
