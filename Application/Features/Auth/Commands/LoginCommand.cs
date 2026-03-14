using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands
{
    public record LoginCommand(
        string Username,  string Password
    ) : IRequest<AuthResponseDto?>;
}
