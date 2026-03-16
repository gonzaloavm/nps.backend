using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.UserAggregate
{
    public class UserSession : BaseEntity<int>
    {
        public int UserId { get; set; }
        public string? IpAddress { get; set; }
        public string? Device { get; set; }
        public string? Location { get; set; }
        public DateTime LastActivityAt { get; set; }
        public bool IsActive { get; set; }
    }
}
