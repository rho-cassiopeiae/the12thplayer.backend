namespace Worker.Infrastructure.FootballDataProvider.Dto {
    public class SeasonDto {
        public class LeagueDataDto {
            public class LeagueDto {
                public long Id { get; set; }
                public string Name { get; set; }
                public string Type { get; set; }
                public bool? IsCup { get; set; }
                public string LogoPath { get; set; }
            }

            public LeagueDto Data { get; set; }
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public LeagueDataDto League { get; set; }
    }
}
