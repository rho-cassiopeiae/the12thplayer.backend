namespace MessageBus.Contracts.Common.Dto {
    public class SeasonDto {
        public long Id { get; set; }
        public string Name { get; set; }
        public LeagueDto League { get; set; }
    }
}
