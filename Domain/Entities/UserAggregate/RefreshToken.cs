using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.UserAggregate
{
    public class RefreshToken : BaseEntity<int>
    {
        public int SessionId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public int? ReplacedByTokenId { get; set; }
    }
}
