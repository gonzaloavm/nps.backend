using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public record Error(string Code, string Message, string? Field = null)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
    }

}
