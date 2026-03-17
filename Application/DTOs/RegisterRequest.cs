using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    public record RegisterRequest(
        string Username,
        string Password,
        string Role
    );
}
