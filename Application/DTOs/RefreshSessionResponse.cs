using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    public record RefreshSessionResponse(
        string Jwt
    );
}
