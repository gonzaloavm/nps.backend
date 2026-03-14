using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public record AuthResponseDto(
        string Token,
        string RefreshToken,
        string Role,
        string Username
    );
}
