namespace MessageBus.Contracts.Requests.Worker.Dto {
    public class GameTimeDto {
        public short? Minute { get; set; }
        public short? ExtraTimeMinute { get; set; }
        public short? AddedTimeMinute { get; set; }
    }
}
