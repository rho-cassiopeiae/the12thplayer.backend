namespace Identity.Infrastructure.Persistence.Models {
    public class RefreshToken {
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
        public long ExpiresAt { get; set; }
    }
}
