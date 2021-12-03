namespace MessageBus.Contracts.Requests.Worker.Dto {
    public class ScoreDto {
        public short LocalTeam { get; set; }
        public short VisitorTeam { get; set; }
        public string HT { get; set; }
        public string FT { get; set; }
        public string ET { get; set; }
        public string PS { get; set; }
    }
}
