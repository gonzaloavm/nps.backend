using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.VoteAggregate
{
    public class Vote : BaseEntity<int>
    {
        public int UserId { get; set; }
        public int Score { get; set; } // Escala 0-10
    }
}
