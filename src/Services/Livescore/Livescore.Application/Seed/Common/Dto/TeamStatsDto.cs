namespace Livescore.Application.Seed.Common.Dto {
    public class TeamStatsDto {
        public class StatsDto {
            public class ShotStatsDto {
                public short? Total { get; set; }
                public short? OnTarget { get; set; }
                public short? OffTarget { get; set; }
                public short? Blocked { get; set; }
                public short? InsideBox { get; set; }
                public short? OutsideBox { get; set; }
            }

            public class PassStatsDto {
                public short? Total { get; set; }
                public short? Accurate { get; set; }
            }

            public ShotStatsDto Shots { get; set; }
            public PassStatsDto Passes { get; set; }
            public short? Fouls { get; set; }
            public short? Corners { get; set; }
            public short? Offsides { get; set; }
            public short? BallPossession { get; set; }
            public short? YellowCards { get; set; }
            public short? RedCards { get; set; }
            public short? Tackles { get; set; }
        }

        public long TeamId { get; set; }
        public StatsDto Stats { get; set; }
    }
}
