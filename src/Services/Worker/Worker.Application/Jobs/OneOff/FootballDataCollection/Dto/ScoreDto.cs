namespace Worker.Application.Jobs.OneOff.FootballDataCollection.Dto {
    public class ScoreDto {
        public short LocalTeam { get; set; }
        public short VisitorTeam { get; set; }
        public string HT { get; set; }
        public string FT { get; set; }
        public string ET { get; set; } // @@??: only extra time goals, ft goals not included
        public string PS { get; set; }
    }
}
