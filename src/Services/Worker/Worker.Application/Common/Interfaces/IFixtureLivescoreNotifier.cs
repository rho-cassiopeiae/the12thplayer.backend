using System.Threading.Tasks;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Common.Interfaces {
    public interface IFixtureLivescoreNotifier {
        Task NotifyFixtureActivated(long fixtureId, long teamId, string vimeoProjectId);

        Task NotifyFixturePrematchUpdated(long fixtureId, long teamId, FixtureDto fixture);

        Task NotifyFixtureLiveUpdated(long fixtureId, long teamId, FixtureDto fixture);

        Task NotifyFixtureFinished(long fixtureId, long teamId);

        Task NotifyFixtureDeactivated(long fixtureId, long teamId, string vimeoProjectId);
    }
}
