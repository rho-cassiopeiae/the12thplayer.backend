using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class TeamStats : ValueObject {
        public class _Stats {
            public class ShotStats {
                public short? Total { get; private set; }
                public short? OnTarget { get; private set; }
                public short? OffTarget { get; private set; }
                public short? Blocked { get; private set; }
                public short? InsideBox { get; private set; }
                public short? OutsideBox { get; private set; }
                
                public ShotStats(
                    short? total, short? onTarget, short? offTarget,
                    short? blocked, short? insideBox, short? outsideBox
                ) {
                    Total = total;
                    OnTarget = onTarget;
                    OffTarget = offTarget;
                    Blocked = blocked;
                    InsideBox = insideBox;
                    OutsideBox = outsideBox;
                }
            }

            public class PassStats {
                public short? Total { get; private set; }
                public short? Accurate { get; private set; }
                
                public PassStats(short? total, short? accurate) {
                    Total = total;
                    Accurate = accurate;
                }
            }

            public ShotStats Shots { get; private set; }
            public PassStats Passes { get; private set; }
            public short? Fouls { get; private set; }
            public short? Corners { get; private set; }
            public short? Offsides { get; private set; }
            public short? BallPossession { get; private set; }
            public short? YellowCards { get; private set; }
            public short? RedCards { get; private set; }
            public short? Tackles { get; private set; }

            public _Stats(
                ShotStats shots, PassStats passes, short? fouls, short? corners, short? offsides,
                short? ballPossession, short? yellowCards, short? redCards, short? tackles
            ) {
                Shots = shots;
                Passes = passes;
                Fouls = fouls;
                Corners = corners;
                Offsides = offsides;
                BallPossession = ballPossession;
                YellowCards = yellowCards;
                RedCards = redCards;
                Tackles = tackles;
            }
        }

        public long TeamId { get; private set; }
        public _Stats Stats { get; private set; }

        public TeamStats(long teamId, _Stats stats) {
            TeamId = teamId;
            Stats = stats;
        }
    }
}
