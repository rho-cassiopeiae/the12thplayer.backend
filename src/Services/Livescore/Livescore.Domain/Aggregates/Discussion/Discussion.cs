using System;
using System.Collections.Generic;

using Livescore.Domain.Base;

namespace Livescore.Domain.Aggregates.Discussion {
    public class Discussion : Entity, IAggregateRoot {
        public long FixtureId { get; private set; }
        public long TeamId { get; private set; }
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public bool Active { get; private set; }

        private List<DiscussionEntry> _entries = new();
        public IReadOnlyList<DiscussionEntry> Entries => _entries;
        
        public Discussion(
            long fixtureId, long teamId, Guid id, string name, bool active
        ) {
            FixtureId = fixtureId;
            TeamId = teamId;
            Id = id;
            Name = name;
            Active = active;
        }

        public void AddEntry(DiscussionEntry entry) {
            _entries.Add(entry);
        }
    }
}
