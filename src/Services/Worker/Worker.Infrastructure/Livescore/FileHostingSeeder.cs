using System;
using System.Threading.Tasks;

using MassTransit;

using MessageBus.Contracts.Requests.Worker;
using MessageBus.Contracts.Responses.FileHostingGateway;

using Worker.Application.Common.Interfaces;

namespace Worker.Infrastructure.Livescore {
    public class FileHostingSeeder : IFileHostingSeeder {
        private readonly IRequestClient<AddFileFoldersForFixture> _addFoldersClient;

        public FileHostingSeeder(IRequestClient<AddFileFoldersForFixture> addFoldersClient) {
            _addFoldersClient = addFoldersClient;
        }

        public async Task<string> AddFoldersFor(long fixtureId, long teamId) {
            var response = await _addFoldersClient.GetResponse<AddFileFoldersForFixtureSuccess>(
                new AddFileFoldersForFixture {
                    CorrelationId = Guid.NewGuid(),
                    FixtureId = fixtureId,
                    TeamId = teamId
                }
            );

            return response.Message.VimeoProjectId;
        }
    }
}
