using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Score { get; set; }
        public DateTime VotedAt { get; set; }
        public User? User { get; set; }
    }
}
