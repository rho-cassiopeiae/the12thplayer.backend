using System.Threading.Tasks;

using Livescore.Application.Common.Dto;

namespace Livescore.Application.Common.Interfaces {
    public interface IInMemoryStore {
        Task SaveChanges();

        void SetFixtureActiveAndOngoing(long fixtureId, long teamId);

        void CreateDiscussionsFor(long fixtureId, long teamId);

        void AddFixtureParticipantsFromLineup(
            long fixtureId, long teamId, TeamLineupDto lineup
        );

        void AddFixtureParticipantsFromMatchEvents(
            long fixtureId, long teamId, TeamMatchEventsDto teamMatchEvents
        );
    }
}
