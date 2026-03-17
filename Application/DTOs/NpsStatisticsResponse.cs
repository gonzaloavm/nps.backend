using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Application.DTOs
{
    public record NpsStatisticsResponse(
        int TotalVotes,
        int Promoters,
        int Detractors,
        int Neutrals,
        double NpsScore,
        string Classification
    );
    }
