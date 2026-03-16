using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.DTOs
{
    public class VoterDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasVoted { get; set; }
    }
}
