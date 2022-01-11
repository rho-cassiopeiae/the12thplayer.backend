namespace MatchPredictions.Application.Common.Dto {
    public class LeagueDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool? IsCup { get; set; }
        public string LogoUrl { get; set; }
    }
}
