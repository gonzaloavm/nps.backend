
using Application.DTOs;
using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands
{
    public record RegisterCommand(string Username, string Password, string Role) : IRequest<Result<RegisterResponse>>;
}
