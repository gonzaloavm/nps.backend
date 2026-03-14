using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public record NPSResultDto(int Promoters, int Neutrals, int Detractors, int Total, double NPS);
}
