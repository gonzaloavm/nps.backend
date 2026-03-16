using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.DTOs
{
    public class NpsStatisticsDto
    {
        public int TotalVotes { get; set; }
        public int Promoters { get; set; }
        public int Detractors { get; set; }
        public int Neutrals { get; set; }
    }
}
