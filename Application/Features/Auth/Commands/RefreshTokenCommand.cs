using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands
{
    public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthResponseDto>; 
}
