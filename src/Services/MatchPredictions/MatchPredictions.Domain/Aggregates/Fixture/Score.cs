using MatchPredictions.Domain.Base;

namespace MatchPredictions.Domain.Aggregates.Fixture {
    public class Score : ValueObject {
        public short LocalTeam { get; private set; }
        public short VisitorTeam { get; private set; }
        public string HT { get; private set; }
        public string FT { get; private set; }
        public string ET { get; private set; }
        public string PS { get; private set; }
        
        public Score(
            short localTeam, short visitorTeam,
            string ht, string ft, string et, string ps
        ) {
            LocalTeam = localTeam;
            VisitorTeam = visitorTeam;
            HT = ht;
            FT = ft;
            ET = et;
            PS = ps;
        }
    }
}
