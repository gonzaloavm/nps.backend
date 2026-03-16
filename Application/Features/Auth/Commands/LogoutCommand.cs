using Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Auth.Commands
{
    public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;
}
