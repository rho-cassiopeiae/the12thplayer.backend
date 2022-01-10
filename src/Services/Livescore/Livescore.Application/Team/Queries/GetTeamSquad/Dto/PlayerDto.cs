namespace Livescore.Application.Team.Queries.GetTeamSquad.Dto {
    public class PlayerDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? BirthDate { get; set; }
        public string CountryName { get; set; }
        public string CountryFlagUrl { get; set; }
        public short Number { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
    }
}
