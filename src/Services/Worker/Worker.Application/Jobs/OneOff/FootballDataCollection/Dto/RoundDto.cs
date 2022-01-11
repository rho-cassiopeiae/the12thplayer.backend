using System;
using System.Collections.Generic;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class RoundDto {
        public long Id { get; set; }
        public int Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<FixtureForMatchPredictionDto> Fixtures { get; set; }
    }
}
