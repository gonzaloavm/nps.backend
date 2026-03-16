using Application.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands
{
    public record RefreshTokenCommand(string RefreshToken, string Ip) : IRequest<Result<RefreshSessionResponse>>; 
}
