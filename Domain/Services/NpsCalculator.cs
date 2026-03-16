using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
    public static class NpsCalculator
    {
        public static double Calculate(IEnumerable<int> scores)
        {
            var total = scores.Count();
            if (total == 0) return 0;

            var promoters = scores.Count(s => s >= 9);
            var detractors = scores.Count(s => s <= 6);

            return ((double)(promoters - detractors) / total) * 100;
        }
    }
}
