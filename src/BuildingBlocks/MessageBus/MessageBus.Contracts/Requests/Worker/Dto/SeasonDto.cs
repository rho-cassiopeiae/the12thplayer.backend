namespace MessageBus.Contracts.Requests.Worker.Dto {
    public class SeasonDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public LeagueDto League { get; set; }
    }
}
