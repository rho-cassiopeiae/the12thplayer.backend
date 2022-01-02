using System.Threading.Tasks;

using Livescore.Application.Common.Dto;

namespace Livescore.Api.Hubs.Clients {
    public interface ILivescoreClient {
        Task UpdateFixtureLivescore(string update);
        Task UpdateFixtureDiscussion(FixtureDiscussionUpdateDto update);
    }
}
