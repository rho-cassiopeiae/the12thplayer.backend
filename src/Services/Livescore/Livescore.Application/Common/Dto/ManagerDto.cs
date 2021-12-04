using System;

namespace Livescore.Application.Common.Dto {
    public class ManagerDto {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public long? CountryId { get; set; }
        public string ImageUrl { get; set; }
    }
}
