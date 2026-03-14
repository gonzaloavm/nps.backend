using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.NPS.Queries
{
    public record GetNPSResultQuery() : IRequest<NPSResultDto>;
}
