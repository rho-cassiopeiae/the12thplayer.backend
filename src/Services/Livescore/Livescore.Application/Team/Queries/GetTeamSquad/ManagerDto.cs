namespace Livescore.Application.Team.Queries.GetTeamSquad {
    public class ManagerDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? BirthDate { get; set; }
        public string CountryName { get; set; }
        public string CountryFlagUrl { get; set; }
        public string ImageUrl { get; set; }
    }
}
