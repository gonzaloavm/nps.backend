using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public record LoginResponse(
        string Token,
        string Role,
        string Username
    );
}
