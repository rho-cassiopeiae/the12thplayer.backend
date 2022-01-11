using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Fixture {
    public class GameTime : ValueObject {
        public short? Minute { get; private set; }
        public short? ExtraTimeMinute { get; private set; }
        public short? AddedTimeMinute { get; private set; }
        
        public GameTime(short? minute, short? extraTimeMinute, short? addedTimeMinute) {
            Minute = minute;
            ExtraTimeMinute = extraTimeMinute;
            AddedTimeMinute = addedTimeMinute;
        }
    }
}
