using System.Collections.Generic;

namespace Livescore.Application.Common.Dto {
    public class TeamLineupDto {
        public class PlayerDto {
            public long Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string DisplayName { get; set; }
            public short Number { get; set; }
            public bool IsCaptain { get; set; }
            public string Position { get; set; }
            public short? FormationPosition { get; set; }
            public string ImageUrl { get; set; }
        }

        public class ManagerDto {
            public long Id { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
        }

        public long TeamId { get; set; }
        public string Formation { get; set; }
        public ManagerDto Manager { get; set; }
        public IEnumerable<PlayerDto> StartingXI { get; set; }
        public IEnumerable<PlayerDto> Subs { get; set; }
    }
}
