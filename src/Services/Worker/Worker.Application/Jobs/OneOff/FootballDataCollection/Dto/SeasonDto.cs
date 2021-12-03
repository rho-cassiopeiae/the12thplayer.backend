namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class SeasonDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public LeagueDto League { get; set; }
    }
}
