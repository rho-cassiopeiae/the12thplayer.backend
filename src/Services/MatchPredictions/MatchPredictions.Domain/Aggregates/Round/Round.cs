using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Round {
    public class Round : Entity, IAggregateRoot {
        public long Id { get; private set; }
        public long SeasonId { get; private set; }
        public int Name { get; private set; }
        public long? StartDate { get; private set; }
        public long? EndDate { get; private set; }
        public bool IsCurrent { get; private set; }

        public Round(long id, long seasonId, int name, long? startDate, long? endDate, bool isCurrent) {
            Id = id;
            SeasonId = seasonId;
            Name = name;
            StartDate = startDate;
            EndDate = endDate;
            IsCurrent = isCurrent;
        }

        public void ChangeStartDate(long? startDate) {
            StartDate = startDate;
        }

        public void ChangeEndDate(long? endDate) {
            EndDate = endDate;
        }

        public void SetCurrent(bool isCurrent) {
            IsCurrent = isCurrent;
        }
    }
}
