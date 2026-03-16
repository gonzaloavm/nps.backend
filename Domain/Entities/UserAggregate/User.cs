namespace Domain.Entities.UserAggregate
{
    public class User : BaseEntity<int>
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int AccessFailedCount { get; set; }
        public bool IsLocked { get; set; }
    }
}
