using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Fixture {
    public class TeamLineup : ValueObject {
        public class Player {
            public long Id { get; private set; }
            public string FirstName { get; private set; }
            public string LastName { get; private set; }
            public short Number { get; private set; }
            public bool IsCaptain { get; private set; }
            public string Position { get; private set; }
            public short? FormationPosition { get; private set; }
            public string ImageUrl { get; private set; }
            public float? Rating { get; private set; }
            
            public Player(
                long id, string firstName, string lastName, short number,
                bool isCaptain, string position, short? formationPosition,
                string imageUrl, float? rating
            ) {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
                Number = number;
                IsCaptain = isCaptain;
                Position = position;
                FormationPosition = formationPosition;
                ImageUrl = imageUrl;
                Rating = rating;
            }
        }

        public class _Manager { // @@NOTE: Have to give it a name different from the 'Manager' property name.
            public long Id { get; private set; }
            public string Name { get; private set; }
            public string ImageUrl { get; private set; }
            
            public _Manager(long id, string name, string imageUrl) {
                Id = id;
                Name = name;
                ImageUrl = imageUrl;
            }
        }

        public long TeamId { get; private set; }
        public string Formation { get; private set; }
        public _Manager Manager { get; private set; }
        public IEnumerable<Player> StartingXI { get; private set; }
        public IEnumerable<Player> Subs { get; private set; }

        public TeamLineup(
            long teamId, string formation, _Manager manager,
            IEnumerable<Player> startingXI, IEnumerable<Player> subs
        ) {
            TeamId = teamId;
            Formation = formation;
            Manager = manager;
            StartingXI = startingXI;
            Subs = subs;
        }
    }
}
