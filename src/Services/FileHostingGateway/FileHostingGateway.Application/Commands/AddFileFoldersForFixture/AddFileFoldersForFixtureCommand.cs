using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using FileHostingGateway.Application.Common.Interfaces;
using FileHostingGateway.Application.Common.Results;

namespace FileHostingGateway.Application.Commands.AddFileFoldersForFixture {
    public class AddFileFoldersForFixtureCommand : IRequest<HandleResult<string>> {
        public long FixtureId { get; init; }
        public long TeamId { get; init; }
    }

    public class AddFileFoldersForFixtureCommandHandler : IRequestHandler<
        AddFileFoldersForFixtureCommand, HandleResult<string>
    > {
        private readonly ILogger<AddFileFoldersForFixtureCommandHandler> _logger;
        private readonly IVimeoGateway _vimeoGateway;

        public AddFileFoldersForFixtureCommandHandler(
            ILogger<AddFileFoldersForFixtureCommandHandler> logger,
            IVimeoGateway vimeoGateway
        ) {
            _logger = logger;
            _vimeoGateway = vimeoGateway;
        }

        public async Task<HandleResult<string>> Handle(
            AddFileFoldersForFixtureCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _vimeoGateway.AddProjectFor(command.FixtureId, command.TeamId);
            if (outcome.IsError) {
                _logger.LogError(outcome.Error.Errors.Values.First().First());

                return new HandleResult<string> {
                    Error = outcome.Error
                };
            }

            return new HandleResult<string> {
                Data = outcome.Data
            };
        }
    }
}
