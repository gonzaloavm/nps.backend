using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class NpsStatisticsResponse
    {
        public int TotalVotes { get; set; }
        public int Promoters { get; set; }
        public int Detractors { get; set; }
        public int Neutrals { get; set; }
        public double NpsScore { get; set; }
        public string Classification { get; set; } = string.Empty; // Ejemplo: "Excelente", "Bueno", etc.
    }
}
