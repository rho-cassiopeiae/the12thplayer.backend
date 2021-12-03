using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class GameTime : ValueObject {
        public short? Minute { get; private set; }
        public short? ExtraTimeMinute { get; private set; }
        public short? AddedTimeMinute { get; private set; }
        
        public GameTime(
            short? minute, short? extraTimeMinute, short? addedTimeMinute
        ) {
            Minute = minute;
            ExtraTimeMinute = extraTimeMinute;
            AddedTimeMinute = addedTimeMinute;
        }
    }
}
