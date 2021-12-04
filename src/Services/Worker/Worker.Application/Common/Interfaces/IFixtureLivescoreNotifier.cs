using System.Threading.Tasks;

using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Common.Interfaces {
    public interface IFixtureLivescoreNotifier {
        Task NotifyFixtureActivated(long fixtureId, long teamId);

        Task NotifyFixturePrematchUpdated(long fixtureId, long teamId, FixtureDto fixture);
    }
}
