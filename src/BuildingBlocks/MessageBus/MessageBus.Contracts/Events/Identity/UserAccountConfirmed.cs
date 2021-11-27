﻿namespace MessageBus.Contracts.Events.Identity {
    public class UserAccountConfirmed : Message {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
